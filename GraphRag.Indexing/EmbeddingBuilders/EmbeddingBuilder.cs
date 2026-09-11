using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Indexing.EmbeddingBuilders;

public abstract class EmbeddingBuilder<T>
{
    private readonly IProjection<T> _projection;

    private readonly IEmbeddingGenerator<string, Embedding<float>> _embedding;
            
    private readonly IEmbeddingDocumentRenderer _renderer;

    protected EmbeddingBuilder(
        IProjection<T> projection,
        IEmbeddingGenerator<string, Embedding<float>> embedding,
        IEmbeddingDocumentRenderer renderer)
    {
        _projection = projection;
        _embedding = embedding;
        _renderer = renderer;
    }

    /// <summary>
    /// Builds a VectorDocument from the given entity.
    /// </summary>
    /// <param name="entity">The entity to build the VectorDocument from.</param>
    /// <returns>A VectorDocument representing the entity.</returns>
    public async Task<VectorDocument> BuildAsync(T entity)
    {
        EmbeddingDocument<T> document = _projection.Project(entity);

        var text = _renderer.Render<T>(document);

        var embedding = await _embedding.GenerateAsync(text);

        return new VectorDocument(
            document.Metadata.EntityHash,
            embedding,
            text);
    }
}
