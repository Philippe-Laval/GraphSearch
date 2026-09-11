using System.ComponentModel;

namespace GraphRag.Core.Models;

public sealed record KnowledgeNode
{
    [Description("Stable identifier local to this extraction, such as topic-1.")]
    public required string Id { get; init; }

    [Description("Canonical and concise name of the topic.")]
    public required string Name { get; init; }

    [Description("Type of node, such as Concept, Person, Organization, Technology, Product, Method or Event.")]
    public required string Type { get; init; }

    [Description("Short description based only on the supplied text.")]
    public required string Description { get; init; }

    [Description("Alternative names or abbreviations appearing in the text.")]
    public IReadOnlyList<string> Aliases { get; init; } = [];

    [Description("Importance from 0 to 1 within the supplied text.")]
    public double Importance { get; init; }

    [Description("Exact or nearly exact passage supporting this node.")]
    public required string Evidence { get; init; }
}
