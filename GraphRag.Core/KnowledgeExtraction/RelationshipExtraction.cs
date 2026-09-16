using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.KnowledgeExtraction;

public sealed record RelationshipExtraction
{
    public required IReadOnlyList<KnowledgeEdge> Edges { get; init; }
}
