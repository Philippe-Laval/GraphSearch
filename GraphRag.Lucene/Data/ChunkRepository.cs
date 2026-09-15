using GraphRag.Lucene.Interfaces;
using GraphRag.Lucene.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GraphRag.Lucene.Data;

/// <summary>
/// ChunkRepository is responsible for managing the storage and retrieval 
/// of documents and their associated chunks in a SQLite database. 
/// It provides methods to initialize the database schema, 
/// insert or update documents and chunks, retrieve documents and chunks, 
/// and manage the indexing queue.
/// </summary>
public sealed class ChunkRepository : IChunkRepository
{
    private readonly string _connectionString;

    public ChunkRepository(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        var fullPath = Path.GetFullPath(databasePath);

        Directory.CreateDirectory(
            Path.GetDirectoryName(fullPath)
            ?? throw new InvalidOperationException("Invalid database path."));

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
            Pooling = true
        }.ToString();
    }

    /// <summary>
    /// Initialise le schéma de base de données SQLite avec les tables, 
    /// index et contraintes nécessaires pour le stockage de documents 
    /// et l'indexation.
    /// </summary>
    public void Initialize()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText =
            """
        PRAGMA journal_mode = WAL;

        CREATE TABLE IF NOT EXISTS documents
        (
            id                  INTEGER PRIMARY KEY,
            relative_path       TEXT NOT NULL UNIQUE,
            content_hash        TEXT NOT NULL,
            last_write_time_utc TEXT NOT NULL,
            metadata_json       TEXT NULL,
            indexed_revision    INTEGER NOT NULL DEFAULT 0
        );

        CREATE TABLE IF NOT EXISTS chunks
        (
            id            INTEGER PRIMARY KEY,
            document_id   INTEGER NOT NULL,
            ordinal       INTEGER NOT NULL,
            text          TEXT NOT NULL,
            metadata_json TEXT NULL,

            FOREIGN KEY (document_id)
                REFERENCES documents(id)
                ON DELETE CASCADE,

            UNIQUE(document_id, ordinal)
        );

        CREATE INDEX IF NOT EXISTS ix_chunks_document_id
            ON chunks(document_id);

        CREATE TABLE IF NOT EXISTS chunk_entities
        (
            chunk_id  INTEGER NOT NULL,
            entity_id TEXT NOT NULL,

            PRIMARY KEY(chunk_id, entity_id),

            FOREIGN KEY (chunk_id)
                REFERENCES chunks(id)
                ON DELETE CASCADE
        );

        CREATE INDEX IF NOT EXISTS ix_chunk_entities_entity_id
            ON chunk_entities(entity_id);

        CREATE TABLE IF NOT EXISTS indexing_queue
        (
            document_id INTEGER PRIMARY KEY,
            queued_utc  TEXT NOT NULL,

            FOREIGN KEY (document_id)
                REFERENCES documents(id)
                ON DELETE CASCADE
        );
        """;

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Finds a document in the database by its relative path.
    /// </summary>
    /// <param name="relativePath">The relative path of the document.</param>
    /// <returns>The document if found; otherwise, <c>null</c>.</returns>
    public SourceDocument? FindDocumentByPath(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        using var connection = OpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText =
            """
        SELECT
            id,
            relative_path,
            content_hash,
            last_write_time_utc,
            metadata_json
        FROM documents
        WHERE relative_path = $relative_path;
        """;

        command.Parameters.AddWithValue(
            "$relative_path",
            relativePath);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return ReadSourceDocument(reader);
    }

    /// <summary>
    /// Determines whether a document with the specified relative path and content hash exists in the database. 
    /// </summary>
    /// <param name="relativePath">The relative path of the document.</param>
    /// <param name="contentHash">The content hash of the document.</param>
    /// <returns><see langword="true"/> if a document with the specified relative path and content hash exists; otherwise, <see
    /// langword="false"/>.</returns>
    public bool IsCurrent(
        string relativePath,
        string contentHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);

        using var connection = OpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText =
            """
        SELECT EXISTS
        (
            SELECT 1
            FROM documents
            WHERE relative_path = $relative_path
              AND content_hash = $content_hash
        );
        """;

        command.Parameters.AddWithValue(
            "$relative_path",
            relativePath);

        command.Parameters.AddWithValue(
            "$content_hash",
            contentHash);

        return Convert.ToInt64(command.ExecuteScalar()) == 1;
    }

    /// <summary>
    /// Replaces a document and its associated chunks in the database.
    /// Queues the document for indexing.
    /// </summary>
    /// <param name="source">The source document to be replaced.</param>
    /// <param name="chunks">The list of chunks associated with the document.</param>
    /// <returns>The persisted document with its associated chunks.</returns>
    public PersistedDocument ReplaceDocument(
        SourceDocument source,
        IReadOnlyList<ChunkDocument> chunks)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(chunks);

        using var connection = OpenConnection();
        using var transaction = connection.BeginTransaction();

        var documentId = UpsertDocument(
            connection,
            transaction,
            source);

        DeleteChunks(
            connection,
            transaction,
            documentId);

        var persistedChunks = InsertChunks(
            connection,
            transaction,
            documentId,
            chunks);

        // Queue the document for indexing after replacing it and its chunks.
        QueueIndexing(
            connection,
            transaction,
            documentId);

        transaction.Commit();

        var persistedSource = source with
        {
            Id = documentId
        };

        return new PersistedDocument(
            persistedSource,
            persistedChunks);
    }

    /// <summary>
    /// Retrieves the chunks associated with a specific document.
    /// </summary>
    /// <param name="documentId">The ID of the document.</param>
    /// <returns>The list of chunks associated with the document.</returns>
    public IReadOnlyList<ChunkDocument> GetDocumentChunks(
        int documentId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText =
            """
        SELECT
            c.id,
            c.document_id,
            c.text,
            c.metadata_json,
            COALESCE(
                json_group_array(ce.entity_id)
                    FILTER (WHERE ce.entity_id IS NOT NULL),
                '[]'
            ) AS entity_ids
        FROM chunks AS c
        LEFT JOIN chunk_entities AS ce
            ON ce.chunk_id = c.id
        WHERE c.document_id = $document_id
        GROUP BY
            c.id,
            c.document_id,
            c.ordinal,
            c.text,
            c.metadata_json
        ORDER BY c.ordinal;
        """;

        command.Parameters.AddWithValue(
            "$document_id",
            documentId);

        using var reader = command.ExecuteReader();
        var chunks = new List<ChunkDocument>();

        while (reader.Read())
        {
            chunks.Add(ReadChunk(reader));
        }

        return chunks;
    }

    /// <summary>
    /// Gets a list of chunks by their IDs. 
    /// This method retrieves the chunks from the database based on the provided chunk IDs, 
    /// ensuring that each chunk is only retrieved once even if duplicate IDs are provided. 
    /// The chunks are returned in the order they are found in the database.
    /// </summary>
    /// <param name="chunkIds">The IDs of the chunks to retrieve.</param>
    /// <returns>The list of chunks corresponding to the provided IDs.</returns>
    public IReadOnlyList<ChunkDocument> GetChunks(
        IEnumerable<int> chunkIds)
    {
        ArgumentNullException.ThrowIfNull(chunkIds);

        var ids = chunkIds
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
        {
            return [];
        }

        using var connection = OpenConnection();

        const int batchSize = 500;
        var chunks = new List<ChunkDocument>(ids.Length);

        foreach (var batch in ids.Chunk(batchSize))
        {
            chunks.AddRange(
                GetChunkBatch(connection, batch));
        }

        return chunks;
    }

    /// <summary>
    /// Get a list of document IDs that are currently queued for indexing.
    /// </summary>
    /// <param name="limit">The maximum number of document IDs to retrieve.</param>
    /// <returns>The list of document IDs currently queued for indexing.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the limit is less than or equal to zero.</exception>
    public IReadOnlyList<int> GetQueuedDocumentIds(
        int limit = 100)
    {
        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }

        using var connection = OpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText =
            """
        SELECT document_id
        FROM indexing_queue
        ORDER BY queued_utc
        LIMIT $limit;
        """;

        command.Parameters.AddWithValue("$limit", limit);

        using var reader = command.ExecuteReader();
        var documentIds = new List<int>();

        while (reader.Read())
        {
            documentIds.Add(
                CheckedInt32(reader.GetInt64(0)));
        }

        return documentIds;
    }

    /// <summary>
    /// Marks a document as indexed by incrementing its indexed revision and removing it from the indexing queue.   
    /// </summary>
    /// <param name="documentId">The identifier of the document to mark as indexed.</param>
    public void MarkIndexed(int documentId)
    {
        using var connection = OpenConnection();
        using var transaction = connection.BeginTransaction();

        using (var update = connection.CreateCommand())
        {
            update.Transaction = transaction;

            update.CommandText =
                """
            UPDATE documents
            SET indexed_revision = indexed_revision + 1
            WHERE id = $document_id;
            """;

            update.Parameters.AddWithValue(
                "$document_id",
                documentId);

            update.ExecuteNonQuery();
        }

        using (var delete = connection.CreateCommand())
        {
            delete.Transaction = transaction;

            delete.CommandText =
                """
            DELETE FROM indexing_queue
            WHERE document_id = $document_id;
            """;

            delete.Parameters.AddWithValue(
                "$document_id",
                documentId);

            delete.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    /// <summary>
    /// Deletes a document from the database.
    /// </summary>
    /// <param name="documentId">The identifier of the document to delete.</param>
    public void DeleteDocument(int documentId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText =
            """
        DELETE FROM documents
        WHERE id = $document_id;
        """;

        command.Parameters.AddWithValue(
            "$document_id",
            documentId);

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Opens a new SQLite database connection with foreign keys enabled.
    /// </summary>
    /// <returns>An opened <see cref="SqliteConnection"/> instance.</returns>
    private SqliteConnection OpenConnection()
    {
        var connection =
            new SqliteConnection(_connectionString);

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();

        return connection;
    }

    /// <summary>
    /// Upserts a document in the database. 
    /// If a document with the same relative path exists, 
    /// it updates its content hash, last write time, and metadata. 
    /// Otherwise, it inserts a new document. 
    /// The method returns the ID of the upserted document.
    /// </summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="transaction">The SQLite transaction to use.</param>
    /// <param name="source">The source document to upsert.</param>
    /// <returns>The ID of the upserted document.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static int UpsertDocument(
        SqliteConnection connection,
        SqliteTransaction transaction,
        SourceDocument source)
    {
        // The relative path is the stable natural key.
        // ID remains unchanged when an existing document is updated.
        using var command = connection.CreateCommand();
        command.Transaction = transaction;

        command.CommandText =
            """
        INSERT INTO documents
        (
            relative_path,
            content_hash,
            last_write_time_utc,
            metadata_json
        )
        VALUES
        (
            $relative_path,
            $content_hash,
            $last_write_time_utc,
            $metadata_json
        )
        ON CONFLICT(relative_path) DO UPDATE SET
            content_hash        = excluded.content_hash,
            last_write_time_utc = excluded.last_write_time_utc,
            metadata_json       = excluded.metadata_json
        RETURNING id;
        """;

        command.Parameters.AddWithValue(
            "$relative_path",
            source.RelativePath);

        command.Parameters.AddWithValue(
            "$content_hash",
            source.ContentHash);

        command.Parameters.AddWithValue(
            "$last_write_time_utc",
            source.LastWriteTimeUtc.ToString("O"));

        command.Parameters.AddWithValue(
            "$metadata_json",
            ToDatabaseValue(source.Metadata));

        var result = command.ExecuteScalar()
            ?? throw new InvalidOperationException(
                "SQLite did not return a document ID.");

        return CheckedInt32(Convert.ToInt64(result));
    }

    /// <summary>
    /// Delete chuncks of a document
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="documentId"></param>
    private static void DeleteChunks(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int documentId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;

        command.CommandText =
            """
        DELETE FROM chunks
        WHERE document_id = $document_id;
        """;

        command.Parameters.AddWithValue(
            "$document_id",
            documentId);

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Insert chunks of a document
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="documentId"></param>
    /// <param name="chunks"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    private static IReadOnlyList<ChunkDocument> InsertChunks(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int documentId,
        IReadOnlyList<ChunkDocument> chunks)
    {
        using var chunkCommand = connection.CreateCommand();
        chunkCommand.Transaction = transaction;

        chunkCommand.CommandText =
            """
        INSERT INTO chunks
        (
            document_id,
            ordinal,
            text,
            metadata_json
        )
        VALUES
        (
            $document_id,
            $ordinal,
            $text,
            $metadata_json
        )
        RETURNING id;
        """;

        var documentIdParameter =
            chunkCommand.Parameters.Add(
                "$document_id",
                SqliteType.Integer);

        var ordinalParameter =
            chunkCommand.Parameters.Add(
                "$ordinal",
                SqliteType.Integer);

        var textParameter =
            chunkCommand.Parameters.Add(
                "$text",
                SqliteType.Text);

        var metadataParameter =
            chunkCommand.Parameters.Add(
                "$metadata_json",
                SqliteType.Text);

        using var entityCommand = connection.CreateCommand();
        entityCommand.Transaction = transaction;

        entityCommand.CommandText =
            """
        INSERT INTO chunk_entities
        (
            chunk_id,
            entity_id
        )
        VALUES
        (
            $chunk_id,
            $entity_id
        );
        """;

        var entityChunkIdParameter =
            entityCommand.Parameters.Add(
                "$chunk_id",
                SqliteType.Integer);

        var entityIdParameter =
            entityCommand.Parameters.Add(
                "$entity_id",
                SqliteType.Text);

        var persistedChunks =
            new List<ChunkDocument>(chunks.Count);

        for (var ordinal = 0;
             ordinal < chunks.Count;
             ordinal++)
        {
            var chunk = chunks[ordinal];

            if (chunk.DocumentId != 0 &&
                chunk.DocumentId != documentId)
            {
                throw new ArgumentException(
                    $"Chunk {chunk.Id} belongs to document " +
                    $"{chunk.DocumentId}, not document {documentId}.",
                    nameof(chunks));
            }

            documentIdParameter.Value = documentId;
            ordinalParameter.Value = ordinal;
            textParameter.Value = chunk.Text;
            metadataParameter.Value =
                ToDatabaseValue(chunk.Metadata);

            var result = chunkCommand.ExecuteScalar()
                ?? throw new InvalidOperationException(
                    "SQLite did not return a chunk ID.");

            var chunkId =
                CheckedInt32(Convert.ToInt64(result));

            foreach (var entityId in chunk.EntityIds
                         .Where(static value =>
                             !string.IsNullOrWhiteSpace(value))
                         .Distinct(StringComparer.Ordinal))
            {
                entityChunkIdParameter.Value = chunkId;
                entityIdParameter.Value = entityId;

                entityCommand.ExecuteNonQuery();
            }

            persistedChunks.Add(
                chunk with
                {
                    Id = chunkId,
                    DocumentId = documentId
                });
        }

        return persistedChunks;
    }

    /// <summary>
    /// Ajoute ou met à jour un document dans la file d'attente d'indexation.   
    /// </summary>
    /// <remarks>Si le document existe déjà dans la file d'attente, 
    /// l'horodatage de mise en file d'attente est mis à jour.</remarks>
    /// <param name="connection">Connexion SQLite à utiliser.</param>
    /// <param name="transaction">Transaction dans laquelle exécuter l'opération.</param>
    /// <param name="documentId">Identifiant du document à mettre en file d'attente.</param>
    private static void QueueIndexing(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int documentId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;

        command.CommandText =
            """
        INSERT INTO indexing_queue
        (
            document_id,
            queued_utc
        )
        VALUES
        (
            $document_id,
            $queued_utc
        )
        ON CONFLICT(document_id) DO UPDATE SET
            queued_utc = excluded.queued_utc;
        """;

        command.Parameters.AddWithValue(
            "$document_id",
            documentId);

        command.Parameters.AddWithValue(
            "$queued_utc",
            DateTime.UtcNow.ToString("O"));

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Récupère un lot de documents de bloc depuis la base de données par leurs identifiants.
    /// </summary>
    /// <param name="connection">Connexion à la base de données pour exécuter la requête.</param>
    /// <param name="ids">Collection d'identifiants de documents de bloc à récupérer.</param>
    /// <returns>Liste en lecture seule des documents de bloc correspondant aux identifiants spécifiés.</returns>
    private static IReadOnlyList<ChunkDocument> GetChunkBatch(
        SqliteConnection connection,
        IReadOnlyList<int> ids)
    {
        using var command = connection.CreateCommand();

        var parameterNames = new string[ids.Count];

        for (var index = 0; index < ids.Count; index++)
        {
            parameterNames[index] = $"$id{index}";

            command.Parameters.AddWithValue(
                parameterNames[index],
                ids[index]);
        }

        command.CommandText =
            $"""
         SELECT
             c.id,
             c.document_id,
             c.text,
             c.metadata_json,
             COALESCE(
                 json_group_array(ce.entity_id)
                     FILTER (WHERE ce.entity_id IS NOT NULL),
                 '[]'
             ) AS entity_ids
         FROM chunks AS c
         LEFT JOIN chunk_entities AS ce
             ON ce.chunk_id = c.id
         WHERE c.id IN ({string.Join(", ", parameterNames)})
         GROUP BY
             c.id,
             c.document_id,
             c.text,
             c.metadata_json;
         """;

        using var reader = command.ExecuteReader();
        var chunks = new List<ChunkDocument>(ids.Count);

        while (reader.Read())
        {
            chunks.Add(ReadChunk(reader));
        }

        return chunks;
    }

    /// <summary>
    /// Read a chunk document from a SqliteDataReader.
    /// </summary>
    /// <param name="reader">The SqliteDataReader to read from.</param>
    /// <returns>A ChunkDocument instance.</returns>
    private static ChunkDocument ReadChunk(
        SqliteDataReader reader)
    {
        var entityIds =
            JsonSerializer.Deserialize<string[]>(
                reader.GetString(4))
            ?? [];

        var metadata = reader.IsDBNull(3)
            ? null
            : JsonSerializer.Deserialize<
                Dictionary<string, string>>(
                    reader.GetString(3));

        return new ChunkDocument(
            Id: CheckedInt32(reader.GetInt64(0)),
            DocumentId: CheckedInt32(reader.GetInt64(1)),
            Text: reader.GetString(2),
            EntityIds: entityIds,
            Metadata: metadata);
    }

    /// <summary>
    /// Read a SourceDocument from a SqliteDataReader.
    /// </summary>
    /// <param name="reader">The SqliteDataReader to read from.</param>
    /// <returns>A SourceDocument instance.</returns>
    private static SourceDocument ReadSourceDocument(
        SqliteDataReader reader)
    {
        var metadata = reader.IsDBNull(4)
            ? null
            : JsonSerializer.Deserialize<
                Dictionary<string, string>>(
                    reader.GetString(4));

        return new SourceDocument(
            Id: CheckedInt32(reader.GetInt64(0)),
            RelativePath: reader.GetString(1),
            ContentHash: reader.GetString(2),
            LastWriteTimeUtc: DateTime.Parse(
                reader.GetString(3),
                provider: null,
                styles:
                    System.Globalization.DateTimeStyles
                        .RoundtripKind),
            Metadata: metadata);
    }

    /// <summary>
    /// Converts a dictionary to a database-compatible value.
    /// </summary>
    /// <param name="value">The dictionary to convert.</param>
    /// <returns><see cref="DBNull.Value"/> if <paramref name="value"/> is null; 
    /// otherwise, a JSON string representation of the
    /// dictionary.</returns>
    private static object ToDatabaseValue(
        Dictionary<string, string>? value)
    {
        return value is null
            ? DBNull.Value
            : JsonSerializer.Serialize(value);
    }

    /// <summary>
    /// Converts a long value to an int, checking for overflow.
    /// </summary>
    /// <param name="value">The long value to convert.</param>
    /// <returns>The converted int value.</returns>
    private static int CheckedInt32(long value)
    {
        // If value exceeds the range of an int, an OverflowException is thrown immediately.
        return checked((int)value);
    }
}
