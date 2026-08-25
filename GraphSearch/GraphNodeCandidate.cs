namespace GraphSearch;


/*
One more thing I'd change for your architecture
   
   Since you're building a real GraphRAG engine rather than just a demo, I would make the neighborhood expansion return distance from the seed as well:
   
   public sealed record GraphNodeCandidate(
       long NodeId,
       int MinDistance,
       long? SeedNodeId);
   
   Then you can apply a distance decay such as:
   
   distance 0 → 1.00
   distance 1 → 0.70
   distance 2 → 0.40
   
   This prevents a node that happens to be connected through several paths from outranking an immediately relevant node.
   
   It also gives you a very useful signal for the final ranking:
   
   FinalScore =
       semantic
     + bm25
     + pagerank
     + edge relevance
     + distance relevance
   
   That would be the version I'd recommend integrating into the retrieval layer of the GraphRAG engine you've been designing, especially before the final subgraph projection sent to the LLM.
 */

public sealed record GraphNodeCandidate(
    long NodeId,
    int MinDistance,
    long? SeedNodeId);