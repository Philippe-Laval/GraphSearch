namespace GraphRag.Ontology.Itsm.Business.TimeSemantics;

/// <summary>
/// Collection of temporal moments in a ticket's life.
/// Kept as distinct fields (not a single "date") because Text-to-SQL frequently
/// needs to know which date dimension applies to "sales in 2025"-style questions.
/// </summary>
public sealed class TicketTimestamps
{
    public DateTimeOffset? CreatedAt { get; init; }
    public DateTimeOffset? AcknowledgedAt { get; init; }
    public DateTimeOffset? AssignedAt { get; init; }
    public DateTimeOffset? FirstResponseAt { get; init; }
    public DateTimeOffset? PendingAt { get; init; }
    public DateTimeOffset? ResolvedAt { get; init; }
    public DateTimeOffset? ClosedAt { get; init; }
    public DateTimeOffset? DueAt { get; init; }
    public DateTimeOffset? SlaResponseDeadline { get; init; }
    public DateTimeOffset? SlaResolutionDeadline { get; init; }
    public DateTimeOffset? LastModifiedAt { get; init; }
}

/// <summary>Named calendar period used for reporting (fiscal, calendar, custom).</summary>
public sealed class TimePeriod
{
    public required string Name { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
    public PeriodBasis Basis { get; init; }
}

public enum PeriodBasis { Calendar, Fiscal, Custom }

/// <summary>Represents working hours vs 24x7, holidays, etc., used to compute SLA elapsed time.</summary>
public sealed class BusinessCalendar
{
    public required string Name { get; init; }
    public string? TimeZoneId { get; init; }
    public IReadOnlyList<DayOfWeek> WorkingDays { get; init; } = [];
    public TimeOnly? WorkingHoursStart { get; init; }
    public TimeOnly? WorkingHoursEnd { get; init; }
    public IReadOnlyList<DateOnly> Holidays { get; init; } = [];
}
