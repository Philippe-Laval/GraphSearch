namespace GraphRag.Core.Models;

/// <summary>
/// ScoredNode is a record that represents a node in a graph along with its associated score and depth.
/// It is used to store information about a node's relevance or importance in the context 
/// of graph analysis or traversal.
/// </summary>
/// <param name="Node">The graph node.</param>
/// <param name="Score">The score associated with the node.</param>
/// <param name="Depth">The depth of the node in the graph traversal.</param>
public sealed record ScoredNode(
    GraphNode Node,
    float Score,
    int Depth);

