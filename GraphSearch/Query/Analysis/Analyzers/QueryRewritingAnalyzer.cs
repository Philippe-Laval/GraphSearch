namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Decorator that runs an inner analyzer, then augments the result with
/// alternative phrasings from an <see cref="IQueryRewriter"/>.
///
/// Rewrites are useful for hybrid retrieval — issue the original + rewrites
/// against BM25 / vector indexes and fuse results.
/// </summary>
public sealed class QueryRewritingAnalyzer : IQueryAnalyzer
{
    private readonly IQueryAnalyzer _inner;
    private readonly IQueryRewriter _rewriter;

    public QueryRewritingAnalyzer(IQueryAnalyzer inner, IQueryRewriter rewriter)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(rewriter);
        _inner = inner;
        _rewriter = rewriter;
    }

    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var analysis = await _inner.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);
        var rewrites = await _rewriter.RewriteAsync(analysis.NormalizedQuery, cancellationToken).ConfigureAwait(false);

        if (rewrites.Count == 0)
        {
            return analysis;
        }

        // Merge with any rewrites the inner analyzer already produced.
        var merged = new List<string>(analysis.Rewrites.Count + rewrites.Count);
        merged.AddRange(analysis.Rewrites);
        foreach (var r in rewrites)
        {
            if (!merged.Contains(r, StringComparer.OrdinalIgnoreCase))
            {
                merged.Add(r);
            }
        }

        return analysis with { Rewrites = merged };
    }
}
