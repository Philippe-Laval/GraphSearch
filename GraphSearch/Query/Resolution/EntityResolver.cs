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

/*
Entity resolution should be hybrid
   
   For a real GraphRAG system, I wouldn't use only semantic similarity here.
   
   I'd do:
   
   ".NET 10"
        │
        ├── Exact match
        │
        ├── Alias match
        │
        ├── BM25
        │
        └── Vector similarity
                │
                ▼
          Candidate entities
                │
                ▼
          Entity reranking
   
   For example:
   
   .NET 10
   │
   ├── exact name          1.00
   ├── alias               0.95
   ├── BM25                0.91
   └── embedding           0.88
   
   Then:
   
   finalScore =
       0.40 * exactScore +
       0.25 * aliasScore +
       0.20 * bm25Score +
       0.15 * vectorScore;
   
   This is especially important for entity resolution because 
   semantic similarity alone can produce dangerous false matches.
 */

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