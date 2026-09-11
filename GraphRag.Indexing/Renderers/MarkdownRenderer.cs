using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Indexing.Renderers;

/// <summary>
/// Renders an EmbeddingDocument as a Markdown string.
/// Le Markdown est celui destiné au LLM.
/// Contexte envoyé au LLM lors des réponses (plus lisible pour le modèle).
/// </summary>
public sealed class MarkdownRenderer : IEmbeddingDocumentRenderer
{
    public string Format => "markdown";

    /// <summary>
    /// Creates a Markdown representation of the EmbeddingDocument.
    /// </summary>
    /// <typeparam name="T">The type of the document's content.</typeparam>
    /// <param name="document">The document to render.</param>
    /// <returns>A Markdown representation of the document.</returns>
    public string Render<T>(EmbeddingDocument<T> document)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# Knowledge Graph");
        sb.AppendLine();
        sb.AppendLine($"Schema : {document.Metadata.Schema}");
        sb.AppendLine($"Type : {document.Metadata.DocumentType}");
        sb.AppendLine();
        sb.AppendLine("```yaml");
        sb.AppendLine(new YamlRenderer().Render(document));
        sb.AppendLine("```");

        return sb.ToString();
    }
}

