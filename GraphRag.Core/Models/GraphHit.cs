namespace GraphRag.Core.Models;

/// <summary>
/// A graph hit represents a node in a graph along with its similarity score, 
/// indicating how closely it matches a given query or criteria.
/// </summary>
/// <param name="Node">The node in the graph.</param>
/// <param name="Similarity">The similarity score of the node.</param>
public sealed record GraphHit(
    GraphNode Node,
    float Similarity);

