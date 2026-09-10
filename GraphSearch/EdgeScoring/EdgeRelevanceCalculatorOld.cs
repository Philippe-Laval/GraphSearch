using GraphSearch.Library.Graphs;

namespace GraphSearch.Library.EdgeScoring;

public class EdgeRelevanceCalculatorOld
{
    /// <summary>
    /// Direction score
    /// </summary>
    /// <param name="edge"></param>
    /// <param name="seedNodeId"></param>
    /// <param name="expectedDirection"></param>
    /// <returns></returns>
    public static double CalculateDirectionScore(
        GraphEdge edge,
        long seedNodeId,
        EdgeDirection expectedDirection)
    {
        var isOutgoing =
            edge.SourceNodeId == seedNodeId;

        var isIncoming =
            edge.TargetNodeId == seedNodeId;

        return expectedDirection switch
        {
            EdgeDirection.Outgoing =>
                isOutgoing ? 1.0 : 0.0,

            EdgeDirection.Incoming =>
                isIncoming ? 1.0 : 0.0,

            EdgeDirection.Either =>
                isOutgoing || isIncoming
                    ? 1.0
                    : 0.0,

            _ => 0.0
        };
    }
    
    public static double CalculatePathScore(int distance)
    {
        return distance switch
        {
            0 => 1.0,
            1 => 1.0,
            2 => 0.7,
            3 => 0.4,
            _ => 0.1
        };
    }
    
    /// <summary>
    /// Calculate Path Score With Decay
    /// distance 0 → 1.00
    /// distance 1 → 0.70
    /// distance 2 → 0.49
    /// distance 3 → 0.34
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="decay"></param>
    /// <returns></returns>
    public static double CalculatePathScoreWithDecay(
        int distance,
        double decay = 0.7)
    {
        if (distance < 0)
            return 0.0;

        return Math.Pow(decay, distance);
    }

   
}