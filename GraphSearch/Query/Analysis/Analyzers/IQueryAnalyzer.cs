namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Analyzes a raw user query and produces a normalized form together
/// with an inferred <see cref="QueryIntent"/>.
/// </summary>
/// <remarks>
/// Implementations may be rules-based, ML-based (NER / classifier),
/// LLM-based, or hybrid. The contract makes no assumption about how the
/// intent is inferred - only that the returned <see cref="AnalyzedQuery"/>
/// carries the original text, a normalized form, and an intent (possibly
/// <see cref="QueryIntent.Unknown"/>).
/// </remarks>
public interface IQueryAnalyzer
{
    /// <summary>
    /// Analyzes the given query, producing a normalized form and inferring intent.
    /// </summary>
    /// <param name="query">The raw query string submitted by the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="AnalyzedQuery"/> describing the query.</returns>
    Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Normalizes the given query string. Convenience wrapper around
    /// <see cref="AnalyzeAsync"/> that only returns the normalized text.
    /// </summary>
    /// <param name="query">The query string to normalize.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The normalized query string.</returns>
    async Task<string> NormalizeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var analyzed = await AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);
        return analyzed.NormalizedQuery;
    }
}