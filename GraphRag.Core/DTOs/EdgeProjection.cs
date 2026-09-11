namespace GraphRag.Core.DTOs;

public sealed record EdgeProjection
{
    public required string Source { get; init; }
    public required string Relation { get; init; }
    public required string Target { get; init; }
}
