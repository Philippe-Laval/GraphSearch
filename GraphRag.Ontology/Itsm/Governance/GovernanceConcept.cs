using GraphRag.Ontology.Itsm.Core;
using GraphRag.Ontology.Itsm.Data;

namespace GraphRag.Ontology.Itsm.Governance;

/// <summary>Governance and constraint layer of the ontology.</summary>
public abstract class GovernanceConcept : OntologyEntity;

/// <summary>How sensitive is the data element from a compliance perspective.</summary>
public sealed class DataClassification : GovernanceConcept
{
    public required DataAsset Asset { get; init; }
    public required Sensitivity Sensitivity { get; init; }
    public IReadOnlyList<string> ComplianceTags { get; init; } = [];
}

public enum Sensitivity { Public, Internal, Confidential, Restricted, Pii, Phi, Pci }

/// <summary>Access rule scoping who/what can query a given asset.</summary>
public sealed class AccessPolicy : GovernanceConcept
{
    public required DataAsset Asset { get; init; }
    public IReadOnlyList<string> AllowedRoles { get; init; } = [];
    public IReadOnlyList<string> DeniedRoles { get; init; } = [];
    public IReadOnlyList<string> AllowedAggregations { get; init; } = [];
    public bool RequiresApproval { get; init; }
}

/// <summary>Ownership of a data asset.</summary>
public sealed class DataOwner : GovernanceConcept
{
    public required DataAsset Asset { get; init; }
    public required string OwnerTeam { get; init; }
    public string? Steward { get; init; }
    public string? DataDomain { get; init; }
}

/// <summary>Freshness / accuracy expectations for the underlying data.</summary>
public sealed class QualityConstraint : GovernanceConcept
{
    public required DataAsset Asset { get; init; }
    public TimeSpan? MaxAcceptableLag { get; init; }
    public double? MinCompletenessPercent { get; init; }
    public string? QualityLevel { get; init; }
    public bool IsDeprecated { get; init; }
}
