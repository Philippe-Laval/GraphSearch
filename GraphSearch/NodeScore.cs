namespace GraphSearch.Library;

/// <summary>
/// Node score represents the scores of a node in a graph search.
/// </summary>
/// <param name="NodeId">The ID of the node.</param>
/// <param name="SemanticScore">The semantic score of the node.</param>
/// <param name="LexicalScore">The lexical score of the node.</param>
/// <param name="GraphScore">The graph score of the node.</param>
/// <param name="EdgeScore">The edge score of the node.</param>
/// <param name="FinalScore">The final score of the node.</param>
public sealed record NodeScore(
    long NodeId,
    double SemanticScore,
    double LexicalScore,
    double GraphScore,
    double EdgeScore,
    double FinalScore);