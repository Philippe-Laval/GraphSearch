namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

public sealed class DictionaryEntityExtractor : IEntityExtractor
{
    private readonly IReadOnlyList<EntityDefinition> _entities;

    public DictionaryEntityExtractor(
        IEnumerable<EntityDefinition> entities)
    {
        // Given: .NET 10
        // it could extract both:
        // .NET
        // .NET 10
        // 
        // We want the longest match.
        // So sort entities by length:
        
        _entities = entities
            .OrderByDescending(x => x.Name.Length)
            .ToArray();
    }

    /// <summary>
    /// Extracts entities from the given query string based on the predefined dictionary of entities.
    /// </summary>
    /// <param name="query">The query string to extract entities from.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of extracted entities.</returns>
    public Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ExtractedEntity>();

        foreach (var entity in _entities)
        {
            var start = 0;

            while (true)
            {
                var index = query.IndexOf(
                    entity.Name,
                    start,
                    StringComparison.OrdinalIgnoreCase);

                if (index < 0)
                    break;

                var result = new ExtractedEntity(
                    entity.Name,
                    entity.Type,
                    index,
                    entity.Name.Length,
                    1.0);
                
                // Prevents overlapping matches
                if (!Overlaps(result, results))
                    results.Add(result);

                start = index + entity.Name.Length;
            }
        }

        return Task.FromResult<
            IReadOnlyList<ExtractedEntity>>(results);
    }
    
    /// <summary>
    /// Check if candidate overlaps with any existing entity.
    /// </summary>
    /// <param name="candidate"></param>
    /// <param name="existing"></param>
    /// <returns></returns>
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

            return start < existingEnd &&
                   end > existingStart;
        });
    }
}