using System.Collections.Concurrent;

namespace GraphSearch.Library.Query.Analysis.Rewriting;

/// <summary>
/// Decorator that memoizes results of an inner <see cref="IQueryRewriter"/>
/// keyed by the normalized query.
///
/// <para>
/// Wrap expensive rewriters (<see cref="LlmQueryRewriter"/>,
/// <see cref="HydeQueryRewriter"/>) so repeated queries do not incur repeated
/// network/compute cost. Because <see cref="QueryRewrite"/> is immutable, it is
/// safe to hand out cached instances directly.
/// </para>
///
/// <para>
/// The cache is unbounded. For production workloads with unbounded query
/// space, replace with an eviction-aware cache (e.g. <c>IMemoryCache</c>) or
/// pass a custom <see cref="StringComparer"/> to normalize case/whitespace
/// at the cache key level.
/// </para>
/// </summary>
public sealed class CachingQueryRewriter : IQueryRewriter
{
    private readonly IQueryRewriter _inner;
    private readonly ConcurrentDictionary<string, IReadOnlyList<QueryRewrite>> _cache;

    public CachingQueryRewriter(IQueryRewriter inner, StringComparer? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(inner);
        _inner = inner;
        _cache = new ConcurrentDictionary<string, IReadOnlyList<QueryRewrite>>(
            comparer ?? StringComparer.Ordinal);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<QueryRewrite>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedQuery);

        // Check the cache first. If the normalized query has been rewritten before, return the cached result.
        if (_cache.TryGetValue(normalizedQuery, out var cached))
        {
            return cached;
        }

        // Otherwise, call the inner rewriter and cache the result.
        var rewrites = await _inner
            .RewriteAsync(normalizedQuery, cancellationToken)
            .ConfigureAwait(false);

        // Cache the result for future calls.
        _cache.TryAdd(normalizedQuery, rewrites);

        return rewrites;
    }
}
