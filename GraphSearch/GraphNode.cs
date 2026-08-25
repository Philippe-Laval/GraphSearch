namespace GraphSearch;

/*
Suppose your graph contains:
   
   Microsoft
      │
      ├── develops ──> .NET
      │                   │
      │                   └── has-version ──> .NET 10
      │
      └── develops ──> C#
   
   You create a deterministic text representation:
   
   Type: Technology
   Name: .NET
   Description: Cross-platform development platform from Microsoft.
   
   Then calculate an embedding for this text.
   
   I would store embeddings for at least:
   
   nodes
   documents/chunks
   communities
   
   Edges can also have embeddings, but I would not make them your primary retrieval mechanism initially.
 */

public sealed record GraphNode(
    long Id,
    string Type,
    string Name,
    string Description);
    
    
    