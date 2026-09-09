# Edge relevance

edgeRelevance is the score that answers:

"Given what the user asked, how relevant are the relationships connecting this candidate node to the query's seed entities?"

It is particularly important in GraphRAG because a node can be semantically similar to the query 
but connected to the wrong part of the graph.

For example:

Query:
"What products developed by Microsoft run on Linux?"

Suppose retrieval finds:

Node A: Windows
Node B: .NET
Node C: Visual Studio Code
Node D: GitHub

Visual Studio Code may have a very good semantic score. But if your graph contains:

Microsoft
│
└── develops ──> Visual Studio Code
│
└── runs-on ──> Linux

then its edge relevance is very high.

Conversely:

Microsoft
│
└── owns ──> LinkedIn

might be semantically related to Microsoft, but the edges don't satisfy the query's intended relationship pattern.

## 1. What should edgeRelevance measure?

I recommend decomposing it:

edgeRelevance
│
├── Relationship type relevance
├── Direction relevance
├── Query-path relevance
├── Edge weight/confidence
├── Edge freshness
└── Path distance

For a first implementation, I'd use:

EdgeRelevance =
0.50 × RelationshipTypeScore
+ 0.25 × DirectionScore
+ 0.15 × EdgeConfidence
+ 0.10 × PathScore

Then you can make it more sophisticated later.

## 2. Start with the query's relationship intent

The first thing we need is to know what relationships the query is asking about.

For:

"What products are developed by Microsoft and run on Linux?"

we might extract:

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

This is why I suggested earlier that your QueryAnalysis should contain relationship information.

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

## 3. Represent graph edges

I'd use something like:

public sealed record GraphEdge(
long Id,
long SourceNodeId,
long TargetNodeId,
string Type,
double Confidence = 1.0,
double Weight = 1.0);

For example:

Microsoft ──develops──> .NET

could be:

new GraphEdge(
Id: 1001,
SourceNodeId: 1,
TargetNodeId: 2,
Type: "develops",
Confidence: 0.98);

And:

.NET ──runs-on──> Linux

could be:

new GraphEdge(
Id: 1002,
SourceNodeId: 2,
TargetNodeId: 10,
Type: "runs-on",
Confidence: 0.94);

## 4. Relationship type relevance

This is the most important component.

Suppose the query asks:

"What products are developed by Microsoft?"

and the candidate is connected through:

Microsoft ──develops──> Product

That's a perfect match:

RelationshipTypeScore = 1.0

But:

Microsoft ──owns──> Product

might be less relevant:

RelationshipTypeScore = 0.3

And:

Microsoft ──located-in──> Redmond

is essentially irrelevant:

RelationshipTypeScore = 0.0
5. Don't use only exact relationship names

This is important.

A query may say:

"products developed by Microsoft"

while the graph uses:

develops

Another graph could use:

developed-by
created-by
author-of
maintains
produces

Therefore, I'd introduce a relationship vocabulary.

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

Initialize:

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

Now:

vocabulary.Similarity(
"develops",
"develop");

returns:

0.8

while:

vocabulary.Similarity(
"owns",
"develop");

returns:

0.0
6. Direction matters

This is another important aspect.

Consider:

Microsoft ──develops──> .NET

If the query is:

"What does Microsoft develop?"

the direction is correct.

But if your graph stores:

.NET ──developed-by──> Microsoft

then you need to understand that these represent the same semantic relationship.

So I'd explicitly represent direction:

public enum EdgeDirection
{
Outgoing,
Incoming,
Either
}

And query relationship requirements:

public sealed record RelationshipRequirement(
string Type,
EdgeDirection Direction);

Example:

new RelationshipRequirement(
"develop",
EdgeDirection.Outgoing);
7. Direction score
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

## 8. Edge confidence

Your graph edges should ideally have a confidence.

For example, if an LLM extracted:

Microsoft ──develops──> .NET

with confidence:

0.96

then:

edge.Confidence = 0.96;

If another edge came from weak extraction:

Microsoft ──uses──> PostgreSQL
confidence = 0.42

then it shouldn't contribute as strongly.

The simplest score is:

double confidenceScore =
Math.Clamp(edge.Confidence, 0.0, 1.0);

## 9. Path relevance

This becomes especially important for your GraphRAG because you're doing depth 1–2 expansion and Personalized PageRank.

Suppose:

Microsoft
│
│ develops
▼
Visual Studio Code
│
│ runs-on
▼
Linux

For:

"Which products developed by Microsoft run on Linux?"

the candidate Visual Studio Code is two hops away from the Linux constraint.

You don't want all paths to be treated equally.

A simple distance function:

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

Or mathematically:

public static double CalculatePathScore(
int distance,
double decay = 0.7)
{
if (distance < 0)
return 0.0;

    return Math.Pow(decay, distance);
}

So:

distance 0 → 1.00
distance 1 → 0.70
distance 2 → 0.49
distance 3 → 0.34

## 10. Complete edge relevance for one edge

Now we can combine the components.

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

## 11. But candidates can have multiple edges

This is where we need to be careful.

Suppose:

Microsoft
│
├── develops ──> .NET
│
├── owns ──────> .NET
│
└── uses ──────> .NET

The candidate .NET has three possible edges.

We shouldn't average them.

We want the strongest relevant relationship.

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

This means:

If there is at least one highly relevant connection, the candidate gets credit for it.

## 12. Multiple required relationships

Now consider:

"Which products developed by Microsoft run on Linux?"

We have:

Microsoft
│
develops
▼
Product
│
runs-on
▼
Linux

Here we actually have two requirements:

[
new RelationshipRequirement(
"develop",
EdgeDirection.Outgoing),

    new RelationshipRequirement(
        "run-on",
        EdgeDirection.Outgoing)
]

A product connected only by:

Microsoft ──develops──> Product

shouldn't score as highly as a product satisfying both:

Microsoft ──develops──> Product
Product ──runs-on──> Linux

So we need a coverage score.

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

Then:

develops = 0.95
runs-on  = 0.92

coverage = 2 / 2 = 1.0

while:

develops = 0.95
runs-on  = 0.10

coverage = 1 / 2 = 0.5
13. Better complete edge relevance

I'd therefore define:

public sealed record EdgeRelevanceResult(
double Score,
double RelationshipScore,
double DirectionScore,
double ConfidenceScore,
double PathScore,
double RequirementCoverage);

And:

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

Now edgeRelevance reflects both:

How strong is the best relevant relationship?
How much of the requested graph pattern does the candidate satisfy?

This is considerably more useful than simply looking at the nearest edge.

## 14. The EdgeContext

I'd create a small structure for the calculation:

public sealed record EdgeContext(
GraphEdge Edge,
long SeedNodeId,
int Distance);

Then:

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

## 15. Putting it into your final ranking

Now we can return to your original formula:

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

I would actually make it a little more explicit:

public sealed record NodeRankingFeatures(
double SemanticScore,
double Bm25Score,
double PageRankScore,
double EdgeRelevance);

Then:

public static double CalculateFinalScore(
NodeRankingFeatures features)
{
return
0.45 * features.SemanticScore +
0.20 * features.Bm25Score +
0.25 * features.PageRankScore +
0.10 * features.EdgeRelevance;
}

## 16. Example

Suppose:

Candidate: Visual Studio Code

semanticScore    = 0.91
bm25Score        = 0.72
pageRankScore    = 0.83
edgeRelevance    = 0.95

Then:

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

So:

Visual Studio Code
Final score = 0.856

## 17. Where edge relevance becomes really powerful

Consider two candidates:

Candidate A
Semantic       0.95
BM25           0.88
PageRank       0.70
Edge relevance 0.20

Final:

0.45×0.95 +
0.20×0.88 +
0.25×0.70 +
0.10×0.20

= 0.7625
Candidate B
Semantic       0.86
BM25           0.70
PageRank       0.81
Edge relevance 0.95

Final:

0.45×0.86 +
0.20×0.70 +
0.25×0.81 +
0.10×0.95

= 0.8215

So Candidate B wins despite having a lower semantic score.

That's exactly what you want in GraphRAG.

The graph structure is telling you:

"This node isn't just semantically related to the question. It is connected to the entities and relationships the question is actually asking about."

## 18. One improvement I'd strongly recommend

For your particular architecture, I would eventually not keep edgeRelevance as one single score.

Keep the components:

public sealed record GraphRelevanceFeatures(
double RelationshipTypeScore,
double DirectionScore,
double EdgeConfidence,
double PathScore,
double RequirementCoverage);

Then calculate:

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

This gives you explainable GraphRAG ranking.

You can log:

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

That will be extremely useful when you start tuning your GraphRAG system.

One final architectural point

Because you're already implementing neighborhood expansion + Personalized PageRank, I'd calculate edgeRelevance during graph traversal, not afterward by querying the graph again.

Your traversal can produce:

public sealed record CandidateNode(
long NodeId,
int MinDistance,
double PageRank,
IReadOnlyList<EdgeContext> SupportingEdges);

Then the pipeline becomes:

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

This is the design I'd use for your C# GraphRAG engine because the same traversal that discovers a node already knows why the node was reached. That "why" is exactly the information needed to calculate a good edgeRelevance score.