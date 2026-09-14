using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Configuration;

/// <summary>Any managed component tracked in the CMDB.</summary>
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

public sealed class Application : ConfigurationItem
{
    public string? Vendor { get; init; }
    public string? VersionNumber { get; init; }
}

public sealed class Server : ConfigurationItem
{
    public string? Hostname { get; init; }
    public string? OperatingSystem { get; init; }
    public string? Environment { get; init; }
}

public sealed class DatabaseCI : ConfigurationItem
{
    public string? Engine { get; init; }
    public string? InstanceName { get; init; }
}

public sealed class NetworkDevice : ConfigurationItem
{
    public string? DeviceType { get; init; }
    public string? IpAddress { get; init; }
}

public sealed class CloudResource : ConfigurationItem
{
    public string? Provider { get; init; }
    public string? Region { get; init; }
    public string? ResourceType { get; init; }
}

public enum CIStatus { Planned, InStock, Deployed, InMaintenance, Retired }
