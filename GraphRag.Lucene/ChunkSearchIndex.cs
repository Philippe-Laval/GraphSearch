using GraphRag.Lucene.Interfaces;
using GraphRag.Lucene.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Globalization;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace GraphRag.Lucene;

// Lucene documents consist of named fields;
// IndexWriter writes them and IndexSearcher searches them.

/// <summary>
/// Lucene-based search index for chunked documents, 
/// allowing for efficient searching and retrieval of 
/// document chunks based on text content and associated metadata.
/// </summary>
public sealed class ChunkSearchIndex : IChunkSearchIndex
{
    private static readonly LuceneVersion Version =
        LuceneVersion.LUCENE_48;

    private readonly LuceneDirectory _directory;
    private readonly StandardAnalyzer _analyzer;
    /// <summary>
    /// Lucene index writer used for adding, updating, and deleting documents in the index.
    /// </summary>
    private readonly IndexWriter _writer;

    /// <summary>
    /// Creates a new instance of the ChunkSearchIndex class, 
    /// initializing the Lucene index at the specified path.
    /// </summary>
    /// <param name="indexPath">The path where the Lucene index will be stored.</param>
    public ChunkSearchIndex(string indexPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(indexPath);

        var directoryInfo =
            new DirectoryInfo(Path.GetFullPath(indexPath));

        directoryInfo.Create();

        _directory = FSDirectory.Open(directoryInfo);
        _analyzer = new StandardAnalyzer(Version);

        var configuration =
            new IndexWriterConfig(Version, _analyzer)
            {
                OpenMode = OpenMode.CREATE_OR_APPEND
            };

        _writer =
            new IndexWriter(_directory, configuration);
    }

    /// <summary>
    /// Replaces the Lucene documents associated with the specified document ID with the provided chunks.
    /// </summary>
    /// <param name="documentId">The ID of the document to replace.</param>
    /// <param name="chunks">The chunks to index for the document.</param>
    public void ReplaceDocument(
        int documentId,
        IReadOnlyList<ChunkDocument> chunks)
    {
        ArgumentNullException.ThrowIfNull(chunks);

        if (documentId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(documentId));
        }

        var documentIdText = FormatId(documentId);

        // Delete existing chunk documents for the specified document ID
        _writer.DeleteDocuments(
            new Term(
                LuceneFields.DocumentId,
                documentIdText));

        foreach (var chunk in chunks)
        {
            if (chunk.Id <= 0)
            {
                throw new InvalidOperationException(
                    "A chunk must be persisted in SQLite " +
                    "before it is indexed in Lucene.");
            }

            if (chunk.DocumentId != documentId)
            {
                throw new InvalidOperationException(
                    $"Chunk {chunk.Id} belongs to document " +
                    $"{chunk.DocumentId}, not {documentId}.");
            }

            // Add the new chunk document to the index
            _writer.AddDocument(
                ToLuceneDocument(chunk));
        }

        _writer.Commit();
    }

    /// <summary>
    /// Searches the Lucene index for chunks matching the specified search text,
    /// optionally filtering by entity ID and specifying the default operator for the query.
    /// </summary>
    /// <param name="searchText">The text to search for.</param>
    /// <param name="maximumResults">The maximum number of results to return.</param>
    /// <param name="entityId">An optional entity ID to filter the results.</param>
    /// <param name="defaultOperator">The default operator to use for the query.</param>
    /// <returns>A list of search hits matching the query.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IReadOnlyList<ChunkSearchHit> Search(
        string searchText,
        int maximumResults = 20,
        string? entityId = null,
        Operator defaultOperator = Operator.OR)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);

        if (maximumResults is <= 0 or > 1_000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumResults));
        }

        // If the index does not exist, return an empty list
        if (!DirectoryReader.IndexExists(_directory))
        {
            return [];
        }

        using var reader = DirectoryReader.Open(_directory);

        // Create an IndexSearcher to perform the search
        var searcher = new IndexSearcher(reader);

        var parser = new QueryParser(
            Version,
            LuceneFields.Text,
            _analyzer)
        {
            DefaultOperator = defaultOperator
        };

        // Parse the search text into a Lucene query, escaping special characters
        var textQuery = parser.Parse(
            QueryParserBase.Escape(searchText));

        Query finalQuery = textQuery;

        // If an entity ID is provided, combine the text query with a term query for the entity ID
        if (!string.IsNullOrWhiteSpace(entityId))
        {
            finalQuery = new BooleanQuery
            {
                {
                    textQuery,
                    Occur.MUST
                },
                {
                    new TermQuery(
                        new Term(
                            LuceneFields.EntityId,
                            entityId)),
                    Occur.MUST
                }
            };
        }

        var topDocuments =
            searcher.Search(finalQuery, maximumResults);

        var results = new List<ChunkSearchHit>(
            topDocuments.ScoreDocs.Length);

        foreach (var scoreDocument
                 in topDocuments.ScoreDocs)
        {
            var document = searcher.Doc(scoreDocument.Doc);

            results.Add(
                new ChunkSearchHit(
                    ChunkId: ParseId(
                        document.Get(
                            LuceneFields.ChunkId)),
                    DocumentId: ParseId(
                        document.Get(
                            LuceneFields.DocumentId)),
                    Score: scoreDocument.Score));
        }

        return results;
    }

    /// <summary>
    /// Delete the Lucene chunk documents associated with the specified document ID from the index.
    /// </summary>
    /// <param name="documentId">The ID of the document to delete.</param>
    public void DeleteDocument(int documentId)
    {
        _writer.DeleteDocuments(
            new Term(
                LuceneFields.DocumentId,
                FormatId(documentId)));

        _writer.Commit();
    }

    public void Dispose()
    {
        _writer.Dispose();
        _analyzer.Dispose();
        _directory.Dispose();
    }

    /// <summary>
    /// Creates a Lucene document from a ChunkDocument,
    /// mapping its properties to the appropriate Lucene fields.
    /// </summary>
    /// <param name="chunk">The ChunkDocument to convert.</param>
    /// <returns>A Lucene Document representing the ChunkDocument.</returns>
    private static Document ToLuceneDocument(
        ChunkDocument chunk)
    {
        var document = new Document
        {
            new StringField(
                LuceneFields.ChunkId,
                FormatId(chunk.Id),
                Field.Store.YES),

            new StringField(
                LuceneFields.DocumentId,
                FormatId(chunk.DocumentId),
                Field.Store.YES),

            new TextField(
                LuceneFields.Text,
                chunk.Text,
                Field.Store.NO)
        };

        foreach (var entityId in chunk.EntityIds
                     .Where(static value =>
                         !string.IsNullOrWhiteSpace(value))
                     .Distinct(StringComparer.Ordinal))
        {
            document.Add(
                new StringField(
                    LuceneFields.EntityId,
                    entityId,
                    Field.Store.NO));
        }

        if (chunk.Metadata is not null)
        {
            foreach (var (key, value) in chunk.Metadata)
            {
                document.Add(
                    new StringField(
                        LuceneFields.MetadataPrefix + NormalizeMetadataKey(key),
                        value,
                        Field.Store.NO));
            }
        }

        return document;
    }

    /// <summary>
    /// Formate un identifiant entier en chaîne invariante de culture.
    /// </summary>
    /// <param name="id">L'identifiant à formater.</param>
    /// <returns>La représentation sous forme de chaîne de l'identifiant.</returns>
    private static string FormatId(int id)
    {
        return id.ToString(
            CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parse une chaîne en un identifiant entier en utilisant la culture invariante.
    /// </summary>
    /// <param name="value">La chaîne à analyser.</param>
    /// <returns>L'identifiant entier correspondant.</returns>
    private static int ParseId(string value)
    {
        return int.Parse(
            value,
            NumberStyles.None,
            CultureInfo.InvariantCulture);
    }

    private static string NormalizeMetadataKey(
        string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var normalized = new string(
            key.ToLowerInvariant()
                .Select(character =>
                    char.IsAsciiLetterOrDigit(character)
                        ? character
                        : '_')
                .ToArray());

        return normalized;
    }
}
