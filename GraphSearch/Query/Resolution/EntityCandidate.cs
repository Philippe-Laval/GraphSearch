namespace GraphSearch.Library.Query.Resolution;

public sealed record EntityCandidate(
    long NodeId,
    string Name,
    string Type,
    double Score);