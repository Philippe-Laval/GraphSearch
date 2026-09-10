using GraphSearch.Library;
using GraphSearch.Library.Graphs;

namespace GraphSearch.Tool;


/*
For a GraphRAG system, I would not try to find the top-K graph nodes directly from the graph structure. The usual approach is a two-stage retrieval pipeline:
   
   User query
       │
       ▼
   Query embedding
       │
       ├───────────────┐
       ▼               ▼
   Vector retrieval   Lexical retrieval (BM25)
       │               │
       └───────┬───────┘
               ▼
          Hybrid ranking
               │
               ▼
         Top-K seed nodes
               │
               ▼
        Graph expansion
        (neighbors / paths)
               │
               ▼
        Reranking / scoring
               │
               ▼
        Relevant subgraph
               │
               ▼
              LLM
              
 */
public static class Program
{
    public static void Main(string[] args)
    {
        // Connecting both algorithms
        // Suppose your vector/BM25 retrieval gives:

        var seeds = new Dictionary<long, double>
        {
            [100] = 0.94, // .NET
            [200] = 0.87, // C#
            [300] = 0.72  // Microsoft
        };

        // First expand


        Graph graph = new Graph();

        var expander = new GraphNeighborhoodExpander(graph);

        var neighborhood = expander.Expand(
            seeds.Keys,
            maxDepth: 2,
            direction: GraphTraversalDirection.Both);

        // You now have:
        //
        //NodeIds
        //100
        //200
        //300
        //101
        //102
        //201
        //301

        // Then PageRank:

        var pageRank = new PersonalizedPageRank(graph);

        var ranked = pageRank.Rank(
            neighborhood.NodeIds,
            seeds,
            iterations: 30,
            dampingFactor: 0.85);

        foreach (var result in ranked.Take(20))
        {
            var node = graph.Nodes[result.NodeId];

            Console.WriteLine(
                $"{node.Name,-30} {result.Score:F6}");
        }

        /*
         You might obtain:
           
           .NET                         0.184521
           C#                           0.161832
           Microsoft                    0.129402
           .NET 10                      0.087124
           ASP.NET                      0.071823
           Visual Studio                0.058921
           Roslyn                       0.047832
           NuGet                        0.041223
           ...
         */

        /*
         One important improvement for GraphRAG
           
           I would not use PageRank alone to determine your final relevance.
           
           PageRank answers:
           "Which nodes are structurally important relative to my seeds?"
           
           It does not necessarily answer:
           "Which nodes are most relevant to the user's question?"
           
           So I would combine the scores:
           
           FinalScore =
               0.45 × SemanticScore
             + 0.20 × BM25Score
             + 0.25 × PageRankScore
             + 0.10 × EdgeRelevance
         */


    }
}