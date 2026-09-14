namespace GraphRag.Ontology.Itsm.Business.Sla;

/// <summary>
/// A concrete measurement of an SLA against a specific ticket.
/// Enables questions like "which support groups have the worst SLA compliance?".
/// </summary>
public sealed class SlaMeasurement
{
    public required string TicketId { get; init; }
    public required ServiceLevelAgreement Sla { get; init; }
    public required DateTimeOffset StartTime { get; init; }
    public DateTimeOffset? StopTime { get; init; }
    public TimeSpan PauseDuration { get; init; }
    public required TimeSpan TargetDuration { get; init; }
    public TimeSpan? ElapsedDuration { get; init; }
    public SlaBreachStatus BreachStatus { get; init; }
}
