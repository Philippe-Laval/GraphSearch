using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Configuration;

/// <summary>Abstract class : Any managed component tracked in the CMDB.</summary>
public abstract class ConfigurationItem : BusinessConcept
{
    public string? AssetTag { get; init; }
    public CIStatus? OperationalStatus { get; init; }
    public Party.Party? Owner { get; init; }

    /// <summary>Technical services that this CI supports.</summary>
    public IReadOnlyList<Service.TechnicalService> Supports { get; init; } = [];

    /// <summary>Other CIs this CI depends on (upstream).</summary>
    public IReadOnlyList<ConfigurationItem> DependsOn { get; init; } = [];
}
