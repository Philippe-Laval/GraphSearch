namespace GraphSearch.Library;

/// <summary>
/// Bidirectional Graph Neighborhood Expander
/// </summary>
public sealed class GraphNeighborhoodExpander
{
    private readonly Graph _graph;

    public GraphNeighborhoodExpander(Graph graph)
    {
        _graph = graph;
    }

    /// <summary>
    /// Expand neighborhood of given seed nodes
    /// </summary>
    /// <param name="seedNodeIds">Seed node IDs</param>
    /// <param name="maxDepth">Max depth of expansion</param>
    /// <param name="direction">Direction of graph traversal</param>
    /// <returns>The expanded graph neighborhood</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public GraphNeighborhood Expand(
        IEnumerable<long> seedNodeIds,
        int maxDepth = 2,
        GraphTraversalDirection direction = GraphTraversalDirection.Both)
    {
        if (maxDepth < 0)
            throw new ArgumentOutOfRangeException(nameof(maxDepth));

        var seeds = seedNodeIds
            .Distinct()
            .ToArray();

        var visited = new HashSet<long>(seeds);
        var frontier = new HashSet<long>(seeds);
        var selectedEdges = new HashSet<GraphEdge>();

        for (var depth = 0; depth < maxDepth; depth++)
        {
            var nextFrontier = new HashSet<long>();

            foreach (var nodeId in frontier)
            {
                if (direction is
                    GraphTraversalDirection.Outgoing or
                    GraphTraversalDirection.Both)
                {
                    VisitEdges(
                        _graph.GetOutgoingEdges(nodeId),
                        visited,
                        nextFrontier,
                        selectedEdges,
                        edge => edge.TargetNodeId);
                }

                if (direction is
                    GraphTraversalDirection.Incoming or
                    GraphTraversalDirection.Both)
                {
                    VisitEdges(
                        _graph.GetIncomingEdges(nodeId),
                        visited,
                        nextFrontier,
                        selectedEdges,
                        edge => edge.SourceNodeId);
                }
            }

            frontier = nextFrontier;

            if (frontier.Count == 0)
                break;
        }

        return new GraphNeighborhood
        {
            NodeIds = visited,
            Edges = selectedEdges.ToArray()
        };
    }

    /// <summary>
    /// Visit edges and update visited nodes and next frontier
    /// </summary>
    /// <param name="edges">The edges to visit</param>
    /// <param name="visited">The set of visited node IDs</param>
    /// <param name="nextFrontier">The next frontier of nodes to visit</param>
    /// <param name="selectedEdges">The set of selected edges</param>
    /// <param name="nextNode">A function to get the next node ID from an edge</param>
    private static void VisitEdges(
        IEnumerable<GraphEdge> edges,
        HashSet<long> visited,
        HashSet<long> nextFrontier,
        HashSet<GraphEdge> selectedEdges,
        Func<GraphEdge, long> nextNode)
    {
        foreach (var edge in edges)
        {
            selectedEdges.Add(edge);

            var nodeId = nextNode(edge);

            if (visited.Add(nodeId))
                nextFrontier.Add(nodeId);
        }
    }
}