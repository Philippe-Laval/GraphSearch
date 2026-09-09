using System.Collections.Concurrent;

namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Decorator that memoizes results of an inner <see cref="IEntityExtractor"/>
/// keyed by the raw query string.
///
/// <para>
/// Wrap expensive extractors (<see cref="LlmEntityExtractor"/>,
/// <see cref="NerEntityExtractor"/> using a remote NER service) so repeated
/// queries do not incur repeated network/compute cost.
/// </para>
///
/// <para>
/// The cache is unbounded. For production workloads with unbounded query
/// space, replace with an eviction-aware cache (e.g. <c>IMemoryCache</c>).
/// </para>
/// </summary>
public sealed class CachingEntityExtractor : IEntityExtractor
{
    private readonly IEntityExtractor _inner;
    private readonly ConcurrentDictionary<string, IReadOnlyList<ExtractedEntity>> _cache;

    public CachingEntityExtractor(IEntityExtractor inner, StringComparer? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(inner);
        _inner = inner;
        _cache = new ConcurrentDictionary<string, IReadOnlyList<ExtractedEntity>>(
            comparer ?? StringComparer.Ordinal);
    }

    public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        if (_cache.TryGetValue(query, out var cached))
        {
            return cached;
        }

        var extracted = await _inner.ExtractAsync(query, cancellationToken).ConfigureAwait(false);
        _cache.TryAdd(query, extracted);
        return extracted;
    }
}
