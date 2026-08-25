using GraphSearch.Query.Analysis;
using GraphSearch.Query.Embeddings;
using GraphSearch.Query.Resolution;

namespace GraphSearch;

/*
  Usage :
  
var analysis =
   await queryProcessor.ProcessAsync(
       "What is Microsoft's relationship with .NET 10?",
       cancellationToken);
       
ou now have:
   
   QueryAnalysis
   │
   ├── OriginalQuery
   │   "What is Microsoft's relationship with .NET 10?"
   │
   ├── NormalizedQuery
   │   "What is Microsoft's relationship with .NET 10?"
   │
   ├── Entities
   │   ├── Microsoft
   │   └── .NET 10
   │
   ├── ResolvedEntities
   │   ├── Microsoft → Node 123
   │   └── .NET 10 → Node 1842
   │
   └── Embedding
       [0.023, -0.182, 0.441, ...]   
       
 Where this goes in your complete GraphRAG pipeline
   
   This is where I would now take your architecture:
   
                   USER QUERY
                      │
                      ▼
           ┌─────────────────────┐
           │  Query Processing   │
           │                     │
           │  Analyzer           │
           │  Entity Extractor   │
           │  Entity Resolver    │
           │  Query Embedding    │
           └──────────┬──────────┘
                      │
         ┌────────────┼─────────────┐
         ▼            ▼             ▼
   Resolved       Embedding      Query Intent
   entities           │             │
         │            │             │
         ▼            ▼             ▼
      Entity       Vector         Retrieval
      retrieval    retrieval       strategy
         │             │
         └──────┬──────┘
                ▼
           BM25 retrieval
                │
                ▼
             RRF fusion
                │
                ▼
           Seed selection
                │
                ▼
       Neighborhood expansion
                │
                ▼
      Personalized PageRank
                │
                ▼
             Reranking
                │
                ▼
          Relevant subgraph
                │
                ▼
      Deterministic projection
                │
                ▼
                LLM   
             
 */

public sealed class GraphRagQueryProcessor
{
    private readonly IQueryAnalyzer _analyzer;
    private readonly IEntityExtractor _extractor;
    private readonly IEntityResolver _resolver;
    private readonly IEmbeddingService _embeddingService;

    public GraphRagQueryProcessor(
        IQueryAnalyzer analyzer,
        IEntityExtractor extractor,
        IEntityResolver resolver,
        IEmbeddingService embeddingService)
    {
        _analyzer = analyzer;
        _extractor = extractor;
        _resolver = resolver;
        _embeddingService = embeddingService;
    }

    public async Task<QueryAnalysis> ProcessAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // 1. Normalize / analyze
        var normalized =
            await _analyzer.NormalizeAsync(
                query,
                cancellationToken);

        // 2. Extract entities
        var entities =
            await _extractor.ExtractAsync(
                normalized,
                cancellationToken);

        // 3. Resolve entities to graph nodes
        var resolved =
            await _resolver.ResolveAsync(
                entities,
                cancellationToken);

        // 4. Create query embedding
        var embedding =
            await _embeddingService.EmbedAsync(
                normalized,
                cancellationToken);

        return new QueryAnalysis
        {
            OriginalQuery = query,
            NormalizedQuery = normalized,
            Entities = entities,
            ResolvedEntities = resolved,
            Embedding = embedding
        };
    }
}