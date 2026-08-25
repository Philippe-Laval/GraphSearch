namespace GraphSearch.Query.Analysis;

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

public sealed record AnalyzedQuery(
    string OriginalQuery,
    string NormalizedQuery,
    QueryIntent Intent);