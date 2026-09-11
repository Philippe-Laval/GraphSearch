using System.Collections.Generic;
using GraphRag.Core.Models;

namespace GraphRag.Graph.Extensions;

/// <summary>
/// Extension methods for GraphCommunity to facilitate modularity calculations.
/// </summary>
public static class GraphCommunityExtensions
{
    /// <summary>
    /// Calculates the modularity contribution of this community within the entire graph.
    /// </summary>
    /// <param name="community">The community.</param>
    /// <param name="totalGraphEdges">The total number of edges in the entire graph.</param>
    /// <returns>The modularity contribution of this community.</returns>
    public static float CalculateModularityContribution(this GraphCommunity community, int totalGraphEdges)
    {
        return Communities.ModularityCalculator.CalculateCommunityModularity(community, totalGraphEdges);
    }

    /// <summary>
    /// Calculates the overall modularity score for a collection of communities.
    /// </summary>
    /// <param name="communities">The collection of communities.</param>
    /// <returns>The overall modularity score for the graph partition.</returns>
    public static float CalculateOverallModularity(this IEnumerable<GraphCommunity> communities)
    {
        return Communities.ModularityCalculator.CalculateGraphModularity(communities);
    }
}
