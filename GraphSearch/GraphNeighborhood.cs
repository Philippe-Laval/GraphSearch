namespace GraphSearch;

/*
    Neighborhood expansion
   
   The first step is:
   
                       Seed
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


public sealed class GraphNeighborhood
{
    public required IReadOnlySet<long> NodeIds { get; init; }

    public required IReadOnlyList<GraphEdge> Edges { get; init; }
}