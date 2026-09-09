# NLP layers for GraphRAG query analysis

Great question — this touches several different NLP layers that are often conflated. Let me untangle them.

## 1. NER is not the same as POS tagging or dependency parsing

Three distinct layers, usually produced by three distinct model heads (even inside one spaCy pipeline):

| Layer		  | What it labels | Example on "Microsoft developed .NET in 2002" |
|:------------|:--------| :--------|
| POS tagging     | Word class of each token   | Microsoft/PROPN developed/VERB .NET/PROPN in/ADP 2002/NUM |
| Dependency parsing | Grammatical role + head token | developed = ROOT; Microsoft = nsubj of developed; .NET = obj of developed |
| Named-Entity Recognition (NER)		| Real-world entity type per span	| Microsoft = ORG, .NET = PRODUCT, 2002 = DATE |

The "root verb" I mentioned belongs to dependency parsing, not NER. I overloaded terminology — sorry.
 
## 2. Verb categories
### 2a. POS-level (Universal Dependencies tag set — the modern standard)

| UD tag  | Meaning  | Example  |
| :------ | :------ | :------ |
| VERB  | Lexical verb  | developed, uses, runs |
| AUX  | Auxiliary / modal / copula  | is, has, will, can |
| PART  | Particle attached to verbs  | up in "give up" |
		
Also useful:
-	Lemma vs inflected form: developed → develop; fonctionne → fonctionner.
-	Tense / Aspect / Mood / Voice / Person / Number — spaCy exposes these under token.morph (Tense=Past, Voice=Pass, ...). Very handy for query understanding: "Which products were developed by Microsoft" (past passive) → you want the develops(Microsoft, X) edge, not develops(X, Microsoft).

### 2b. Dependency-level (grammatical function)

| Dep label  | Role  | Notes  |
| :------ | :------ | :------ |
| ROOT  | Main predicate of the sentence  | "developed" in the example |
| xcomp / ccomp  | Clausal complement verb  | "want to use" — use is xcomp of want |
| advcl  | Adverbial clause verb  | "runs on Linux when installed" — installed is advcl |
| acl / acl:relcl  | Relative-clause verb modifying a noun  | "products developed by Microsoft" — developed is acl of products |
| conj  | Coordinated verb  | "developed and maintained" — maintained is conj of developed |

For GraphRAG this is gold: the acl and acl:relcl verbs are exactly the ones that describe edges between the entities in the query (Microsoft --develops--> Product).

### 2c. Semantic-role / lexical categories (verb classes)

Beyond grammar:
-	Transitive vs intransitive: does the verb take a direct object? (develops X vs runs)
-	Stative vs eventive: own is stative (a state), acquire is eventive (a change).
-	Relational vs non-relational: the whole point of my RelationalVerbLexicon — "developed, owns, runs on" express edges; "want, seem, be" don't.
-	VerbNet classes (Levin's verb classes): e.g. create, build, develop, produce → class 26 "verbs of creation". Useful when mapping paraphrases to canonical graph relations.
-	FrameNet frames: "Manufacturing" frame includes build, make, manufacture, produce — all reduce to the same GraphRAG edge type.
 
## 3. Noun categories
### 3a. POS

| UD tag  | Meaning  | Example  |
| :------ | :------ | :------ |
| NOUN  | Common noun  | products, technologies, company |
| PROPN | Proper noun  | Microsoft, Linux, .NET |
| PRON  | Pronoun      | it, they, which |	

### 3b. Dependency roles for nouns

| Dep  | Role  | Why it matters |
| :------ | :------ | :------ |
| nsubj / nsubjpass | Subject (active / passive) | Determines edge direction: "Microsoft develops X" vs "X was developed by Microsoft" |
| obj / iobj | Direct / indirect object | The other endpoint of the edge |
| obl | Oblique (prep. phrase) | Often the second hop: "runs on Linux" |
| nmod | Nominal modifier | "Microsoft's products" — possession / relation |
| compound | Compound noun | "Windows Server" — merge into one entity |
| appos | Apposition | "Satya Nadella, CEO of Microsoft" — coreference |

### 3c. Semantic / NER categories

Coarse-grained (the classic 4-class BIO models like dslim/bert-base-NER):


| Label  | Covers  |
| :------ | :------ |
| PER  | People  |
| ORG  | Companies, agencies, teams |
| LOC / GPE  | Places, countries, cities |
| MISC  | Everything else notable |

Fine-grained (OntoNotes 5 — used by spaCy en_core_web_sm/lg/trf):

PERSON, NORP (nationality/religion), FAC (facility), ORG, GPE, LOC, PRODUCT, EVENT, WORK_OF_ART, LAW, LANGUAGE, DATE, TIME, PERCENT, MONEY, QUANTITY, ORDINAL, CARDINAL.

Very fine-grained / domain-specific:
-	BioNLP: GENE, PROTEIN, DISEASE, DRUG (models like en_ner_bc5cdr_md from scispaCy).
-	Legal: STATUTE, COURT, JUDGE (Blackstone).
-	Finance: TICKER, INSTRUMENT.
-	Your own: Technology, ProgrammingLanguage, OperatingSystem — the categories your DictionaryEntityExtractor already uses.
 
## 4. Other useful groupings for GraphRAG
### 4a. Nominal compounds / multi-word entities

Real entities are often multi-token: "Visual Studio Code", ".NET Runtime". 
spaCy handles this via doc.ents (span-level). Never rely on token-level entity tags.

### 4b. Coreference chains

"Microsoft acquired GitHub. It now runs it." — resolving It → Microsoft and it → GitHub is a separate model 
(spaCy has experimental_coref, HF has coref-roberta). Critical for multi-sentence queries.

### 4c. Numeric / temporal expressions

Detected by NER (DATE, TIME, MONEY, QUANTITY) then normalized by temporal taggers like SUTime or spaCy's entity_ruler+dateparser. 
Enables filters: "products released after 2020" → year >= 2020.

### 4d. Wh-words (interrogatives)

POS-tagged as PRON or ADV with the fine tags I mentioned earlier — WDT (which), WP (who, what), WP$ (whose), 
WRB (when, where, why, how). These pin down query intent.

### 4e. Semantic role labeling (SRL)

The next step up. Given "Microsoft developed .NET", SRL tags roles:
-	ARG0 (Agent) = Microsoft
-	V (Predicate) = developed
-	ARG1 (Patient) = .NET

This directly maps to (subject, predicate, object) triples — perfect for knowledge-graph construction. Libraries: AllenNLP SRL, or LLM-based extractors.

### 4f. Open Information Extraction (OpenIE)

Domain-agnostic triple extraction: "Microsoft, developed, .NET". 
Tools: Stanford OpenIE, MinIE. Less accurate than SRL but no schema needed. 
Good for bootstrapping a graph from unstructured text.

### 4g. Relation Extraction (RE)

Given two entities in a sentence, classify the relation between them. 
This is the piece that turns NER output into graph edges. 
Models: rebel-large (HF), spaCy's experimental_relation_extractor, or LLM few-shot.
 
# 5. Best practices for a GraphRAG query pipeline

Ordered by cost/benefit:
1.	Do NER first to anchor entities → resolve them against your graph. This alone gets you most of the way.
2.	Add POS + dependency parsing for:
-	Detecting root verbs / acl verbs → candidate edge types (my current approximation).
-	Detecting the wh-word and its POS-fine tag → precise intent.
-	Detecting subject vs object → edge direction.
3.	Lemmatize everything before dictionary lookup. Reason: "developed", "develops", "developing" should all collapse to develop. 
	My current RelationalVerbLexicon fakes it with enumerated forms — real POS + lemma is cleaner.
4.	Use spans, not tokens. Merge PROPN + PROPN + PROPN runs (compound entities) and NER doc.ents before matching.
5.	Prefer BIO consistency at the span level. Never rely on the raw model tag of a single subword token — always aggregate 
	(which HuggingFaceOnnxNerService does).
6.	Add coreference resolution as soon as your queries can be multi-sentence.
7.	Move to SRL / RE when the vocabulary of relations you care about is stable enough to warrant a trained model. 
	Until then, dependency-based heuristics + a curated verb → relation map (RelationalVerbLexicon + your RelationshipVocabulary)
	is a very reasonable baseline.
8.	Always store confidence. Every stage (NER, dep parsing, RE) is probabilistic.
	Downstream fusion (e.g., re-ranking with the embedding score) needs those numbers.
9.	Language-agnostic canonical forms. Just like WhWordLexicon maps combien → how many, 
	do the same for verbs: développé → develop. 
	One graph vocabulary, N languages feeding into it.
10.	Golden test set. Even 200 hand-labeled queries per language will catch 90% of regressions when you swap models.
 
# 6. TL;DR: what "root verbs" really meant in my NerQueryAnalyzer

It's a coarse proxy for two things that a real dependency parser would give you separately:
-	The sentence's main predicate (ROOT verb) → primary intent verb.
-	Any acl / acl:relcl verbs → relational verbs describing edges between entities.

If/when you wire spaCy up, you can replace my "collect every VERB" heuristic with something much more precise:

```python
main_predicate = [t for t in doc if t.dep_ == "ROOT" and t.pos_ == "VERB"]
edge_verbs     = [t for t in doc if t.dep_ in {"acl", "acl:relcl", "xcomp"} and t.pos_ == "VERB"]
```

Both then fed through the same RelationshipVocabulary mapping to canonical graph edges. 
That's the clean version of what my Azure/HF backends currently approximate with a static verb dictionary.

