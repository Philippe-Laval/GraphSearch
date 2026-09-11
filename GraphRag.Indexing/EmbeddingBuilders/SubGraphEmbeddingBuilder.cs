using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;
using Microsoft.Extensions.AI;

namespace GraphRag.Indexing.EmbeddingBuilders;

public sealed class SubGraphEmbeddingBuilder : EmbeddingBuilder<SubGraph2>
{
    public SubGraphEmbeddingBuilder(IProjection<SubGraph2> projection,
        IEmbeddingGenerator<string, Embedding<float>> embedding,
        IEmbeddingDocumentRenderer renderer)
        : base(projection, embedding, renderer)
    {
    }
}
