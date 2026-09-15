using GraphRag.Lucene.Models;

namespace GraphRag.Lucene.Interfaces
{
    public interface IChunkRepository
    {
        /// <summary>
        /// Initialise le schéma de base de données SQLite avec les tables, 
        /// index et contraintes nécessaires pour le stockage de documents 
        /// et l'indexation.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Deletes a document from the database.
        /// </summary>
        /// <param name="documentId">The identifier of the document to delete.</param>
        void DeleteDocument(int documentId);

        /// <summary>
        /// Finds a document in the database by its relative path.
        /// </summary>
        /// <param name="relativePath">The relative path of the document.</param>
        /// <returns>The document if found; otherwise, <c>null</c>.</returns>
        SourceDocument? FindDocumentByPath(string relativePath);

        /// <summary>
        /// Gets a list of chunks by their IDs. 
        /// This method retrieves the chunks from the database based on the provided chunk IDs, 
        /// ensuring that each chunk is only retrieved once even if duplicate IDs are provided. 
        /// The chunks are returned in the order they are found in the database.
        /// </summary>
        /// <param name="chunkIds">The IDs of the chunks to retrieve.</param>
        /// <returns>The list of chunks corresponding to the provided IDs.</returns>
        IReadOnlyList<ChunkDocument> GetChunks(IEnumerable<int> chunkIds);

        /// <summary>
        /// Retrieves the chunks associated with a specific document.
        /// </summary>
        /// <param name="documentId">The ID of the document.</param>
        /// <returns>The list of chunks associated with the document.</returns>
        IReadOnlyList<ChunkDocument> GetDocumentChunks(int documentId);

        /// <summary>
        /// Get a list of document IDs that are currently queued for indexing.
        /// </summary>
        /// <param name="limit">The maximum number of document IDs to retrieve.</param>
        /// <returns>The list of document IDs currently queued for indexing.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the limit is less than or equal to zero.</exception>
        IReadOnlyList<int> GetQueuedDocumentIds(int limit = 100);

        /// <summary>
        /// Determines whether a document with the specified relative path and content hash exists in the database. 
        /// </summary>
        /// <param name="relativePath">The relative path of the document.</param>
        /// <param name="contentHash">The content hash of the document.</param>
        /// <returns><see langword="true"/> if a document with the specified relative path and content hash exists; otherwise, <see
        /// langword="false"/>.</returns>
        bool IsCurrent(string relativePath, string contentHash);

        /// <summary>
        /// Marks a document as indexed by incrementing its indexed revision and removing it from the indexing queue.   
        /// </summary>
        /// <param name="documentId">The identifier of the document to mark as indexed.</param>
        void MarkIndexed(int documentId);
        
        /// <summary>
        /// Replaces the content of a document with new chunks.
        /// </summary>
        /// <param name="source">The source document to replace.</param>
        /// <param name="chunks">The new chunks to associate with the document.</param>
        /// <returns>The persisted document after replacement.</returns>
        PersistedDocument ReplaceDocument(SourceDocument source, IReadOnlyList<ChunkDocument> chunks);
    }
}