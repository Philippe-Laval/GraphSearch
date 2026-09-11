using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Indexing.Algorithms;

public sealed class EmbeddingDocumentPipeline
{
    public required IYamlProjection<SubGraph2> _projection { get; init; }
    public IEmbeddingDocumentEnhancer? _enhancer { get; init; } = null;

    public async Task<string> BuildAsync(SubGraph2 graph, CancellationToken cancellationToken = default)
    {
        var yaml = _projection.Project(graph);

        // If an enhancer is provided, enhance the YAML representation of the graph
        if (_enhancer is null)
            return yaml;

        return await _enhancer.EnhanceAsync(yaml, cancellationToken);
    }
}
