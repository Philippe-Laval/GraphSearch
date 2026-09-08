namespace GraphSearch.Query.Analysis;

public interface IQueryAnalyzer
{
    /// <summary>
    /// Normalizes the given query string by removing unnecessary whitespace, punctuation, and other non-essential characters.
    /// </summary>
    /// <param name="query">The query string to normalize.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the normalized query string.</returns>
    Task<string> NormalizeAsync(
        string query,
        CancellationToken cancellationToken = default);
}