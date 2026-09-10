using GraphSearch.Library.Query.Analysis;

namespace GraphSearch.Library.Query.Resolution;

/*
The extractor found:
".NET 10"

The resolver needs to find:
GraphNode
Id = 1842
Type = Technology
Name = ".NET 10" 
 */

public interface IEntityResolver
{
    Task<IReadOnlyList<ResolvedEntity>> ResolveAsync(
        IReadOnlyList<ExtractedEntity> entities,
        CancellationToken cancellationToken = default);
}