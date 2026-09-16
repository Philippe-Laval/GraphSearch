using System.Collections.Concurrent;
using GraphRag.NLP.Models;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// Decorator that memoizes results of an inner <see cref="IEntityExtractor"/>
    /// keyed by the raw text.
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
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            if (_cache.TryGetValue(text, out var cached))
            {
                return cached;
            }

            var extracted = await _inner.ExtractAsync(text, cancellationToken).ConfigureAwait(false);
            _cache.TryAdd(text, extracted);
            return extracted;
        }
    }
}
