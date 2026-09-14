using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Any event happening during a ticket's lifecycle.
/// Modelling these explicitly enables queries such as
/// "How many tickets were in Pending state during August?".
/// </summary>
public abstract class OperationalEvent : BusinessConcept
{
    public required DateTimeOffset EventTimestamp { get; init; }
    public Party.Agent? PerformedBy { get; init; }
    public string? Comment { get; init; }
}

public sealed class AssignmentEvent : OperationalEvent
{
    public Party.AssignmentGroup? FromGroup { get; init; }
    public Party.AssignmentGroup? ToGroup { get; init; }
    public Party.Agent? FromAgent { get; init; }
    public Party.Agent? ToAgent { get; init; }
}

public sealed class StatusTransition : OperationalEvent
{
    public required Classification.Status FromStatus { get; init; }
    public required Classification.Status ToStatus { get; init; }
    public TimeSpan? TimeInFromStatus { get; init; }
}

public sealed class Escalation : OperationalEvent
{
    public string? EscalationReason { get; init; }
    public string? NewTier { get; init; }
}

public sealed class Reassignment : OperationalEvent
{
    public string? Reason { get; init; }
}

public sealed class Resolution : OperationalEvent
{
    public string? ResolutionCode { get; init; }
    public string? ResolutionNotes { get; init; }
}

public sealed class Reopen : OperationalEvent
{
    public string? Reason { get; init; }
}

public sealed class Closure : OperationalEvent
{
    public string? ClosureCode { get; init; }
}
