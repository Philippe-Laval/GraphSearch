namespace GraphSearch.Query.Resolution;

// This is deliberately independent of Lucene, ChromaDB, SQL, Neo4j, etc.
// You can implement IEntityIndex using whatever backend you eventually choose.
//
// https://www.trychroma.com/products/chromadb

/*
  would implement all three behind the same IEntityIndex interface, so your EntityResolver doesn't care whether the backing store is Lucene.NET, ChromaDB, or SQL.
   
 One important distinction first:
   
   - Lucene.NET → excellent for exact/lexical entity resolution and fuzzy matching.
   - ChromaDB → excellent for semantic entity resolution using embeddings.
   - SQL → excellent for deterministic exact/alias matching and can be your 
   canonical entity store.
   
   For your GraphRAG, I would actually use SQL as the source of truth, 
   with Lucene and Chroma as indexes over the same entities.
   
   Lucene.NET supports fuzzy matching through FuzzyQuery, and its IndexSearcher 
   is intended to be reused for an unchanged index. 
   
   Chroma supports nearest-neighbor queries with embeddings and metadata filtering.
 */

/*
            EntityResolver
                │
                ▼
          IEntityIndex
         /     |      \
        /      |       \
       ▼       ▼        ▼
   Lucene    Chroma     SQL
 */

public interface IEntityIndex
{
    Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default);
}