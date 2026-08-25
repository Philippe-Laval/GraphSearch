namespace GraphSearch;

/*
Personalized PageRank

   Now we have:

   Query
     │
     ▼
   Vector/BM25
     │
     ▼
   Seed nodes
     │
     ▼
   Neighborhood expansion
     │
     ▼
   Subgraph

   Suppose the query retrieval gave:

   .NET       0.94
   C#         0.87
   Microsoft  0.72

   We don't want PageRank to start uniformly.

   We want these nodes to have the initial probability:

   .NET       █████████
   C#         ████████
   Microsoft  ██████

   That's what Personalized PageRank does.
 */

public sealed class PersonalizedPageRank
{
    private readonly Graph _graph;

    public PersonalizedPageRank(Graph graph)
    {
        _graph = graph;
    }

    public IReadOnlyList<PageRankResult> Rank(
        IReadOnlyCollection<long> nodeIds,
        IReadOnlyDictionary<long, double> personalization,
        int iterations = 30,
        double dampingFactor = 0.85)
    {
        if (nodeIds.Count == 0)
            return [];

        if (iterations <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(iterations));

        if (dampingFactor <= 0 || dampingFactor >= 1)
            throw new ArgumentOutOfRangeException(
                nameof(dampingFactor));

        var nodes = nodeIds.ToHashSet();

        var teleport = NormalizePersonalization(
            nodes,
            personalization);

        var rank = teleport.ToDictionary(
            x => x.Key,
            x => x.Value);

        for (var iteration = 0;
             iteration < iterations;
             iteration++)
        {
            var next = nodes.ToDictionary(
                nodeId => nodeId,
                _ => 0.0);

            double danglingMass = 0;

            foreach (var nodeId in nodes)
            {
                var currentRank = rank[nodeId];

                var outgoing = _graph
                    .GetOutgoingEdges(nodeId)
                    .Where(e => nodes.Contains(e.TargetNodeId))
                    .Where(e => e.Weight > 0)
                    .ToArray();

                if (outgoing.Length == 0)
                {
                    danglingMass += currentRank;
                    continue;
                }

                var totalWeight =
                    outgoing.Sum(e => e.Weight);

                foreach (var edge in outgoing)
                {
                    var transitionProbability =
                        edge.Weight / totalWeight;

                    next[edge.TargetNodeId] +=
                        dampingFactor *
                        currentRank *
                        transitionProbability;
                }
            }

            // Redistribute dangling nodes.
            if (danglingMass > 0)
            {
                foreach (var nodeId in nodes)
                {
                    next[nodeId] +=
                        dampingFactor *
                        danglingMass *
                        teleport[nodeId];
                }
            }

            // Teleport back toward the query seeds.
            foreach (var nodeId in nodes)
            {
                next[nodeId] +=
                    (1.0 - dampingFactor) *
                    teleport[nodeId];
            }

            rank = next;
        }

        return rank
            .Select(x =>
                new PageRankResult(
                    x.Key,
                    x.Value))
            .OrderByDescending(x => x.Score)
            .ToArray();
    }

    private static Dictionary<long, double>
        NormalizePersonalization(
            HashSet<long> nodes,
            IReadOnlyDictionary<long, double> personalization)
    {
        var result = nodes.ToDictionary(
            nodeId => nodeId,
            _ => 0.0);

        foreach (var pair in personalization)
        {
            if (!nodes.Contains(pair.Key))
                continue;

            if (pair.Value > 0)
                result[pair.Key] = pair.Value;
        }

        var total = result.Values.Sum();

        if (total <= 0)
        {
            var uniform = 1.0 / nodes.Count;

            foreach (var nodeId in nodes)
                result[nodeId] = uniform;

            return result;
        }

        foreach (var nodeId in nodes)
            result[nodeId] /= total;

        return result;
    }
}