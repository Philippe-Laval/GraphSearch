namespace GraphRag.Core.Models;

/// <summary>
/// CommunityStability represents the stability of a community by providing 
/// a score and a status indicating how stable the community is compared to its previous state.
/// </summary>
public sealed record CommunityStability
{
    public required float Score { get; init; }
    public required StabilityStatus Status { get; init; }
}
