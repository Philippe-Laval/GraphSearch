namespace GraphRag.ITSM.Entities;

/// <summary>Base for all catalog / offering / service entities. TPH.</summary>
public abstract class Service : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? OwnerId { get; set; }
    public Person? Owner { get; set; }

    public ICollection<ServiceSupportGroup> SupportGroups { get; set; } = [];
    public ICollection<TicketAffectedService> Tickets { get; set; } = [];
}

public class BusinessService : Service
{
    public string? BusinessCriticality { get; set; }
    public ICollection<TechnicalServiceEnablement> EnabledBy { get; set; } = [];
    public ICollection<BusinessServiceConsumer> ConsumedBy { get; set; } = [];
}

public class TechnicalService : Service
{
    public string? EnvironmentName { get; set; }
    public ICollection<TechnicalServiceEnablement> Enables { get; set; } = [];
    public ICollection<ConfigurationItemSupport> SupportedByCIs { get; set; } = [];
}

public class ServiceOffering : Service
{
    public string? SupportTier { get; set; }
    public Guid? BusinessServiceId { get; set; }
    public BusinessService? BusinessService { get; set; }
}

public class ServiceCatalogItem : Service
{
    public string? CatalogCategory { get; set; }
    public TimeSpan? StandardFulfillmentTime { get; set; }
}

public class ServiceSupportGroup
{
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public Guid AssignmentGroupId { get; set; }
    public AssignmentGroup AssignmentGroup { get; set; } = null!;
    public bool IsPrimary { get; set; }
}

public class TechnicalServiceEnablement
{
    public Guid TechnicalServiceId { get; set; }
    public TechnicalService TechnicalService { get; set; } = null!;
    public Guid BusinessServiceId { get; set; }
    public BusinessService BusinessService { get; set; } = null!;
}

public class BusinessServiceConsumer
{
    public Guid BusinessServiceId { get; set; }
    public BusinessService BusinessService { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
}
