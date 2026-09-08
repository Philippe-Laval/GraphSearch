namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// QueryIntent represents the intent of a query, which can be one of the following:
/// </summary>
public enum QueryIntent
{
    Unknown,

    EntityLookup,
    Relationship,
    MultiHopRelationship,
    Aggregation,
    Comparison,
    Explanation,
    General
}