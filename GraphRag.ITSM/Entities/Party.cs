namespace GraphRag.ITSM.Entities;

/// <summary>People (end users, requesters, agents). Stored in one table (TPH) with a Discriminator.</summary>
public class Person : EntityBase
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }
}

public class Agent : Person
{
    public string? SupportTier { get; set; }
    public ICollection<AgentGroupMembership> GroupMemberships { get; set; } = [];
}

public class Organization : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? BusinessUnit { get; set; }
    public Guid? ParentOrganizationId { get; set; }
    public Organization? ParentOrganization { get; set; }
    public ICollection<Organization> Children { get; set; } = [];
    public ICollection<Person> Members { get; set; } = [];
}

public class AssignmentGroup : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Description { get; set; }
    public string? SupportTier { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<AgentGroupMembership> Memberships { get; set; } = [];
    public ICollection<ServiceSupportGroup> SupportedServices { get; set; } = [];
}

/// <summary>Join entity: an agent may belong to several groups over time.</summary>
public class AgentGroupMembership
{
    public Guid AgentId { get; set; }
    public Agent Agent { get; set; } = null!;
    public Guid AssignmentGroupId { get; set; }
    public AssignmentGroup AssignmentGroup { get; set; } = null!;
    public DateTimeOffset AssignedAt { get; set; }
    public DateTimeOffset? RemovedAt { get; set; }
    public string? RoleInGroup { get; set; }
}

public class Vendor : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? ContractReference { get; set; }
}
