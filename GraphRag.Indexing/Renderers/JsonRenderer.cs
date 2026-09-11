using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;
using System.Text.Json;

namespace GraphRag.Indexing.Renderers;

/// <summary>
/// Renders an EmbeddingDocument as a JSON string.
/// Le json est celui destiné aux échanges par API, tests automatisés et diagnostics.
/// </summary>
public sealed class JsonRenderer : IEmbeddingDocumentRenderer
{
    public string Format => "json";

    /// <summary>
    /// Creates a JSON representation of the EmbeddingDocument.
    /// </summary>
    /// <typeparam name="T">The type of the document's content.</typeparam>
    /// <param name="document">The document to render.</param>
    /// <returns>A JSON representation of the document.</returns>
    public string Render<T>(EmbeddingDocument<T> document)
    {
        return JsonSerializer.Serialize(
            document,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
    }
}

