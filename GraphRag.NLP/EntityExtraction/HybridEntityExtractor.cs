using GraphRag.NLP.Models;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// Composite extractor that runs several <see cref="IEntityExtractor"/> instances in priority order
    /// and merges their results.
    /// </summary>
    public sealed class HybridEntityExtractor : IEntityExtractor
    {
        private readonly IReadOnlyList<(IEntityExtractor Extractor, int Priority)> _tiers;
        private readonly bool _continueOnException;
        private readonly Action<Exception, IEntityExtractor>? _onException;

        public HybridEntityExtractor(
            IEnumerable<IEntityExtractor> extractors,
            bool continueOnException = true,
            Action<Exception, IEntityExtractor>? onException = null)
        {
            ArgumentNullException.ThrowIfNull(extractors);
            var list = extractors.ToList();
            if (list.Count == 0)
            {
                throw new ArgumentException("At least one extractor is required.", nameof(extractors));
            }

            _tiers = list
                .Select((extractor, index) => (extractor, priority: list.Count - index))
                .ToList();

            _continueOnException = continueOnException;
            _onException = onException;
        }

        public HybridEntityExtractor(params IEntityExtractor[] extractors)
            : this((IEnumerable<IEntityExtractor>)extractors)
        {
        }

        public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

            foreach (var (extractor, priority) in _tiers)
            {
                IReadOnlyList<ExtractedEntity> partial;
                try
                {
                    partial = await extractor
                        .ExtractAsync(text, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex) when (_continueOnException)
                {
                    _onException?.Invoke(ex, extractor);
                    continue;
                }

                foreach (var entity in partial)
                {
                    EntitySpanMerger.AddOrReplace(accumulator, entity, priority);
                }
            }

            return EntitySpanMerger.Finalize(accumulator);
        }
    }
}
