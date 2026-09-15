using GraphRag.Ontology.Itsm.Business.Metrics;
using GraphRag.Ontology.Itsm.Sql;

namespace GraphRag.Conceptual.Entities;

/// <summary>
/// First-class business metric definition (MTTR, Backlog, SLA compliance, …).
/// Kept separate from its SQL implementation because the same metric can have
/// different formulas per dialect / per organization.
/// </summary>
public class BusinessMetricDefinition : ConceptualEntity
{
    /// <summary>Concept representing the metric ("metric:MTTR").</summary>
    public Guid ConceptId { get; set; }
    public Concept Concept { get; set; } = null!;

    public AggregationFunction Aggregation { get; set; }
    public string? PreferredDefinition { get; set; }
    public string? Formula { get; set; }
    public string? Grain { get; set; }
    public string? QualifyingPopulation { get; set; }
    public TimeBasis TimeBasis { get; set; }
    public string? Unit { get; set; }

    /// <summary>Concept referring to the business calendar to use (nullable = wall-clock).</summary>
    public Guid? BusinessCalendarConceptId { get; set; }
    public Concept? BusinessCalendarConcept { get; set; }

    public ICollection<MetricImplementation> Implementations { get; set; } = new List<MetricImplementation>();
}

/// <summary>
/// Dialect-specific SQL expression implementing a metric.
/// </summary>
public class MetricImplementation : ConceptualEntity
{
    public Guid MetricDefinitionId { get; set; }
    public BusinessMetricDefinition MetricDefinition { get; set; } = null!;

    public SqlDialect Dialect { get; set; }

    /// <summary>SQL expression fragment, e.g. "AVG(DATEDIFF(minute, opened_at, resolved_at))".</summary>
    public string Expression { get; set; } = string.Empty;

    /// <summary>Comma-separated list of tables required by the expression.</summary>
    public string? RequiredTables { get; set; }
}
