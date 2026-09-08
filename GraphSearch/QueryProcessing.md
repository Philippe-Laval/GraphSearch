# Can you help me code in C# these steps : Query Processing │ ├── Query Parser / Analyzer │ ├── Entity Extractor │ ├── Entity Resolver │ └── Query Embedding

For your GraphRAG architecture, I would make these four components independent, 
because later you can replace the LLM-based entity extractor, the embedding provider, or the entity resolver 
without changing the rest of the pipeline.

I would structure it like this:

GraphRag/
└── Query/
├── Analysis/
│   ├── QueryAnalyzer.cs
│   ├── QueryAnalysis.cs
│   ├── EntityExtractor.cs
│   └── ExtractedEntity.cs
│
├── Resolution/
│   ├── EntityResolver.cs
│   ├── ResolvedEntity.cs
│   └── IEntityIndex.cs
│
├── Embeddings/
│   ├── QueryEmbeddingService.cs
│   └── IEmbeddingService.cs
│
└── GraphRagQueryProcessor.cs


Natural language
		│
		▼
┌──────────────────┐
│ Query Analyzer   │
└───────┬──────────┘
		│
		├───────────────┐
		▼               ▼
Entity extraction    Query normalization
		│
		▼
┌──────────────────┐
│ Entity Resolver  │
└───────┬──────────┘
		│
		▼
Graph entity IDs
		│
		└───────────────┐
						▼
				┌─────────────────┐
				│ Query Embedding │
				└───────┬─────────┘
						│
						▼
				QueryAnalysisResult



