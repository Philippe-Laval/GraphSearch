using GraphSearch.Library.Query.Analysis;

namespace GraphSearch.Library.Query.Resolution;

public interface IEntityResolver
{
    Task<IReadOnlyList<ResolvedEntity>> ResolveAsync(
        IReadOnlyList<ExtractedEntity> entities,
        CancellationToken cancellationToken = default);
}