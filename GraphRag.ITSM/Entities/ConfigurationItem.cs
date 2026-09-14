namespace GraphRag.ITSM.Entities;

public enum CIOperationalStatus { Planned, InStock, Deployed, InMaintenance, Retired }

/// <summary>Configuration items (CMDB). TPH.</summary>
public abstract class ConfigurationItem : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? AssetTag { get; set; }
    public CIOperationalStatus OperationalStatus { get; set; } = CIOperationalStatus.Deployed;
    public string? Environment { get; set; }

    public Guid? OwnerId { get; set; }
    public Person? Owner { get; set; }

    public ICollection<ConfigurationItemSupport> SupportedTechnicalServices { get; set; } = [];
    public ICollection<ConfigurationItemDependency> DependsOn { get; set; } = [];
    public ICollection<ConfigurationItemDependency> DependedOnBy { get; set; } = [];
    public ICollection<TicketAffectedCI> Tickets { get; set; } = [];
}

public class Application : ConfigurationItem
{
    public string? Vendor { get; set; }
    public string? VersionNumber { get; set; }
}

public class Server : ConfigurationItem
{
    public string? Hostname { get; set; }
    public string? OperatingSystem { get; set; }
    public string? IpAddress { get; set; }
}

public class DatabaseCI : ConfigurationItem
{
    public string? Engine { get; set; }
    public string? InstanceName { get; set; }
}

public class NetworkDevice : ConfigurationItem
{
    public string? DeviceType { get; set; }
    public string? IpAddress { get; set; }
}

public class CloudResource : ConfigurationItem
{
    public string? Provider { get; set; }
    public string? Region { get; set; }
    public string? ResourceType { get; set; }
}

/// <summary>CI -> TechnicalService supports link.</summary>
public class ConfigurationItemSupport
{
    public Guid ConfigurationItemId { get; set; }
    public ConfigurationItem ConfigurationItem { get; set; } = null!;
    public Guid TechnicalServiceId { get; set; }
    public TechnicalService TechnicalService { get; set; } = null!;
}

/// <summary>Self-referencing many-to-many for CI dependencies.</summary>
public class ConfigurationItemDependency
{
    public Guid SourceId { get; set; }
    public ConfigurationItem Source { get; set; } = null!;
    public Guid TargetId { get; set; }
    public ConfigurationItem Target { get; set; } = null!;
    public string? DependencyType { get; set; }
}
