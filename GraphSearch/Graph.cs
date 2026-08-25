namespace GraphSearch;

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

    public void AddNode(GraphNode node)
    {
        _nodes[node.Id] = node;
    }

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

    public IReadOnlyList<GraphEdge> GetOutgoingEdges(long nodeId)
        => _outgoing.TryGetValue(nodeId, out var edges)
            ? edges
            : [];

    public IReadOnlyList<GraphEdge> GetIncomingEdges(long nodeId)
        => _incoming.TryGetValue(nodeId, out var edges)
            ? edges
            : [];
}