namespace GraphSearch.Library.Query.Resolution;


/*
For your GraphRAG, I think this is the most interesting implementation.
   
   Don't choose between Lucene, Chroma and SQL.
   
   Use all three:
   
                       Entity Resolver
                             │
                             ▼
                   CompositeEntityIndex
                    /         |         \
                   /          |          \
                  ▼           ▼           ▼
                SQL        Lucene      Chroma
              exact        lexical     semantic
                  \          |          /
                   \         |         /
                    └────────┼─────────┘
                             ▼
                        RRF / Fusion
                             │
                             ▼
                     Entity candidates
 */


public sealed class CompositeEntityIndex : IEntityIndex
{
    private readonly IEntityIndex _sql;
    private readonly IEntityIndex _lucene;
    private readonly IEntityIndex _chroma;

    public CompositeEntityIndex(
        IEntityIndex sql,
        IEntityIndex lucene,
        IEntityIndex chroma)
    {
        _sql = sql;
        _lucene = lucene;
        _chroma = chroma;
    }

    public async Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var results =
            await Task.WhenAll(
                _sql.SearchAsync(
                    text,
                    entityType,
                    topK,
                    cancellationToken),

                _lucene.SearchAsync(
                    text,
                    entityType,
                    topK,
                    cancellationToken),

                _chroma.SearchAsync(
                    text,
                    entityType,
                    topK,
                    cancellationToken));

        return Fuse(
            results,
            topK);
    }

    private static IReadOnlyList<EntityCandidate> Fuse(
        IReadOnlyList<EntityCandidate>[] resultSets,
        int topK)
    {
        const int rrfK = 60;

        var scores =
            new Dictionary<long, double>();

        var entities =
            new Dictionary<long, EntityCandidate>();

        foreach (var resultSet in resultSets)
        {
            for (var rank = 0;
                 rank < resultSet.Count;
                 rank++)
            {
                var candidate = resultSet[rank];

                entities[candidate.NodeId] =
                    candidate;

                var score =
                    1.0 / (rrfK + rank + 1);

                scores[candidate.NodeId] =
                    scores.GetValueOrDefault(
                        candidate.NodeId) + score;
            }
        }

        return scores
            .OrderByDescending(x => x.Value)
            .Take(topK)
            .Select(x =>
            {
                var candidate = entities[x.Key];

                return candidate with
                {
                    Score = x.Value
                };
            })
            .ToArray();
    }
}