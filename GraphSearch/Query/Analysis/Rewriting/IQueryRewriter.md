# IQueryRewriter Best Practices

Here are the best practices for implementing IQueryRewriter in a GraphRAG setting.

# 1. Purpose — why rewrite at all

Rewriting exists to bridge the vocabulary gap between how users ask and how documents/graph nodes are indexed. 
It boosts recall in hybrid retrieval (BM25 + vector + graph traversal) without requiring the user to know the corpus vocabulary. 
A good rewriter is additive: you keep the original query and add complementary phrasings — never replace.

# 2. Core design principles

| Principle  | What it means  |
| :------ | :------ |
| Preserve intent, vary surface  | Rewrites must be paraphrases, not new questions. Never introduce entities/facts absent from the input. |
| Return the original too  | The first entry of the result should be the input query (or its normalized form). Downstream code should never have to remember to add it back. |
| Deterministic when possible  | Cache-friendliness matters — same input → same output. LLM-based rewriters should use temperature 0 (or seed) and be wrapped in a cache. |
| Bounded fan-out  | Cap rewrites (typically 3–8). Retrieval cost grows linearly with the number of variants. |
| Diversify, don't duplicate  | Post-filter near-duplicates (normalized string equality, edit distance, or embedding cosine). Two identical rewrites waste a retrieval slot. |
| Fail open  | On any error, return [query]. Rewriting is an enhancement, never a hard dependency. |
| Idempotent normalization first  | Trim, casefold, unify quotes/dashes before rewriting so the cache key is stable. |
| Language-aware  | Detect language and rewrite in the same language (or produce a cross-lingual variant deliberately, not accidentally). |


# 3. Rewriting strategies (worth having as separate implementations)
1.	Synonym / lexical substitution — the one you already have (SynonymQueryRewriter). Cheap, deterministic, no external calls. Ideal baseline.
2.	Template / rule — regex → template ("who made X" → "who developed X", "who created X", "creator of X"). Great for question-form normalization.
3.	Stemming/lemmatization variants — feed both surface and lemma form to a BM25 index that isn't already stemmed.
4.	Entity expansion — replace an entity mention with its aliases from the knowledge graph ("MS" → "Microsoft", "the Redmond company"). Requires an entity index.
5.	HyDE (Hypothetical Document Embeddings) — LLM generates a fake ideal answer paragraph; you embed that paragraph and retrieve against it. Powerful for vector recall on sparse queries.
6.	Multi-Query / Query2Doc — LLM generates N diverse rephrasings of the question, retrieval runs on all, results are fused (RRF). Well-documented in RAG literature.
7.	Sub-query decomposition — LLM breaks a multi-hop question ("Who founded the company that makes .NET?") into steps. This is really a planner, so if you go this route, expose it via a separate interface (IQueryDecomposer) rather than overloading IQueryRewriter.
8.	Graph-schema-aware rewriting — take the parsed GraphQueryPattern (which you already emit) and re-verbalize it. Keeps rewrites grounded in the graph vocabulary.
9.	Back-translation — translate to another language and back. Free paraphrases when you already have an MT model.
10.	LLM few-shot paraphraser — the general-purpose fallback. Costly, so cache aggressively.

# 4. Composition patterns (the real payoff)
-	CompositeQueryRewriter — run several rewriters in parallel, union their outputs, then dedupe/cap.
-	CachingQueryRewriter — decorator keyed by the normalized query.
-	FallbackQueryRewriter — try the LLM first; if it throws or exceeds a timeout, fall back to synonym/template.
-	ResultFusionRewriter (RRF) — if you fan out to N rewrites during retrieval, apply Reciprocal Rank Fusion downstream. Rewriter itself just supplies variants; fusion is retrieval-side.
-	ThrottlingQueryRewriter — bounded concurrency + timeout around remote calls, so a slow LLM never blocks the pipeline beyond a fixed budget.

# 5. Practical contract improvements to consider
Your current interface returns only IReadOnlyList<string>. In production RAG you eventually want richer output:

```csharp

public sealed record QueryRewrite(
    string Text,
    RewriteKind Kind,      // Synonym, Template, LlmParaphrase, HyDE, EntityAlias, ...
    double Weight = 1.0,   // downstream fusion weight
    string? Language = null);

Task<IReadOnlyList<QueryRewrite>> RewriteAsync(string query, CancellationToken ct = default);
```

Reasons: HyDE variants should probably only feed the vector retriever (not BM25); entity-alias rewrites should get higher weight in graph traversal; downstream fusion needs Kind/Weight to weight results correctly. You can add this later without breaking IQueryRewriter — introduce IQueryRewriter2 and keep the current one as a shim.

# 6. Testing / evaluation
-	Golden set — 30–100 hand-labelled (query, expected_rewrites) pairs. Assert containment, not exact match.
-	Semantic preservation — cosine-sim between original and each rewrite embedding should stay above a threshold (e.g. 0.75). Catches LLM hallucinations that drift off topic.
-	Retrieval lift metric — measure Recall@k with and without rewrites on an evaluation corpus. That's the only metric that ultimately matters.
-	Negative tests — inject prompt-injection strings ("ignore previous instructions…") to ensure the rewriter doesn't propagate them.

# 7. Common pitfalls to avoid
-	Rewriting entity names. Never substitute "Microsoft" with "Google" because they're both ORG. Freeze detected entities/spans before rewriting.
-	Over-rewriting short queries. For 1–2 token queries paraphrasing rarely helps; skip and let the retriever handle it.
-	Cascade explosions. If Composite calls Composite, cap total rewrites globally, not per level.
-	Language drift. Log detected language of each rewrite and reject mismatches unless cross-lingual is opted in.
-	Cache leakage. Cache key must include language, options, and (if applicable) the rewriter version — otherwise a config change silently serves stale variants.

If you'd like, I can implement a few of these next — a CompositeQueryRewriter, a CachingQueryRewriter, 
and either an LlmQueryRewriter or a HydeQueryRewriter are the highest-leverage additions to complement 
your existing SynonymQueryRewriter.
