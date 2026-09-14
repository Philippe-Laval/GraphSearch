using GraphRag.Ontology.Itsm.Data;

namespace GraphRag.Ontology.Itsm.Governance;

/// <summary>
/// Ownership of a data asset.
/// </summary>
public sealed class DataOwner : GovernanceConcept
{
    public required DataAsset Asset { get; init; }
    public required string OwnerTeam { get; init; }
    public string? Steward { get; init; }
    public string? DataDomain { get; init; }
}
