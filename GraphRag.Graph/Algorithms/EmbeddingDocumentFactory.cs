using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Graph.Algorithms;

public sealed class EmbeddingDocumentFactory
{
    /// <summary>
    /// Creates an EmbeddingDocument with the provided parameters.
    /// </summary>
    /// <param name="graphId">The unique identifier of the graph.</param>
    /// <param name="type">The type of the document.</param>
    /// <param name="content">The content of the document.</param>
    /// <param name="nodeCount">The number of nodes in the graph.</param>
    /// <param name="edgeCount">The number of edges in the graph.</param>
    /// <param name="serializedContent">The serialized representation of the content.</param>
    /// <returns>An instance of <see cref="EmbeddingDocument"/> with the provided parameters.</returns>
    public static EmbeddingDocument Create(
        string graphId,
        string type,
        object content,
        int nodeCount,
        int edgeCount,
        string serializedContent)
    {
        return new EmbeddingDocument
        {
            Metadata = new Metadata
            {
                Schema = "graph-rag/v1",
                GeneratorVersion = "1.0.0",
                DocumentType = type,
                GraphId = graphId,
                GeneratedAtUtc = DateTime.UtcNow,
                EntityHash = GraphHash.Compute(serializedContent),
                NodeCount = nodeCount,
                EdgeCount = edgeCount,
                Culture = "fr-FR"
            },

            Content = content
        };
    }
}
