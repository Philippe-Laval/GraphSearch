namespace GraphRag.ITSM.Entities;

public enum SlaKind { Response, Resolution, Availability }
public enum SlaBreachStatus { OnTrack, AtRisk, Breached, Met, NotApplicable }

/// <summary>SLA definition. First-class per the ontology guidance.</summary>
public class ServiceLevelAgreement : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public SlaKind Kind { get; set; }
    public TimeSpan TargetDuration { get; set; }
    public double? TargetAvailabilityPercent { get; set; }

    public Guid? AppliesToPriorityId { get; set; }
    public Priority? AppliesToPriority { get; set; }

    public Guid? AppliesToServiceId { get; set; }
    public Service? AppliesToService { get; set; }

    public Guid? BusinessCalendarId { get; set; }
    public BusinessCalendar? BusinessCalendar { get; set; }

    public bool IsActive { get; set; } = true;
}

/// <summary>Concrete measurement of an SLA against a ticket.</summary>
public class SlaMeasurement : EntityBase
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public Guid ServiceLevelAgreementId { get; set; }
    public ServiceLevelAgreement ServiceLevelAgreement { get; set; } = null!;

    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? StopTime { get; set; }
    public TimeSpan PauseDuration { get; set; }
    public TimeSpan TargetDuration { get; set; }
    public TimeSpan? ElapsedDuration { get; set; }
    public SlaBreachStatus BreachStatus { get; set; }
}

/// <summary>Business calendar (working hours, holidays) used to compute SLA time.</summary>
public class BusinessCalendar : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = "UTC";

    /// <summary>Bitmask of DayOfWeek (Sunday=1, Monday=2, ...) for working days.</summary>
    public int WorkingDaysMask { get; set; }

    public TimeOnly? WorkingHoursStart { get; set; }
    public TimeOnly? WorkingHoursEnd { get; set; }

    public ICollection<CalendarHoliday> Holidays { get; set; } = [];
}

public class CalendarHoliday
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BusinessCalendarId { get; set; }
    public BusinessCalendar BusinessCalendar { get; set; } = null!;
    public DateOnly Date { get; set; }
    public string? Name { get; set; }
}
