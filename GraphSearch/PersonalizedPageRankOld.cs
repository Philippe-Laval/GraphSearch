using GraphSearch.Library.Graphs;

namespace GraphSearch.Library;

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

public sealed class PersonalizedPageRankOld
{
    private readonly Graph _graph;

    public PersonalizedPageRankOld(Graph graph)
    {
        _graph = graph;
    }

    public IReadOnlyList<PageRankResult> Rank(
        IReadOnlyCollection<long> nodeIds,
        IReadOnlyDictionary<long, double> personalization,
        int iterations = 20,
        double dampingFactor = 0.85)
    { 
        /*
There is one subtle issue here:
the personalization dictionary must be normalized before the iterations.
We should therefore normalize it once and use that normalized distribution.
*/
        
        if (nodeIds.Count == 0)
            return [];

        if (iterations <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(iterations));

        if (dampingFactor is < 0 or >= 1)
            throw new ArgumentOutOfRangeException(
                nameof(dampingFactor));

        var nodes = nodeIds.ToHashSet();

        var rank = CreateInitialRank(
            nodes,
            personalization);

        for (var i = 0; i < iterations; i++)
        {
            rank = Iterate(
                nodes,
                rank,
                personalization,
                dampingFactor);
        }

        return rank
            .Select(x => new PageRankResult(
                x.Key,
                x.Value))
            .OrderByDescending(x => x.Score)
            .ToArray();
    }

    /// <summary>
    /// Creates the initial rank distribution based on the provided personalization vector.
    /// </summary>
    /// <param name="nodes">Set of node IDs to include in the result.</param>
    /// <param name="personalization">Personalization vector mapping node IDs to their initial probabilities.</param>
    /// <returns>Dictionary containing all nodes with non-negative weights summing to 1.</returns>
    private static Dictionary<long, double> CreateInitialRank(
        HashSet<long> nodes,
        IReadOnlyDictionary<long, double> personalization)
    {
        var result = new Dictionary<long, double>();

        double total = 0;

        foreach (var nodeId in nodes)
        {
            personalization.TryGetValue(
                nodeId,
                out var value);

            result[nodeId] = value;
            total += value;
        }

        // No personalization supplied:
        // use uniform distribution.
        if (total <= 0)
        {
            var uniform = 1.0 / nodes.Count;

            foreach (var nodeId in nodes)
                result[nodeId] = uniform;

            return result;
        }

        // Normalize personalization.
        foreach (var nodeId in nodes)
            result[nodeId] /= total;

        return result;
    }

    /// <summary>
    /// Effectue une itération de l'algorithme de Personalized PageRank.
    /// </summary>
    /// <param name="nodes">Ensemble des identifiants de nœuds à inclure dans le calcul.</param>
    /// <param name="rank">Dictionnaire contenant les scores de PageRank actuels par identifiant de nœud.</param>
    /// <param name="personalization">Vecteur de personnalisation mapping des identifiants de nœuds à leurs probabilités initiales.</param>
    /// <param name="dampingFactor">Facteur d'amortissement pour l'algorithme de PageRank.</param>
    /// <returns>Dictionnaire contenant les scores de PageRank mis à jour après l'itération.</returns>
    private Dictionary<long, double> Iterate(
        HashSet<long> nodes,
        Dictionary<long, double> rank,
        IReadOnlyDictionary<long, double> personalization,
        double dampingFactor)
    {
        var next = nodes.ToDictionary(
            nodeId => nodeId,
            _ => 0.0);

        foreach (var nodeId in nodes)
        {
            var currentRank = rank[nodeId];

            var outgoing = _graph
                .GetOutgoingEdges(nodeId)
                .Where(e => nodes.Contains(e.TargetNodeId))
                .ToArray();

            if (outgoing.Length == 0)
            {
                // Dangling node.
                DistributeDanglingRank(
                    nodes,
                    next,
                    currentRank * dampingFactor);

                continue;
            }

            var totalWeight = outgoing.Sum(
                e => Math.Max(0, e.Weight));

            if (totalWeight <= 0)
                continue;

            foreach (var edge in outgoing)
            {
                var weight =
                    Math.Max(0, edge.Weight) /
                    totalWeight;

                next[edge.TargetNodeId] +=
                    dampingFactor *
                    currentRank *
                    weight;
            }
        }

        // Personalization / teleportation.
        foreach (var nodeId in nodes)
        {
            personalization.TryGetValue(
                nodeId,
                out var personalizationValue);

            next[nodeId] +=
                (1.0 - dampingFactor) *
                personalizationValue;
        }

        return next;
    }

    /// <summary>
    /// Distributes the rank of dangling nodes evenly across all nodes in the graph.
    /// </summary>
    /// <param name="nodes">Set of node IDs to distribute the rank to.</param>
    /// <param name="next">Dictionary containing the next rank values for each node.</param>
    /// <param name="value">The rank value to be distributed.</param>
    private static void DistributeDanglingRank(
        HashSet<long> nodes,
        Dictionary<long, double> next,
        double value)
    {
        var contribution =
            value / nodes.Count;

        foreach (var nodeId in nodes)
            next[nodeId] += contribution;
    }
}