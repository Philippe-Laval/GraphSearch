using GraphRag.Ontology.Itsm.Governance;

namespace GraphRag.Conceptual.Entities;

/// <summary>
/// Sensitivity/compliance classification for a data asset.
/// </summary>
public class DataClassificationRule : ConceptualEntity
{
    public Guid AssetId { get; set; }
    public DataAssetRef Asset { get; set; } = null!;

    public Sensitivity Sensitivity { get; set; }

    /// <summary>Comma-separated tags: "PII,GDPR,HIPAA".</summary>
    public string? ComplianceTags { get; set; }

    public string? Justification { get; set; }
}

/// <summary>
/// Access-control rules attached to an asset.
/// Enables "should this user be allowed to ask that?".
/// </summary>
public class AccessPolicyRule : ConceptualEntity
{
    public Guid AssetId { get; set; }
    public DataAssetRef Asset { get; set; } = null!;

    public string? AllowedRolesCsv { get; set; }
    public string? DeniedRolesCsv { get; set; }
    public string? AllowedAggregationsCsv { get; set; }
    public bool RequiresApproval { get; set; }
    public string? PolicyNotes { get; set; }
}

/// <summary>
/// Ownership record for an asset.
/// </summary>
public class DataOwnerRecord : ConceptualEntity
{
    public Guid AssetId { get; set; }
    public DataAssetRef Asset { get; set; } = null!;

    public string OwnerTeam { get; set; } = string.Empty;
    public string? Steward { get; set; }
    public string? DataDomain { get; set; }
}

/// <summary>
/// Freshness/quality expectations for the asset.
/// </summary>
public class QualityConstraint : ConceptualEntity
{
    public Guid AssetId { get; set; }
    public DataAssetRef Asset { get; set; } = null!;

    public TimeSpan? MaxAcceptableLag { get; set; }
    public double? MinCompletenessPercent { get; set; }
    public string? QualityLevel { get; set; }
    public bool IsDeprecated { get; set; }
}
