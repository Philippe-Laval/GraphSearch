using GraphSearch.Query.Analysis;

namespace GraphSearch.Query.Resolution;

/*
    ".NET 10"
        │
        ▼
   Entity Resolver
        │
        ▼
   GraphNodeId = 1842
 */

public sealed record ResolvedEntity(
    ExtractedEntity Extracted,
    long GraphNodeId,
    double Confidence);