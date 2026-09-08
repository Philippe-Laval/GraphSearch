# GraphSearch spaCy NER microservice

A small FastAPI service exposing spaCy NER + POS + lemmatization for English and
French. Consumed by `SpacyNerService` in the `GraphSearch.Library` project via
`INerService`.

## Install

```powershell
# From this folder
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
python -m spacy download en_core_web_sm
python -m spacy download fr_core_news_sm
```

## Run

```powershell
uvicorn app:app --host 127.0.0.1 --port 8080
```

## Contract

`POST /analyze`

```json
{ "text": "Which products developed by Microsoft run on Linux?", "language": "en" }
```

Response matches the C# `NerAnalysis` record:

```json
{
  "lemma": "which product develop by Microsoft run on Linux",
  "entities": [
	{ "text": "Microsoft", "type": "ORG", "start": 30, "length": 9, "confidence": 1.0 }
  ],
  "rootVerbs": ["develop", "run"],
  "interrogativeLemma": "which"
}
```

`interrogativeLemma` is **normalized to the English canonical lemma** even for
French queries, so `NerQueryAnalyzer` can keep a single intent table:

| Français        | Canonical  |
|-----------------|------------|
| combien (de)    | how many   |
| pourquoi        | why        |
| comment         | how        |
| quel/quelle/... | which      |
| qui             | who        |
| que / quoi      | what       |
| quand           | when       |
| où              | where      |
