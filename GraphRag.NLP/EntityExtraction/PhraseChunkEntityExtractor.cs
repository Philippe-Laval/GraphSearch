using GraphRag.NLP.Chunking;
using GraphRag.NLP.Models;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// Emits raw phrase candidates using an <see cref="IPhraseChunkService"/>.
    /// Useful as a recall booster in a hybrid extraction pipeline.
    /// </summary>
    public sealed class PhraseChunkEntityExtractor : IEntityExtractor
    {
        private readonly IPhraseChunkService _chunker;
        private readonly string _defaultType;
        private readonly double _confidence;
        private readonly int _minLength;

        public PhraseChunkEntityExtractor(
            IPhraseChunkService chunker,
            string defaultType = "NounPhrase",
            double confidence = 0.5,
            int minLength = 3)
        {
            ArgumentNullException.ThrowIfNull(chunker);
            ArgumentException.ThrowIfNullOrWhiteSpace(defaultType);
            _chunker = chunker;
            _defaultType = defaultType;
            _confidence = confidence;
            _minLength = minLength;
        }

        public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            var chunks = await _chunker.ExtractChunksAsync(text, cancellationToken).ConfigureAwait(false);
            if (chunks.Count == 0)
            {
                return Array.Empty<ExtractedEntity>();
            }

            var accumulator = new List<(ExtractedEntity Entity, int Priority)>();
            foreach (var chunk in chunks)
            {
                if (chunk.Length < _minLength)
                {
                    continue;
                }

                var entity = new ExtractedEntity(
                    Text: chunk.Text,
                    Type: _defaultType,
                    Start: chunk.Start,
                    Length: chunk.Length,
                    Confidence: _confidence);

                EntitySpanMerger.AddOrReplace(accumulator, entity, priority: 0);
            }

            return EntitySpanMerger.Finalize(accumulator);
        }
    }
}
