namespace GraphSearch.Library.EdgeScoring;

public sealed record EdgeRelevanceResult(
    double Score,
    double RelationshipScore,
    double DirectionScore,
    double ConfidenceScore,
    double PathScore,
    double RequirementCoverage);