namespace GraphSearch;

/*
    Neighborhood expansion
   
   The first step is:
   
                       Seed X
                        │
                ┌───────┼───────┐
                ▼       ▼       ▼
               A        B       C       depth 1
              / \      / \       \
             D   E    F   G       H     depth 2
   
   If your seed is X, with maxDepth = 2, we return:
   
   X
   A B C
   D E F G H
   
   But importantly, we also return the edges, because the final GraphRAG context needs relationships.
 */


/// <summary>
/// Graph Neighborhood expansion
/// </summary>
public sealed class GraphNeighborhood
{
    /// <summary>
    /// The nodes in the graph that are the seed for the neighborhood expansion.
    /// </summary>
    public required IReadOnlySet<long> NodeIds { get; init; }

    /// <summary>
    /// The edges in the graph that connect the nodes in the neighborhood expansion.
    /// </summary>
    public required IReadOnlyList<GraphEdge> Edges { get; init; }
}