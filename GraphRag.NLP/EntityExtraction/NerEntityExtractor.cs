using GraphRag.NLP.Models;
using GraphRag.NLP.Ner;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// Entity extractor backed by any <see cref="INerService"/> implementation.
    /// </summary>
    public sealed class NerEntityExtractor : IEntityExtractor
    {
        private readonly INerService _ner;
        private readonly HashSet<string>? _typeAllowList;

        public NerEntityExtractor(
            INerService ner,
            IEnumerable<string>? typeAllowList = null)
        {
            ArgumentNullException.ThrowIfNull(ner);
            _ner = ner;
            _typeAllowList = typeAllowList is null
                ? null
                : new HashSet<string>(typeAllowList, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            var analysis = await _ner.AnalyzeAsync(text, cancellationToken).ConfigureAwait(false);
            if (_typeAllowList is null)
            {
                return analysis.Entities;
            }

            return analysis.Entities
                .Where(e => e.Type is not null && _typeAllowList.Contains(e.Type))
                .ToList();
        }
    }
}
