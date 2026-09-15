using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.BM25.Interfaces;

public interface IRetriever
{
    /// <summary>
    /// Retrieve the top K search results for the given query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="topK">The number of top results to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of search results.</returns>
    Task<IReadOnlyList<SearchResult>> RetrieveAsync(
        string query,
        int topK,
        CancellationToken cancellationToken = default);
}