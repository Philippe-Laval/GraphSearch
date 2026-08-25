namespace GraphSearch;

public sealed record NodeScore(
    long NodeId,
    double SemanticScore,
    double LexicalScore,
    double GraphScore,
    double EdgeScore,
    double FinalScore);