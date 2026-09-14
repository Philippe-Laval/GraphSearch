namespace GraphRag.Ontology.Itsm.Business.TimeSemantics;

/// <summary>Named calendar period used for reporting (fiscal, calendar, custom).</summary>
public sealed class TimePeriod
{
    public required string Name { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
    public PeriodBasis Basis { get; init; }
}
