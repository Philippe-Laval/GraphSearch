namespace GraphSearch.Library.Query.Analysis;

/*
For example:
   
   "What is .NET?"
          ↓
   EntityLookup
   
   "What is the relationship between Microsoft and .NET?"
          ↓
   Relationship
   
   "Which products developed by Microsoft run on Linux?"
          ↓
   MultiHopRelationship
 */

/// <summary>
/// AnalyzedQuery represents the result of analyzing a user's query, 
/// including the original query, its normalized form, and the inferred intent.
/// </summary>
/// <param name="OriginalQuery">The original query submitted by the user.</param>
/// <param name="NormalizedQuery">The normalized form of the query.</param>
/// <param name="Intent">The inferred intent of the query.</param>
public sealed record AnalyzedQuery(
    string OriginalQuery,
    string NormalizedQuery,
    QueryIntent Intent);