using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/// <summary>
/// A context for a graph edge, containing the source node, the edge itself, and the target node.
/// </summary>
/// <param name="Source">The source node of the edge.</param>
/// <param name="Edge">The edge connecting the source and target nodes.</param>
/// <param name="Target">The target node of the edge.</param>
public sealed record GraphEdgeContext(
    GraphNode Source,
    GraphEdge Edge,
    GraphNode Target);
