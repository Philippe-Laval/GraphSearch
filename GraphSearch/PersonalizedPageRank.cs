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

/// <summary>
/// <summary>
/// Personalized PageRank algorithm that ranks nodes in a graph based on a given personalization vector.
/// </summary>
public sealed class PersonalizedPageRank
{
    private readonly Graph _graph;

    public PersonalizedPageRank(Graph graph)
    {
        _graph = graph;
    }

    /// <summary>
    /// Ranks the given nodes in the graph using the Personalized PageRank algorithm.
    /// </summary>
    /// <param name="nodeIds">The IDs of the nodes to rank.</param>
    /// <param name="personalization">The personalization vector mapping node IDs to their initial probabilities.</param>
    /// <param name="iterations">The number of iterations to perform.</param>
    /// <param name="dampingFactor">The damping factor for the PageRank algorithm.</param>
    /// <returns>A list of PageRank results sorted by score in descending order.</returns>
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

    /// <summary>
    /// Normalise un vecteur de personnalisation sur l’ensemble des nœuds fourni.
    /// </summary>
    /// <remarks>Les nœuds absents de <paramref name="personalization"/> ou associés à une valeur non positive
    /// reçoivent initialement 0. Si aucune masse positive n’est disponible, une distribution uniforme est appliquée à
    /// tous les nœuds. Les entrées de <paramref name="personalization"/> dont la clé n’appartient pas à <paramref
    /// name="nodes"/> sont ignorées.</remarks>
    /// <param name="nodes">Ensemble des identifiants de nœuds à inclure dans le résultat.</param>
    /// <param name="personalization">Poids de personnalisation par identifiant de nœud.</param>
    /// <returns>Dictionnaire contenant tous les nœuds avec des poids non négatifs dont la somme vaut 1.</returns>
    private static Dictionary<long, double>
        NormalizePersonalization(
            HashSet<long> nodes,
            IReadOnlyDictionary<long, double> personalization)
    {
        // Initialize result dictionary with all nodes set to 0.
        var result = nodes.ToDictionary(
            nodeId => nodeId,
            _ => 0.0);

        // Populate result with positive personalization values for nodes that exist in the graph.
        foreach (var pair in personalization)
        {
            if (!nodes.Contains(pair.Key))
                continue;

            if (pair.Value > 0)
                result[pair.Key] = pair.Value;
        }

        // Calculate the total of the personalization values.
        var total = result.Values.Sum();

        // if no positive personalization is supplied, use uniform distribution.
        if (total <= 0)
        {
            var uniform = 1.0 / nodes.Count;

            foreach (var nodeId in nodes)
                result[nodeId] = uniform;

            return result;
        }

        // The sum of the personalization values is positive, normalize them to sum to 1.
        foreach (var nodeId in nodes)
            result[nodeId] /= total;

        return result;
    }
}