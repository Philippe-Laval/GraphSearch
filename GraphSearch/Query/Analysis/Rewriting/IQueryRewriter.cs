namespace GraphSearch.Library.Query.Analysis.Rewriting;

/// <summary>
/// Produces alternative phrasings for a query to feed hybrid retrieval.
///
/// Examples:
///   "Microsoft's tech"          -> "technologies used by Microsoft"
///   "who made .NET?"            -> "who developed .NET"
/// </summary>
public interface IQueryRewriter
{
    /// <summary>
    /// Produces alternative phrasings for a query to feed hybrid retrieval.
    /// </summary>
    /// <param name="normalizedQuery"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<string>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default);
}
