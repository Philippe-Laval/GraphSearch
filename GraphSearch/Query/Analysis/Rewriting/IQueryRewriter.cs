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
    Task<IReadOnlyList<string>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default);
}
