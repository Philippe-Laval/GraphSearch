using GraphSearch.Query.Analysis;

namespace GraphSearch.Query.Resolution;

public interface IEntityResolver
{
    Task<IReadOnlyList<ResolvedEntity>> ResolveAsync(
        IReadOnlyList<ExtractedEntity> entities,
        CancellationToken cancellationToken = default);
}