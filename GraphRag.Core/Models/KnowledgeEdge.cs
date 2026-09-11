using System.ComponentModel;

namespace GraphRag.Core.Models;

public sealed record KnowledgeEdge
{
    [Description("Identifier of the source node.")]
    public required string SourceId { get; init; }

    [Description("Identifier of the target node.")]
    public required string TargetId { get; init; }

    [Description(
        "Normalized relation type, for example USES, PART_OF, DEPENDS_ON, CAUSES, " +
        "IMPLEMENTS, PRODUCES, LOCATED_IN, COMPETES_WITH or RELATED_TO.")]
    public required string Relation { get; init; }

    [Description("Short explanation of the relationship.")]
    public required string Description { get; init; }

    [Description("Confidence from 0 to 1.")]
    public double Confidence { get; init; }

    [Description("Exact or nearly exact passage supporting the relationship.")]
    public required string Evidence { get; init; }
}
