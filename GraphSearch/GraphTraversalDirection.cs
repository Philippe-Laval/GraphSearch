namespace GraphSearch.Library;

/// <summary>
/// Graph traversal direction for neighborhood expansion. 
/// This enum defines the direction in which the graph will be traversed to find neighboring nodes.
/// </summary>
public enum GraphTraversalDirection
{
    /// <summary>
    /// Represents traversal in the outgoing direction, meaning from a node to its connected nodes via outgoing edges.
    /// </summary>
    Outgoing,
    /// <summary>
    /// Represents traversal in the incoming direction, meaning from a node to its connected nodes via incoming edges.
    /// </summary>
    Incoming,
    /// <summary>
    /// Represents traversal in both directions, meaning from a node to its connected nodes via both incoming and outgoing edges.
    /// </summary>
    Both
}