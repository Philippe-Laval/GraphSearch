namespace GraphRag.Core.DTOs;

public sealed record SubGraphProjection
{
    public required IReadOnlyCollection<NodeProjection> Nodes { get; init; }
    public required IReadOnlyCollection<EdgeProjection> Edges { get; init; }
}
