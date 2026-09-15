using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Sla;

/// <summary>
/// Abstract class : SLA modelled as a first-class concept rather than a boolean "sla_breached" flag.
/// </summary>
public abstract class ServiceLevelAgreement : BusinessConcept
{
    public required TimeSpan TargetDuration { get; init; }
    public Classification.Priority? AppliesToPriority { get; init; }
    public Service.Service? AppliesToService { get; init; }
    public TimeSemantics.BusinessCalendar? BusinessCalendar { get; init; }
}

/// <summary>
/// Response SLA
/// </summary>
public sealed class ResponseSla : ServiceLevelAgreement;

/// <summary>
/// Resolution SLA
/// </summary>
public sealed class ResolutionSla : ServiceLevelAgreement;

/// <summary>
/// Availability SLA
/// </summary>
public sealed class AvailabilitySla : ServiceLevelAgreement
{
    /// <summary>
    /// Target availability percentage
    /// </summary>
    public double? TargetAvailabilityPercent { get; init; }
}
