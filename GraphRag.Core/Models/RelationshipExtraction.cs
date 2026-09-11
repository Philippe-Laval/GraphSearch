using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

public sealed record RelationshipExtraction
{
    public required IReadOnlyList<KnowledgeEdge> Edges { get; init; }
}
