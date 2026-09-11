using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GraphRag.Core.Models;

public sealed record KnowledgeGraph
{
    [Description("Important topics, concepts, organizations, people, products or technologies found in the text.")]
    public required IReadOnlyList<KnowledgeNode> Nodes { get; init; }

    [Description("Meaningful directed relationships between nodes.")]
    public required IReadOnlyList<KnowledgeEdge> Edges { get; init; }
}
