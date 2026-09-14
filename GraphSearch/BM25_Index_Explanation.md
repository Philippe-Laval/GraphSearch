> What is a BM25 Index. How can i create one in c# ?

BM25 (Best Matching 25) is one of the most important ranking algorithms in Information Retrieval. It is the de facto standard used by search engines like Elasticsearch, OpenSearch, Lucene, and many RAG systems before vector search became popular.

For a **GraphRAG** architecture, BM25 is actually extremely useful because it complements vector search:

* **Vector search** finds semantically similar chunks.
* **BM25** finds chunks containing the exact keywords.
* **Graph traversal** finds related entities.

Many production GraphRAG systems combine all three.

---

# What is BM25?

Suppose you have 1 million document chunks.

When the user asks:

> "How does Louvain community detection work?"

BM25 computes a relevance score for every chunk based on:

* which query words appear
* how many times they appear
* how rare those words are
* document length

Unlike TF-IDF, BM25 avoids giving too much importance to repeated words.

The simplified formula is

$$
score(D,Q)=\sum_{t \in Q} IDF(t)\cdot
\frac{f(t,D)(k_1+1)}
{f(t,D)+k_1(1-b+b\cdot |D|/avgdl)}
$$

Where

* **f(t,D)** = frequency of word t in document D
* **IDF(t)** = inverse document frequency
* **|D|** = document length
* **avgdl** = average document length
* **k1** ≈ 1.2–2.0
* **b** ≈ 0.75

---

# Example

Suppose you have

Chunk A

```
Louvain algorithm is a community detection algorithm.
```

Chunk B

```
Leiden is an improvement over Louvain.
```

Query

```
community detection Louvain
```

BM25 gives

```
Chunk A -> 15.7
Chunk B -> 8.2
```

because Chunk A contains all important words.

---

# Creating a BM25 index

A BM25 index stores

```
Word
 ├── document frequency
 ├── posting list
      ├── Doc 10 -> frequency=3
      ├── Doc 24 -> frequency=1
      ├── Doc 35 -> frequency=2
```

Example

```
louvain

Document Count = 250

Posting List

Doc 10 : 4
Doc 31 : 1
Doc 55 : 2
```

When searching

```
community detection
```

you only inspect documents containing those words.

---

# Data structures in C#

A simple representation might be:

```csharp
public class Posting
{
    public int DocumentId { get; set; }
    public int Frequency { get; set; }
}

public class TermInfo
{
    public int DocumentFrequency { get; set; }

    public List<Posting> Postings { get; } = new();
}

public class Bm25Index
{
    public Dictionary<string, TermInfo> Terms { get; } = new();

    public Dictionary<int, int> DocumentLengths { get; } = new();

    public int DocumentCount { get; set; }

    public double AverageDocumentLength { get; set; }
}
```

---

# Building the index

Suppose each chunk is

```csharp
public record Chunk(
    int Id,
    string Text);
```

Tokenize

```csharp
var words = Regex.Matches(text.ToLower(), @"\w+")
                 .Select(m => m.Value);
```

Count frequencies

```csharp
var frequencies = words
    .GroupBy(x => x)
    .ToDictionary(g => g.Key, g => g.Count());
```

Then update the index

```csharp
foreach (var (term, freq) in frequencies)
{
    if (!index.Terms.TryGetValue(term, out var info))
    {
        info = new TermInfo();
        index.Terms[term] = info;
    }

    info.DocumentFrequency++;

    info.Postings.Add(new Posting
    {
        DocumentId = chunk.Id,
        Frequency = freq
    });
}
```

Store

```csharp
index.DocumentLengths[chunk.Id] = words.Count();
```

Finally compute

```csharp
index.AverageDocumentLength =
    index.DocumentLengths.Values.Average();
```

---

# Searching

For every query term

```csharp
foreach(var term in queryTerms)
{
    var posting = index.Terms[term];

    foreach(var doc in posting.Postings)
    {
        score += BM25(...);
    }
}
```

Only documents in the posting list are scored.

Complexity is

```
O(number of matching documents)
```

instead of

```
O(all documents)
```

---

# BM25 function

```csharp
double Score(
    int frequency,
    int docLength,
    int documentFrequency,
    int documentCount,
    double averageDocLength,
    double k1 = 1.5,
    double b = 0.75)
{
    double idf =
        Math.Log(
            1 +
            (documentCount - documentFrequency + 0.5) /
            (documentFrequency + 0.5));

    double numerator =
        frequency * (k1 + 1);

    double denominator =
        frequency +
        k1 * (1 - b + b * docLength / averageDocLength);

    return idf * numerator / denominator;
}
```

---

# Should you implement your own?

For learning or for a custom GraphRAG engine, implementing BM25 yourself is very feasible—it can be done in a few hundred lines of C#.

For production, I recommend using a mature library because they handle:

* efficient compressed posting lists
* incremental updates
* stemming
* stop words
* phrase queries
* Boolean queries
* Unicode tokenization
* optimized scoring

Good options include:

* **Lucene.NET** (the .NET port of Apache Lucene), which provides a high-performance inverted index with BM25 ranking out of the box.
* **Elasticsearch** or **OpenSearch**, which use Lucene internally and expose BM25 through a REST API.

---

# How BM25 fits into a modern GraphRAG pipeline

Given the GraphRAG architecture we've discussed, a common retrieval flow is:

```
                 Documents
                      │
          Chunking (500–1000 tokens)
                      │
      ┌───────────────┴────────────────┐
      │                                │
 Vector embeddings                BM25 index
      │                                │
 Vector DB                    Inverted index
      └───────────────┬────────────────┘
                      │
              Hybrid Retrieval
           (Reciprocal Rank Fusion)
                      │
           Entity extraction / Graph
                      │
            Graph expansion (1–2 hops)
                      │
           Rerank → Context → LLM
```

This hybrid approach combines:

* **BM25** for precise keyword matching (e.g., identifiers, API names, class names, error messages).
* **Vector search** for semantic similarity.
* **Graph traversal** for retrieving related entities and relationships.

For technical documentation, codebases, and knowledge graphs, this combination consistently outperforms using any single retrieval method alone.

> What is Hybrid Retrieval   (Reciprocal Rank Fusion). Can you show C# code ?

Reciprocal Rank Fusion (RRF) is one of the simplest and most effective techniques for **hybrid retrieval**. It is used by many production search systems (including Azure AI Search, Elasticsearch, and OpenSearch) because it combines multiple ranked result lists without requiring score normalization.

For a GraphRAG system, **RRF is the approach I would recommend**. It lets you combine:

* **BM25** (exact keyword matching)
* **Vector search** (semantic similarity)
* **Graph search** (entity neighborhood or path expansion)

---

# Why not simply add the scores?

Suppose you have these scores:

## BM25

| Document | Score |
| -------- | ----: |
| A        |  18.2 |
| B        |  12.1 |
| C        |   4.8 |

## Vector Search

| Document | Score |
| -------- | ----: |
| C        |  0.93 |
| A        |  0.89 |
| D        |  0.87 |

These scores are **not comparable**:

* BM25 ranges from 0 to 100+.
* Cosine similarity ranges from 0 to 1.
* Graph scores may be based on path length, PageRank, or edge weights.

Adding them directly would produce arbitrary results.

---

# Reciprocal Rank Fusion

Instead of using the raw scores, RRF uses only the **rank** of each result.

The formula is:

$$
\text{RRF}(d)=\sum_i \frac{1}{k+r_i(d)}
$$

Where:

* **k** is typically **60**
* **r** is the rank (1 for first place, 2 for second, etc.)
* Missing documents contribute 0.

---

## Example

### BM25 ranking

| Rank | Document |
| ---: | -------- |
|    1 | A        |
|    2 | B        |
|    3 | C        |

### Vector ranking

| Rank | Document |
| ---: | -------- |
|    1 | C        |
|    2 | A        |
|    3 | D        |

With **k = 60**:

### Document A

```
1/(60+1)
+
1/(60+2)

=
0.01639
+
0.01613

=
0.03252
```

### Document C

```
1/(60+3)
+
1/(60+1)

=
0.01587
+
0.01639

=
0.03226
```

### Document B

```
1/(60+2)

=
0.01613
```

### Document D

```
1/(60+3)

=
0.01587
```

Final ranking:

| Document |  Score |
| -------- | -----: |
| A        | 0.0325 |
| C        | 0.0323 |
| B        | 0.0161 |
| D        | 0.0159 |

Notice that **A** wins because it ranks highly in **both** retrieval methods.

---

# Generic C# implementation

Let's define a result type:

```csharp
public sealed record SearchResult(
    int DocumentId,
    double Score);
```

Each search engine returns:

```csharp
List<SearchResult>
```

For example:

```csharp
var bm25 = new List<SearchResult>
{
    new(1,18.2),
    new(2,12.1),
    new(3,4.8)
};

var vector = new List<SearchResult>
{
    new(3,0.93),
    new(1,0.89),
    new(4,0.87)
};
```

---

## RRF implementation

```csharp
public static class ReciprocalRankFusion
{
    public static IReadOnlyList<SearchResult> Fuse(
        params IReadOnlyList<SearchResult>[] rankings)
    {
        const double k = 60;

        var scores = new Dictionary<int,double>();

        foreach (var ranking in rankings)
        {
            for (int rank = 0; rank < ranking.Count; rank++)
            {
                var docId = ranking[rank].DocumentId;

                scores.TryAdd(docId, 0);

                scores[docId] +=
                    1.0 / (k + rank + 1);
            }
        }

        return scores
            .OrderByDescending(x => x.Value)
            .Select(x => new SearchResult(
                x.Key,
                x.Value))
            .ToList();
    }
}
```

Usage:

```csharp
var results = ReciprocalRankFusion.Fuse(
    bm25,
    vector);

foreach(var r in results)
{
    Console.WriteLine($"{r.DocumentId} {r.Score}");
}
```

Output:

```
1
3
2
4
```

---

# Weighted RRF

Sometimes you want BM25 to have more influence than vector search.

The weighted formula is:

$$
\sum_i w_i \cdot \frac{1}{k+r_i}
$$

Example weights:

| Source | Weight |
| ------ | -----: |
| BM25   |    1.0 |
| Vector |    0.8 |
| Graph  |    1.4 |

Implementation:

```csharp
public static IReadOnlyList<SearchResult> FuseWeighted(
    IEnumerable<(double Weight, IReadOnlyList<SearchResult> Ranking)> inputs)
{
    const double k = 60;

    var scores = new Dictionary<int,double>();

    foreach (var (weight, ranking) in inputs)
    {
        int rank = 1;

        foreach (var result in ranking)
        {
            scores.TryAdd(result.DocumentId, 0);

            scores[result.DocumentId] +=
                weight / (k + rank);

            rank++;
        }
    }

    return scores
        .OrderByDescending(x => x.Value)
        .Select(x => new SearchResult(
            x.Key,
            x.Value))
        .ToList();
}
```

---

# Applying RRF to GraphRAG

In a GraphRAG pipeline, you often have three independent retrieval stages:

```text
                 User Query
                      │
      ┌───────────────┼────────────────┐
      │               │                │
      ▼               ▼                ▼
   BM25          Vector Search     Graph Search
      │               │                │
      │               │                │
      └───────────────┼────────────────┘
                      ▼
            Reciprocal Rank Fusion
                      │
               Top 100 candidates
                      │
         Cross-encoder / LLM reranker
                      │
               Top 10–20 chunks
                      │
              Context for the LLM
```

The graph search branch can retrieve chunks linked to entities near the entities mentioned in the query (for example, expanding one or two hops from a matched node). RRF naturally combines those graph-derived candidates with lexical and semantic retrieval.

---

# Scaling to millions of chunks

With millions of chunks, you **do not** run RRF over every document. Each retrieval subsystem returns only its top candidates:

* BM25 → top 100
* Vector search → top 100
* Graph search → top 100

RRF then merges at most **300 candidate results**, which is effectively an **O(N)** operation over the returned candidates, not over the entire corpus.

---

## An architecture I would recommend for your .NET GraphRAG

Based on your earlier questions about building a GraphRAG engine in C#, I would structure retrieval as a set of interchangeable providers:

```csharp
public interface IRetriever
{
    Task<IReadOnlyList<SearchResult>> RetrieveAsync(
        string query,
        int topK,
        CancellationToken cancellationToken = default);
}
```

Implementations would include:

* `Bm25Retriever`
* `VectorRetriever`
* `GraphRetriever`

A `HybridRetriever` would invoke these retrievers (potentially in parallel), apply weighted RRF to their ranked results, and return the fused candidate list. This design keeps each retrieval strategy independent and makes it easy to add future retrievers (such as metadata filtering or SQL retrieval) without changing the fusion logic.


> Can you implement Bm25Retriever using Lucene.NET


Yes. In fact, **Lucene.NET is exactly what I would use** for the BM25 part of a GraphRAG implementation in C#. It already implements:

* BM25 scoring
* inverted index
* phrase queries
* Boolean queries
* stop words
* analyzers
* incremental indexing
* very fast retrieval (millions of documents)

You should think of Lucene as your **lexical retrieval engine**, while your vector database (or HNSW index) handles semantic retrieval.

---

# NuGet packages

```text
Lucene.Net
Lucene.Net.Analysis.Common
Lucene.Net.QueryParser
```

Version 4.8.0-beta is currently the latest Lucene.NET release.

---

# Model

I usually store much more than just the text.

```csharp
public sealed record ChunkDocument(
    Guid Id,
    string DocumentId,
    string Text,
    string[] EntityIds,
    Dictionary<string,string>? Metadata = null);
```

The **Guid** identifies the chunk.

---

# SearchResult

```csharp
public sealed record SearchResult(
    Guid ChunkId,
    double Score);
```

---

# IRetriever

```csharp
public interface IRetriever
{
    Task<IReadOnlyList<SearchResult>> RetrieveAsync(
        string query,
        int topK,
        CancellationToken cancellationToken = default);
}
```

---

# Creating the index

```csharp
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

public sealed class LuceneIndex
{
    public IndexWriter Writer { get; }

    public LuceneIndex(string folder)
    {
        var directory = FSDirectory.Open(folder);

        var analyzer =
            new StandardAnalyzer(LuceneVersion.LUCENE_48);

        var config =
            new IndexWriterConfig(
                LuceneVersion.LUCENE_48,
                analyzer);

        Writer = new IndexWriter(directory, config);
    }
}
```

---

# Indexing chunks

```csharp
using Lucene.Net.Documents;

public async Task IndexAsync(IEnumerable<ChunkDocument> chunks)
{
    foreach (var chunk in chunks)
    {
        var doc = new Document
        {
            new StringField(
                "ChunkId",
                chunk.Id.ToString(),
                Field.Store.YES),

            new StringField(
                "DocumentId",
                chunk.DocumentId,
                Field.Store.YES),

            new TextField(
                "Content",
                chunk.Text,
                Field.Store.YES),

            new StringField(
                "Entities",
                string.Join(",", chunk.EntityIds),
                Field.Store.YES)
        };

        Writer.AddDocument(doc);
    }

    Writer.Commit();

    await Task.CompletedTask;
}
```

Notice the use of:

* `TextField` → tokenized (used for BM25)
* `StringField` → exact value (IDs, metadata)

---

# Enable BM25

Lucene uses a similarity implementation.

```csharp
config.Similarity = new BM25Similarity(
    k1: 1.2f,
    b: 0.75f);
```

Those are the standard parameters.

---

# Bm25Retriever

```csharp
using Lucene.Net.Search;
using Lucene.Net.QueryParsers.Classic;

public sealed class Bm25Retriever : IRetriever
{
    private readonly IndexSearcher _searcher;
    private readonly QueryParser _parser;

    public Bm25Retriever(string folder)
    {
        var directory = FSDirectory.Open(folder);

        var reader =
            DirectoryReader.Open(directory);

        _searcher = new IndexSearcher(reader);

        _searcher.Similarity =
            new BM25Similarity();

        _parser =
            new QueryParser(
                LuceneVersion.LUCENE_48,
                "Content",
                new StandardAnalyzer(
                    LuceneVersion.LUCENE_48));
    }

    public Task<IReadOnlyList<SearchResult>> RetrieveAsync(
        string query,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var luceneQuery =
            _parser.Parse(query);

        var topDocs =
            _searcher.Search(
                luceneQuery,
                topK);

        var results =
            new List<SearchResult>();

        foreach (var hit in topDocs.ScoreDocs)
        {
            var doc =
                _searcher.Doc(hit.Doc);

            results.Add(
                new SearchResult(
                    Guid.Parse(doc.Get("ChunkId")),
                    hit.Score));
        }

        return Task.FromResult<IReadOnlyList<SearchResult>>(results);
    }
}
```

Usage:

```csharp
var retriever =
    new Bm25Retriever("Index");

var results =
    await retriever.RetrieveAsync(
        "community detection using Louvain",
        20);
```

---

# Improving queries

Instead of parsing the entire query as a single string, you can build richer Lucene queries.

Example:

```csharp
var builder = new BooleanQuery();

builder.Add(
    new TermQuery(new Term("Content", "louvain")),
    Occur.SHOULD);

builder.Add(
    new TermQuery(new Term("Content", "community")),
    Occur.SHOULD);

builder.Add(
    new PhraseQuery
    {
        new Term("Content", "community"),
        new Term("Content", "detection")
    },
    Occur.SHOULD);

var query = builder;
```

This gives you more control over ranking than relying solely on `QueryParser`.

---

# Incremental indexing

One of Lucene's strengths is that you don't need to rebuild the index every time.

```csharp
Writer.UpdateDocument(
    new Term("ChunkId", chunk.Id.ToString()),
    document);

Writer.Commit();
```

Deletes are equally simple:

```csharp
Writer.DeleteDocuments(
    new Term("ChunkId", chunkId));
```

---

# Filtering by metadata

You can combine content search with exact metadata filters.

For example:

```text
Content contains "Leiden"
AND
DocumentId = "paper123"
```

using:

```csharp
var query = new BooleanQuery
{
    {
        new TermQuery(new Term("DocumentId", "paper123")),
        Occur.FILTER
    },
    {
        new QueryParser(
            LuceneVersion.LUCENE_48,
            "Content",
            analyzer)
            .Parse("Leiden"),
        Occur.MUST
    }
};
```

`FILTER` clauses don't affect the BM25 score—they simply restrict the candidate set.

---

# Integrating with GraphRAG

Given the architecture you've been designing over the last few questions, I would store enough information in each Lucene document to bridge lexical search with your graph and vector layers:

```text
Chunk
├── ChunkId
├── DocumentId
├── Content
├── EntityIds
├── CommunityId
├── SectionId
├── Metadata...
```

The retrieval pipeline would then look like:

```text
             Query
               │
      ┌────────┴────────┐
      │                 │
   Lucene BM25      Vector Search
      │                 │
      └────────┬────────┘
               │
        Reciprocal Rank Fusion
               │
        Top ~100 chunk IDs
               │
      Expand entities in graph
               │
      Fetch neighboring chunks
               │
      Cross-encoder reranker
               │
         Final LLM context
```

One enhancement I'd strongly recommend for a production GraphRAG is to make `Bm25Retriever` **return the stored `ChunkDocument` (or at least its metadata) in addition to the score**, rather than just the `ChunkId`. That avoids an extra lookup after retrieval and gives the graph expansion stage immediate access to entity IDs, community IDs, document IDs, and other metadata needed to continue the pipeline.

> What is a Cross-encoder reranker ? Can you write some C# code ?


A **Cross-Encoder reranker** is one of the most important components of a high-quality RAG or GraphRAG system. If I had to rank the impact of retrieval components on answer quality, it would roughly be:

1. **Hybrid retrieval (BM25 + Vector + Graph)** → finds good candidates.
2. **Cross-encoder reranker** → orders those candidates by true relevance.
3. **LLM** → generates the final answer.

Many people stop after hybrid retrieval, but the reranker often provides the biggest boost in retrieval precision.

---

# The problem

Suppose the user asks:

> "How does the Louvain algorithm optimize modularity?"

Your retrievers return:

| Rank | Chunk                                                |
| ---: | ---------------------------------------------------- |
|    1 | Louvain is a graph clustering algorithm.             |
|    2 | Leiden improves Louvain stability.                   |
|    3 | Modularity optimization is the objective of Louvain. |
|    4 | Community detection has many applications.           |

Although chunk 3 is the best answer, it wasn't ranked first.

---

## Why vector search makes mistakes

Vector search embeds the **query**:

```
How does Louvain optimize modularity?
```

and each **chunk** independently:

```
Chunk A:
Louvain is a graph clustering algorithm.
```

The similarity is then computed with cosine distance:

```
cos(query_embedding, chunk_embedding)
```

The query and chunk never directly interact during encoding.

---

# Cross-Encoder

A Cross-Encoder takes the **query and the chunk together** as a single input:

```
[CLS]

Query:
How does Louvain optimize modularity?

[SEP]

Chunk:
Louvain maximizes modularity by repeatedly moving vertices between communities...

[SEP]
```

The transformer performs attention across **both texts simultaneously**, allowing the model to learn detailed relationships between the query and the candidate passage.

Instead of an embedding, it outputs a single relevance score:

```
0.98
```

Another chunk might score:

```
0.41
```

The reranker sorts candidates by this score.

---

# Retrieval pipeline

A typical GraphRAG pipeline looks like this:

```text
              User Query
                    │
      ┌─────────────┼─────────────┐
      │             │             │
      ▼             ▼             ▼
     BM25        Vector       Graph Search
      │             │             │
      └─────────────┼─────────────┘
                    ▼
           Reciprocal Rank Fusion
                    │
              Top 100 chunks
                    │
                    ▼
          Cross-Encoder Reranker
                    │
               Top 10 chunks
                    │
                    ▼
                  LLM
```

Notice that the reranker **doesn't search**. It only reorders the candidates that have already been retrieved.

---

# Popular reranking models

Some widely used models include:

| Model                                 | Comments                             |
| ------------------------------------- | ------------------------------------ |
| BAAI/bge-reranker-base                | Excellent quality/speed balance      |
| BAAI/bge-reranker-large               | Higher accuracy, slower              |
| mixedbread-ai/mxbai-rerank-base-v1    | Very strong general-purpose reranker |
| cross-encoder/ms-marco-MiniLM-L-6-v2  | Fast and lightweight                 |
| cross-encoder/ms-marco-MiniLM-L-12-v2 | More accurate than L6                |

Unlike embedding models, rerankers return a **single score**, not a vector.

---

# Architecture

A clean design is to define a reranker interface:

```csharp
public interface IReranker
{
    Task<IReadOnlyList<SearchResult>> RerankAsync(
        string query,
        IReadOnlyList<SearchResult> candidates,
        CancellationToken cancellationToken = default);
}
```

Assume `SearchResult` contains both the score and the chunk text:

```csharp
public sealed record SearchResult(
    Guid ChunkId,
    string Text,
    double Score);
```

---

# Using ONNX Runtime

Most modern rerankers are distributed as Hugging Face models. A common production approach in .NET is:

```
Hugging Face
        │
 Optimum Export
        │
     ONNX Model
        │
Microsoft.ML.OnnxRuntime
        │
        C#
```

Required package:

```text
Microsoft.ML.OnnxRuntime
```

---

# Loading the model

```csharp
using Microsoft.ML.OnnxRuntime;

public sealed class OnnxReranker
{
    private readonly InferenceSession _session;

    public OnnxReranker(string modelPath)
    {
        _session = new InferenceSession(modelPath);
    }
}
```

---

# Tokenization

The tokenizer builds a single sequence from the query and chunk:

```
[CLS]

query

[SEP]

chunk

[SEP]
```

This produces the tensors:

```csharp
input_ids
attention_mask
token_type_ids
```

For simplicity, imagine a helper:

```csharp
Encoding encoding =
    tokenizer.Encode(query, chunk);
```

where `Encoding` contains the tensors required by the model.

---

# Running inference

```csharp
var inputs = new List<NamedOnnxValue>
{
    NamedOnnxValue.CreateFromTensor(
        "input_ids",
        encoding.InputIds),

    NamedOnnxValue.CreateFromTensor(
        "attention_mask",
        encoding.AttentionMask),

    NamedOnnxValue.CreateFromTensor(
        "token_type_ids",
        encoding.TokenTypeIds)
};

using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results =
    _session.Run(inputs);

float score =
    results.First()
           .AsEnumerable<float>()
           .First();
```

The output is a single relevance score.

---

# Reranking candidates

```csharp
public async Task<IReadOnlyList<SearchResult>> RerankAsync(
    string query,
    IReadOnlyList<SearchResult> candidates,
    CancellationToken ct = default)
{
    var reranked = new List<SearchResult>();

    foreach (var candidate in candidates)
    {
        double score = Score(query, candidate.Text);

        reranked.Add(
            candidate with { Score = score });
    }

    return reranked
        .OrderByDescending(c => c.Score)
        .ToList();
}
```

The `Score` method performs tokenization and ONNX inference as described above.

---

# Batch inference

Calling the model once per chunk is inefficient. Most rerankers support batching, so instead of:

```
Query + Chunk1
Query + Chunk2
Query + Chunk3
...
```

you prepare a batch:

```
(Query, Chunk1)
(Query, Chunk2)
(Query, Chunk3)
...
```

and score all candidates in a single inference call. This significantly improves throughput on both CPUs and GPUs.

---

# Where it fits in your GraphRAG

Based on the architecture we've been discussing (Lucene.NET for BM25, vector search, graph expansion, and RRF), I would structure the pipeline like this:

```text
                Documents
                     │
          Chunk + Entity Extraction
                     │
        ┌────────────┼────────────┐
        │            │            │
     Lucene       Vector DB    Knowledge Graph
        │            │            │
        └────────────┼────────────┘
                     ▼
          Reciprocal Rank Fusion
                     │
             Top 100 candidates
                     │
        Graph neighborhood expansion
                     │
             Top 50–100 chunks
                     │
          Cross-Encoder Reranker
                     │
              Top 10–20 chunks
                     │
              Prompt Construction
                     │
                    LLM
```

## A recommendation for your .NET implementation

Since you're building a GraphRAG engine in C#, I'd separate reranking from the inference backend:

```csharp
public interface IReranker
{
    Task<IReadOnlyList<SearchResult>> RerankAsync(
        string query,
        IReadOnlyList<SearchResult> candidates,
        CancellationToken cancellationToken = default);
}
```

Then provide implementations such as:

* `OnnxCrossEncoderReranker` (runs locally with ONNX Runtime)
* `OpenAIReranker` (if using an external reranking API in the future)
* `NoOpReranker` (returns candidates unchanged for testing)

This keeps the rest of your retrieval pipeline independent of the specific reranking technology and makes it easy to swap models as better rerankers become available.


