using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Metrics;

/// <summary>
/// A business metric definition. Modelled explicitly because different organizations
/// define MTTR / Backlog / Reopen Rate differently.
/// </summary>
public abstract class BusinessMetric : BusinessConcept
{
    /// <summary>Aggregation function applied at the leaf level.</summary>
    public AggregationFunction Aggregation { get; init; }

    /// <summary>Natural-language definition preferred by the business.</summary>
    public string? PreferredDefinition { get; init; }

    /// <summary>Symbolic or SQL-agnostic formula (e.g. "resolved_at - created_at").</summary>
    public string? Formula { get; init; }

    /// <summary>Grain at which the metric is computed (e.g. one row per Ticket, one row per Incident).</summary>
    public string? Grain { get; init; }

    /// <summary>Population the metric qualifies over (e.g. "ResolvedIncident").</summary>
    public string? QualifyingPopulation { get; init; }

    /// <summary>Whether elapsed time is measured in wall-clock or business-hours.</summary>
    public TimeBasis TimeBasis { get; init; }

    public TimeSemantics.BusinessCalendar? BusinessCalendar { get; init; }

    public string? Unit { get; init; }
}

public enum AggregationFunction { Count, CountDistinct, Sum, Avg, Min, Max, Median, Percentile, Ratio, Custom }

public enum TimeBasis { WallClock, BusinessHours }

public sealed class TicketVolume : BusinessMetric;
public sealed class OpenTicketCount : BusinessMetric;
public sealed class Backlog : BusinessMetric;
public sealed class ResolutionTime : BusinessMetric;
public sealed class ResponseTime : BusinessMetric;
public sealed class FirstResponseTime : BusinessMetric;
public sealed class MeanTimeToResolve : BusinessMetric;
public sealed class ReopenRate : BusinessMetric;
public sealed class SlaComplianceRate : BusinessMetric;
public sealed class EscalationRate : BusinessMetric;
