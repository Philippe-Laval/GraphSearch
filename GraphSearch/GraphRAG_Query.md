# To query the top K elements of a graph in C# and .NET 10

> I am programming in C# and .net 10. I have a graph of data. I am trying to create a graph rag. How to query the k top elements of the graph given a query ?

Yes. For a GraphRAG system, I would **not** try to find the top-K graph nodes directly from the graph structure. The usual approach is a **two-stage retrieval pipeline**:

```text
User query
    │
    ▼
Query embedding
    │
    ├───────────────┐
    ▼               ▼
Vector retrieval   Lexical retrieval (BM25)
    │               │
    └───────┬───────┘
            ▼
       Hybrid ranking
            │
            ▼
      Top-K seed nodes
            │
            ▼
     Graph expansion
     (neighbors / paths)
            │
            ▼
     Reranking / scoring
            │
            ▼
     Relevant subgraph
            │
            ▼
           LLM
```

The important distinction is:

> **Top-K retrieval gives you the starting nodes; graph traversal gives you the context around those nodes.**

### 1. Give every graph entity a searchable representation

For example:

```csharp
public sealed record GraphNode(
    long Id,
    string Type,
    string Name,
    string Description);
```

Suppose your graph contains:

```text
Microsoft
   │
   ├── develops ──> .NET
   │                   │
   │                   └── has-version ──> .NET 10
   │
   └── develops ──> C#
```

You create a deterministic text representation:

```text
Type: Technology
Name: .NET
Description: Cross-platform development platform from Microsoft.
```

Then calculate an embedding for this text.

I would store embeddings for at least:

* **nodes**
* **documents/chunks**
* **communities**

Edges can also have embeddings, but I would not make them your primary retrieval mechanism initially.

---

## 2. Retrieve the initial Top-K

Given:

```text
"What technologies does Microsoft use for cross-platform development?"
```

create an embedding:

```csharp
var queryEmbedding = await embeddingService
    .GenerateEmbeddingAsync(query);
```

Then search your vector index:

```csharp
var candidates = await vectorIndex.SearchAsync(
    queryEmbedding,
    topK: 20);
```

You might get:

```text
.NET       0.91
C#         0.87
Microsoft  0.84
.NET Core  0.81
Azure      0.62
Java       0.31
```

**Don't immediately give the top 5 to the LLM.**

The next step is much more important for GraphRAG.

---

# 3. Expand the Top-K through the graph

Suppose `.NET` and `C#` are your strongest nodes.

Retrieve their neighborhood:

```text
             Microsoft
              /     \
         develops   develops
            /         \
          .NET        C#
            |
       has-version
            |
          .NET 10
```

For example:

```csharp
public async Task<IReadOnlyCollection<GraphNode>> ExpandAsync(
    IEnumerable<long> seedNodeIds,
    int maxDepth)
{
    var visited = new HashSet<long>();
    var result = new List<GraphNode>();

    foreach (var seedId in seedNodeIds)
    {
        await ExpandNodeAsync(
            seedId,
            depth: 0,
            maxDepth,
            visited,
            result);
    }

    return result;
}
```

But I strongly recommend **not doing unrestricted BFS**.

Instead, use a weighted expansion.

---

# 4. Score neighboring nodes

Suppose the query retrieved:

```text
.NET      semantic = 0.91
C#        semantic = 0.87
```

Now `.NET` has these neighbors:

```text
Microsoft
.NET 10
ASP.NET
NuGet
Visual Studio
```

You can score each neighbor using several signals:

```text
Score =
    α × semantic_similarity
  + β × graph_relevance
  + γ × lexical_score
  + δ × edge_relevance
  + ε × community_relevance
```

For example:

```csharp
public sealed record NodeScore(
    long NodeId,
    double SemanticScore,
    double LexicalScore,
    double GraphScore,
    double EdgeScore,
    double FinalScore);
```

And:

```csharp
double CalculateScore(
    double semantic,
    double lexical,
    double graph,
    double edge)
{
    return
        0.50 * semantic +
        0.15 * lexical +
        0.20 * graph +
        0.15 * edge;
}
```

The exact weights should eventually be tuned against your evaluation dataset.

---

# 5. A very useful technique: Personalized PageRank

For GraphRAG, I would seriously consider **Personalized PageRank**.

The vector search identifies your seed nodes:

```text
.NET
C#
Microsoft
```

You use those nodes as the personalization vector.

Then PageRank propagates relevance through the graph:

```text
.NET ────────> .NET 10
 │
 ├───────────> ASP.NET
 │
 └───────────> NuGet

C# ──────────> Roslyn
 │
 └───────────> Visual Studio
```

Nodes close to the highly relevant seeds receive higher scores.

Conceptually:

```text
Query
  │
  ▼
Embedding search
  │
  ▼
Top 20 seeds
  │
  ▼
Personalized PageRank
  │
  ▼
Top 50 graph nodes
```

This works particularly well because it combines:

**semantic relevance + graph structure.**

---

# 6. Even better: combine vector search + BM25 + graph ranking

Since you were previously looking at BM25 and Reciprocal Rank Fusion, I would use that architecture here.

You can have:

```text
                     Query
                       │
          ┌────────────┼────────────┐
          ▼            ▼            ▼
     Vector search   BM25      Entity matching
          │            │            │
          └────────────┼────────────┘
                       ▼
                      RRF
                       │
                       ▼
                 Seed nodes
                       │
                       ▼
              Graph expansion
                       │
                       ▼
            Personalized PageRank
                       │
                       ▼
                   Reranker
                       │
                       ▼
                Final subgraph
```

For example:

```csharp
public sealed record RetrievalResult(
    long NodeId,
    double VectorScore,
    double Bm25Score,
    double RrfScore,
    double GraphScore,
    double FinalScore);
```

RRF can be:

```csharp
public static double RrfScore(
    int vectorRank,
    int bm25Rank,
    int k = 60)
{
    return
        1.0 / (k + vectorRank) +
        1.0 / (k + bm25Rank);
}
```

This is much more robust than relying exclusively on embeddings.

---

# 7. Don't return only nodes — return a subgraph

This is one of the most important design decisions.

If the query is:

> "Who developed technology X and what products use it?"

Returning:

```text
Technology X
Person A
Product B
```

is insufficient.

You want:

```text
Person A
   │
 developed
   ▼
Technology X
   │
   ├── used-by ──> Product B
   │
   └── owned-by ─> Company C
```

So your retrieval result should be something like:

```csharp
public sealed class GraphRagResult
{
    public IReadOnlyList<GraphNode> Nodes { get; init; } = [];
    public IReadOnlyList<GraphEdge> Edges { get; init; } = [];
}
```

The **edges are part of the retrieved context**.

---

# 8. I would use several retrieval levels

For the GraphRAG architecture you've been developing, I'd actually use **three indexes**:

```text
                    Query
                      │
        ┌─────────────┼──────────────┐
        ▼             ▼              ▼
     Chunk index   Node index   Community index
        │             │              │
        └─────────────┼──────────────┘
                      ▼
                 Fusion/Rerank
                      │
                      ▼
                 Seed entities
                      │
                      ▼
                Graph traversal
```

### Chunk index

Useful for:

```text
"What exactly does the document say?"
```

### Node index

Useful for:

```text
"Which entities are relevant?"
```

### Community index

Useful for:

```text
"What is the overall topic?"
"What are the main concepts related to X?"
```

This is especially useful when your graph becomes large.

---

# 9. A practical C# architecture

Given that you're building this in .NET 10, I'd structure it roughly like this:

```text
GraphRag/
│
├── Retrieval/
│   ├── IGraphRetriever.cs
│   ├── HybridGraphRetriever.cs
│   ├── VectorRetriever.cs
│   ├── Bm25Retriever.cs
│   ├── RrfFusion.cs
│   └── GraphExpander.cs
│
├── Ranking/
│   ├── IGraphRanker.cs
│   ├── PersonalizedPageRank.cs
│   ├── NodeScorer.cs
│   └── Reranker.cs
│
├── Graph/
│   ├── GraphNode.cs
│   ├── GraphEdge.cs
│   ├── IGraphStore.cs
│   └── IGraphTraversal.cs
│
├── Embeddings/
│   ├── IEmbeddingService.cs
│   └── IVectorIndex.cs
│
└── Query/
    ├── GraphRagQuery.cs
    └── GraphRagResult.cs
```

Then the central API can be very simple:

```csharp
public interface IGraphRagRetriever
{
    Task<GraphRagResult> RetrieveAsync(
        string query,
        int topK,
        CancellationToken cancellationToken = default);
}
```

Implementation:

```csharp
public async Task<GraphRagResult> RetrieveAsync(
    string query,
    int topK,
    CancellationToken cancellationToken = default)
{
    // 1. Semantic retrieval
    var vectorResults =
        await _vectorRetriever.SearchAsync(
            query,
            20,
            cancellationToken);

    // 2. BM25 retrieval
    var lexicalResults =
        await _bm25Retriever.SearchAsync(
            query,
            20,
            cancellationToken);

    // 3. Fuse results
    var seeds = _rrfFusion.Combine(
        vectorResults,
        lexicalResults);

    // 4. Expand graph around seeds
    var expanded =
        await _graphExpander.ExpandAsync(
            seeds.Take(10),
            maxDepth: 2,
            cancellationToken);

    // 5. Graph ranking
    var ranked =
        await _graphRanker.RankAsync(
            query,
            seeds,
            expanded,
            cancellationToken);

    // 6. Select final subgraph
    return _subgraphBuilder.Build(
        ranked.Take(topK));
}
```

---

## What I would recommend for your project

Since you're already working on **BM25, vector embeddings, RRF, reranking, communities and a Cypher-like query engine**, I would make your GraphRAG retrieval pipeline:

```text
                    Natural language query
                              │
                              ▼
                     Query understanding
                              │
               ┌──────────────┼──────────────┐
               ▼              ▼              ▼
          Vector search      BM25       Entity lookup
               │              │              │
               └──────────────┼──────────────┘
                              ▼
                         RRF fusion
                              │
                              ▼
                     20-50 seed nodes
                              │
                              ▼
                 Graph neighborhood expansion
                        depth 1-2
                              │
                              ▼
                  Personalized PageRank
                              │
                              ▼
                       Cross encoder
                         reranking
                              │
                              ▼
                    Top 20-100 nodes
                              │
                              ▼
                  Connected subgraph
                              │
                              ▼
                  Deterministic projection
                              │
                              ▼
                             LLM
```

The key idea is that **K should not mean "K nodes found by a vector search."** Instead:

> **K = the number of final relevant graph elements after semantic retrieval + graph expansion + graph ranking + reranking.**

For example, you might retrieve **20 semantic seeds**, expand to **500 graph nodes**, rank those 500, and finally keep **50 nodes + their relevant edges** as the GraphRAG context.

That architecture scales much better and produces much more coherent answers than simply taking the top-K vector results.

If you want, the next useful step would be to implement a complete **`GraphRagRetriever` in C#/.NET 10**, including **vector search → BM25 → RRF → graph expansion → Personalized PageRank → top-K subgraph**, using interfaces so you can plug in your existing graph database and ChromaDB/Lucene.NET components.


> Can you code the two steps : Graph neighborhood expansion ( depth 1-2 ) and Personalized PageRank in C# ?

Yes. I would implement these as **two independent components**:

1. `GraphNeighborhoodExpander` — starts from your retrieved seed nodes and expands 1–2 hops.
2. `PersonalizedPageRank` — takes the resulting subgraph and propagates the query relevance from the seed nodes.

This fits nicely with the GraphRAG architecture we've been discussing.

## 1. Basic graph model

I'll assume a directed graph, but the PageRank implementation can treat edges as undirected if that's more appropriate for your GraphRAG.

```csharp
public sealed record GraphNode(
    long Id,
    string Type,
    string Name);

public sealed record GraphEdge(
    long SourceId,
    long TargetId,
    string Type,
    double Weight = 1.0);

public sealed class Graph
{
    private readonly Dictionary<long, GraphNode> _nodes = [];
    private readonly List<GraphEdge> _edges = [];

    private readonly Dictionary<long, List<GraphEdge>> _outgoing = [];

    public IReadOnlyDictionary<long, GraphNode> Nodes => _nodes;
    public IReadOnlyList<GraphEdge> Edges => _edges;

    public void AddNode(GraphNode node)
    {
        _nodes[node.Id] = node;
    }

    public void AddEdge(GraphEdge edge)
    {
        _edges.Add(edge);

        if (!_outgoing.TryGetValue(edge.SourceId, out var edges))
        {
            edges = [];
            _outgoing[edge.SourceId] = edges;
        }

        edges.Add(edge);
    }

    public IReadOnlyList<GraphEdge> GetOutgoingEdges(long nodeId)
    {
        return _outgoing.TryGetValue(nodeId, out var edges)
            ? edges
            : [];
    }
}
```

---

# 2. Neighborhood expansion

The first step is:

```text
                    Seed
                     │
             ┌───────┼───────┐
             ▼       ▼       ▼
            A        B       C       depth 1
           / \      / \       \
          D   E    F   G       H     depth 2
```

If your seed is `X`, with `maxDepth = 2`, we return:

```text
X
A B C
D E F G H
```

But importantly, we also return the **edges**, because the final GraphRAG context needs relationships.

### Result model

```csharp
public sealed class GraphNeighborhood
{
    public required IReadOnlySet<long> NodeIds { get; init; }

    public required IReadOnlyList<GraphEdge> Edges { get; init; }
}
```

### Expander

```csharp
public sealed class GraphNeighborhoodExpander
{
    private readonly Graph _graph;

    public GraphNeighborhoodExpander(Graph graph)
    {
        _graph = graph;
    }

    public GraphNeighborhood Expand(
        IEnumerable<long> seedNodeIds,
        int maxDepth = 2)
    {
        if (maxDepth < 0)
            throw new ArgumentOutOfRangeException(nameof(maxDepth));

        var seeds = seedNodeIds
            .Distinct()
            .ToArray();

        var visited = new HashSet<long>(seeds);

        var frontier = new HashSet<long>(seeds);

        var selectedEdges = new List<GraphEdge>();

        for (var depth = 0; depth < maxDepth; depth++)
        {
            var nextFrontier = new HashSet<long>();

            foreach (var nodeId in frontier)
            {
                foreach (var edge in _graph.GetOutgoingEdges(nodeId))
                {
                    selectedEdges.Add(edge);

                    if (visited.Add(edge.TargetId))
                    {
                        nextFrontier.Add(edge.TargetId);
                    }
                }
            }

            frontier = nextFrontier;

            if (frontier.Count == 0)
                break;
        }

        return new GraphNeighborhood
        {
            NodeIds = visited,
            Edges = selectedEdges
        };
    }
}
```

For a GraphRAG system, however, I recommend supporting **both outgoing and incoming edges**.

For example:

```text
Microsoft ──develops──> .NET
```

If `.NET` is your seed, an outgoing-only traversal won't find `Microsoft`.

So let's improve it.

---

# 3. Bidirectional expansion

Add incoming-edge support to `Graph`:

```csharp
public sealed class Graph
{
    private readonly Dictionary<long, GraphNode> _nodes = [];
    private readonly List<GraphEdge> _edges = [];

    private readonly Dictionary<long, List<GraphEdge>> _outgoing = [];
    private readonly Dictionary<long, List<GraphEdge>> _incoming = [];

    public IReadOnlyDictionary<long, GraphNode> Nodes => _nodes;
    public IReadOnlyList<GraphEdge> Edges => _edges;

    public void AddNode(GraphNode node)
    {
        _nodes[node.Id] = node;
    }

    public void AddEdge(GraphEdge edge)
    {
        _edges.Add(edge);

        if (!_outgoing.TryGetValue(edge.SourceId, out var outgoing))
        {
            outgoing = [];
            _outgoing[edge.SourceId] = outgoing;
        }

        outgoing.Add(edge);

        if (!_incoming.TryGetValue(edge.TargetId, out var incoming))
        {
            incoming = [];
            _incoming[edge.TargetId] = incoming;
        }

        incoming.Add(edge);
    }

    public IReadOnlyList<GraphEdge> GetOutgoingEdges(long nodeId)
        => _outgoing.TryGetValue(nodeId, out var edges)
            ? edges
            : [];

    public IReadOnlyList<GraphEdge> GetIncomingEdges(long nodeId)
        => _incoming.TryGetValue(nodeId, out var edges)
            ? edges
            : [];
}
```

Now the expander can have a direction:

```csharp
public enum GraphTraversalDirection
{
    Outgoing,
    Incoming,
    Both
}
```

And:

```csharp
public sealed class GraphNeighborhoodExpander
{
    private readonly Graph _graph;

    public GraphNeighborhoodExpander(Graph graph)
    {
        _graph = graph;
    }

    public GraphNeighborhood Expand(
        IEnumerable<long> seedNodeIds,
        int maxDepth = 2,
        GraphTraversalDirection direction =
            GraphTraversalDirection.Both)
    {
        if (maxDepth < 0)
            throw new ArgumentOutOfRangeException(nameof(maxDepth));

        var seeds = seedNodeIds
            .Distinct()
            .ToArray();

        var visited = new HashSet<long>(seeds);
        var frontier = new HashSet<long>(seeds);
        var selectedEdges = new HashSet<GraphEdge>();

        for (var depth = 0; depth < maxDepth; depth++)
        {
            var nextFrontier = new HashSet<long>();

            foreach (var nodeId in frontier)
            {
                if (direction is
                    GraphTraversalDirection.Outgoing or
                    GraphTraversalDirection.Both)
                {
                    VisitEdges(
                        _graph.GetOutgoingEdges(nodeId),
                        visited,
                        nextFrontier,
                        selectedEdges,
                        edge => edge.TargetId);
                }

                if (direction is
                    GraphTraversalDirection.Incoming or
                    GraphTraversalDirection.Both)
                {
                    VisitEdges(
                        _graph.GetIncomingEdges(nodeId),
                        visited,
                        nextFrontier,
                        selectedEdges,
                        edge => edge.SourceId);
                }
            }

            frontier = nextFrontier;

            if (frontier.Count == 0)
                break;
        }

        return new GraphNeighborhood
        {
            NodeIds = visited,
            Edges = selectedEdges.ToArray()
        };
    }

    private static void VisitEdges(
        IEnumerable<GraphEdge> edges,
        HashSet<long> visited,
        HashSet<long> nextFrontier,
        HashSet<GraphEdge> selectedEdges,
        Func<GraphEdge, long> nextNode)
    {
        foreach (var edge in edges)
        {
            selectedEdges.Add(edge);

            var nodeId = nextNode(edge);

            if (visited.Add(nodeId))
                nextFrontier.Add(nodeId);
        }
    }
}
```

This is already a good production starting point.

---

# 4. Personalized PageRank

Now we have:

```text
Query
  │
  ▼
Vector/BM25
  │
  ▼
Seed nodes
  │
  ▼
Neighborhood expansion
  │
  ▼
Subgraph
```

Suppose the query retrieval gave:

```text
.NET       0.94
C#         0.87
Microsoft  0.72
```

We don't want PageRank to start uniformly.

We want these nodes to have the initial probability:

```text
.NET       █████████
C#         ████████
Microsoft  ██████
```

That's what **Personalized PageRank** does.

---

# 5. PageRank implementation

Let's create a result type:

```csharp
public sealed record PageRankResult(
    long NodeId,
    double Score);
```

And the algorithm:

```csharp
public sealed class PersonalizedPageRank
{
    private readonly Graph _graph;

    public PersonalizedPageRank(Graph graph)
    {
        _graph = graph;
    }

    public IReadOnlyList<PageRankResult> Rank(
        IReadOnlyCollection<long> nodeIds,
        IReadOnlyDictionary<long, double> personalization,
        int iterations = 20,
        double dampingFactor = 0.85)
    {
        if (nodeIds.Count == 0)
            return [];

        if (iterations <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(iterations));

        if (dampingFactor is < 0 or >= 1)
            throw new ArgumentOutOfRangeException(
                nameof(dampingFactor));

        var nodes = nodeIds.ToHashSet();

        var rank = CreateInitialRank(
            nodes,
            personalization);

        for (var i = 0; i < iterations; i++)
        {
            rank = Iterate(
                nodes,
                rank,
                personalization,
                dampingFactor);
        }

        return rank
            .Select(x => new PageRankResult(
                x.Key,
                x.Value))
            .OrderByDescending(x => x.Score)
            .ToArray();
    }

    private static Dictionary<long, double> CreateInitialRank(
        HashSet<long> nodes,
        IReadOnlyDictionary<long, double> personalization)
    {
        var result = new Dictionary<long, double>();

        double total = 0;

        foreach (var nodeId in nodes)
        {
            personalization.TryGetValue(
                nodeId,
                out var value);

            result[nodeId] = value;
            total += value;
        }

        // No personalization supplied:
        // use uniform distribution.
        if (total <= 0)
        {
            var uniform = 1.0 / nodes.Count;

            foreach (var nodeId in nodes)
                result[nodeId] = uniform;

            return result;
        }

        // Normalize personalization.
        foreach (var nodeId in nodes)
            result[nodeId] /= total;

        return result;
    }

    private Dictionary<long, double> Iterate(
        HashSet<long> nodes,
        Dictionary<long, double> rank,
        IReadOnlyDictionary<long, double> personalization,
        double dampingFactor)
    {
        var next = nodes.ToDictionary(
            nodeId => nodeId,
            _ => 0.0);

        foreach (var nodeId in nodes)
        {
            var currentRank = rank[nodeId];

            var outgoing = _graph
                .GetOutgoingEdges(nodeId)
                .Where(e => nodes.Contains(e.TargetId))
                .ToArray();

            if (outgoing.Length == 0)
            {
                // Dangling node.
                DistributeDanglingRank(
                    nodes,
                    next,
                    currentRank * dampingFactor);

                continue;
            }

            var totalWeight = outgoing.Sum(
                e => Math.Max(0, e.Weight));

            if (totalWeight <= 0)
                continue;

            foreach (var edge in outgoing)
            {
                var weight =
                    Math.Max(0, edge.Weight) /
                    totalWeight;

                next[edge.TargetId] +=
                    dampingFactor *
                    currentRank *
                    weight;
            }
        }

        // Personalization / teleportation.
        foreach (var nodeId in nodes)
        {
            personalization.TryGetValue(
                nodeId,
                out var personalizationValue);

            next[nodeId] +=
                (1.0 - dampingFactor) *
                personalizationValue;
        }

        return next;
    }

    private static void DistributeDanglingRank(
        HashSet<long> nodes,
        Dictionary<long, double> next,
        double value)
    {
        var contribution =
            value / nodes.Count;

        foreach (var nodeId in nodes)
            next[nodeId] += contribution;
    }
}
```

There is one subtle issue here: the personalization dictionary must be normalized before the iterations. We should therefore normalize it once and use that normalized distribution.

Here's a cleaner production version.

---

# 6. Improved implementation

```csharp
public sealed class PersonalizedPageRank
{
    private readonly Graph _graph;

    public PersonalizedPageRank(Graph graph)
    {
        _graph = graph;
    }

    public IReadOnlyList<PageRankResult> Rank(
        IReadOnlyCollection<long> nodeIds,
        IReadOnlyDictionary<long, double> personalization,
        int iterations = 30,
        double dampingFactor = 0.85)
    {
        if (nodeIds.Count == 0)
            return [];

        if (iterations <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(iterations));

        if (dampingFactor <= 0 || dampingFactor >= 1)
            throw new ArgumentOutOfRangeException(
                nameof(dampingFactor));

        var nodes = nodeIds.ToHashSet();

        var teleport = NormalizePersonalization(
            nodes,
            personalization);

        var rank = teleport.ToDictionary(
            x => x.Key,
            x => x.Value);

        for (var iteration = 0;
             iteration < iterations;
             iteration++)
        {
            var next = nodes.ToDictionary(
                nodeId => nodeId,
                _ => 0.0);

            double danglingMass = 0;

            foreach (var nodeId in nodes)
            {
                var currentRank = rank[nodeId];

                var outgoing = _graph
                    .GetOutgoingEdges(nodeId)
                    .Where(e => nodes.Contains(e.TargetId))
                    .Where(e => e.Weight > 0)
                    .ToArray();

                if (outgoing.Length == 0)
                {
                    danglingMass += currentRank;
                    continue;
                }

                var totalWeight =
                    outgoing.Sum(e => e.Weight);

                foreach (var edge in outgoing)
                {
                    var transitionProbability =
                        edge.Weight / totalWeight;

                    next[edge.TargetId] +=
                        dampingFactor *
                        currentRank *
                        transitionProbability;
                }
            }

            // Redistribute dangling nodes.
            if (danglingMass > 0)
            {
                foreach (var nodeId in nodes)
                {
                    next[nodeId] +=
                        dampingFactor *
                        danglingMass *
                        teleport[nodeId];
                }
            }

            // Teleport back toward the query seeds.
            foreach (var nodeId in nodes)
            {
                next[nodeId] +=
                    (1.0 - dampingFactor) *
                    teleport[nodeId];
            }

            rank = next;
        }

        return rank
            .Select(x =>
                new PageRankResult(
                    x.Key,
                    x.Value))
            .OrderByDescending(x => x.Score)
            .ToArray();
    }

    private static Dictionary<long, double>
        NormalizePersonalization(
            HashSet<long> nodes,
            IReadOnlyDictionary<long, double> personalization)
    {
        var result = nodes.ToDictionary(
            nodeId => nodeId,
            _ => 0.0);

        foreach (var pair in personalization)
        {
            if (!nodes.Contains(pair.Key))
                continue;

            if (pair.Value > 0)
                result[pair.Key] = pair.Value;
        }

        var total = result.Values.Sum();

        if (total <= 0)
        {
            var uniform = 1.0 / nodes.Count;

            foreach (var nodeId in nodes)
                result[nodeId] = uniform;

            return result;
        }

        foreach (var nodeId in nodes)
            result[nodeId] /= total;

        return result;
    }
}
```

---

# 7. Connecting both algorithms

Now the interesting part.

Suppose your vector/BM25 retrieval gives:

```csharp
var seeds = new Dictionary<long, double>
{
    [100] = 0.94, // .NET
    [200] = 0.87, // C#
    [300] = 0.72  // Microsoft
};
```

First expand:

```csharp
var expander = new GraphNeighborhoodExpander(graph);

var neighborhood = expander.Expand(
    seeds.Keys,
    maxDepth: 2,
    direction: GraphTraversalDirection.Both);
```

You now have:

```text
NodeIds
    100
    200
    300
    101
    102
    201
    301
    ...
```

Then PageRank:

```csharp
var pageRank = new PersonalizedPageRank(graph);

var ranked = pageRank.Rank(
    neighborhood.NodeIds,
    seeds,
    iterations: 30,
    dampingFactor: 0.85);
```

Then:

```csharp
foreach (var result in ranked.Take(20))
{
    var node = graph.Nodes[result.NodeId];

    Console.WriteLine(
        $"{node.Name,-30} {result.Score:F6}");
}
```

You might obtain:

```text
.NET                         0.184521
C#                           0.161832
Microsoft                    0.129402
.NET 10                      0.087124
ASP.NET                      0.071823
Visual Studio                0.058921
Roslyn                       0.047832
NuGet                        0.041223
...
```

---

# 8. One important improvement for GraphRAG

I would **not use PageRank alone** to determine your final relevance.

PageRank answers:

> "Which nodes are structurally important relative to my seeds?"

It does **not** necessarily answer:

> "Which nodes are most relevant to the user's question?"

So I would combine the scores:

```text
FinalScore =
    0.45 × SemanticScore
  + 0.20 × BM25Score
  + 0.25 × PageRankScore
  + 0.10 × EdgeRelevance
```

For example:

```csharp
public static double CalculateFinalScore(
    double semanticScore,
    double bm25Score,
    double pageRankScore,
    double edgeRelevance)
{
    return
        0.45 * semanticScore +
        0.20 * bm25Score +
        0.25 * pageRankScore +
        0.10 * edgeRelevance;
}
```

This gives you a much better GraphRAG ranking:

```text
                Query
                  │
       ┌──────────┼──────────┐
       ▼          ▼          ▼
    Vector       BM25     Entities
       │          │          │
       └──────────┼──────────┘
                  ▼
                 RRF
                  │
                  ▼
             Seed nodes
                  │
                  ▼
          ┌───────────────┐
          │ Neighborhood  │
          │   depth 1-2   │
          └───────┬───────┘
                  │
                  ▼
          Personalized
             PageRank
                  │
                  ▼
        Semantic + BM25 +
        PageRank + edges
                  │
                  ▼
             Final Top-K
                  │
                  ▼
             Subgraph
                  │
                  ▼
                LLM
```

### One more thing I'd change for your architecture

Since you're building a **real GraphRAG engine rather than just a demo**, I would make the neighborhood expansion return **distance from the seed** as well:

```csharp
public sealed record GraphNodeCandidate(
    long NodeId,
    int MinDistance,
    long? SeedNodeId);
```

Then you can apply a distance decay such as:

```text
distance 0 → 1.00
distance 1 → 0.70
distance 2 → 0.40
```

This prevents a node that happens to be connected through several paths from outranking an immediately relevant node.

It also gives you a very useful signal for the final ranking:

```text
FinalScore =
    semantic
  + bm25
  + pagerank
  + edge relevance
  + distance relevance
```

That would be the version I'd recommend integrating into the **retrieval layer of the GraphRAG engine you've been designing**, especially before the final subgraph projection sent to the LLM.


> I have some questions. In your explaination, you notes :
```
               Query 
                 │
    ┌────────────┼────────────┐ 
    ▼            ▼            ▼ 
Vector search   BM25      Entity matching
```

Yes. That part is important, because **Vector search, BM25, and Entity matching are three different ways of interpreting the same user query**. They complement each other rather than doing the same thing.

For a GraphRAG system, I would think of them like this:

```text
                         User Query
                             │
          ┌──────────────────┼──────────────────┐
          ▼                  ▼                  ▼
    Vector Search          BM25          Entity Matching
          │                  │                  │
   "What is semantically   "What words       "Which graph
      similar?"             occur?"          entities are named?"
          │                  │                  │
          └──────────────────┼──────────────────┘
                             ▼
                       Result Fusion
                             │
                             ▼
                        Seed Nodes
```

### 1. Vector search

This answers:

> **"Which graph nodes are semantically similar to the query?"**

Suppose the query is:

> "How does Microsoft build software for multiple operating systems?"

The query embedding might retrieve:

```text
.NET              0.92
.NET Core         0.90
C#                0.86
.NET 10           0.84
Visual Studio     0.78
Java              0.61
```

It can find `.NET` even though the query doesn't literally contain the word `.NET`.

That's its biggest advantage.

For your GraphRAG, I'd have a vector index containing representations of:

```text
Node
 ├── Name
 ├── Type
 └── Description
```

and potentially:

```text
Document Chunk
Community
```

---

### 2. BM25

BM25 answers a different question:

> **"Which graph documents/nodes contain important words from the query?"**

For:

> "How does Microsoft build software for multiple operating systems?"

BM25 might strongly favor:

```text
Microsoft
software
operating systems
.NET
cross-platform
```

It is excellent for **exact terminology**, names, acronyms, identifiers, versions, etc.

For example, if the query is:

> "What is .NET 10?"

BM25 is extremely useful because `.NET 10` is a very specific term.

Vector search might retrieve:

```text
.NET 10       0.96
.NET 9        0.94
.NET 8        0.93
.NET          0.91
.NET Core     0.88
```

BM25 can give `.NET 10` a particularly strong signal because it matches the exact token.

---

### 3. Entity matching

This is different again.

Entity matching asks:

> **"Does the query explicitly refer to entities that exist in my graph?"**

For example:

> "What relationship exists between Microsoft and .NET 10?"

Entity extraction might identify:

```text
Microsoft → Entity ID 123
.NET 10   → Entity ID 456
```

Now you have something extremely valuable:

```text
Microsoft ───────?───────> .NET 10
```

You can search the graph directly for relationships between these entities.

This can be **much more precise than semantic search**.

---

# Why have all three?

Consider this query:

> **"What is Microsoft's relationship with the .NET 10 runtime?"**

### Vector search

Might return:

```text
.NET
.NET 10
.NET Runtime
Microsoft
.NET Core
```

Good semantic understanding.

### BM25

Might return:

```text
.NET 10
Microsoft
.NET 10 Runtime
Microsoft .NET documentation
```

Good lexical matching.

### Entity matching

Could identify:

```text
Microsoft
.NET 10
```

with exact graph IDs.

That's extremely useful because now you can immediately inspect:

```text
Microsoft
    │
    ├── develops ──> .NET
    │
    └── develops ──> .NET 10
```

---

# The important part: they shouldn't necessarily return the same type of result

This is where I would slightly modify my previous architecture.

I would have:

```text
                         Query
                           │
              ┌────────────┼────────────┐
              ▼            ▼            ▼
          Vector          BM25       Entity
          Search         Search      Matching
              │            │            │
              ▼            ▼            ▼
          Node IDs       Node IDs     Node IDs
              │            │            │
              └────────────┼────────────┘
                           ▼
                       RRF / Fusion
                           │
                           ▼
                       Seed Nodes
```

But **Entity Matching deserves special treatment**.

I wouldn't simply throw it into RRF in every case.

For example, if the query explicitly says:

> "What products are developed by Microsoft?"

and you identify:

```text
Microsoft → node 123
```

then node `123` should probably become a **high-confidence seed**, regardless of whether vector search ranked it #1.

Then you can expand:

```text
Microsoft
    │
    ├── develops ──> .NET
    ├── develops ──> C#
    ├── develops ──> TypeScript
    └── owns ──────> GitHub
```

This is one of the places where GraphRAG differs fundamentally from ordinary RAG.

---

# I would therefore use two kinds of seeds

### Explicit seeds

Entities explicitly identified in the query:

```text
Microsoft
.NET 10
GitHub
```

### Semantic seeds

Nodes found through retrieval:

```text
.NET
C#
Visual Studio
.NET Runtime
```

Then combine them:

```text
                   Query
                     │
       ┌─────────────┴─────────────┐
       │                           │
 Explicit entities          Semantic retrieval
       │                           │
       ▼                           ▼
 High-confidence seeds       Candidate seeds
       │                           │
       └─────────────┬─────────────┘
                     ▼
                 Seed ranking
                     │
                     ▼
              Graph expansion
```

This is particularly powerful for your use case.

---

# There is another important distinction

Entity matching can be used **before** retrieval and **after** retrieval.

### Before retrieval

Extract:

```text
"Microsoft"
".NET 10"
```

Then resolve them to graph nodes.

This gives you anchors.

### After retrieval

Suppose vector search gives you:

```text
.NET 10
.NET
.NET Runtime
ASP.NET Core
```

You can check whether these candidates correspond to known graph entities and resolve them to canonical IDs.

This helps eliminate duplicate representations:

```text
".NET 10"
"Microsoft .NET 10"
".NET Framework 10"
```

if your entity-resolution system knows that some of these refer to the same entity—or, importantly, **knows when they don't**.

---

# And this leads to an important architectural point

Since you're implementing your own GraphRAG engine in C#, I would separate these concepts:

```text
Query Processing
│
├── Query Parser / Analyzer
│
├── Entity Extractor
│
├── Entity Resolver
│
└── Query Embedding
        │
        ▼
Retrieval
│
├── Vector Retriever
├── BM25 Retriever
└── Entity Retriever
        │
        ▼
Fusion
│
└── Seed Selection
        │
        ▼
Graph Retrieval
│
├── Neighborhood Expansion
├── Path Search
└── Personalized PageRank
        │
        ▼
Reranking
        │
        ▼
Subgraph
        │
        ▼
LLM
```

That separation will become very useful later when you implement more sophisticated queries such as:

> "Which technologies developed by Microsoft are used by products that run on Linux?"

Here, pure vector search isn't enough. You need to recognize entities and then **reason over graph paths**:

```text
Microsoft
    │
  develops
    ▼
Technology
    │
   used-by
    ▼
Product
    │
 runs-on
    ▼
Linux
```

So I'd consider **Entity Matching not merely another retrieval method, but the bridge between natural-language query understanding and your graph query engine**.

And this connects very nicely to the **Cypher → AST → Binder → Logical Plan → Physical Plan** architecture you've been building: eventually, the entity matcher can provide the bound graph entities that your logical plan operates on.


# Can you help me code in C# these steps : 

```
Query Processing 
│ ├── Query Parser / Analyzer
│ ├── Entity Extractor
│ ├── Entity Resolver
│ └── Query Embedding

```

Absolutely. For your GraphRAG architecture, I would make these **four components independent**, because later you can replace the LLM-based entity extractor, the embedding provider, or the entity resolver without changing the rest of the pipeline.

I would structure it like this:

```text
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
```

The overall flow becomes:

```text
Natural language
      │
      ▼
┌──────────────────┐
│ Query Analyzer   │
└────────┬─────────┘
         │
         ├───────────────┐
         ▼               ▼
 Entity extraction    Query normalization
         │
         ▼
┌──────────────────┐
│ Entity Resolver  │
└────────┬─────────┘
         │
         ▼
  Graph entity IDs
         │
         └───────────────┐
                         ▼
                ┌─────────────────┐
                │ Query Embedding │
                └────────┬────────┘
                         │
                         ▼
              QueryAnalysisResult
```

## 1. Domain models

Let's start with the data structures.

### Extracted entity

This is what the NLP/LLM layer found in the query.

```csharp
public sealed record ExtractedEntity(
    string Text,
    string? Type,
    int Start,
    int Length,
    double Confidence);
```

For:

> "What is Microsoft's relationship with .NET 10?"

you might get:

```text
Microsoft
    Type = Organization
    Confidence = 0.98

.NET 10
    Type = Technology
    Confidence = 0.96
```

---

## 2. Resolved entity

The extractor doesn't know your graph's internal IDs.

The resolver does.

```csharp
public sealed record ResolvedEntity(
    ExtractedEntity Extracted,
    long GraphNodeId,
    double Confidence);
```

So:

```text
".NET 10"
     │
     ▼
Entity Resolver
     │
     ▼
GraphNodeId = 1842
```

---

# 3. Query analysis result

We'll combine everything into one object.

```csharp
public sealed class QueryAnalysis
{
    public required string OriginalQuery { get; init; }

    public required string NormalizedQuery { get; init; }

    public required IReadOnlyList<ExtractedEntity> Entities
        { get; init; }

    public required IReadOnlyList<ResolvedEntity> ResolvedEntities
        { get; init; }

    public required ReadOnlyMemory<float> Embedding
        { get; init; }
}
```

`ReadOnlyMemory<float>` is a good choice for embeddings in .NET because most embedding APIs return something compatible with it and it avoids unnecessary allocations/copies.

---

# 4. Query Analyzer

The analyzer should initially remain fairly simple.

Its job isn't to perform retrieval.

It prepares the query.

```csharp
public interface IQueryAnalyzer
{
    Task<string> NormalizeAsync(
        string query,
        CancellationToken cancellationToken = default);
}
```

Implementation:

```csharp
public sealed class QueryAnalyzer : IQueryAnalyzer
{
    public Task<string> NormalizeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var normalized = query
            .Trim()
            .ReplaceLineEndings(" ");

        return Task.FromResult(normalized);
    }
}
```

This looks trivial, but I'd keep it as a separate component because later you may want to do things like:

```text
"What technologies does Microsoft use?"
             │
             ▼
"What technologies are used/developed by Microsoft?"
```

or extract query intent.

---

# 5. Query intent

I'd actually introduce this now because it will become useful later.

```csharp
public enum QueryIntent
{
    Unknown,

    EntityLookup,
    Relationship,
    MultiHopRelationship,
    Aggregation,
    Comparison,
    Explanation,
    General
}
```

Then:

```csharp
public sealed record AnalyzedQuery(
    string OriginalQuery,
    string NormalizedQuery,
    QueryIntent Intent);
```

For example:

```text
"What is .NET?"
       ↓
EntityLookup

"What is the relationship between Microsoft and .NET?"
       ↓
Relationship

"Which products developed by Microsoft run on Linux?"
       ↓
MultiHopRelationship
```

For now you can implement this heuristically, and later replace it with an LLM.

---

# 6. Entity extraction

I'd define the interface so the implementation could be:

* regex/rules
* dictionary
* NER model
* LLM
* hybrid

```csharp
public interface IEntityExtractor
{
    Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default);
}
```

For a first implementation, let's create a simple dictionary-based extractor.

```csharp
public sealed class DictionaryEntityExtractor
    : IEntityExtractor
{
    private readonly IReadOnlyList<EntityDefinition> _entities;

    public DictionaryEntityExtractor(
        IEnumerable<EntityDefinition> entities)
    {
        _entities = entities.ToArray();
    }

    public Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ExtractedEntity>();

        foreach (var entity in _entities)
        {
            var start = 0;

            while (true)
            {
                var index = query.IndexOf(
                    entity.Name,
                    start,
                    StringComparison.OrdinalIgnoreCase);

                if (index < 0)
                    break;

                results.Add(
                    new ExtractedEntity(
                        entity.Name,
                        entity.Type,
                        index,
                        entity.Name.Length,
                        1.0));

                start = index + entity.Name.Length;
            }
        }

        return Task.FromResult<
            IReadOnlyList<ExtractedEntity>>(results);
    }
}
```

And:

```csharp
public sealed record EntityDefinition(
    string Name,
    string Type);
```

Example:

```csharp
var extractor = new DictionaryEntityExtractor(
[
    new("Microsoft", "Organization"),
    new(".NET", "Technology"),
    new(".NET 10", "Technology"),
    new("C#", "ProgrammingLanguage"),
    new("Linux", "OperatingSystem")
]);
```

---

# 7. Important: longest match

There's an issue with that implementation.

Given:

```text
.NET 10
```

it could extract both:

```text
.NET
.NET 10
```

We want the longest match.

So sort entities by length:

```csharp
_entities = entities
    .OrderByDescending(x => x.Name.Length)
    .ToArray();
```

Then prevent overlapping matches:

```csharp
private static bool Overlaps(
    ExtractedEntity candidate,
    IEnumerable<ExtractedEntity> existing)
{
    var start = candidate.Start;
    var end = start + candidate.Length;

    return existing.Any(entity =>
    {
        var existingStart = entity.Start;
        var existingEnd =
            existingStart + entity.Length;

        return start < existingEnd &&
               end > existingStart;
    });
}
```

Then:

```csharp
if (!Overlaps(result, results))
    results.Add(result);
```

This becomes much more important once you have thousands or millions of entities.

---

# 8. Entity Resolver

The extractor found:

```text
".NET 10"
```

The resolver needs to find:

```text
GraphNode
Id = 1842
Type = Technology
Name = ".NET 10"
```

Define:

```csharp
public interface IEntityResolver
{
    Task<IReadOnlyList<ResolvedEntity>> ResolveAsync(
        IReadOnlyList<ExtractedEntity> entities,
        CancellationToken cancellationToken = default);
}
```

The resolver shouldn't directly depend on your graph database.

Instead, introduce an entity index.

```csharp
public interface IEntityIndex
{
    Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? type,
        int topK,
        CancellationToken cancellationToken = default);
}
```

Candidate:

```csharp
public sealed record EntityCandidate(
    long NodeId,
    string Name,
    string Type,
    double Score);
```

---

# 9. Simple entity resolver

```csharp
public sealed class EntityResolver : IEntityResolver
{
    private readonly IEntityIndex _index;

    public EntityResolver(IEntityIndex index)
    {
        _index = index;
    }

    public async Task<IReadOnlyList<ResolvedEntity>> ResolveAsync(
        IReadOnlyList<ExtractedEntity> entities,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ResolvedEntity>();

        foreach (var entity in entities)
        {
            var candidates =
                await _index.SearchAsync(
                    entity.Text,
                    entity.Type,
                    topK: 5,
                    cancellationToken);

            var best = candidates
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            if (best is null)
                continue;

            var confidence =
                entity.Confidence *
                best.Score;

            results.Add(
                new ResolvedEntity(
                    entity,
                    best.NodeId,
                    confidence));
        }

        return results;
    }
}
```

This is deliberately independent of Lucene, ChromaDB, SQL, Neo4j, etc.

You can implement `IEntityIndex` using whatever backend you eventually choose.

---

# 10. Entity resolution should be hybrid

For a real GraphRAG system, I wouldn't use only semantic similarity here.

I'd do:

```text
".NET 10"
     │
     ├── Exact match
     │
     ├── Alias match
     │
     ├── BM25
     │
     └── Vector similarity
             │
             ▼
       Candidate entities
             │
             ▼
       Entity reranking
```

For example:

```text
.NET 10
│
├── exact name          1.00
├── alias               0.95
├── BM25                0.91
└── embedding           0.88
```

Then:

```csharp
finalScore =
    0.40 * exactScore +
    0.25 * aliasScore +
    0.20 * bm25Score +
    0.15 * vectorScore;
```

This is especially important for entity resolution because **semantic similarity alone can produce dangerous false matches**.

---

# 11. Query embedding

Now the final piece.

Define:

```csharp
public interface IEmbeddingService
{
    Task<ReadOnlyMemory<float>> EmbedAsync(
        string text,
        CancellationToken cancellationToken = default);
}
```

Then:

```csharp
public sealed class QueryEmbeddingService
{
    private readonly IEmbeddingService _embeddingService;

    public QueryEmbeddingService(
        IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    public Task<ReadOnlyMemory<float>> CreateAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        return _embeddingService.EmbedAsync(
            query,
            cancellationToken);
    }
}
```

The important point is that **your GraphRAG code doesn't know whether you're using OpenAI, Azure OpenAI, Ollama, a local model, etc.**

---

# 12. Complete Query Processor

Now we can connect everything.

```csharp
public sealed class GraphRagQueryProcessor
{
    private readonly IQueryAnalyzer _analyzer;
    private readonly IEntityExtractor _extractor;
    private readonly IEntityResolver _resolver;
    private readonly IEmbeddingService _embeddingService;

    public GraphRagQueryProcessor(
        IQueryAnalyzer analyzer,
        IEntityExtractor extractor,
        IEntityResolver resolver,
        IEmbeddingService embeddingService)
    {
        _analyzer = analyzer;
        _extractor = extractor;
        _resolver = resolver;
        _embeddingService = embeddingService;
    }

    public async Task<QueryAnalysis> ProcessAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // 1. Normalize / analyze
        var normalized =
            await _analyzer.NormalizeAsync(
                query,
                cancellationToken);

        // 2. Extract entities
        var entities =
            await _extractor.ExtractAsync(
                normalized,
                cancellationToken);

        // 3. Resolve entities to graph nodes
        var resolved =
            await _resolver.ResolveAsync(
                entities,
                cancellationToken);

        // 4. Create query embedding
        var embedding =
            await _embeddingService.EmbedAsync(
                normalized,
                cancellationToken);

        return new QueryAnalysis
        {
            OriginalQuery = query,
            NormalizedQuery = normalized,
            Entities = entities,
            ResolvedEntities = resolved,
            Embedding = embedding
        };
    }
}
```

Usage:

```csharp
var analysis =
    await queryProcessor.ProcessAsync(
        "What is Microsoft's relationship with .NET 10?",
        cancellationToken);
```

You now have:

```text
QueryAnalysis
│
├── OriginalQuery
│   "What is Microsoft's relationship with .NET 10?"
│
├── NormalizedQuery
│   "What is Microsoft's relationship with .NET 10?"
│
├── Entities
│   ├── Microsoft
│   └── .NET 10
│
├── ResolvedEntities
│   ├── Microsoft → Node 123
│   └── .NET 10 → Node 1842
│
└── Embedding
    [0.023, -0.182, 0.441, ...]
```

---

# 13. Where this goes in your complete GraphRAG pipeline

This is where I would now take your architecture:

```text
                         USER QUERY
                             │
                             ▼
                  ┌─────────────────────┐
                  │  Query Processing    │
                  │                     │
                  │  Analyzer           │
                  │  Entity Extractor   │
                  │  Entity Resolver    │
                  │  Query Embedding    │
                  └──────────┬──────────┘
                             │
                ┌────────────┼─────────────┐
                ▼            ▼             ▼
          Resolved       Embedding      Query Intent
          entities           │             │
                │             │             │
                ▼             ▼             ▼
             Entity       Vector         Retrieval
             retrieval    retrieval       strategy
                │             │
                └──────┬──────┘
                       ▼
                  BM25 retrieval
                       │
                       ▼
                    RRF fusion
                       │
                       ▼
                  Seed selection
                       │
                       ▼
              Neighborhood expansion
                       │
                       ▼
             Personalized PageRank
                       │
                       ▼
                    Reranking
                       │
                       ▼
                 Relevant subgraph
                       │
                       ▼
             Deterministic projection
                       │
                       ▼
                       LLM
```

### One architectural change I'd strongly recommend

Don't make `QueryAnalysis` only contain the embedding and entities. Add **query constraints** eventually:

```csharp
public sealed class QueryAnalysis
{
    public required string OriginalQuery { get; init; }
    public required string NormalizedQuery { get; init; }

    public QueryIntent Intent { get; init; }

    public required IReadOnlyList<ExtractedEntity> Entities
        { get; init; }

    public required IReadOnlyList<ResolvedEntity> ResolvedEntities
        { get; init; }

    public required ReadOnlyMemory<float> Embedding
        { get; init; }

    public IReadOnlyList<string> RelationshipTypes
        { get; init; } = [];

    public IReadOnlyList<string> NodeTypes
        { get; init; } = [];

    public int? RequestedTopK
        { get; init; }
}
```

That will eventually allow a query like:

> **"Which products developed by Microsoft run on Linux?"**

to be represented approximately as:

```text
Intent:
    MultiHopRelationship

Entities:
    Microsoft
    Linux

Relationship constraints:
    develops
    runs-on

Node constraints:
    Product

Graph pattern:

Microsoft
    │
    develops
    ▼
Product
    │
    runs-on
    ▼
Linux
```

And **that is the point where your natural-language GraphRAG query processor starts becoming a true query planner**, rather than simply a vector-search wrapper.

Given the Cypher/ANTLR/Binder architecture you've been building, the next logical step is to make `QueryAnalysis` produce a **graph query intent / logical pattern**, which can then feed directly into your `Binder` and `LogicalOperator` pipeline.


# Can you implement IEntityIndex for Lucene, ChromaDB and SQL in C#


Yes. I would implement all three behind the same `IEntityIndex` interface, so your `EntityResolver` doesn't care whether the backing store is **Lucene.NET, ChromaDB, or SQL**.

One important distinction first:

* **Lucene.NET** → excellent for exact/lexical entity resolution and fuzzy matching.
* **ChromaDB** → excellent for semantic entity resolution using embeddings.
* **SQL** → excellent for deterministic exact/alias matching and can be your canonical entity store.

For your GraphRAG, I would actually use **SQL as the source of truth**, with Lucene and Chroma as indexes over the same entities.

Lucene.NET supports fuzzy matching through `FuzzyQuery`, and its `IndexSearcher` is intended to be reused for an unchanged index. ([Lucene.NET][1]) Chroma supports nearest-neighbor queries with embeddings and metadata filtering. ([GitHub][2])

---

# 1. Common interface

I'd slightly improve the interface we created earlier.

```csharp
public interface IEntityIndex
{
    Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default);
}
```

And:

```csharp
public sealed record EntityCandidate(
    long NodeId,
    string Name,
    string Type,
    double Score);
```

This gives us:

```text
                    EntityResolver
                         │
                         ▼
                  IEntityIndex
                  /     |      \
                 /      |       \
                ▼       ▼        ▼
            Lucene    Chroma     SQL
```

---

# 2. SQL implementation

Let's start with SQL because this should be your **canonical entity repository**.

I'll use SQLite for the example. `Microsoft.Data.Sqlite` provides the normal ADO.NET abstractions and can be used independently of EF Core. ([Microsoft Learn][3])

Install:

```bash
dotnet add package Microsoft.Data.Sqlite
```

## Schema

I'd create:

```sql
CREATE TABLE GraphEntity
(
    Id          INTEGER PRIMARY KEY,
    Name        TEXT NOT NULL,
    Type        TEXT NOT NULL,
    Description TEXT
);

CREATE TABLE GraphEntityAlias
(
    EntityId    INTEGER NOT NULL,
    Alias       TEXT NOT NULL,

    PRIMARY KEY (EntityId, Alias),

    FOREIGN KEY (EntityId)
        REFERENCES GraphEntity(Id)
);

CREATE INDEX IX_GraphEntity_Name
    ON GraphEntity(Name);

CREATE INDEX IX_GraphEntity_Type
    ON GraphEntity(Type);

CREATE INDEX IX_GraphEntityAlias_Alias
    ON GraphEntityAlias(Alias);
```

For example:

```text
GraphEntity

Id      Name          Type
---------------------------------
1       Microsoft     Organization
2       .NET          Technology
3       .NET 10       Technology
4       C#            Language
```

and:

```text
GraphEntityAlias

EntityId    Alias
-------------------------
2           DotNet
2           .NET Framework
3           DotNet 10
4           C Sharp
```

---

## SQL index implementation

```csharp
using Microsoft.Data.Sqlite;

public sealed class SqlEntityIndex : IEntityIndex
{
    private readonly string _connectionString;

    public SqlEntityIndex(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        if (topK <= 0)
            return [];

        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            connection.CreateCommand();

        command.CommandText = """
            SELECT
                e.Id,
                e.Name,
                e.Type,

                CASE
                    WHEN lower(e.Name) = lower($text)
                        THEN 1.0

                    WHEN lower(a.Alias) = lower($text)
                        THEN 0.95

                    WHEN lower(e.Name) LIKE lower($prefix)
                        THEN 0.80

                    WHEN lower(a.Alias) LIKE lower($prefix)
                        THEN 0.75

                    ELSE 0.50
                END AS Score

            FROM GraphEntity e

            LEFT JOIN GraphEntityAlias a
                ON a.EntityId = e.Id

            WHERE
                (
                    lower(e.Name) = lower($text)
                    OR lower(a.Alias) = lower($text)

                    OR lower(e.Name) LIKE lower($prefix)
                    OR lower(a.Alias) LIKE lower($prefix)

                    OR lower(e.Name) LIKE lower($contains)
                    OR lower(a.Alias) LIKE lower($contains)
                )

                AND
                (
                    $type IS NULL
                    OR e.Type = $type
                )

            ORDER BY Score DESC, e.Name

            LIMIT $limit;
            """;

        command.Parameters.AddWithValue(
            "$text",
            text);

        command.Parameters.AddWithValue(
            "$prefix",
            $"{text}%");

        command.Parameters.AddWithValue(
            "$contains",
            $"%{text}%");

        command.Parameters.AddWithValue(
            "$type",
            (object?)entityType ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "$limit",
            topK);

        var results = new List<EntityCandidate>();

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(
                new EntityCandidate(
                    reader.GetInt64(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDouble(3)));
        }

        return results;
    }
}
```

This implementation is deliberately simple.

For a production SQL implementation, I'd use **SQLite FTS5** rather than `%LIKE%` once you have a significant number of entities. SQLite's standard .NET provider can be used with FTS5. ([Microsoft Learn][4])

---

# 3. SQL + FTS5

For a large graph, create:

```sql
CREATE VIRTUAL TABLE GraphEntitySearch
USING fts5(
    Name,
    Type,
    Description,
    EntityId UNINDEXED
);
```

Then index:

```text
.NET 10
Technology
Microsoft .NET 10 runtime...
1842
```

You can also put aliases into the searchable content.

Then your search becomes much more suitable for entity resolution.

For example:

```sql
SELECT
    EntityId,
    Name,
    Type,
    bm25(GraphEntitySearch) AS Score
FROM GraphEntitySearch
WHERE GraphEntitySearch MATCH $query
ORDER BY Score
LIMIT $limit;
```

Because SQLite's BM25 score is lower-is-better, you'd normalize it before returning it from `IEntityIndex`.

---

# 4. Lucene.NET implementation

This is probably the **best lexical implementation** for your GraphRAG.

I would index:

```text
id
name
type
aliases
description
```

Example document:

```text
id          = 1842
name        = .NET 10
type        = Technology
aliases     = DotNet 10
description = Microsoft .NET 10 runtime
```

---

## Lucene index creation

```csharp
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

public sealed class LuceneEntityIndexWriter
{
    private readonly string _path;

    public LuceneEntityIndexWriter(string path)
    {
        _path = path;
    }

    public void Index(
        IEnumerable<GraphEntity> entities)
    {
        using var directory =
            FSDirectory.Open(_path);

        var analyzer =
            new StandardAnalyzer(
                LuceneVersion.LUCENE_48);

        var config =
            new IndexWriterConfig(
                LuceneVersion.LUCENE_48,
                analyzer);

        using var writer =
            new IndexWriter(
                directory,
                config);

        foreach (var entity in entities)
        {
            var document = new Document
            {
                new StringField(
                    "id",
                    entity.Id.ToString(),
                    Field.Store.YES),

                new TextField(
                    "name",
                    entity.Name,
                    Field.Store.YES),

                new StringField(
                    "type",
                    entity.Type,
                    Field.Store.YES),

                new TextField(
                    "aliases",
                    string.Join(
                        " ",
                        entity.Aliases),
                    Field.Store.YES),

                new TextField(
                    "description",
                    entity.Description ?? "",
                    Field.Store.NO)
            };

            writer.UpdateDocument(
                new Term(
                    "id",
                    entity.Id.ToString()),
                document);
        }

        writer.Commit();
    }
}
```

Models:

```csharp
public sealed record GraphEntity(
    long Id,
    string Name,
    string Type,
    IReadOnlyList<string> Aliases,
    string? Description);
```

---

# 5. Lucene search

Now:

```csharp
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
```

Implementation:

```csharp
public sealed class LuceneEntityIndex : IEntityIndex, IDisposable
{
    private readonly DirectoryReader _reader;
    private readonly IndexSearcher _searcher;
    private readonly QueryParser _parser;

    public LuceneEntityIndex(string path)
    {
        var directory =
            FSDirectory.Open(path);

        _reader =
            DirectoryReader.Open(directory);

        _searcher =
            new IndexSearcher(_reader);

        var analyzer =
            new StandardAnalyzer(
                LuceneVersion.LUCENE_48);

        _parser =
            new QueryParser(
                LuceneVersion.LUCENE_48,
                "name",
                analyzer);
    }

    public Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        if (topK <= 0)
            return Task.FromResult<
                IReadOnlyList<EntityCandidate>>([]);

        var query =
            BuildQuery(text, entityType);

        var hits =
            _searcher.Search(
                query,
                topK);

        var results =
            new List<EntityCandidate>();

        foreach (var hit in hits.ScoreDocs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var document =
                _searcher.Doc(hit.Doc);

            var id =
                long.Parse(
                    document.Get("id"));

            results.Add(
                new EntityCandidate(
                    id,
                    document.Get("name"),
                    document.Get("type"),
                    NormalizeScore(
                        hit.Score)));
        }

        return Task.FromResult<
            IReadOnlyList<EntityCandidate>>(results);
    }

    private Query BuildQuery(
        string text,
        string? entityType)
    {
        var escaped =
            QueryParser.Escape(text);

        var boolean =
            new BooleanQuery();

        // Exact name match.
        boolean.Add(
            new TermQuery(
                new Term(
                    "name",
                    text.ToLowerInvariant())),
            Occur.SHOULD);

        // Normal lexical query.
        var parsed =
            _parser.Parse(escaped);

        boolean.Add(
            parsed,
            Occur.SHOULD);

        // Alias search.
        boolean.Add(
            new TermQuery(
                new Term(
                    "aliases",
                    text.ToLowerInvariant())),
            Occur.SHOULD);

        // Optional entity type.
        if (!string.IsNullOrWhiteSpace(entityType))
        {
            boolean.Add(
                new TermQuery(
                    new Term(
                        "type",
                        entityType)),
                Occur.MUST);
        }

        return boolean;
    }

    private static double NormalizeScore(
        float score)
    {
        // Converts Lucene's unbounded-ish score
        // into approximately 0..1.

        return score / (1.0 + score);
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}
```

One caveat: because `StandardAnalyzer` tokenizes fields, exact matching of things like `.NET 10` needs some care. In production I'd use **two fields**:

```text
name
name_exact
```

where:

```text
name       → TextField
name_exact → StringField
```

Then:

```text
.NET 10
```

can be matched exactly while still allowing normal lexical search.

---

# 6. Add fuzzy matching

This is particularly useful for entity resolution.

For example:

```text
Microsoft
Microsft
Microsof
```

Lucene.NET has `FuzzyQuery`, based on edit distance. ([Lucene.NET][1])

We can add:

```csharp
boolean.Add(
    new FuzzyQuery(
        new Term(
            "name",
            text.ToLowerInvariant()),
        maxEdits: 2),
    Occur.SHOULD);
```

However, **don't use fuzzy matching indiscriminately**.

For short names, versions, acronyms, etc., it can produce bad candidates. The Lucene documentation itself notes limitations around very short terms. ([Lucene.NET][1])

I'd use it only as a fallback:

```text
1. Exact
2. Alias
3. BM25
4. Fuzzy
```

---

# 7. ChromaDB implementation

For Chroma, I'd use the collection as the **semantic entity index**.

The Chroma API supports querying using embeddings and returning metadata/distances. ([GitHub][2])

Since you're working in C#, one convenient route is the Chroma connector available through Semantic Kernel. Microsoft documents `ChromaClient.QueryEmbeddingsAsync`, which accepts `ReadOnlyMemory<float>[]`. ([Microsoft Learn][5])

Install:

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Chroma
```

Then define:

```csharp
public sealed class ChromaEntityIndex : IEntityIndex
{
    private readonly ChromaClient _client;
    private readonly string _collectionId;
    private readonly IEmbeddingService _embeddingService;

    public ChromaEntityIndex(
        ChromaClient client,
        string collectionId,
        IEmbeddingService embeddingService)
    {
        _client = client;
        _collectionId = collectionId;
        _embeddingService = embeddingService;
    }

    public async Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var embedding =
            await _embeddingService.EmbedAsync(
                text,
                cancellationToken);

        var result =
            await _client.QueryEmbeddingsAsync(
                _collectionId,
                [embedding],
                topK,
                [
                    "metadatas",
                    "distances"
                ],
                cancellationToken);

        return ConvertResults(result);
    }

    private static IReadOnlyList<EntityCandidate>
        ConvertResults(
            ChromaQueryResultModel result)
    {
        // See note below about the exact model
        // shape of the installed SK connector version.

        var candidates =
            new List<EntityCandidate>();

        // Map Chroma metadata + distance
        // into EntityCandidate here.

        return candidates;
    }
}
```

The exact generated model properties have changed across Semantic Kernel/Chroma connector versions, so I would avoid pretending that one particular property layout is universal.

The important mapping is:

```text
Chroma
  │
  ├── id
  ├── metadata.nodeId
  ├── metadata.name
  ├── metadata.type
  └── distance
         │
         ▼
EntityCandidate
```

And Chroma's `query()` returns IDs, metadata and distances aligned by result position. ([Chroma Cookbook][6])

---

# 8. What I would actually store in Chroma

I wouldn't store only:

```text
.NET 10
```

I'd create an embedding document:

```text
Type: Technology
Name: .NET 10
Aliases: DotNet 10, Microsoft .NET 10
Description: Cross-platform runtime and development platform...
```

Then metadata:

```json
{
    "nodeId": 1842,
    "type": "Technology",
    "name": ".NET 10"
}
```

The embedding represents the **meaning of the entity**, while metadata gives you deterministic information.

---

# 9. But I would change `IEntityIndex`

We're now seeing that the three implementations have different strengths.

I would actually make the abstraction slightly richer:

```csharp
public interface IEntityIndex
{
    Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        EntitySearchRequest request,
        CancellationToken cancellationToken = default);
}
```

with:

```csharp
public sealed record EntitySearchRequest(
    string Text,
    string? EntityType = null,
    ReadOnlyMemory<float>? Embedding = null,
    int TopK = 10,
    EntitySearchMode Mode = EntitySearchMode.Auto);
```

And:

```csharp
public enum EntitySearchMode
{
    Exact,
    Lexical,
    Semantic,
    Fuzzy,
    Auto
}
```

Now:

```text
SQL
    → Exact / lexical

Lucene
    → Lexical / fuzzy

Chroma
    → Semantic
```

---

# 10. Even better: CompositeEntityIndex

For **your GraphRAG**, I think this is the most interesting implementation.

Don't choose between Lucene, Chroma and SQL.

Use all three:

```text
                    Entity Resolver
                          │
                          ▼
                CompositeEntityIndex
                 /         |         \
                /          |          \
               ▼           ▼           ▼
             SQL        Lucene      Chroma
           exact        lexical     semantic
               \          |          /
                \         |         /
                 └────────┼─────────┘
                          ▼
                     RRF / Fusion
                          │
                          ▼
                  Entity candidates
```

For example:

```csharp
public sealed class CompositeEntityIndex : IEntityIndex
{
    private readonly IEntityIndex _sql;
    private readonly IEntityIndex _lucene;
    private readonly IEntityIndex _chroma;

    public CompositeEntityIndex(
        IEntityIndex sql,
        IEntityIndex lucene,
        IEntityIndex chroma)
    {
        _sql = sql;
        _lucene = lucene;
        _chroma = chroma;
    }

    public async Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var results =
            await Task.WhenAll(
                _sql.SearchAsync(
                    text,
                    entityType,
                    topK,
                    cancellationToken),

                _lucene.SearchAsync(
                    text,
                    entityType,
                    topK,
                    cancellationToken),

                _chroma.SearchAsync(
                    text,
                    entityType,
                    topK,
                    cancellationToken));

        return Fuse(
            results,
            topK);
    }

    private static IReadOnlyList<EntityCandidate> Fuse(
        IReadOnlyList<EntityCandidate>[] resultSets,
        int topK)
    {
        const int rrfK = 60;

        var scores =
            new Dictionary<long, double>();

        var entities =
            new Dictionary<long, EntityCandidate>();

        foreach (var resultSet in resultSets)
        {
            for (var rank = 0;
                 rank < resultSet.Count;
                 rank++)
            {
                var candidate = resultSet[rank];

                entities[candidate.NodeId] =
                    candidate;

                var score =
                    1.0 / (rrfK + rank + 1);

                scores[candidate.NodeId] =
                    scores.GetValueOrDefault(
                        candidate.NodeId) + score;
            }
        }

        return scores
            .OrderByDescending(x => x.Value)
            .Take(topK)
            .Select(x =>
            {
                var candidate =
                    entities[x.Key];

                return candidate with
                {
                    Score = x.Value
                };
            })
            .ToArray();
    }
}
```

This gives you a very nice property:

### Query

> `What is Microsoft's .NET 10 runtime?`

### SQL

```text
Microsoft       1.00
.NET 10         1.00
```

### Lucene

```text
.NET 10         0.94
Microsoft       0.88
.NET            0.82
```

### Chroma

```text
.NET 10 Runtime 0.96
.NET             0.91
.NET 9           0.88
```

### Fusion

```text
.NET 10
Microsoft
.NET
```

The exact entity is therefore supported by **three independent signals**.

---

# 11. One architectural change I'd make to your previous `EntityResolver`

Because we're now doing hybrid entity resolution, I'd change it from:

```csharp
confidence = entity.Confidence * best.Score;
```

to something more explicit:

```csharp
public sealed record ResolvedEntity(
    ExtractedEntity Extracted,
    long GraphNodeId,
    string CanonicalName,
    string EntityType,
    double EntityConfidence,
    double RetrievalScore,
    double FinalScore);
```

Then:

```text
ExtractedEntity
      │
      ▼
EntityIndex
      │
      ├── SQL exact score
      ├── Lucene lexical score
      └── Chroma semantic score
               │
               ▼
             RRF
               │
               ▼
        Entity candidate
               │
               ▼
       Entity confidence
               │
               ▼
       ResolvedEntity
```

That will make debugging your GraphRAG **much easier**.

You can eventually inspect a result and say:

```text
Entity: ".NET 10"
Graph ID: 1842

Extraction confidence: 0.97

SQL exact:             1.00
Lucene lexical:        0.93
Chroma semantic:       0.96

RRF:                   0.0487

Final confidence:      0.94
```

That is far better than having a mysterious `0.91` score.

---

## My recommendation for your architecture

Given the GraphRAG system you're building, I would use:

| Component          | Role                                               |
| ------------------ | -------------------------------------------------- |
| **SQL**            | Canonical entity database                          |
| **Lucene.NET**     | Exact/lexical/fuzzy entity retrieval               |
| **ChromaDB**       | Semantic entity retrieval                          |
| **RRF**            | Combine Lucene + Chroma rankings                   |
| **EntityResolver** | Decide which graph node is actually meant          |
| **Graph**          | Store relationships and perform expansion/PageRank |

And importantly, **don't use Chroma as your canonical entity store**. Your graph database/SQL layer should own the entity identity; Lucene and Chroma are indexes that can be rebuilt.

This also fits extremely well with the architecture you've been developing: once `ResolvedEntity.GraphNodeId` is available, those IDs can become the **bound variables/seeds for your graph retrieval and eventually your Cypher logical plan**.

[1]: https://lucenenet.apache.org/docs/4.8.0-beta00017/api/core/Lucene.Net.Search.FuzzyQuery.html?utm_source=chatgpt.com "Class FuzzyQuery | Apache Lucene.NET 4.8.0-beta00017 Documentation"
[2]: https://github.com/chroma-core/chroma/blob/main/chromadb/api/models/Collection.py?utm_source=chatgpt.com "chroma/chromadb/api/models/Collection.py at main · chroma-core/chroma · GitHub"
[3]: https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?utm_source=chatgpt.com "Overview - Microsoft.Data.Sqlite | Microsoft Learn"
[4]: https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/custom-versions?utm_source=chatgpt.com "Custom SQLite versions - Microsoft.Data.Sqlite | Microsoft Learn"
[5]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.connectors.chroma.chromaclient?view=semantic-kernel-dotnet&utm_source=chatgpt.com "ChromaClient Class (Microsoft.SemanticKernel.Connectors.Chroma) | Microsoft Learn"
[6]: https://cookbook.chromadb.dev/core/collections/?utm_source=chatgpt.com "Collections - Chroma Cookbook"

# How to calculate edgeRelevance

> You indicated : public static double CalculateFinalScore( double semanticScore, double bm25Score, double pageRankScore, double edgeRelevance) { return 0.45 * semanticScore + 0.20 * bm25Score + 0.25 * pageRankScore + 0.10 * edgeRelevance; } but you didn't indicate how to calculate edgeRelevance. Can you give explaination with detailled C# code


Yes. `edgeRelevance` is the score that answers:

> **"Given what the user asked, how relevant are the relationships connecting this candidate node to the query's seed entities?"**

It is particularly important in GraphRAG because a node can be semantically similar to the query but connected to the wrong part of the graph.

For example:

```text
Query:
"What products developed by Microsoft run on Linux?"
```

Suppose retrieval finds:

```text
Node A: Windows
Node B: .NET
Node C: Visual Studio Code
Node D: GitHub
```

`Visual Studio Code` may have a very good semantic score. But if your graph contains:

```text
Microsoft
   │
   └── develops ──> Visual Studio Code
                         │
                         └── runs-on ──> Linux
```

then its **edge relevance is very high**.

Conversely:

```text
Microsoft
   │
   └── owns ──> LinkedIn
```

might be semantically related to Microsoft, but the edges don't satisfy the query's intended relationship pattern.

---

# 1. What should `edgeRelevance` measure?

I recommend decomposing it:

```text
edgeRelevance
      │
      ├── Relationship type relevance
      ├── Direction relevance
      ├── Query-path relevance
      ├── Edge weight/confidence
      ├── Edge freshness
      └── Path distance
```

For a first implementation, I'd use:

```text
EdgeRelevance =
    0.50 × RelationshipTypeScore
  + 0.25 × DirectionScore
  + 0.15 × EdgeConfidence
  + 0.10 × PathScore
```

Then you can make it more sophisticated later.

---

# 2. Start with the query's relationship intent

The first thing we need is to know what relationships the query is asking about.

For:

> "What products are developed by Microsoft and run on Linux?"

we might extract:

```csharp
QueryIntent = MultiHopRelationship;

RequiredRelationships =
[
    "develops",
    "runs-on"
];

RequiredNodeTypes =
[
    "Product",
    "OperatingSystem"
];
```

This is why I suggested earlier that your `QueryAnalysis` should contain relationship information.

```csharp
public sealed class QueryAnalysis
{
    public required string OriginalQuery { get; init; }

    public required string NormalizedQuery { get; init; }

    public QueryIntent Intent { get; init; }

    public required IReadOnlyList<ExtractedEntity> Entities
        { get; init; }

    public required IReadOnlyList<ResolvedEntity> ResolvedEntities
        { get; init; }

    public required ReadOnlyMemory<float> Embedding
        { get; init; }

    public IReadOnlyList<string> RelationshipTypes
        { get; init; } = [];

    public IReadOnlyList<string> NodeTypes
        { get; init; } = [];
}
```

---

# 3. Represent graph edges

I'd use something like:

```csharp
public sealed record GraphEdge(
    long Id,
    long SourceNodeId,
    long TargetNodeId,
    string Type,
    double Confidence = 1.0,
    double Weight = 1.0);
```

For example:

```text
Microsoft ──develops──> .NET
```

could be:

```csharp
new GraphEdge(
    Id: 1001,
    SourceNodeId: 1,
    TargetNodeId: 2,
    Type: "develops",
    Confidence: 0.98);
```

And:

```text
.NET ──runs-on──> Linux
```

could be:

```csharp
new GraphEdge(
    Id: 1002,
    SourceNodeId: 2,
    TargetNodeId: 10,
    Type: "runs-on",
    Confidence: 0.94);
```

---

# 4. Relationship type relevance

This is the most important component.

Suppose the query asks:

> "What products are developed by Microsoft?"

and the candidate is connected through:

```text
Microsoft ──develops──> Product
```

That's a perfect match:

```text
RelationshipTypeScore = 1.0
```

But:

```text
Microsoft ──owns──> Product
```

might be less relevant:

```text
RelationshipTypeScore = 0.3
```

And:

```text
Microsoft ──located-in──> Redmond
```

is essentially irrelevant:

```text
RelationshipTypeScore = 0.0
```

---

# 5. Don't use only exact relationship names

This is important.

A query may say:

> "products developed by Microsoft"

while the graph uses:

```text
develops
```

Another graph could use:

```text
developed-by
created-by
author-of
maintains
produces
```

Therefore, I'd introduce a relationship vocabulary.

```csharp
public sealed class RelationshipVocabulary
{
    private readonly Dictionary<string, HashSet<string>>
        _synonyms = new(StringComparer.OrdinalIgnoreCase);

    public void Add(
        string canonical,
        params string[] synonyms)
    {
        _synonyms[canonical] =
            new HashSet<string>(
                synonyms,
                StringComparer.OrdinalIgnoreCase);
    }

    public double Similarity(
        string actual,
        string requested)
    {
        if (string.Equals(
            actual,
            requested,
            StringComparison.OrdinalIgnoreCase))
        {
            return 1.0;
        }

        if (!_synonyms.TryGetValue(
                requested,
                out var synonyms))
        {
            return 0.0;
        }

        return synonyms.Contains(actual)
            ? 0.8
            : 0.0;
    }
}
```

Initialize:

```csharp
var vocabulary =
    new RelationshipVocabulary();

vocabulary.Add(
    "develop",
    "develops",
    "developed-by",
    "created",
    "created-by",
    "builds",
    "built-by");

vocabulary.Add(
    "use",
    "uses",
    "used-by",
    "utilizes",
    "depends-on");

vocabulary.Add(
    "run-on",
    "runs-on",
    "supports",
    "supported-by",
    "compatible-with");

vocabulary.Add(
    "own",
    "owns",
    "owned-by");

vocabulary.Add(
    "work-for",
    "works-for",
    "employed-by",
    "employee-of");
```

Now:

```csharp
vocabulary.Similarity(
    "develops",
    "develop");
```

returns:

```text
0.8
```

while:

```csharp
vocabulary.Similarity(
    "owns",
    "develop");
```

returns:

```text
0.0
```

---

# 6. Direction matters

This is another important aspect.

Consider:

```text
Microsoft ──develops──> .NET
```

If the query is:

> "What does Microsoft develop?"

the direction is correct.

But if your graph stores:

```text
.NET ──developed-by──> Microsoft
```

then you need to understand that these represent the same semantic relationship.

So I'd explicitly represent direction:

```csharp
public enum EdgeDirection
{
    Outgoing,
    Incoming,
    Either
}
```

And query relationship requirements:

```csharp
public sealed record RelationshipRequirement(
    string Type,
    EdgeDirection Direction);
```

Example:

```csharp
new RelationshipRequirement(
    "develop",
    EdgeDirection.Outgoing);
```

---

# 7. Direction score

```csharp
public static double CalculateDirectionScore(
    GraphEdge edge,
    long seedNodeId,
    EdgeDirection expectedDirection)
{
    var isOutgoing =
        edge.SourceNodeId == seedNodeId;

    var isIncoming =
        edge.TargetNodeId == seedNodeId;

    return expectedDirection switch
    {
        EdgeDirection.Outgoing =>
            isOutgoing ? 1.0 : 0.0,

        EdgeDirection.Incoming =>
            isIncoming ? 1.0 : 0.0,

        EdgeDirection.Either =>
            isOutgoing || isIncoming
                ? 1.0
                : 0.0,

        _ => 0.0
    };
}
```

---

# 8. Edge confidence

Your graph edges should ideally have a confidence.

For example, if an LLM extracted:

```text
Microsoft ──develops──> .NET
```

with confidence:

```text
0.96
```

then:

```csharp
edge.Confidence = 0.96;
```

If another edge came from weak extraction:

```text
Microsoft ──uses──> PostgreSQL
confidence = 0.42
```

then it shouldn't contribute as strongly.

The simplest score is:

```csharp
double confidenceScore =
    Math.Clamp(edge.Confidence, 0.0, 1.0);
```

---

# 9. Path relevance

This becomes especially important for your GraphRAG because you're doing **depth 1–2 expansion and Personalized PageRank**.

Suppose:

```text
Microsoft
    │
    │ develops
    ▼
Visual Studio Code
    │
    │ runs-on
    ▼
Linux
```

For:

> "Which products developed by Microsoft run on Linux?"

the candidate `Visual Studio Code` is two hops away from the `Linux` constraint.

You don't want all paths to be treated equally.

A simple distance function:

```csharp
public static double CalculatePathScore(
    int distance)
{
    return distance switch
    {
        0 => 1.0,
        1 => 1.0,
        2 => 0.7,
        3 => 0.4,
        _ => 0.1
    };
}
```

Or mathematically:

```csharp
public static double CalculatePathScore(
    int distance,
    double decay = 0.7)
{
    if (distance < 0)
        return 0.0;

    return Math.Pow(decay, distance);
}
```

So:

```text
distance 0 → 1.00
distance 1 → 0.70
distance 2 → 0.49
distance 3 → 0.34
```

---

# 10. Complete edge relevance for one edge

Now we can combine the components.

```csharp
public sealed class EdgeRelevanceCalculator
{
    private readonly RelationshipVocabulary _vocabulary;

    public EdgeRelevanceCalculator(
        RelationshipVocabulary vocabulary)
    {
        _vocabulary = vocabulary;
    }

    public double Calculate(
        GraphEdge edge,
        long seedNodeId,
        RelationshipRequirement requirement,
        int pathDistance)
    {
        var relationshipScore =
            _vocabulary.Similarity(
                edge.Type,
                requirement.Type);

        var directionScore =
            CalculateDirectionScore(
                edge,
                seedNodeId,
                requirement.Direction);

        var confidenceScore =
            Math.Clamp(
                edge.Confidence,
                0.0,
                1.0);

        var pathScore =
            CalculatePathScore(
                pathDistance);

        return
            0.50 * relationshipScore +
            0.25 * directionScore +
            0.15 * confidenceScore +
            0.10 * pathScore;
    }

    private static double CalculateDirectionScore(
        GraphEdge edge,
        long seedNodeId,
        EdgeDirection expectedDirection)
    {
        var outgoing =
            edge.SourceNodeId == seedNodeId;

        var incoming =
            edge.TargetNodeId == seedNodeId;

        return expectedDirection switch
        {
            EdgeDirection.Outgoing =>
                outgoing ? 1.0 : 0.0,

            EdgeDirection.Incoming =>
                incoming ? 1.0 : 0.0,

            EdgeDirection.Either =>
                outgoing || incoming
                    ? 1.0
                    : 0.0,

            _ => 0.0
        };
    }

    private static double CalculatePathScore(
        int distance)
    {
        return Math.Pow(
            0.7,
            Math.Max(distance, 0));
    }
}
```

---

# 11. But candidates can have multiple edges

This is where we need to be careful.

Suppose:

```text
Microsoft
   │
   ├── develops ──> .NET
   │
   ├── owns ──────> .NET
   │
   └── uses ──────> .NET
```

The candidate `.NET` has **three possible edges**.

We shouldn't average them.

We want the strongest relevant relationship.

```csharp
public double CalculateForCandidate(
    long candidateNodeId,
    IEnumerable<(GraphEdge Edge, long SeedNodeId, int Distance)> edges,
    IReadOnlyList<RelationshipRequirement> requirements)
{
    var scores = new List<double>();

    foreach (var item in edges)
    {
        foreach (var requirement in requirements)
        {
            var score = Calculate(
                item.Edge,
                item.SeedNodeId,
                requirement,
                item.Distance);

            scores.Add(score);
        }
    }

    return scores.Count == 0
        ? 0.0
        : scores.Max();
}
```

This means:

> If there is at least one highly relevant connection, the candidate gets credit for it.

---

# 12. Multiple required relationships

Now consider:

> **"Which products developed by Microsoft run on Linux?"**

We have:

```text
Microsoft
   │
 develops
   ▼
Product
   │
 runs-on
   ▼
Linux
```

Here we actually have **two requirements**:

```csharp
[
    new RelationshipRequirement(
        "develop",
        EdgeDirection.Outgoing),

    new RelationshipRequirement(
        "run-on",
        EdgeDirection.Outgoing)
]
```

A product connected only by:

```text
Microsoft ──develops──> Product
```

shouldn't score as highly as a product satisfying both:

```text
Microsoft ──develops──> Product
Product ──runs-on──> Linux
```

So we need a coverage score.

```csharp
public static double CalculateRequirementCoverage(
    IReadOnlyList<RelationshipRequirement> requirements,
    IReadOnlyList<double> requirementScores)
{
    if (requirements.Count == 0)
        return 0.0;

    var covered =
        requirementScores.Count(
            score => score >= 0.7);

    return
        (double)covered /
        requirements.Count;
}
```

Then:

```text
develops = 0.95
runs-on  = 0.92

coverage = 2 / 2 = 1.0
```

while:

```text
develops = 0.95
runs-on  = 0.10

coverage = 1 / 2 = 0.5
```

---

# 13. Better complete edge relevance

I'd therefore define:

```csharp
public sealed record EdgeRelevanceResult(
    double Score,
    double RelationshipScore,
    double DirectionScore,
    double ConfidenceScore,
    double PathScore,
    double RequirementCoverage);
```

And:

```csharp
public double Calculate(
    IEnumerable<EdgeContext> edges,
    IReadOnlyList<RelationshipRequirement> requirements)
{
    if (requirements.Count == 0)
        return 0.0;

    var requirementScores =
        new double[requirements.Count];

    foreach (var edge in edges)
    {
        for (var i = 0;
             i < requirements.Count;
             i++)
        {
            var requirement =
                requirements[i];

            var score =
                CalculateSingleEdge(
                    edge,
                    requirement);

            requirementScores[i] =
                Math.Max(
                    requirementScores[i],
                    score);
        }
    }

    var coverage =
        requirementScores.Count(
            x => x >= 0.7)
        / (double)requirements.Count;

    var strongest =
        requirementScores.Max();

    return
        0.70 * strongest +
        0.30 * coverage;
}
```

Now `edgeRelevance` reflects both:

1. **How strong is the best relevant relationship?**
2. **How much of the requested graph pattern does the candidate satisfy?**

This is considerably more useful than simply looking at the nearest edge.

---

# 14. The `EdgeContext`

I'd create a small structure for the calculation:

```csharp
public sealed record EdgeContext(
    GraphEdge Edge,
    long SeedNodeId,
    int Distance);
```

Then:

```csharp
var edgeContexts =
    new[]
    {
        new EdgeContext(
            DevelopsEdge,
            microsoftId,
            1),

        new EdgeContext(
            RunsOnEdge,
            productId,
            1)
    };
```

---

# 15. Putting it into your final ranking

Now we can return to your original formula:

```csharp
public static double CalculateFinalScore(
    double semanticScore,
    double bm25Score,
    double pageRankScore,
    double edgeRelevance)
{
    return
        0.45 * semanticScore +
        0.20 * bm25Score +
        0.25 * pageRankScore +
        0.10 * edgeRelevance;
}
```

I would actually make it a little more explicit:

```csharp
public sealed record NodeRankingFeatures(
    double SemanticScore,
    double Bm25Score,
    double PageRankScore,
    double EdgeRelevance);
```

Then:

```csharp
public static double CalculateFinalScore(
    NodeRankingFeatures features)
{
    return
        0.45 * features.SemanticScore +
        0.20 * features.Bm25Score +
        0.25 * features.PageRankScore +
        0.10 * features.EdgeRelevance;
}
```

---

# 16. Example

Suppose:

```text
Candidate: Visual Studio Code

semanticScore    = 0.91
bm25Score        = 0.72
pageRankScore    = 0.83
edgeRelevance    = 0.95
```

Then:

```text
Final =
    0.45 × 0.91
  + 0.20 × 0.72
  + 0.25 × 0.83
  + 0.10 × 0.95

= 0.4095
+ 0.144
+ 0.2075
+ 0.095

= 0.856
```

So:

```text
Visual Studio Code
Final score = 0.856
```

---

# 17. Where edge relevance becomes really powerful

Consider two candidates:

### Candidate A

```text
Semantic       0.95
BM25           0.88
PageRank       0.70
Edge relevance 0.20
```

Final:

```text
0.45×0.95 +
0.20×0.88 +
0.25×0.70 +
0.10×0.20

= 0.7625
```

### Candidate B

```text
Semantic       0.86
BM25           0.70
PageRank       0.81
Edge relevance 0.95
```

Final:

```text
0.45×0.86 +
0.20×0.70 +
0.25×0.81 +
0.10×0.95

= 0.8215
```

So **Candidate B wins despite having a lower semantic score**.

That's exactly what you want in GraphRAG.

The graph structure is telling you:

> "This node isn't just semantically related to the question. It is connected to the entities and relationships the question is actually asking about."

---

# 18. One improvement I'd strongly recommend

For your particular architecture, I would eventually **not keep `edgeRelevance` as one single score**.

Keep the components:

```csharp
public sealed record GraphRelevanceFeatures(
    double RelationshipTypeScore,
    double DirectionScore,
    double EdgeConfidence,
    double PathScore,
    double RequirementCoverage);
```

Then calculate:

```csharp
public static double CalculateEdgeRelevance(
    GraphRelevanceFeatures f)
{
    return
        0.40 * f.RelationshipTypeScore +
        0.15 * f.DirectionScore +
        0.15 * f.EdgeConfidence +
        0.10 * f.PathScore +
        0.20 * f.RequirementCoverage;
}
```

This gives you **explainable GraphRAG ranking**.

You can log:

```text
Node: Visual Studio Code

Semantic             0.86
BM25                 0.70
PageRank             0.81

Relationship type    1.00
Direction            1.00
Edge confidence      0.94
Path                 0.70
Requirement coverage 1.00

Edge relevance       0.934

Final score          0.819
```

That will be extremely useful when you start tuning your GraphRAG system.

### One final architectural point

Because you're already implementing **neighborhood expansion + Personalized PageRank**, I'd calculate `edgeRelevance` **during graph traversal**, not afterward by querying the graph again.

Your traversal can produce:

```csharp
public sealed record CandidateNode(
    long NodeId,
    int MinDistance,
    double PageRank,
    IReadOnlyList<EdgeContext> SupportingEdges);
```

Then the pipeline becomes:

```text
Query
  │
  ├── semantic retrieval
  ├── BM25
  └── entity resolution
          │
          ▼
       Seed nodes
          │
          ▼
   Neighborhood expansion
          │
          ├── SupportingEdges
          ├── Distance
          └── PageRank
          │
          ▼
    EdgeRelevanceCalculator
          │
          ▼
     FinalRanker
          │
          ▼
     Top-K subgraph
```

This is the design I'd use for your C# GraphRAG engine because **the same traversal that discovers a node already knows why the node was reached**. That "why" is exactly the information needed to calculate a good `edgeRelevance` score.
