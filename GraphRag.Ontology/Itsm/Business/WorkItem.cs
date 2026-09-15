using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business;

/// <summary>
/// Abstract class: Any tracked unit of work in the ITSM domain (tickets, tasks, knowledge cases).
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
