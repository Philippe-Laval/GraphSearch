using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Indexing.EmbeddingBuilders;

public sealed class NodeEmbeddingBuilder : EmbeddingBuilder<GraphNode>
{
    public NodeEmbeddingBuilder(IProjection<GraphNode> projection,
        IEmbeddingGenerator<string, Embedding<float>> embedding,
        IEmbeddingDocumentRenderer renderer)
        : base(projection, embedding, renderer)
    {
    }
}
