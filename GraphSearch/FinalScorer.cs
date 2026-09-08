namespace GraphSearch.Library;

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

/// <summary>
/// FinalScorer calculates the final score of a node based on various scoring metrics.
/// </summary>
public class FinalScorer
{
    /// <summary>
    /// Calcule un score final pondéré à partir de scores de pertinence complémentaires.
    /// </summary>
    /// <remarks>Pondérations appliquées : 0,45 (sémantique), 0,20 (BM25), 0,25 (PageRank) et 0,10 (pertinence
    /// des arêtes).</remarks>
    /// <param name="semanticScore">Score de similarité sémantique.</param>
    /// <param name="bm25Score">Score de pertinence lexical BM25.</param>
    /// <param name="pageRankScore">Score d’autorité basé sur PageRank.</param>
    /// <param name="edgeRelevance">Score de pertinence des relations ou arêtes.</param>
    /// <returns>Score final obtenu par combinaison linéaire pondérée des scores fournis.</returns>
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