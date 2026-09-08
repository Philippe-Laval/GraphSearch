# Here's the plan (grouped by responsibility, not implementation) — 8 subfolders under Analysis\:

Subfolder			Contents
Model\				Records/enums: AnalyzedQuery, QueryIntent, QueryAnalysis, QueryAnalysisImproved, ExtractedEntity, EntityDefinition, GraphQueryPattern
Analyzers\			IQueryAnalyzer + all 12 implementations + Analyzers.md
EntityExtraction\	IEntityExtractor, EntityExtractor, DictionaryEntityExtractor(Factory)
Ner\				INerService, IHuggingFaceTokenizer, Spacy/Azure/HF services + related .md files
Language\			ILanguageDetector, HeuristicLanguageDetector
Rewriting\			IQueryRewriter, SynonymQueryRewriter
Lexicons\			WhWordLexicon, RelationalVerbLexicon, wh-word.md
Llm\				IChatCompletionClient

Namespaces will match folder paths (per the convention set earlier). 
A single GlobalUsings.cs re-exports the 8 sub-namespaces so cross-cutting references 
(both inside Analysis\ and from GraphRagQueryProcessor / Resolution\) require zero per-file using edits.

# Grouping rationale (best practices applied)

•	By responsibility, not implementation. All Xxx*NerService classes live together under Ner\ regardless of vendor (spaCy vs Azure vs HF-ONNX). Same for the analyzers.
•	Interfaces sit with their implementations. Common .NET convention — IQueryAnalyzer in Analyzers\, INerService in Ner\, etc. Easier to discover than a separate Interfaces\ folder.
•	Model\ is dependency-free (except Query.Resolution for ResolvedEntity). Every other folder can safely reference Model — no cycles.
•	Docs travel with the code they describe (Analyzers.md in Analyzers\, HuggingFaceOnnxNer.md in Ner\, wh-word.md in Lexicons\).
•	Namespaces mirror folders (GraphSearch.Library.Query.Analysis.Analyzers, etc.) — consistent with the convention you set earlier.

# Zero-friction cross-namespace access

Two new GlobalUsings.cs files (one in GraphSearch\, one in GraphSearch.Library.Tests\) re-export all 8 sub-namespaces. 
This means:
•	No churn on existing consumer files (GraphRagQueryProcessor, Query.Resolution\*, tests).
•	New files inside Analysis\ sub-folders don't need cross-folder using boilerplate.
•	The folder split is purely organizational — IntelliSense and reference tooling see the same flat surface as before.