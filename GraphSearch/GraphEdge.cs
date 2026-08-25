namespace GraphSearch;

public sealed record GraphEdge(
    long Id,
    long SourceNodeId,
    long TargetNodeId,
    string Type,
    double Confidence = 1.0,
    double Weight = 1.0);
    
public sealed record GraphEdge2(
    long Id,
    long SourceNodeId,
    long TargetNodeId,
    string Type,
    double Confidence = 1.0,
    double Weight = 1.0);