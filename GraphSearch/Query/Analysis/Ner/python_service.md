# Python spaCy microservice + a C# SpacyNerService

I'll build this as a small Python spaCy microservice + a C# SpacyNerService : 
INerService that talks to it over HTTP. 

Language selection is passed from C# (uses your existing ILanguageDetector), 
and the service normalizes wh-words to English lemmas so NerQueryAnalyzer's 
intent map works unchanged for FR queries.

Python microservice — python/spacy-ner/
-   app.py — FastAPI /analyze endpoint. Loads en_core_web_sm and fr_core_news_sm lazily via lru_cache. Extracts:
-	lemma — full lemmatized query
-	entities — from doc.ents (label, char span)
-	rootVerbs — lemmas of every VERB token, dedup'd, insertion order preserved
-	interrogativeLemma — detected from the first 3 tokens (POS tags for EN, lemma lookup for FR) with special-case handling of multi-word cues (how many, combien de, etc.)
-	French wh-words are normalized to their English canonical lemma (combien → how many, pourquoi → why, …), so NerQueryAnalyzer's intent table works unchanged for FR queries.
-	requirements.txt — FastAPI, uvicorn, spaCy, pydantic.
-	README.md — install + run instructions.

C# client — SpacyNerService : INerService
-	Talks to the service via HttpClient (base address set by caller).
-	Language selection: explicit overload → ILanguageDetector → configured default.
-	Deserializes into your existing NerAnalysis / ExtractedEntity types.

## Quick setup

```
cd python\spacy-ner
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
python -m spacy download en_core_web_sm
python -m spacy download fr_core_news_sm
uvicorn app:app --host 127.0.0.1 --port 8080
```

Wire it into your analyzer pipeline

```
var httpClient = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:8080/") };

INerService ner = new SpacyNerService(
    httpClient,
    languageDetector: new HeuristicLanguageDetector(),   // routes EN/FR automatically
    defaultLanguage: "en");

IQueryAnalyzer analyzer = new NerQueryAnalyzer(ner);
```

Query samples that should now classify correctly across both languages:
-	"Quels produits développés par Microsoft fonctionnent sur Linux ?" → MultiHopRelationship (2 verbs + 2 entities)
-	"Combien de produits Microsoft ?" → Aggregation (combien → how many)
-	"Pourquoi .NET est-il rapide ?" → Explanation (pourquoi → why)
-	"Qui a créé Linux ?" → EntityLookup (qui → who + 1 entity)
