namespace GraphSearch.EdgeScoring;

public sealed record NodeRankingFeatures(
    double SemanticScore,
    double Bm25Score,
    double PageRankScore,
    double EdgeRelevance);