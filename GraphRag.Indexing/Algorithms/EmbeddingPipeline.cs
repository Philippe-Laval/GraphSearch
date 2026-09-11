using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace GraphRag.Indexing.Algorithms;

public sealed class EmbeddingPipeline : IEmbeddingDocumentPipeline
{
    public required IYamlProjection<SubGraph2> Projection { get; init; }
    public required ISerializer Serializer { get; init; }
    public IEmbeddingDocumentEnhancer? Enhancer { get; init; } = null;

    public async Task<string> BuildAsync(
        SubGraph2 graph,
        CancellationToken cancellationToken = default)
    {
        var projection = Projection.Project(graph);
        
        var yaml = Serializer.Serialize(projection);

        if (Enhancer == null)
            return yaml;

        return await Enhancer.EnhanceAsync(yaml, cancellationToken);
    }
}