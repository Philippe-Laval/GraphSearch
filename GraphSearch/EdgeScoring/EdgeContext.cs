namespace GraphSearch.EdgeScoring;

public sealed record EdgeContext(
    GraphEdge Edge,
    long SeedNodeId,
    int Distance);