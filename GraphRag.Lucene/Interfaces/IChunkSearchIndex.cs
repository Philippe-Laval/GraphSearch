using GraphRag.Lucene.Models;
using Lucene.Net.QueryParsers.Classic;

namespace GraphRag.Lucene.Interfaces
{
    public interface IChunkSearchIndex : IDisposable
    {
        /// <summary>
        /// Delete the Lucene chunk documents associated with the specified document ID from the index.
        /// </summary>
        /// <param name="documentId">The ID of the document to delete.</param>
        void DeleteDocument(int documentId);

        /// <summary>
        /// Replaces the Lucene documents associated with the specified document ID with the provided chunks.
        /// </summary>
        /// <param name="documentId">The ID of the document to replace.</param>
        /// <param name="chunks">The chunks to index for the document.</param>
        void ReplaceDocument(int documentId, IReadOnlyList<ChunkDocument> chunks);

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
        IReadOnlyList<ChunkSearchHit> Search(string searchText, 
            int maximumResults = 20, 
            string? entityId = null,
            Operator defaultOperator = Operator.OR);
    }
}