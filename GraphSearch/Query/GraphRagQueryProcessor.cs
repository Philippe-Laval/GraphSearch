using GraphSearch.Library.Embeddings;
using GraphSearch.Library.Query.Analysis;
using GraphSearch.Library.Query.Resolution;

namespace GraphSearch.Library.Query;

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

/// <summary>
/// Graph Rag Query Processor
/// </summary>
public sealed class GraphRagQueryProcessor
{
    private readonly IQueryAnalyzer _analyzer;
    private readonly IEntityExtractor _extractor;
    private readonly IEntityResolver _resolver;
    private readonly IEmbeddingService _embeddingService;

    /// <summary>
    /// Initialise une nouvelle instance de la classe <c>GraphRagQueryProcessor</c> avec les dépendances requises pour
    /// analyser les requêtes, extraire et résoudre les entités, et générer des embeddings.
    /// </summary>
    /// <param name="analyzer">Analyseur utilisé pour interpréter la requête entrante.</param>
    /// <param name="extractor">Extracteur utilisé pour identifier les entités dans la requête.</param>
    /// <param name="resolver">Résolveur utilisé pour faire correspondre les entités extraites aux entités du graphe.</param>
    /// <param name="embeddingService">Service utilisé pour produire les embeddings nécessaires au traitement sémantique.</param>
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

    /// <summary>
    /// Processes a query and returns a detailed analysis.
    /// </summary>
    /// <param name="query">The query to process</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>A <see cref="QueryAnalysis"/> containing the analysis results</returns>
    public async Task<QueryAnalysis> ProcessAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // 1. Analyze (normalize + intent + optional rewrites / language / graph pattern)
        var analyzed =
            await _analyzer.AnalyzeAsync(
                query,
                cancellationToken);

        // 2. Extract entities from the normalized text
        var entities =
            await _extractor.ExtractAsync(
                analyzed.NormalizedQuery,
                cancellationToken);

        // 3. Resolve entities to graph nodes
        var resolved =
            await _resolver.ResolveAsync(
                entities,
                cancellationToken);

        // 4. Create query embedding
        var embedding =
            await _embeddingService.EmbedAsync(
                analyzed.NormalizedQuery,
                cancellationToken);

        return new QueryAnalysis
        {
            OriginalQuery = analyzed.OriginalQuery,
            NormalizedQuery = analyzed.NormalizedQuery,
            Entities = entities,
            ResolvedEntities = resolved,
            Embedding = embedding,
            Intent = analyzed.Intent,
            IntentConfidence = analyzed.IntentConfidence,
            Rewrites = analyzed.Rewrites,
            DetectedLanguage = analyzed.DetectedLanguage,
            GraphPattern = analyzed.GraphPattern,
        };
    }
}