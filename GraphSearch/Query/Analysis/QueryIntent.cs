namespace GraphSearch.Query.Analysis;

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