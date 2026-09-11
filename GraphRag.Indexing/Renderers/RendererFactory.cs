using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Indexing.Renderers;

public sealed class RendererFactory
{
    private readonly Dictionary<string,IEmbeddingDocumentRenderer> _renderers;

    public RendererFactory(IEnumerable<IEmbeddingDocumentRenderer> renderers)
    {
        _renderers = renderers.ToDictionary(x => x.Format);
    }

    public IEmbeddingDocumentRenderer Get(string format) => _renderers[format];
}
