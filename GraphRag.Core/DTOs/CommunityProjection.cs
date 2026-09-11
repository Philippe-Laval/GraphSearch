namespace GraphRag.Core.DTOs;

public sealed record CommunityProjection
{
    public required Guid Id { get; init; }

    public required int Level { get; init; }

    public required float Modularity { get; init; }

    public required IReadOnlyCollection<NodeProjection> Nodes { get; init; }

    public required IReadOnlyCollection<EdgeProjection> Edges { get; init; }
}