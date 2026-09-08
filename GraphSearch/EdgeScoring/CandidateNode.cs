namespace GraphSearch.Library.EdgeScoring;

/*
Query
   │
   ├── semantic retrieval
   ├── BM25
   └── entity resolution
           │
           ▼
        Seed nodes
           │
           ▼
    Neighborhood expansion
           │
           ├── SupportingEdges
           ├── Distance
           └── PageRank
           │
           ▼
     EdgeRelevanceCalculator
           │
           ▼
      FinalRanker
           │
           ▼
      Top-K subgraph
 */

public sealed record CandidateNode(
    long NodeId,
    int MinDistance,
    double PageRank,
    IReadOnlyList<EdgeContext> SupportingEdges);