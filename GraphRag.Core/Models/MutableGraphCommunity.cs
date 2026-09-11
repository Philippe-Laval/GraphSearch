using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/// <summary>
/// A mutable representation of a graph community, containing its unique identifier, 
/// nodes, edges, hierarchical level, and modularity score.
/// </summary>
public sealed record MutableGraphCommunity
{
    public required Guid Id { get; init; }

    public required List<GraphNode> Nodes { get; init; }

    public required List<GraphEdge> Edges { get; init; }

    public required int Level { get; init; }

    public required float Modularity { get; init; }
}
