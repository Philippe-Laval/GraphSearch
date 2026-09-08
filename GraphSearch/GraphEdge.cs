namespace GraphSearch;

/// <summary>
/// A graph edge represents a directed connection between two nodes in a graph.
/// </summary>
/// <param name="Id">The unique identifier of the edge</param>
/// <param name="SourceNodeId">The ID of the source node</param>
/// <param name="TargetNodeId">The ID of the target node</param>
/// <param name="Type">The type of the edge</param>
/// <param name="Confidence">The confidence of the edge</param>
/// <param name="Weight">The weight of the edge</param>
public sealed record GraphEdge(
    long Id,
    long SourceNodeId,
    long TargetNodeId,
    string Type,
    double Confidence = 1.0,
    double Weight = 1.0);
