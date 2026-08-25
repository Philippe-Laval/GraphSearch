namespace GraphSearch;

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

public class FinalScorer
{
    public static double CalculateFinalScore(
        double semanticScore,
        double bm25Score,
        double pageRankScore,
        double edgeRelevance)
    {
        return
            0.45 * semanticScore +
            0.20 * bm25Score +
            0.25 * pageRankScore +
            0.10 * edgeRelevance;
    }
    
    public static double CalculateScore(
        double semantic,
        double lexical,
        double graph,
        double edge)
    {
        return
            0.50 * semantic +
            0.15 * lexical +
            0.20 * graph +
            0.15 * edge;
    }
}