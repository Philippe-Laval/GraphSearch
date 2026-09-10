// Re-exports the sub-namespaces of GraphSearch.Library.Query.Analysis so cross-
// cutting types (AnalyzedQuery, QueryIntent, IQueryAnalyzer, ExtractedEntity,
// INerService, ...) resolve without per-file using directives.
//
// This keeps the folder structure (grouping by responsibility) without forcing
// callers to spell out every sub-namespace.
global using GraphSearch.Library.Query.Analysis.Analyzers;
global using GraphSearch.Library.Query.Analysis.EntityExtraction;
global using GraphSearch.Library.Query.Analysis.Language;
global using GraphSearch.Library.Query.Analysis.Lexicons;
global using GraphSearch.Library.Query.Analysis.Llm;
global using GraphSearch.Library.Query.Analysis.Model;
global using GraphSearch.Library.Query.Analysis.Ner;
global using GraphSearch.Library.Query.Analysis.PhraseChunkService;
global using GraphSearch.Library.Query.Analysis.Rewriting;
