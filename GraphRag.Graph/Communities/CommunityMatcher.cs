using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;

namespace GraphRag.Graph.Communities;

public sealed class CommunityMatcher : ICommunityMatcher
{
    public CommunityStability Compare(
        CommunityFingerprint previous,
        CommunityFingerprint current)
    {
        float score = 0;

        if (previous.CanonicalHash == current.CanonicalHash)
            score += 0.40f;

        if (previous.CoreHash == current.CoreHash)
            score += 0.30f;

        if (previous.StructureHash == current.StructureHash)
            score += 0.20f;

        if (previous.KeywordHash == current.KeywordHash)
            score += 0.10f;

        return new CommunityStability
        {
            Score = score,
            Status =
                score switch
                {
                    > 0.95f => StabilityStatus.Identical,
                    > 0.70f => StabilityStatus.Updated,
                    _ => StabilityStatus.New
                }
        };
    }
}