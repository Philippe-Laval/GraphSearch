namespace GraphSearch.EdgeScoring;

public sealed record GraphRelevanceFeatures(
    double RelationshipTypeScore,
    double DirectionScore,
    double EdgeConfidence,
    double PathScore,
    double RequirementCoverage);