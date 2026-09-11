using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Graph.Communities;

public sealed class CommunityAnalyzer : ICommunityAnalyzer
{
    /// <summary>
    /// Analyse une communauté de graphe et calcule diverses métriques statistiques.    
    /// </summary>
    /// <param name="community">La communauté de graphe à analyser.</param>
    /// <returns>Métriques statistiques incluant le nombre de nœuds, le nombre d'arêtes, la densité, le degré moyen, la
    /// distribution des types de nœuds, la distribution des types de relations et les mots-clés principaux.</returns>
    public CommunityStatistics Analyze(GraphCommunity community)
    {
        var nodeTypes =
            community.Nodes
                .GroupBy(x => x.Type)
                .ToDictionary(
                    x => x.Key,
                    x => x.Count());

        var relationTypes =
            community.Edges
                .GroupBy(x => x.Type)
                .ToDictionary(
                    x => x.Key,
                    x => x.Count());

        return new CommunityStatistics
        {
            NodeCount =
                community.Nodes.Count,

            EdgeCount =
                community.Edges.Count,

            Density =
                ComputeDensity(community),

            AverageDegree =
                ComputeAverageDegree(community),

            NodeTypes = nodeTypes,

            RelationTypes = relationTypes,

            TopKeywords =
                ComputeKeywords(community)
        };
    }

    /// <summary>
    /// Get the top keywords from the labels and descriptions of the nodes in the community.
    /// </summary>
    /// <param name="community">The community of nodes to analyze.</param>
    /// <returns>A list of the top keywords.</returns>
    private IReadOnlyList<string> ComputeKeywords(GraphCommunity community)
    {
        var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var node in community.Nodes)
        {
            var words = new List<string>();

            if (!string.IsNullOrWhiteSpace(node.Label))
            {
                words.AddRange(ExtractWords(node.Label));
            }

            if (!string.IsNullOrWhiteSpace(node.Description))
            {
                words.AddRange(ExtractWords(node.Description));
            }

            foreach (var word in words)
            {
                if (word.Length >= 3)
                {
                    wordCounts.TryGetValue(word, out var count);
                    wordCounts[word] = count + 1;
                }
            }
        }

        return wordCounts
            .OrderByDescending(x => x.Value)
            .Take(10)
            .Select(x => x.Key)
            .ToList();
    }

    /// <summary>
    /// Extracts words of 3 or more characters from the specified text by splitting on common separators.
    /// </summary>
    /// <param name="text">The text to extract words from.</param>
    /// <returns>A collection of words with a minimum length of 3 characters.</returns>
    private static IEnumerable<string> ExtractWords(string text)
    {
        var separators = new[] { ' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?', 
            '(', ')', '[', ']', '{', '}', '<', '>', '/', '\\', '|', '-', '_', '+', '=' };

        var words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return words.Where(w => w.Length >= 3);
    }

    /// <summary>
    /// Calculates the average degree of a graph community.
    /// The degree of a node in a graph is the number of edges connected to it.
    /// The average degree is therefore the sum of all degrees divided by the number of nodes.
    /// 
    /// To calculate the average degree:
    /// 1. For each node, count how many edges are connected to that node
    /// 2. An edge can connect a node either as a Source or as a Target
    /// 3. Sum all the degrees
    /// 4. Divide by the number of nodes
    /// </summary>
    /// <param name="community">The graph community to analyze</param>
    /// <returns>The average degree of the nodes in the community</returns>
    private float ComputeAverageDegree(GraphCommunity community)
    {
        if (community.Nodes.Count == 0)
        {
            return 0f;
        }

        var nodeDegrees = new Dictionary<Guid, int>();

        foreach (var node in community.Nodes)
        {
            nodeDegrees[node.Id] = 0;
        }

        foreach (var edge in community.Edges)
        {
            if (nodeDegrees.ContainsKey(edge.SourceId))
            {
                nodeDegrees[edge.SourceId]++;
            }

            if (nodeDegrees.ContainsKey(edge.TargetId))
            {
                nodeDegrees[edge.TargetId]++;
            }
        }

        var totalDegree = nodeDegrees.Values.Sum();

        return (float)totalDegree / community.Nodes.Count;
    }

    /// <summary>
    /// Calcule la densité d'une communauté de graphe.
    /// La densité mesure le ratio d'arêtes existantes par rapport au nombre maximum d'arêtes possibles.
    /// 
    /// Pour un graphe non-orienté, la formule est :
    /// Densité = 2 × |E| / (|V| × (|V| - 1))
    /// 
    /// où |E| = nombre d'arêtes et |V| = nombre de nœuds
    /// 
    /// La densité varie entre 0 (aucune arête) et 1 (graphe complet où chaque nœud est connecté à tous les autres).
    /// </summary>
    /// <param name="community">La communauté de graphe à analyser</param>
    /// <returns>La densité du graphe (entre 0 et 1)</returns>
    private float ComputeDensity(GraphCommunity community)
    {
        var nodeCount = community.Nodes.Count;

        if (nodeCount <= 1)
        {
            return 0f;
        }

        var edgeCount = community.Edges.Count;
        var maxPossibleEdges = nodeCount * (nodeCount - 1);

        return (2f * edgeCount) / maxPossibleEdges;
    }
}