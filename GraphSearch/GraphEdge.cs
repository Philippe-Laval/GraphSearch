namespace GraphSearch;

public sealed record GraphEdge(
    long SourceId,
    long TargetId,
    string Type,
    double Weight = 1.0);