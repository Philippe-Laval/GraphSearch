using GraphRag.Ontology.Itsm.Data;

namespace GraphRag.Ontology.Itsm.Governance;

/// <summary>Access rule scoping who/what can query a given asset.</summary>
public sealed class AccessPolicy : GovernanceConcept
{
    public required DataAsset Asset { get; init; }
    public IReadOnlyList<string> AllowedRoles { get; init; } = [];
    public IReadOnlyList<string> DeniedRoles { get; init; } = [];
    public IReadOnlyList<string> AllowedAggregations { get; init; } = [];
    public bool RequiresApproval { get; init; }
}
