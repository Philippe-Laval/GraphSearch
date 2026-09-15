namespace GraphRag.ITSM.Entities;

/// <summary>
/// Base for anything that happens during a ticket's life. TPH.
/// </summary>
public abstract class OperationalEvent : EntityBase
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public DateTimeOffset EventTimestamp { get; set; }

    public Guid? PerformedById { get; set; }
    public Agent? PerformedBy { get; set; }

    public string? Comment { get; set; }
}

public class StatusTransition : OperationalEvent
{
    public Guid FromStatusId { get; set; }
    public Status FromStatus { get; set; } = null!;
    public Guid ToStatusId { get; set; }
    public Status ToStatus { get; set; } = null!;
    public TimeSpan? TimeInFromStatus { get; set; }
}

public class AssignmentEvent : OperationalEvent
{
    public Guid? FromGroupId { get; set; }
    public AssignmentGroup? FromGroup { get; set; }
    public Guid? ToGroupId { get; set; }
    public AssignmentGroup? ToGroup { get; set; }
    public Guid? FromAgentId { get; set; }
    public Agent? FromAgent { get; set; }
    public Guid? ToAgentId { get; set; }
    public Agent? ToAgent { get; set; }
}

public class EscalationEvent : OperationalEvent
{
    public string? Reason { get; set; }
    public string? NewTier { get; set; }
}

public class ReassignmentEvent : OperationalEvent
{
    public string? Reason { get; set; }
}

public class ResolutionEvent : OperationalEvent
{
    public string? ResolutionCode { get; set; }
    public string? ResolutionNotes { get; set; }
}

public class ReopenEvent : OperationalEvent
{
    public string? Reason { get; set; }
}

public class ClosureEvent : OperationalEvent
{
    public string? ClosureCode { get; set; }
}
