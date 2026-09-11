using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using Microsoft.Extensions.AI;

namespace GraphRag.Indexing.EmbeddingBuilders;

public sealed class EdgeEmbeddingBuilder : EmbeddingBuilder<GraphEdgeContext>
{
    public EdgeEmbeddingBuilder(IProjection<GraphEdgeContext> projection,
        IEmbeddingGenerator<string, Embedding<float>> embedding,
        IEmbeddingDocumentRenderer renderer)
        : base(projection, embedding, renderer)
    {
    }
}
