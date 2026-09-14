> I am developping with C#. I am working on a GraphRAG project. I need extract structured information from chunk. What is best practices ? Is it possible to extract data without using a LLM ?

Yes. In a GraphRAG pipeline, you can absolutely extract structured information from chunks without an LLM. Whether you *should* depends mostly on how predictable your documents and ontology are.

For a C# project, I’d recommend separating extraction into a pipeline like:

**chunk → candidate extraction → normalization → relationship extraction → validation → graph upsert**

Microsoft GraphRAG itself now supports this idea: its standard indexing uses an LLM for entity/relationship extraction, but its **FastGraphRAG** path uses NLP instead for graph extraction to reduce LLM usage. ([Microsoft sur GitHub][1])

### 1. Define your graph schema before extracting

Don't start by asking “what information can I extract?” Start with a constrained ontology, for example:

```csharp
public record Entity(
    string Id,
    string Name,
    string Type,
    string? Description,
    string SourceChunkId);

public record Relationship(
    string SourceId,
    string TargetId,
    string Type,
    string? Evidence,
    double Confidence);
```

For an industrial corpus, your types might be:

```text
Equipment
Component
Material
Process
Person
Organization
Location
Standard
Document
FailureMode
```

and relationships:

```text
PART_OF
MANUFACTURED_BY
LOCATED_AT
USES_MATERIAL
COMPLIES_WITH
CAUSES
REPAIRED_BY
DEPENDS_ON
```

This constrained schema makes everything downstream dramatically easier.

Microsoft GraphRAG follows essentially this pattern: extracted entities have a title/type/description, and relationships have source/target/description. It then merges subgraphs extracted from individual text units. ([Microsoft sur GitHub][1])

### 2. For deterministic information, don't use an LLM

A surprising amount of useful graph data can be extracted reliably with ordinary C#.

For example:

```text
Pump P-101 was installed at Plant A in 2024.
It operates at 12 bar.
P-101 uses motor M-200.
```

You can extract:

```text
Entity: P-101        type=Pump
Entity: Plant A      type=Location
Entity: M-200        type=Motor

P-101 --LOCATED_AT--> Plant A
P-101 --USES--> M-200

Property:
P-101.pressure = 12 bar
P-101.installationYear = 2024
```

with combinations of:

**Regex** — excellent for IDs, dates, measurements, standards, references, serial numbers.

```csharp
var equipmentRegex =
    new Regex(@"\b[A-Z]{1,4}-\d{2,6}\b");

var pressureRegex =
    new Regex(@"(?<value>\d+(?:\.\d+)?)\s*(?<unit>bar|psi|kPa|MPa)",
        RegexOptions.IgnoreCase);
```

**Dictionaries / gazetteers** — excellent when the set of possible concepts is known.

```csharp
var equipmentTerms = new Dictionary<string, string>
{
    ["pump"] = "Pump",
    ["compressor"] = "Compressor",
    ["motor"] = "Motor",
    ["valve"] = "Valve"
};
```

**Rule-based patterns** — often extremely effective for technical documents.

For instance:

```text
X is part of Y
X consists of Y
X contains Y
```

can all map to:

```text
X --PART_OF / CONTAINS--> Y
```

depending on direction.

### 3. NLP models are the middle ground

You don't have to choose between regex and GPT.

You can run classical NLP/ML locally for:

* named entity recognition
* POS tagging
* dependency parsing
* sentence segmentation
* coreference
* entity similarity
* relation classification

Typical architecture:

```text
Chunk
  ↓
Sentence splitter
  ↓
NER
  ↓
Entity normalization
  ↓
Dependency parser / relation classifier
  ↓
Graph
```

Microsoft's FastGraphRAG is conceptually similar: NLP replaces LLM-based extraction for portions of indexing. ([Microsoft sur GitHub][1])

In .NET, I'd also consider **ONNX Runtime**. You can run Hugging Face-style NER or classification models locally from C# without operating a Python service.

For example:

```text
BERT / DeBERTa NER model
        ↓
      ONNX
        ↓
Microsoft.ML.OnnxRuntime
        ↓
        C#
```

That can be a very attractive production architecture if you want deterministic cost and on-premise inference.

### 4. Entity resolution is actually more important than entity extraction

This is one of the biggest GraphRAG implementation traps.

Suppose three chunks contain:

```text
Microsoft Corporation
Microsoft
MSFT
```

If you blindly create three nodes, your graph deteriorates rapidly.

I recommend an entity pipeline such as:

```text
raw mention
    ↓
canonicalization
    ↓
alias lookup
    ↓
exact identifier match
    ↓
fuzzy/string similarity
    ↓
embedding similarity
    ↓
(optional LLM adjudication)
    ↓
canonical entity
```

For example:

```csharp
public record EntityMention(
    string Text,
    string Type,
    int Start,
    int End,
    string ChunkId);

public record CanonicalEntity(
    Guid Id,
    string Name,
    string Type,
    IReadOnlyCollection<string> Aliases);
```

Keep **mentions and canonical entities separate**. That's especially valuable for provenance and debugging.

### 5. Keep the evidence for every edge

For GraphRAG, I strongly recommend never creating:

```text
A → B
```

alone.

Store:

```text
A → B
relationship = PART_OF
chunkId = 8fb...
sentence = "Motor M-200 is part of Pump P-101."
confidence = 0.97
extractionMethod = RuleBased
```

This lets you answer:

> Why does the graph say that?

and lets you reprocess bad extraction later.

GraphRAG similarly keeps `text_unit_ids` associated with entities and relationships, preserving where graph information came from. ([GitHub][2])

### 6. Use confidence scores

Don't treat every extraction as equally trustworthy.

For example:

```text
Exact regex pattern          1.00
Known dictionary match       0.98
NER model                    0.90
Relation classifier          0.85
Fuzzy entity resolution      0.75
LLM inferred relationship    0.70
```

Then you can establish thresholds:

```csharp
if (candidate.Confidence >= 0.90)
    Accept(candidate);
else if (candidate.Confidence >= 0.70)
    QueueForValidation(candidate);
else
    Reject(candidate);
```

The actual thresholds should come from evaluation data, rather than hard-coding those example values.

### 7. A hybrid extractor is usually the strongest architecture

For most production systems, I would **not** do:

```text
chunk → LLM → JSON → database
```

for everything.

Instead:

```text
                   ┌── Regex
                   │
                   ├── Dictionaries
Chunk ─────────────┼── NER model
                   │
                   ├── Domain rules
                   │
                   └── LLM fallback
                         ↓
                    Normalizer
                         ↓
                  Entity resolution
                         ↓
                      Graph
```

Use an LLM only for ambiguous semantics.

For example:

```text
"The overheating appears to result from insufficient lubrication."
```

Extracting

```text
Insufficient lubrication
          ↓ CAUSES
Overheating
```

requires substantially more semantic understanding than finding:

```text
Pump P-104
ISO 9001
12.5 bar
2026-03-12
```

So I'd reserve the expensive model for the first case.

### 8. Structured output matters when you do use an LLM

When an LLM is used, avoid extracting arbitrary prose and subsequently parsing it.

Give it a strict JSON schema such as:

```json
{
  "entities": [
    {
      "name": "P-101",
      "type": "Equipment"
    }
  ],
  "relationships": [
    {
      "source": "P-101",
      "target": "M-200",
      "type": "USES",
      "evidence": "P-101 uses motor M-200."
    }
  ]
}
```

Then deserialize directly into C# records:

```csharp
public sealed record ExtractionResult(
    IReadOnlyList<ExtractedEntity> Entities,
    IReadOnlyList<ExtractedRelationship> Relationships);

public sealed record ExtractedEntity(
    string Name,
    string Type);

public sealed record ExtractedRelationship(
    string Source,
    string Target,
    string Type,
    string Evidence);
```

Current Microsoft GraphRAG documentation specifically notes that models used with its pipeline need reliable structured outputs conforming to the expected JSON schema. ([Microsoft sur GitHub][3])

### 9. Chunking deserves careful attention

Extraction accuracy is strongly affected by chunk boundaries.

Bad:

```text
Chunk 1:
"The P-101 pump is connected to"

Chunk 2:
"the cooling loop C-12."
```

Now neither chunk contains the full relationship.

Good GraphRAG chunking therefore generally needs:

```text
Document
 ↓
Sections
 ↓
Paragraphs/sentences
 ↓
semantic chunks
 ↓
small overlap
```

Microsoft GraphRAG supports token- or sentence-based chunking, configurable chunk size and overlap, and it can prepend document metadata to each chunk. ([GitHub][4])

For extraction, I prefer preserving sentence boundaries rather than blindly taking every N tokens.

### 10. One architecture I'd recommend for your C# project

Something along these lines scales nicely:

```text
                       ┌────────────────────┐
                       │ Document ingestion │
                       └─────────┬──────────┘
                                 │
                         ┌───────▼───────┐
                         │   Chunking    │
                         └───────┬───────┘
                                 │
               ┌─────────────────▼─────────────────┐
               │      Structured Extraction        │
               │                                   │
               │  Regex / dictionary / NER / rules │
               │                  │                │
               │              LLM fallback         │
               └─────────────────┬─────────────────┘
                                 │
                       ┌─────────▼─────────┐
                       │ Entity resolution │
                       └─────────┬─────────┘
                                 │
                       ┌─────────▼─────────┐
                       │    Validation     │
                       └─────────┬─────────┘
                                 │
                       ┌─────────▼─────────┐
                       │  Knowledge graph  │
                       └─────────┬─────────┘
                                 │
                   ┌─────────────▼─────────────┐
                   │ Embeddings + communities │
                   └─────────────┬─────────────┘
                                 │
                            GraphRAG
```

And importantly, GraphRAG doesn't require you to use its own entity extractor. Microsoft documents a **Bring Your Own Graph** workflow where you provide `entities`, `relationships`, and optionally `text_units`, then run GraphRAG's later community/summarization stages. ([GitHub][2])

So for a C# application, **you could build the whole extraction pipeline yourself in .NET and use GraphRAG only downstream**.

My default recommendation would therefore be:

**Regex/rules for highly structured domain information → local NER/ONNX for entities → deterministic normalization/entity resolution → LLM only for hard relationships/claims.**

That typically gives better cost, latency, reproducibility, and debuggability than sending every chunk through an LLM, while retaining LLM-quality semantic extraction where it actually adds value.

[1]: https://microsoft.github.io/graphrag/index/default_dataflow/?utm_source=chatgpt.com "Dataflow - GraphRAG"
[2]: https://github.com/microsoft/graphrag/blob/main/docs/index/byog.md?utm_source=chatgpt.com "graphrag/docs/index/byog.md at main · microsoft/graphrag · GitHub"
[3]: https://microsoft.github.io/graphrag/config/models/?utm_source=chatgpt.com "Language Model Selection - GraphRAG"
[4]: https://github.com/microsoft/graphrag/blob/main/docs/config/yaml.md?utm_source=chatgpt.com "graphrag/docs/config/yaml.md at main · microsoft/graphrag · GitHub"
