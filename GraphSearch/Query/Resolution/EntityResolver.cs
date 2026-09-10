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

// The resolver shouldn't directly depend on your graph database
// This is deliberately independent of Lucene, ChromaDB, SQL, Neo4j, etc.
// You can implement IEntityIndex using whatever backend you eventually choose.

public sealed class EntityResolver : IEntityResolver
{
    private readonly IEntityIndex _index;

    public EntityResolver(IEntityIndex index)
    {
        _index = index;
    }

    public async Task<IReadOnlyList<ResolvedEntity>> ResolveAsync(
        IReadOnlyList<ExtractedEntity> entities,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ResolvedEntity>();

        foreach (var entity in entities)
        {
            var candidates =
                await _index.SearchAsync(
                    entity.Text,
                    entity.Type,
                    topK: 5,
                    cancellationToken);

            var best = candidates
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            if (best is null)
                continue;

            var confidence = entity.Confidence * best.Score;

            results.Add(
                new ResolvedEntity(
                    entity,
                    best.NodeId,
                    confidence));
        }

        return results;
    }
}