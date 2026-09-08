namespace GraphSearch;

/// <summary>
/// Graph with outgoing edges
/// </summary>
public sealed class GraphWithOutgoingEdge
{
    private readonly Dictionary<long, GraphNode> _nodes = [];
    private readonly List<GraphEdge> _edges = [];
    private readonly Dictionary<long, List<GraphEdge>> _outgoing = [];

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
    /// This will also update the outgoing edge lists for the source nodes.
    /// </summary>
    /// <param name="edge">The edge to add to the graph.</param>
    public void AddEdge(GraphEdge edge)
    {
        _edges.Add(edge);

        if (!_outgoing.TryGetValue(edge.SourceNodeId, out var edges))
        {
            edges = [];
            _outgoing[edge.SourceNodeId] = edges;
        }

        edges.Add(edge);
    }

    /// <summary>
    /// Get outgoing edges for a given node
    /// </summary>
    /// <param name="nodeId">The ID of the node for which to retrieve outgoing edges.</param>
    /// <returns>A read-only list of outgoing edges for the specified node.</returns>
    public IReadOnlyList<GraphEdge> GetOutgoingEdges(long nodeId)
    {
        return _outgoing.TryGetValue(nodeId, out var edges)
            ? edges
            : [];
    }
}