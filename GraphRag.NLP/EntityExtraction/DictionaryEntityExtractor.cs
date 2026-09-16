using GraphRag.NLP.Models;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// Extracts entities from text based on a predefined dictionary of entities.
    /// </summary>
    public sealed class DictionaryEntityExtractor : IEntityExtractor
    {
        private readonly IReadOnlyList<EntityDefinition> _entities;

        public DictionaryEntityExtractor(IEnumerable<EntityDefinition> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            _entities = entities
                .OrderByDescending(x => x.Name.Length)
                .ToArray();
        }

        public Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);
            cancellationToken.ThrowIfCancellationRequested();

            var results = new List<ExtractedEntity>();

            foreach (var entity in _entities)
            {
                var start = 0;

                while (true)
                {
                    var index = text.IndexOf(
                        entity.Name,
                        start,
                        StringComparison.OrdinalIgnoreCase);

                    if (index < 0)
                    {
                        break;
                    }

                    var result = new ExtractedEntity(
                        entity.Name,
                        entity.Type,
                        index,
                        entity.Name.Length,
                        1.0);

                    if (!Overlaps(result, results))
                    {
                        results.Add(result);
                    }

                    start = index + entity.Name.Length;
                }
            }

            return Task.FromResult<IReadOnlyList<ExtractedEntity>>(results);
        }

        private static bool Overlaps(
            ExtractedEntity candidate,
            IEnumerable<ExtractedEntity> existing)
        {
            var start = candidate.Start;
            var end = start + candidate.Length;

            return existing.Any(entity =>
            {
                var existingStart = entity.Start;
                var existingEnd = existingStart + entity.Length;

                return start < existingEnd && end > existingStart;
            });
        }
    }
}
