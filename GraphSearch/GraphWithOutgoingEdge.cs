namespace GraphSearch;

public sealed class GraphWithOutgoingEdge
{
    private readonly Dictionary<long, GraphNode> _nodes = [];
    private readonly List<GraphEdge> _edges = [];

    private readonly Dictionary<long, List<GraphEdge>> _outgoing = [];

    public IReadOnlyDictionary<long, GraphNode> Nodes => _nodes;
    public IReadOnlyList<GraphEdge> Edges => _edges;

    public void AddNode(GraphNode node)
    {
        _nodes[node.Id] = node;
    }

    public void AddEdge(GraphEdge edge)
    {
        _edges.Add(edge);

        if (!_outgoing.TryGetValue(edge.SourceId, out var edges))
        {
            edges = [];
            _outgoing[edge.SourceId] = edges;
        }

        edges.Add(edge);
    }

    public IReadOnlyList<GraphEdge> GetOutgoingEdges(long nodeId)
    {
        return _outgoing.TryGetValue(nodeId, out var edges)
            ? edges
            : [];
    }
}