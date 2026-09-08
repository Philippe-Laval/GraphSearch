using GraphSearch.Library.Query.Resolution;

namespace GraphSearch.Library.Query.Analysis;

/*
One architectural change I'd strongly recommend

   Don't make QueryAnalysis only contain the embedding and entities. 
   Add query constraints eventually:

   That will eventually allow a query like:
   "Which products developed by Microsoft run on Linux?"

   to be represented approximately as:

   Intent:
       MultiHopRelationship

   Entities:
       Microsoft
       Linux

   Relationship constraints:
       develops
       runs-on

   Node constraints:
       Product

   Graph pattern:

    Microsoft
       │
     develops
       ▼
     Product
       │
    runs-on
       ▼
     Linux

   And that is the point where your natural-language GraphRAG query processor 
   starts becoming a true query planner, rather than simply a vector-search wrapper.

   Given the Cypher/ANTLR/Binder architecture you've been building, 
   the next logical step is to make QueryAnalysis produce a 
   graph query intent / logical pattern, which can then feed directly 
   into your Binder and LogicalOperator pipeline.
 */

public sealed class QueryAnalysisImproved
{
    public required string OriginalQuery { get; init; }
    public required string NormalizedQuery { get; init; }
   
    public QueryIntent Intent { get; init; }
   
    public required IReadOnlyList<ExtractedEntity> Entities { get; init; }
   
    public required IReadOnlyList<ResolvedEntity> ResolvedEntities { get; init; }
   
    public required ReadOnlyMemory<float> Embedding { get; init; }
   
    public IReadOnlyList<string> RelationshipTypes { get; init; } = [];
   
    public IReadOnlyList<string> NodeTypes { get; init; } = [];
   
    public int? RequestedTopK { get; init; }
}