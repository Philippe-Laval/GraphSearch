using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Metrics;

/// <summary>
/// Abstract class : A business metric definition. 
/// Modelled explicitly because different organizations
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

/// <summary>
/// Représente le volume de tickets suivi comme indicateur métier.
/// </summary>
/// <remarks>Exprime le nombre total de tickets pour une période ou un contexte donné dans le cadre du suivi des
/// performances métier.</remarks>
public sealed class TicketVolume : BusinessMetric;

/// <summary>
/// Représente la métrique métier du nombre de tickets actuellement ouverts.
/// </summary>
public sealed class OpenTicketCount : BusinessMetric;

/// <summary>
/// Représente une métrique métier mesurant le volume de travail en attente.
/// </summary>
/// <remarks>Permet de suivre l’accumulation des éléments non terminés pour le pilotage de la capacité et des
/// priorités.</remarks>
public sealed class Backlog : BusinessMetric;

/// <summary>
/// Représente une métrique métier qui mesure le temps nécessaire à la résolution d’un élément.
/// </summary>
public sealed class ResolutionTime : BusinessMetric;

/// <summary>
/// Représente une métrique métier mesurant le temps de réponse d’une opération ou d’un service.
/// </summary>
/// <remarks>Utilisée pour le suivi des performances et l’analyse de l’évolution des temps de réponse.</remarks>
public sealed class ResponseTime : BusinessMetric;

/// <summary>
/// Représente le délai entre une demande initiale et la première réponse dans un processus métier.
/// </summary>
/// <remarks>Mesure la réactivité opérationnelle et la performance de prise en charge.</remarks>
public sealed class FirstResponseTime : BusinessMetric;

/// <summary>
/// Représente une métrique métier mesurant le temps moyen nécessaire pour résoudre un incident.
/// </summary>
/// <remarks>Utilisée pour suivre l’efficacité opérationnelle du processus de résolution et comparer les
/// performances sur différentes périodes.</remarks>
public sealed class MeanTimeToResolve : BusinessMetric;

/// <summary>
/// Représente une métrique métier du taux de réouverture des éléments après leur clôture.
/// </summary>
/// <remarks>Utilisée pour suivre la qualité de résolution et identifier les tendances de réouverture.</remarks>
public sealed class ReopenRate : BusinessMetric;

/// <summary>
/// Représente un indicateur métier du taux de conformité aux accords de niveau de service (SLA).
/// </summary>
public sealed class SlaComplianceRate : BusinessMetric;

/// <summary>
/// Représente le taux d’escalade des cas dans un processus métier.
/// </summary>
/// <remarks>Métrique métier utilisée pour suivre l’évolution des escalades et appuyer l’analyse
/// opérationnelle.</remarks>
public sealed class EscalationRate : BusinessMetric;
