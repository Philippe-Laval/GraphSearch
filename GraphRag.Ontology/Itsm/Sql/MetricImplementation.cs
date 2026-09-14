using GraphRag.Ontology.Itsm.Business.Metrics;

namespace GraphRag.Ontology.Itsm.Sql;

/// <summary>
/// Binds a <see cref="BusinessMetric"/> to its dialect-specific SQL expression.
/// </summary>
public sealed class MetricImplementation : SqlConcept
{
    public required BusinessMetric Metric { get; init; }
    public required SqlExpression Expression { get; init; }
}
