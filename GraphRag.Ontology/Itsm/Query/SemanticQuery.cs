using GraphRag.Ontology.Itsm.Business.Metrics;
using GraphRag.Ontology.Itsm.Business.TimeSemantics;
using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Query;

/// <summary>
/// A semantic query - the intermediate representation between a natural language
/// question and the emitted SQL. This is the "Semantic Query" step in:
/// NL -> Semantic query -> Ontology grounding -> Database grounding -> SQL.
/// </summary>
public sealed class SemanticQuery : OntologyEntity
{
    public required QueryIntent Intent { get; init; }
    public required BusinessConcept TargetEntity { get; init; }
    public BusinessMetric? Metric { get; init; }
    public IReadOnlyList<Filter> Filters { get; init; } = [];
    public IReadOnlyList<Grouping> Groupings { get; init; } = [];
    public Ranking? Ranking { get; init; }
    public TimeConstraint? TimeConstraint { get; init; }
    public IReadOnlyList<Join> Joins { get; init; } = [];
    public int? Limit { get; init; }
    public string? OriginalQuestion { get; init; }
}

public enum QueryIntent
{
    Lookup,
    Count,
    Aggregation,
    Ranking,
    Comparison,
    Trend,
    Distribution,
    Existence,
}

/// <summary>A boolean predicate over concept attributes.</summary>
public sealed class Filter : OntologyEntity
{
    public required BusinessConcept Subject { get; init; }
    public required string Attribute { get; init; }
    public required FilterOperator Operator { get; init; }
    public object? Value { get; init; }
    public IReadOnlyList<object>? Values { get; init; }
}

public enum FilterOperator
{
    Equals,
    NotEquals,
    In,
    NotIn,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Between,
    Like,
    IsNull,
    IsNotNull,
}

public sealed class Aggregation : OntologyEntity
{
    public required BusinessMetric Metric { get; init; }
    public AggregationFunction? OverrideFunction { get; init; }
}

public sealed class Grouping : OntologyEntity
{
    public required BusinessConcept ByConcept { get; init; }
    public string? Attribute { get; init; }
    public GroupingGranularity? Granularity { get; init; }
}

public enum GroupingGranularity { Day, Week, Month, Quarter, Year, FiscalMonth, FiscalQuarter, FiscalYear }

public sealed class Ranking : OntologyEntity
{
    public required BusinessMetric By { get; init; }
    public SortDirection Direction { get; init; } = SortDirection.Descending;
    public int? Limit { get; init; }
}

public enum SortDirection { Ascending, Descending }

/// <summary>
/// Explicit temporal semantics. Two independent aspects: the *when* (period)
/// and the *which date* (date basis such as CreatedAt, ResolvedAt).
/// </summary>
public sealed class TimeConstraint : OntologyEntity
{
    public RelativePeriod? RelativePeriod { get; init; }
    public TimePeriod? AbsolutePeriod { get; init; }
    public DateBasis DateBasis { get; init; }
    public BusinessCalendar? BusinessCalendar { get; init; }
}

public enum RelativePeriod
{
    Today,
    Yesterday,
    ThisWeek,
    LastWeek,
    ThisMonth,
    LastMonth,
    ThisQuarter,
    LastQuarter,
    ThisYear,
    LastYear,
    LastNDays,
    LastNMonths,
}

/// <summary>Which timestamp on the ticket is used for the time filter.</summary>
public enum DateBasis
{
    CreatedAt,
    AssignedAt,
    ResolvedAt,
    ClosedAt,
    DueAt,
    LastModifiedAt,
}

public sealed class Join : OntologyEntity
{
    public required BusinessConcept From { get; init; }
    public required BusinessConcept To { get; init; }
    public JoinType Type { get; init; }
}

public enum JoinType { Inner, LeftOuter, RightOuter, FullOuter, Cross }
