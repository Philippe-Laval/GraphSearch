using GraphSearch.Library.Graphs;

namespace GraphSearch.Library.Query
{
    // The edges are part of the retrieved context.

    public sealed class GraphRagResult
    {
        public IReadOnlyList<GraphNode> Nodes { get; init; } = [];
        public IReadOnlyList<GraphEdge> Edges { get; init; } = [];
    }
}
