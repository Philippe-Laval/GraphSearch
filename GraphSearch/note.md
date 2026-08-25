My recommendation for your architecture

Given the GraphRAG system you're building, I would use:

Component	Role
SQL	Canonical entity database
Lucene.NET	Exact/lexical/fuzzy entity retrieval
ChromaDB	Semantic entity retrieval
RRF	Combine Lucene + Chroma rankings
EntityResolver	Decide which graph node is actually meant
Graph	Store relationships and perform expansion/PageRank

And importantly, don't use Chroma as your canonical entity store. Your graph database/SQL layer should own the entity identity; Lucene and Chroma are indexes that can be rebuilt.

This also fits extremely well with the architecture you've been developing: once ResolvedEntity.GraphNodeId is available, those IDs can become the bound variables/seeds for your graph retrieval and eventually your Cypher logical plan.