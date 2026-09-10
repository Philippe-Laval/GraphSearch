namespace GraphSearch.Library.Graphs;

/// <summary>
/// Page rank result represents the result of a page rank algorithm applied to a graph.
/// </summary>
/// <param name="NodeId">The ID of the node.</param>
/// <param name="Score">The page rank score of the node.</param>
public sealed record PageRankResult(
    long NodeId,
    double Score);