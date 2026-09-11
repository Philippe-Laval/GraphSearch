using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/// <summary>
/// GraphSnapshot represents a snapshot of a graph at a specific point in time, containing nodes and edges.
/// </summary>
/// <param name="Nodes">The collection of nodes in the graph snapshot.</param>
/// <param name="Edges">The collection of edges in the graph snapshot.</param>
public sealed record GraphSnapshot(
    IReadOnlyCollection<GraphNode> Nodes,
    IReadOnlyCollection<GraphEdge> Edges)
{
    /// <summary>
    /// Gets all edges connected to a specific node by its ID.
    /// </summary>
    /// <param name="id">The ID of the node.</param>
    /// <returns>An enumerable of edges connected to the specified node.</returns>
    public IEnumerable<GraphEdge> GetEdges(Guid id)
    {
        foreach (var edge in Edges)
        {
            if (edge.SourceId == id || edge.TargetId == id)
            {
                yield return edge;
            }
        }
    }

    /// <summary>
    /// Extracts a subgraph containing only the nodes and edges from the specified community.
    /// </summary>
    /// <param name="community">The community to extract the subgraph from.</param>
    /// <returns>A graph snapshot containing the nodes from the community and edges where both endpoints are in the
    /// community.</returns>
    public GraphSnapshot Extract(MutableGraphCommunity community)
    {
        var nodeIds = community.Nodes
            .Select(n => n.Id)
            .ToHashSet();

        var subNodes = Nodes
            .Where(n => nodeIds.Contains(n.Id))
            .ToList();

        var subEdges = Edges
            .Where(e => nodeIds.Contains(e.SourceId) && nodeIds.Contains(e.TargetId))
            .ToList();

        return new GraphSnapshot(subNodes, subEdges);
    }
}
