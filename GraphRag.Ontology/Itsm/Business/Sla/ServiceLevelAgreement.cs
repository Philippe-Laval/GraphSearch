using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Sla;

/// <summary>
/// SLA modelled as a first-class concept rather than a boolean "sla_breached" flag.
/// </summary>
public abstract class ServiceLevelAgreement : BusinessConcept
{
    public required TimeSpan TargetDuration { get; init; }
    public Classification.Priority? AppliesToPriority { get; init; }
    public Service.Service? AppliesToService { get; init; }
    public TimeSemantics.BusinessCalendar? BusinessCalendar { get; init; }
}

public sealed class ResponseSla : ServiceLevelAgreement;
public sealed class ResolutionSla : ServiceLevelAgreement;
public sealed class AvailabilitySla : ServiceLevelAgreement
{
    public double? TargetAvailabilityPercent { get; init; }
}
