using GraphRag.BM25.Interfaces;
using GraphRag.Core.Models;

namespace GraphRag.BM25.Retrievers;

public class GraphRetriever : IRetriever
{
    /// <inheritdoc/>
    public Task<IReadOnlyList<SearchResult>> RetrieveAsync(string query, int topK, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<SearchResult>>(new List<SearchResult>());
    }
}
