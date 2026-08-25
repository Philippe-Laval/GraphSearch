namespace GraphSearch;

public sealed class GraphWithOutgoingEdgeNeighborhoodExpander
{
    private readonly GraphWithOutgoingEdge _graphWithOutgoingEdge;

    public GraphWithOutgoingEdgeNeighborhoodExpander(GraphWithOutgoingEdge graphWithOutgoingEdge)
    {
        _graphWithOutgoingEdge = graphWithOutgoingEdge;
    }

    public GraphNeighborhood Expand(
        IEnumerable<long> seedNodeIds,
        int maxDepth = 2)
    {
        if (maxDepth < 0)
            throw new ArgumentOutOfRangeException(nameof(maxDepth));

        var seeds = seedNodeIds
            .Distinct()
            .ToArray();

        var visited = new HashSet<long>(seeds);

        var frontier = new HashSet<long>(seeds);

        var selectedEdges = new List<GraphEdge>();

        for (var depth = 0; depth < maxDepth; depth++)
        {
            var nextFrontier = new HashSet<long>();

            foreach (var nodeId in frontier)
            {
                foreach (var edge in _graphWithOutgoingEdge.GetOutgoingEdges(nodeId))
                {
                    selectedEdges.Add(edge);

                    if (visited.Add(edge.TargetNodeId))
                    {
                        nextFrontier.Add(edge.TargetNodeId);
                    }
                }
            }

            frontier = nextFrontier;

            if (frontier.Count == 0)
                break;
        }

        return new GraphNeighborhood
        {
            NodeIds = visited,
            Edges = selectedEdges
        };
    }
}