using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business;

/// <summary>
/// Any tracked unit of work in the ITSM domain (tickets, tasks, knowledge cases).
/// </summary>
public abstract class WorkItem : BusinessConcept
{
    public Party.Party? ReportedBy { get; init; }

    public Party.Party? AffectedUser { get; init; }

    public Party.Agent? AssignedAgent { get; init; }

    public Party.AssignmentGroup? AssignedGroup { get; init; }

    public Party.Agent? Resolver { get; init; }

    public Party.AssignmentGroup? ResolutionGroup { get; init; }

    public Classification.Status? CurrentStatus { get; init; }

    public Classification.Priority? Priority { get; init; }

    public Classification.Impact? Impact { get; init; }

    public Classification.Urgency? Urgency { get; init; }

    public Classification.Severity? Severity { get; init; }

    public Classification.Category? Category { get; init; }

    public Classification.Category? Subcategory { get; init; }

    public IReadOnlyList<Configuration.ConfigurationItem> AffectedConfigurationItems { get; init; } = [];

    public IReadOnlyList<Service.BusinessService> AffectedBusinessServices { get; init; } = [];

    public IReadOnlyList<Service.TechnicalService> AffectedTechnicalServices { get; init; } = [];

    public IReadOnlyList<Sla.ServiceLevelAgreement> GovernedBy { get; init; } = [];

    public IReadOnlyList<Events.OperationalEvent> LifecycleEvents { get; init; } = [];

    public TimeSemantics.TicketTimestamps Timestamps { get; init; } = new();
}

/// <summary>Base class for all ticket kinds. Concrete types are Incident, ServiceRequest, Problem, ChangeRequest, Task.</summary>
public abstract class Ticket : WorkItem;

public sealed class Incident : Ticket
{
    /// <summary>Underlying root-cause problem, if identified.</summary>
    public Problem? CausedBy { get; init; }

    /// <summary>Change request that resolved this incident, if any.</summary>
    public ChangeRequest? ResolvedByChange { get; init; }
}

public sealed class ServiceRequest : Ticket
{
    public Service.ServiceCatalogItem? Requests { get; init; }
}

public sealed class Problem : Ticket
{
    public IReadOnlyList<Incident> RelatedIncidents { get; init; } = [];

    public KnowledgeCase? KnownErrorDocumentedBy { get; init; }
}

public sealed class ChangeRequest : Ticket
{
    public ChangeRiskLevel? RiskLevel { get; init; }

    public ChangeType? ChangeType { get; init; }
}

public sealed class TicketTask : Ticket
{
    public Ticket? ParentTicket { get; init; }
}

public sealed class KnowledgeCase : WorkItem
{
    public string? ArticleReference { get; init; }
}

public enum ChangeRiskLevel { Low, Medium, High, Critical }

public enum ChangeType { Standard, Normal, Emergency }
