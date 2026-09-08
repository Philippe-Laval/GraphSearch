namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Pass-through analyzer: returns the input as-is with <see cref="QueryIntent.Unknown"/>.
/// Useful for tests, benchmarks, and pipelines that want to disable analysis without
/// changing wiring.
/// </summary>
public sealed class NoOpQueryAnalyzer : IQueryAnalyzer
{
    public Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new AnalyzedQuery(
            OriginalQuery: query,
            NormalizedQuery: query,
            Intent: QueryIntent.Unknown));
    }
}
