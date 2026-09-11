using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.DTOs;

public sealed record NodeProjection
{
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Label { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyDictionary<string, object?> Properties { get; init; }
}
