namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// Logical graph pattern extracted from a natural-language query.
///
/// This is the bridge between free-form text and the graph query planner:
/// once populated, the pattern can drive Binder / LogicalOperator construction
/// instead of relying purely on vector search.
///
/// Example — "Which products developed by Microsoft run on Linux?":
///   AnchorEntities      = ["Microsoft", "Linux"]
///   NodeTypes           = ["Product"]
///   RelationshipTypes   = ["develops", "runs-on"]
/// </summary>
public sealed record GraphQueryPattern(
    IReadOnlyList<string> AnchorEntities,
    IReadOnlyList<string> NodeTypes,
    IReadOnlyList<string> RelationshipTypes)
{
    public static GraphQueryPattern Empty { get; } =
        new(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
}
