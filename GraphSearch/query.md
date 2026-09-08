                Query
                  │
       ┌──────────┼──────────┐
       ▼          ▼          ▼
    Vector       BM25     Entities
       │          │          │
       └──────────┼──────────┘
                  ▼
                 RRF
                  │
                  ▼
             Seed nodes
                  │
                  ▼
          ┌───────────────┐
          │ Neighborhood  │
          │   depth 1-2   │
          └───────┬───────┘
                  │
                  ▼
          Personalized
             PageRank
                  │
                  ▼
        Semantic + BM25 +
        PageRank + edges
                  │
                  ▼
             Final Top-K
                  │
                  ▼
             Subgraph
                  │
                  ▼
                LLM


# why three different retrieval methods?

That part is important, because Vector search, BM25, and Entity matching are 
three different ways of interpreting the same user query. 
They complement each other rather than doing the same thing.

For a GraphRAG system, I would think of them like this:

                         User Query
                             │
          ┌──────────────────┼──────────────────┐
          ▼                  ▼                  ▼
    Vector Search          BM25          Entity Matching
          │                  │                  │
  "What is semantically   "What words       "Which graph
     similar?"             occur?"          entities are named?"
          │                  │                  │
          └──────────────────┼──────────────────┘
                             ▼
                         Result Fusion
                             │                   
                             ▼
                         Seed Nodes

## 1. Vector search

This answers:

"Which graph nodes are semantically similar to the query?"

Suppose the query is:

"How does Microsoft build software for multiple operating systems?"

The query embedding might retrieve:

.NET              0.92
.NET Core         0.90
C#                0.86
.NET 10           0.84
Visual Studio     0.78
Java              0.61

It can find .NET even though the query doesn't literally contain the word .NET.

That's its biggest advantage.

For your GraphRAG, I'd have a vector index containing representations of:

Node
├── Name
├── Type
└── Description

and potentially:

Document Chunk
Community

## 2. BM25

BM25 answers a different question:

"Which graph documents/nodes contain important words from the query?"

For:

"How does Microsoft build software for multiple operating systems?"

BM25 might strongly favor:

Microsoft
software
operating systems
.NET
cross-platform

It is excellent for exact terminology, names, acronyms, identifiers, versions, etc.

For example, if the query is:

"What is .NET 10?"

BM25 is extremely useful because .NET 10 is a very specific term.

Vector search might retrieve:

.NET 10       0.96
.NET 9        0.94
.NET 8        0.93
.NET          0.91
.NET Core     0.88

BM25 can give .NET 10 a particularly strong signal because it matches the exact token.

## 3. Entity matching

This is different again.

Entity matching asks:

"Does the query explicitly refer to entities that exist in my graph?"

For example:

"What relationship exists between Microsoft and .NET 10?"

Entity extraction might identify:

Microsoft → Entity ID 123
.NET 10   → Entity ID 456

Now you have something extremely valuable:

Microsoft ───────?───────> .NET 10

You can search the graph directly for relationships between these entities.

This can be much more precise than semantic search.

Why have all three?

Consider this query:

"What is Microsoft's relationship with the .NET 10 runtime?"

Vector search

Might return:

.NET
.NET 10
.NET Runtime
Microsoft
.NET Core

Good semantic understanding.

BM25

Might return:

.NET 10
Microsoft
.NET 10 Runtime
Microsoft .NET documentation

Good lexical matching.

Entity matching

Could identify:

Microsoft
.NET 10

with exact graph IDs.

That's extremely useful because now you can immediately inspect:

Microsoft
│
├── develops ──> .NET
│
└── develops ──> .NET 10

The important part: they shouldn't necessarily return the same type of result

This is where I would slightly modify my previous architecture.

I would have:

                         Query
                           │
              ┌────────────┼────────────┐
              ▼            ▼            ▼
          Vector          BM25       Entity
          Search         Search      Matching
              │            │            │
              ▼            ▼            ▼
          Node IDs       Node IDs     Node IDs
              │            │            │
              └────────────┼────────────┘
                           ▼
                       RRF / Fusion
                           │
                           ▼
                       Seed Nodes

But Entity Matching deserves special treatment.

I wouldn't simply throw it into RRF in every case.

For example, if the query explicitly says:

"What products are developed by Microsoft?"

and you identify:

Microsoft → node 123

then node 123 should probably become a high-confidence seed, regardless of whether vector search ranked it #1.

Then you can expand:

Microsoft
│
├── develops ──> .NET
├── develops ──> C#
├── develops ──> TypeScript
└── owns ──────> GitHub

This is one of the places where GraphRAG differs fundamentally from ordinary RAG.

I would therefore use two kinds of seeds

1. Explicit seeds

Entities explicitly identified in the query:
Microsoft
.NET 10
GitHub

2. Semantic seeds

Nodes found through retrieval:
.NET
C#
Visual Studio
.NET Runtime

Then combine them:

                    Query
                      │
        ┌─────────────┴─────────────┐
        │                           │
    Explicit entities          Semantic retrieval
        │                           │
        ▼                           ▼
    High-confidence seeds       Candidate seeds
        │                           │
        └─────────────┬─────────────┘
                      ▼
                  Seed ranking
                      │
                      ▼
               Graph expansion

This is particularly powerful for your use case.

There is another important distinction

Entity matching can be used before retrieval and after retrieval.

1. Before retrieval

Extract:
"Microsoft"
".NET 10"

Then resolve them to graph nodes.
This gives you anchors.

2. After retrieval

Suppose vector search gives you:
.NET 10
.NET
.NET Runtime
ASP.NET Core

You can check whether these candidates correspond to known graph entities and resolve them to canonical IDs.

This helps eliminate duplicate representations:

".NET 10"
"Microsoft .NET 10"
".NET Framework 10"

if your entity-resolution system knows that some of these refer to the same entity—or, importantly, knows when they don't.

And this leads to an important architectural point

Since you're implementing your own GraphRAG engine in C#, I would separate these concepts:

Query Processing
│
├── Query Parser / Analyzer
│
├── Entity Extractor
│
├── Entity Resolver
│
└── Query Embedding
│
▼
Retrieval
│
├── Vector Retriever
├── BM25 Retriever
└── Entity Retriever
│
▼
Fusion
│
└── Seed Selection
│
▼
Graph Retrieval
│
├── Neighborhood Expansion
├── Path Search
└── Personalized PageRank
│
▼
Reranking
│
▼
Subgraph
│
▼
LLM

That separation will become very useful later when you implement more sophisticated queries such as:

"Which technologies developed by Microsoft are used by products that run on Linux?"

Here, pure vector search isn't enough. You need to recognize entities and then reason over graph paths:

Microsoft
│
develops
▼
Technology
│
used-by
▼
Product
│
runs-on
▼
Linux

So I'd consider Entity Matching not merely another retrieval method, 
but the bridge between natural-language query understanding and your graph query engine.

And this connects very nicely to the 
Cypher → AST → Binder → Logical Plan → Physical Plan architecture 
you've been building: eventually, the entity matcher can provide 
the bound graph entities that your logical plan operates on.
