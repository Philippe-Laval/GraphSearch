using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/// <summary>
/// An edge in a graph, representing a relationship between two nodes.
/// </summary>
/// <param name="Id">The unique identifier of the edge.</param>
/// <param name="SourceId">The unique identifier of the source node.</param>
/// <param name="TargetId">The unique identifier of the target node.</param>
/// <param name="Type">The type of the edge.</param>
/// <param name="Properties">Additional properties of the edge.</param>
public sealed record GraphEdge(
    Guid Id,
    Guid SourceId,
    Guid TargetId,
    string Type,
    IReadOnlyDictionary<string, object?> Properties);
