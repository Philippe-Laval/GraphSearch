namespace GraphRag.Ontology.Itsm.Business.TimeSemantics;

/// <summary>
/// Represents working hours vs 24x7, holidays, etc., used to compute SLA elapsed time.
/// </summary>
public sealed class BusinessCalendar
{
    public required string Name { get; init; }
    public string? TimeZoneId { get; init; }
    public IReadOnlyList<DayOfWeek> WorkingDays { get; init; } = [];
    public TimeOnly? WorkingHoursStart { get; init; }
    public TimeOnly? WorkingHoursEnd { get; init; }
    public IReadOnlyList<DateOnly> Holidays { get; init; } = [];
}
