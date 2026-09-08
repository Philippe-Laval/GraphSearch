using System.Collections.Concurrent;

namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// Decorator that caches analyzed queries keyed by the raw input string.
/// Useful when the inner analyzer is expensive (LLM, remote NER, embedding model).
/// </summary>
/// <remarks>
/// The cache is unbounded. For production workloads with unbounded query space,
/// wrap this with an eviction policy (e.g. <c>IMemoryCache</c>) instead.
/// </remarks>
public sealed class CachingQueryAnalyzer : IQueryAnalyzer
{
    private readonly IQueryAnalyzer _inner;
    private readonly ConcurrentDictionary<string, AnalyzedQuery> _cache;

    public CachingQueryAnalyzer(IQueryAnalyzer inner, StringComparer? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(inner);
        _inner = inner;
        _cache = new ConcurrentDictionary<string, AnalyzedQuery>(comparer ?? StringComparer.Ordinal);
    }

    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        if (_cache.TryGetValue(query, out var cached))
        {
            return cached;
        }

        var analyzed = await _inner.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);
        _cache.TryAdd(query, analyzed);
        return analyzed;
    }
}
