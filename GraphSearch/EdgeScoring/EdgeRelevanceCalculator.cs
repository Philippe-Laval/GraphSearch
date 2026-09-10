using GraphSearch.Library.Graphs;

namespace GraphSearch.Library.EdgeScoring;

public sealed class EdgeRelevanceCalculator
{
    private readonly RelationshipVocabulary _vocabulary;

    public EdgeRelevanceCalculator(
        RelationshipVocabulary vocabulary)
    {
        _vocabulary = vocabulary;
    }

    public double CalculateForCandidate(
        long candidateNodeId,
        IEnumerable<(GraphEdge Edge, long SeedNodeId, int Distance)> edges,
        IReadOnlyList<RelationshipRequirement> requirements)
    {
        var scores = new List<double>();

        foreach (var item in edges)
        {
            foreach (var requirement in requirements)
            {
                var score = Calculate(
                    item.Edge,
                    item.SeedNodeId,
                    requirement,
                    item.Distance);

                scores.Add(score);
            }
        }

        return scores.Count == 0
            ? 0.0
            : scores.Max();
    }
    
    public double Calculate(
        GraphEdge edge,
        long seedNodeId,
        RelationshipRequirement requirement,
        int pathDistance)
    {
        var relationshipScore =
            _vocabulary.Similarity(
                edge.Type,
                requirement.Type);

        var directionScore =
            CalculateDirectionScore(
                edge,
                seedNodeId,
                requirement.Direction);

        var confidenceScore =
            Math.Clamp(
                edge.Confidence,
                0.0,
                1.0);

        var pathScore =
            CalculatePathScore(
                pathDistance);

        return
            0.50 * relationshipScore +
            0.25 * directionScore +
            0.15 * confidenceScore +
            0.10 * pathScore;
    }

    /// <summary>
    /// Direction score
    /// </summary>
    /// <param name="edge"></param>
    /// <param name="seedNodeId"></param>
    /// <param name="expectedDirection"></param>
    /// <returns></returns>
    private static double CalculateDirectionScore(
        GraphEdge edge,
        long seedNodeId,
        EdgeDirection expectedDirection)
    {
        var outgoing =
            edge.SourceNodeId == seedNodeId;

        var incoming =
            edge.TargetNodeId == seedNodeId;

        return expectedDirection switch
        {
            EdgeDirection.Outgoing =>
                outgoing ? 1.0 : 0.0,

            EdgeDirection.Incoming =>
                incoming ? 1.0 : 0.0,

            EdgeDirection.Either =>
                outgoing || incoming
                    ? 1.0
                    : 0.0,

            _ => 0.0
        };
    }

    /// <summary>
    /// Calculate Path Distance Score With Decay
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="decay"></param>
    /// <returns></returns>
    private static double CalculatePathScore(int distance, double decay = 0.7)
    {
        return Math.Pow(
            decay,
            Math.Max(distance, 0));
    }
    
    public static double CalculateRequirementCoverage(
        IReadOnlyList<RelationshipRequirement> requirements,
        IReadOnlyList<double> requirementScores)
    {
        if (requirements.Count == 0)
            return 0.0;

        var covered =
            requirementScores.Count(
                score => score >= 0.7);

        return
            (double)covered /
            requirements.Count;
    }
    
    
    public double Calculate(
        IEnumerable<EdgeContext> edges,
        IReadOnlyList<RelationshipRequirement> requirements)
    {
        if (requirements.Count == 0)
            return 0.0;

        var requirementScores =
            new double[requirements.Count];

        foreach (var edge in edges)
        {
            for (var i = 0;
                 i < requirements.Count;
                 i++)
            {
                var requirement =
                    requirements[i];

                var score =
                    CalculateSingleEdge(
                        edge,
                        requirement);

                requirementScores[i] =
                    Math.Max(
                        requirementScores[i],
                        score);
            }
        }

        var coverage =
            requirementScores.Count(
                x => x >= 0.7)
            / (double)requirements.Count;

        var strongest =
            requirementScores.Max();

        return
            0.70 * strongest +
            0.30 * coverage;
    }
    
    double CalculateSingleEdge(EdgeContext edgeContext, RelationshipRequirement relationshipRequirement)
    {
        throw new NotImplementedException();
    }
    
    public static double CalculateFinalScore(
        NodeRankingFeatures features)
    {
        return
            0.45 * features.SemanticScore +
            0.20 * features.Bm25Score +
            0.25 * features.PageRankScore +
            0.10 * features.EdgeRelevance;
    }
    
    public static double CalculateEdgeRelevance(
        GraphRelevanceFeatures f)
    {
        return
            0.40 * f.RelationshipTypeScore +
            0.15 * f.DirectionScore +
            0.15 * f.EdgeConfidence +
            0.10 * f.PathScore +
            0.20 * f.RequirementCoverage;
    }
}