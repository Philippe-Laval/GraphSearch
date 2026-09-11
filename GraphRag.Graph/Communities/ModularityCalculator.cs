using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GraphRag.Graph.Communities;

/// <summary>
/// Provides methods to calculate modularity scores for graph communities.
/// </summary>
/// <remarks>
/// Modularity measures how strongly a graph is divided into communities.
/// A high modularity score indicates strong community structure, where many edges
/// connect nodes within communities and few edges connect nodes between communities.
/// </remarks>
public static class ModularityCalculator
{
    /// <summary>
    /// Calculates the modularity score for the entire graph partition.
    /// </summary>
    /// <param name="communities">The collection of communities that partition the graph.</param>
    /// <returns>
    /// The modularity score Q, where:
    /// - Higher positive Q indicates strong community structure
    /// - Q ≈ 0 suggests connectivity similar to a random graph
    /// - Negative Q indicates fewer internal connections than expected
    /// </returns>
    /// <remarks>
    /// Implements the formula:
    /// Q = (1/2m) * Σ(A_ij - k_i*k_j/(2m)) * δ(c_i, c_j)
    /// where:
    /// A_ij indicates whether nodes i and j are connected,
    /// k_i and k_j are their degrees,
    /// δ(c_i, c_j) equals 1 when the nodes i and j belong to the same community and equals 0 otherwise.
    /// m is the total number of edges in the graph.
    /// </remarks>
    public static float CalculateGraphModularity(IEnumerable<GraphCommunity> communities)
    {
        ArgumentNullException.ThrowIfNull(communities);

        var communityList = communities.ToList();
        if (communityList.Count == 0)
        {
            return 0f;
        }

        // Build node to community mapping
        var nodeToCommunity = new Dictionary<Guid, Guid>();
        foreach (var community in communityList)
        {
            foreach (var node in community.Nodes)
            {
                nodeToCommunity[node.Id] = community.Id;
            }
        }

        // Calculate total number of edges (m)
        var allEdges = communityList
            .SelectMany(c => c.Edges)
            .Distinct()
            .ToList();

        int totalEdges = allEdges.Count;
        if (totalEdges == 0)
        {
            return 0f;
        }

        double twoM = 2.0 * totalEdges;

        // Calculate node degrees
        var nodeDegrees = CalculateNodeDegrees(allEdges);

        // Calculate modularity
        double modularitySum = 0.0;

        // For each pair of nodes
        var allNodes = nodeToCommunity.Keys.ToList();
        foreach (var nodeI in allNodes)
        {
            foreach (var nodeJ in allNodes)
            {
                // A_ij: 1 if nodes are connected, 0 otherwise
                int aij = allEdges.Any(e =>
                    (e.SourceId == nodeI && e.TargetId == nodeJ) ||
                    (e.SourceId == nodeJ && e.TargetId == nodeI)) ? 1 : 0;

                // Get degrees
                int ki = nodeDegrees.GetValueOrDefault(nodeI, 0);
                int kj = nodeDegrees.GetValueOrDefault(nodeJ, 0);

                // δ(c_i, c_j): 1 if nodes are in the same community, 0 otherwise
                int sameCommunity = nodeToCommunity[nodeI] == nodeToCommunity[nodeJ] ? 1 : 0;

                // Add to modularity sum
                modularitySum += (aij - (ki * kj / twoM)) * sameCommunity;
            }
        }

        return (float)(modularitySum / twoM);
    }

    /// <summary>
    /// Calculates the modularity contribution of a single community.
    /// </summary>
    /// <param name="community">The community to analyze.</param>
    /// <param name="totalEdges">The total number of edges in the entire graph.</param>
    /// <returns>
    /// The modularity contribution Q_C of the community C, calculated as:
    /// Q_C = l_C/m - (d_C/(2m))²
    /// where 
    /// l_C is the number of edges within community C.
    /// d_C is the sum of the degrees of its nodes.
    /// m is the total number of edges in the graph.
    /// </returns>
    public static float CalculateCommunityModularity(GraphCommunity community, int totalEdges)
    {
        ArgumentNullException.ThrowIfNull(community);

        if (totalEdges == 0)
        {
            return 0f;
        }

        // Get all node Ids in the community (we need this to calculate degrees correctly)
        var nodeIds = community.Nodes.Select(n => n.Id).ToHashSet();

        // l_C: Number of edges within the community C
        int internalEdges = community.Edges.Count(e =>
            nodeIds.Contains(e.SourceId) && nodeIds.Contains(e.TargetId));

        // Calculate d_C: Sum of degrees of nodes in the community C
        var nodeDegrees = CalculateNodeDegrees(community.Edges);
        int totalDegree = community.Nodes
            .Sum(node => nodeDegrees.GetValueOrDefault(node.Id, 0));

        double m = totalEdges;
        double twoM = 2.0 * m;
        double lc = internalEdges;
        double dc = totalDegree;

        // Q_C = l_C/m - (d_C/(2m))²
        double modularity = (lc / m) - Math.Pow(dc / twoM, 2);

        return (float)modularity;
    }

    /// <summary>
    /// Calculates the degree of each node based on the provided edges.
    /// The degree of a node in a graph is the number of edges connected to it.
    /// </summary>
    /// <param name="edges">The edges in the graph or community.</param>
    /// <returns>A dictionary mapping node IDs to their degrees.</returns>
    /// <remarks>
    /// For an undirected graph, each edge contributes to the degree of both its source and target nodes.
    /// </remarks>
    private static Dictionary<Guid, int> CalculateNodeDegrees(IEnumerable<GraphEdge> edges)
    {
        var degrees = new Dictionary<Guid, int>();

        foreach (var edge in edges)
        {
            // For undirected graphs, each edge contributes to both nodes
            if (!degrees.TryGetValue(edge.SourceId, out int sourceDegree))
            {
                degrees[edge.SourceId] = 1;
            }
            else
            {
                degrees[edge.SourceId] = sourceDegree + 1;
            }

            if (!degrees.TryGetValue(edge.TargetId, out int targetDegree))
            {
                degrees[edge.TargetId] = 1;
            }
            else
            {
                degrees[edge.TargetId] = targetDegree + 1;
            }
        }

        return degrees;
    }
}
