namespace GraphSearch.Library.Graphs;

/*
 * For a GraphRAG system, however, I recommend supporting both outgoing and incoming edges.
 *
 *  For example:
 *  Microsoft ──develops──> .NET
 *  
 *  If .NET is your seed, an outgoing-only traversal won't find Microsoft.
 */

/// <summary>
/// Graph with incoming and outgoing edges
/// </summary>
public sealed class Graph
{
    private readonly Dictionary<long, GraphNode> _nodes = [];
    private readonly List<GraphEdge> _edges = [];

    private readonly Dictionary<long, List<GraphEdge>> _outgoing = [];
    private readonly Dictionary<long, List<GraphEdge>> _incoming = [];

    public IReadOnlyDictionary<long, GraphNode> Nodes => _nodes;
    public IReadOnlyList<GraphEdge> Edges => _edges;

    /// <summary>
    /// Add a node to the graph
    /// </summary>
    /// <param name="node">The node to add to the graph.</param>
    public void AddNode(GraphNode node)
    {
        _nodes[node.Id] = node;
    }

    /// <summary>
    /// Add a edge to the graph.
    /// This will also update the outgoing and incoming edge lists for the source and target nodes.
    /// </summary>
    /// <param name="edge">The edge to add to the graph.</param>
    public void AddEdge(GraphEdge edge)
    {
        _edges.Add(edge);

        if (!_outgoing.TryGetValue(edge.SourceNodeId, out var outgoing))
        {
            outgoing = [];
            _outgoing[edge.SourceNodeId] = outgoing;
        }

        outgoing.Add(edge);

        if (!_incoming.TryGetValue(edge.TargetNodeId, out var incoming))
        {
            incoming = [];
            _incoming[edge.TargetNodeId] = incoming;
        }

        incoming.Add(edge);
    }

    /// <summary>
    /// Get outgoing edges for a given node
    /// </summary>
    /// <param name="nodeId">The ID of the node for which to retrieve outgoing edges.</param>
    /// <returns>A read-only list of outgoing edges for the specified node.</returns>
    public IReadOnlyList<GraphEdge> GetOutgoingEdges(long nodeId)
        => _outgoing.TryGetValue(nodeId, out var edges)
            ? edges
            : [];

    /// <summary>
    /// Get incoming edges for a given node
    /// </summary>
    /// <param name="nodeId">The ID of the node for which to retrieve incoming edges.</param>
    /// <returns>A read-only list of incoming edges for the specified node.</returns>
    public IReadOnlyList<GraphEdge> GetIncomingEdges(long nodeId)
        => _incoming.TryGetValue(nodeId, out var edges)
            ? edges
            : [];
}