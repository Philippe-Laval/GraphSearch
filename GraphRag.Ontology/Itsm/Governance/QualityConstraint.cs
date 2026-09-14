using GraphRag.Ontology.Itsm.Data;

namespace GraphRag.Ontology.Itsm.Governance;

/// <summary>
/// Freshness / accuracy expectations for the underlying data.
/// </summary>
public sealed class QualityConstraint : GovernanceConcept
{
    public required DataAsset Asset { get; init; }
    public TimeSpan? MaxAcceptableLag { get; init; }
    public double? MinCompletenessPercent { get; init; }
    public string? QualityLevel { get; init; }
    public bool IsDeprecated { get; init; }
}
