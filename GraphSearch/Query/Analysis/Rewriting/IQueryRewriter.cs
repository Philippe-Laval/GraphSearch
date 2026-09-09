namespace GraphSearch.Library.Query.Analysis.Rewriting;

public sealed record QueryRewrite(
    string Text,
    RewriteKind Kind,      // Synonym, Template, LlmParaphrase, HyDE, EntityAlias, ...
    double Weight = 1.0,   // downstream fusion weight
    string? Language = null);

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
    Task<IReadOnlyList<QueryRewrite>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default);
}
