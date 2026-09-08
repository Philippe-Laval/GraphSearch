"""
spaCy NER microservice for GraphSearch.

Endpoints:
    GET  /health                 -> { "status": "ok", "languages": [...] }
    POST /analyze                -> NerAnalysis (see schema below)

Request:
    {
      "text": "Which products developed by Microsoft run on Linux?",
      "language": "en"        # or "fr"; if omitted, defaults to "en"
    }

Response (matches the C# NerAnalysis record):
    {
      "lemma": "which product develop by Microsoft run on Linux",
      "entities": [
        { "text": "Microsoft", "type": "ORG", "start": 30, "length": 9, "confidence": 1.0 },
        { "text": "Linux",     "type": "MISC","start": 50, "length": 5, "confidence": 1.0 }
      ],
      "rootVerbs": ["develop", "run"],
      "interrogativeLemma": "which"    # always normalized to the English lemma
    }

Design notes:
- Both English (en_core_web_sm) and French (fr_core_news_sm) models are loaded
  lazily on first use.
- French wh-words are translated to their canonical English lemma so downstream
  C# code (NerQueryAnalyzer) can use one intent table for all supported languages.
- spaCy does not expose per-entity confidence in the small pipelines, so we
  report 1.0. Swap in a transformer pipeline (_trf) if you need calibrated scores.
"""

from __future__ import annotations

from functools import lru_cache
from typing import Optional

import spacy
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field

# --- Model loading -----------------------------------------------------------

# name -> spaCy pipeline id
_MODELS = {
    "en": "en_core_web_sm",
    "fr": "fr_core_news_sm",
}


@lru_cache(maxsize=len(_MODELS))
def _load(language: str):
    if language not in _MODELS:
        raise HTTPException(
            status_code=400,
            detail=f"Unsupported language '{language}'. Supported: {sorted(_MODELS)}",
        )
    try:
        return spacy.load(_MODELS[language])
    except OSError as ex:  # model not installed
        raise HTTPException(
            status_code=500,
            detail=(
                f"spaCy model '{_MODELS[language]}' is not installed. "
                f"Run: python -m spacy download {_MODELS[language]}"
            ),
        ) from ex


# --- Wh-word normalization ---------------------------------------------------

# English wh-words (already canonical).
_EN_WH = {"what", "who", "whom", "whose", "which", "when", "where", "why", "how"}

# English multi-word cues detected on the *first two* lowercased tokens.
_EN_WH_MULTI = {
    ("how", "many"): "how many",
    ("how", "much"): "how much",
    ("how", "does"): "how does",
    ("how", "do"): "how do",
}

# French wh-word lemma -> English canonical lemma.
_FR_WH_MAP = {
    "qui": "who",
    "que": "what",
    "quoi": "what",
    "quel": "which",
    "lequel": "which",
    "quand": "when",
    "où": "where",
    "pourquoi": "why",
    "comment": "how",
    "combien": "how many",  # "combien de X" is always aggregation
}

# French multi-word cues, again on the first two lowercased tokens.
_FR_WH_MULTI = {
    ("combien", "de"): "how many",
    ("comment", "est"): "how does",
    ("comment", "sont"): "how do",
}


def _detect_wh(doc, language: str) -> Optional[str]:
    if len(doc) == 0:
        return None

    first = doc[0].lower_
    second = doc[1].lower_ if len(doc) > 1 else ""

    multi_table = _EN_WH_MULTI if language == "en" else _FR_WH_MULTI
    canonical = multi_table.get((first, second))
    if canonical is not None:
        return canonical

    if language == "en":
        # English: rely on POS tag + fallback to the lowercased text.
        for tok in doc[:3]:
            if tok.tag_ in {"WDT", "WP", "WP$", "WRB"}:
                return tok.lemma_.lower()
            if tok.lower_ in _EN_WH:
                return tok.lower_
        return None

    # French: use lemma lookup on the first few tokens.
    for tok in doc[:3]:
        lemma = tok.lemma_.lower()
        mapped = _FR_WH_MAP.get(lemma) or _FR_WH_MAP.get(tok.lower_)
        if mapped:
            return mapped
    return None


# --- Root-verb extraction ----------------------------------------------------

def _root_verbs(doc) -> list[str]:
    verbs: list[str] = []
    for tok in doc:
        if tok.pos_ == "VERB":
            lemma = tok.lemma_.lower()
            if lemma and lemma not in verbs:
                verbs.append(lemma)
    return verbs


# --- Entity extraction -------------------------------------------------------

def _entities(doc) -> list[dict]:
    out: list[dict] = []
    for ent in doc.ents:
        out.append({
            "text": ent.text,
            "type": ent.label_,
            "start": ent.start_char,
            "length": ent.end_char - ent.start_char,
            "confidence": 1.0,
        })
    return out


# --- FastAPI app -------------------------------------------------------------

class AnalyzeRequest(BaseModel):
    text: str = Field(..., min_length=1)
    language: Optional[str] = Field(default="en")


class Entity(BaseModel):
    text: str
    type: Optional[str]
    start: int
    length: int
    confidence: float


class AnalyzeResponse(BaseModel):
    lemma: Optional[str]
    entities: list[Entity]
    rootVerbs: list[str]
    interrogativeLemma: Optional[str]


app = FastAPI(title="GraphSearch spaCy NER", version="1.0.0")


@app.get("/health")
def health() -> dict:
    return {"status": "ok", "languages": sorted(_MODELS)}


@app.post("/analyze", response_model=AnalyzeResponse)
def analyze(req: AnalyzeRequest) -> AnalyzeResponse:
    language = (req.language or "en").lower()
    nlp = _load(language)

    doc = nlp(req.text)

    lemma_tokens = [tok.lemma_ for tok in doc if not tok.is_space]
    lemma = " ".join(lemma_tokens).strip() or None

    return AnalyzeResponse(
        lemma=lemma,
        entities=_entities(doc),
        rootVerbs=_root_verbs(doc),
        interrogativeLemma=_detect_wh(doc, language),
    )
