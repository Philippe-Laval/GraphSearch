using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Graph.Algorithms;

public sealed class SubGraphGenerator
{
    private readonly IKnowledgeGraph _graph;
    private readonly RelationWeights _weights;

    public SubGraphGenerator(
        IKnowledgeGraph graph,
        RelationWeights weights)
    {
        _graph = graph;
        _weights = weights;
    }

    /// <summary>
    /// Génère un sous-graphe à partir des nœuds de départ spécifiés.
    /// </summary>
    /// <param name="seeds">Les nœuds de départ pour la génération du sous-graphe.</param>
    /// <param name="maxDepth">Une observation faite par Microsoft GraphRAG : deux sauts suffisent généralement.</param>
    /// <param name="maxNodes">Le nombre maximum de nœuds à inclure dans le sous-graphe.</param>
    /// <returns>Une liste de nœuds scorés représentant le sous-graphe généré.</returns>
    public IReadOnlyList<ScoredNode> Generate(
        IEnumerable<GraphHit> seeds,
        int maxDepth = 2,
        int maxNodes = 30)
    {
        var visited = new Dictionary<Guid, ScoredNode>();

        // Une PriorityQueue garantit que les meilleurs candidats sont explorés en premier.
        // C'est proche de l'algorithme Best First Search.
        var queue = new PriorityQueue<ScoredNode, float>();

        foreach (var hit in seeds)
        {
            var scored = new ScoredNode(
                hit.Node,
                hit.Similarity,
                0);

            visited[hit.Node.Id] = scored;

            queue.Enqueue(scored, -scored.Score);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Depth >= maxDepth)
                continue;

            foreach (var edge in _graph.GetEdges(current.Node.Id))
            {
                var neighbourId =
                    edge.SourceId == current.Node.Id
                        ? edge.TargetId
                        : edge.SourceId;

                if (visited.ContainsKey(neighbourId))
                    continue;

                var neighbour = _graph.GetNode(neighbourId);

                var score =
                    ComputeScore(
                        current.Score,
                        edge,
                        neighbour,
                        current.Depth + 1);

                var scored =
                    new ScoredNode(
                        neighbour,
                        score,
                        current.Depth + 1);

                visited.Add(neighbour.Id, scored);

                queue.Enqueue(scored, -score);

                if (visited.Count >= maxNodes)
                    break;
            }
        }

        return visited.Values
            .OrderByDescending(x => x.Score)
            .ToList();
    }

    private float ComputeScore(
        float parentScore,
        GraphEdge edge,
        GraphNode node,
        int depth)
    {
        float relationWeight = _weights.GetWeight(edge.Type);

        // La formule « depthPenalty = 0.75^depth » est une approche courante pour réduire
        // l'importance des nœuds plus éloignés dans un graphe.
        float depthPenalty = MathF.Pow(0.75f, depth);

        // La formule « hubPenalty = 1 / sqrt(degree) » est très utilisée en théorie des graphes.
        // Le +1 permet de ne pas avoir de division par zéro pour les noeuds isolés.
        float hubPenalty = 1f / MathF.Sqrt(_graph.Degree(node.Id) + 1);

        /*
         Dans les systèmes les plus performants, le score d'un nœud ne dépend pas uniquement de son parent, mais de plusieurs signaux combinés :

Score =
0.50 × Similarité vectorielle
+ 0.20 × Importance de la relation
+ 0.15 × Centralité du nœud
+ 0.10 × Fraîcheur (si applicable)
+ 0.05 × Popularité ou fréquence d'utilisation

Cette approche est plus robuste qu'un simple calcul multiplicatif et permet d'ajuster facilement les pondérations selon ton domaine métier.
         */

        return
            parentScore 
            * relationWeight
            * depthPenalty
            * hubPenalty;
    }
}

