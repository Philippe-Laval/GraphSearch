using GraphRag.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GraphRag.Core.Interfaces;

public interface IKnowledgeGraph
{
    IReadOnlyCollection<GraphNode> Nodes { get; }
    IReadOnlyCollection<GraphEdge> Edges { get; }

    /// <summary>
    /// Get the edges connected to the specified node.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <returns>The edges connected to the specified node.</returns>
    IEnumerable<GraphEdge> GetEdges(Guid nodeId);

    /// <summary>
    /// Get the node with the specified ID from the graph.
    /// </summary>
    /// <param name="id">The ID of the node.</param>
    /// <returns>The node with the specified ID.</returns>
    GraphNode GetNode(Guid id);

    /// <summary>
    /// Get the degree of a node in the graph, which is the number of edges connected to it.
    /// </summary>
    /// <param name="id">The ID of the node.</param>
    /// <returns>The degree of the node.</returns>
    int Degree(Guid id);

    /// <summary>
    /// Get the total number of edges in the graph.
    /// </summary>
    /// <returns>The total number of edges in the graph.</returns>
    int GetTotalEdges() => Edges.Count;
}