namespace GraphRag.ITSM.Entities;

public enum ChangeRiskLevel { Low, Medium, High, Critical }
public enum ChangeType { Standard, Normal, Emergency }

/// <summary>Base ticket entity. Stored TPH: one table, discriminator = TicketType.</summary>
public abstract class Ticket : EntityBase
{
    public string Number { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid? RequesterId { get; set; }
    public Person? Requester { get; set; }

    public Guid? AffectedUserId { get; set; }
    public Person? AffectedUser { get; set; }

    public Guid? AssignedAgentId { get; set; }
    public Agent? AssignedAgent { get; set; }

    public Guid? AssignedGroupId { get; set; }
    public AssignmentGroup? AssignedGroup { get; set; }

    public Guid? ResolverId { get; set; }
    public Agent? Resolver { get; set; }

    public Guid? ResolutionGroupId { get; set; }
    public AssignmentGroup? ResolutionGroup { get; set; }

    public Guid CurrentStatusId { get; set; }
    public Status CurrentStatus { get; set; } = null!;

    public Guid? PriorityId { get; set; }
    public Priority? Priority { get; set; }

    public Guid? ImpactId { get; set; }
    public Impact? Impact { get; set; }

    public Guid? UrgencyId { get; set; }
    public Urgency? Urgency { get; set; }

    public Guid? SeverityId { get; set; }
    public Severity? Severity { get; set; }

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid? SubcategoryId { get; set; }
    public Category? Subcategory { get; set; }

    // Lifecycle timestamps - flattened for easy indexing / grouping.
    public DateTimeOffset OpenedAt { get; set; }
    public DateTimeOffset? AcknowledgedAt { get; set; }
    public DateTimeOffset? AssignedAt { get; set; }
    public DateTimeOffset? FirstResponseAt { get; set; }
    public DateTimeOffset? PendingAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public DateTimeOffset? SlaResponseDeadline { get; set; }
    public DateTimeOffset? SlaResolutionDeadline { get; set; }

    public int ReopenCount { get; set; }
    public bool WasEscalated { get; set; }

    public ICollection<TicketAffectedCI> AffectedConfigurationItems { get; set; } = [];
    public ICollection<TicketAffectedService> AffectedServices { get; set; } = [];
    public ICollection<TicketRelation> RelatedFrom { get; set; } = [];
    public ICollection<TicketRelation> RelatedTo { get; set; } = [];
    public ICollection<OperationalEvent> Events { get; set; } = [];
    public ICollection<SlaMeasurement> SlaMeasurements { get; set; } = [];
}

public class Incident : Ticket
{
    public Guid? CausedByProblemId { get; set; }
    public Problem? CausedByProblem { get; set; }

    public Guid? ResolvedByChangeId { get; set; }
    public ChangeRequest? ResolvedByChange { get; set; }
}

public class ServiceRequest : Ticket
{
    public Guid? RequestedCatalogItemId { get; set; }
    public ServiceCatalogItem? RequestedCatalogItem { get; set; }
}

public class Problem : Ticket
{
    public string? KnownErrorArticleRef { get; set; }
    public string? RootCause { get; set; }
    public string? Workaround { get; set; }
}

public class ChangeRequest : Ticket
{
    public ChangeRiskLevel RiskLevel { get; set; }
    public ChangeType ChangeType { get; set; }
    public DateTimeOffset? PlannedStart { get; set; }
    public DateTimeOffset? PlannedEnd { get; set; }
    public DateTimeOffset? ActualStart { get; set; }
    public DateTimeOffset? ActualEnd { get; set; }
}

public class TicketTask : Ticket
{
    public Guid? ParentTicketId { get; set; }
    public Ticket? ParentTicket { get; set; }
}

/// <summary>Ticket <-> ConfigurationItem link.</summary>
public class TicketAffectedCI
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public Guid ConfigurationItemId { get; set; }
    public ConfigurationItem ConfigurationItem { get; set; } = null!;
    public bool IsPrimary { get; set; }
}

/// <summary>Ticket <-> Service link.</summary>
public class TicketAffectedService
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public bool IsPrimary { get; set; }
}

public enum TicketRelationKind
{
    RelatedTo,
    DuplicateOf,
    Blocks,
    BlockedBy,
    CausedBy,
    Causes,
    ResolvedBy,
    Resolves,
}

/// <summary>Arbitrary ticket-to-ticket relationships.</summary>
public class TicketRelation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SourceTicketId { get; set; }
    public Ticket SourceTicket { get; set; } = null!;
    public Guid TargetTicketId { get; set; }
    public Ticket TargetTicket { get; set; } = null!;
    public TicketRelationKind RelationKind { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
