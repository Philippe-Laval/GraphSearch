using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GraphRag.Core.Models;

public class SubGraph
{
    public required IReadOnlyList<GraphNode> Nodes { get; init; }

    public required IReadOnlyList<GraphEdge> Edges { get; init; }
}


/// <summary>
/// SubGraph2 is a record that represents a subgraph consisting of a collection of nodes and edges.
/// </summary>
/// <param name="Nodes">The nodes in the subgraph.</param>
/// <param name="Edges">The edges in the subgraph.</param>
public sealed record SubGraph2(
    IReadOnlyCollection<GraphNode> Nodes,
    IReadOnlyCollection<GraphEdgeContext> Edges);