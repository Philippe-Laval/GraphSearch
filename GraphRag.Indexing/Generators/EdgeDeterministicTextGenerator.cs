using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System.Text;

namespace GraphRag.Indexing.Generators;

/// <summary>
/// Deterministic text generator for graph edges.
/// Produces a structured output (Markdown or YAML) with all fields in a consistent order.
/// </summary>
public sealed class EdgeDeterministicTextGenerator : ITextGenerator<GraphEdge>
{
    private readonly DeterministicTextFormat _format;

    public EdgeDeterministicTextGenerator(DeterministicTextFormat format = DeterministicTextFormat.Markdown)
    {
        _format = format;
    }

    /// <summary>
    /// Generate the deterministic text representation of a graph edge in the specified format (Markdown or YAML).
    /// </summary>
    /// <param name="edge">The graph edge to generate text for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated text.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified format is not supported.</exception>
    public Task<string> GenerateAsync(GraphEdge edge, CancellationToken cancellationToken = default)
    {
        var result = _format switch
        {
            DeterministicTextFormat.Markdown => GenerateMarkdown(edge),
            DeterministicTextFormat.Yaml => GenerateYaml(edge),
            _ => throw new ArgumentOutOfRangeException(nameof(_format), _format, "Unsupported format")
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Generates a Markdown representation of a graph edge, including its ID, type, source, and target.
    /// </summary>
    /// <param name="edge">The graph edge to format.</param>
    /// <returns>A string in Markdown format containing the edge's information.</returns>
    private static string GenerateMarkdown(GraphEdge edge)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# Edge: {EscapeMarkdown(edge.Type)}");
        sb.AppendLine();
        sb.AppendLine("## Information");
        sb.AppendLine();
        sb.AppendLine($"- **ID**: `{edge.Id}`");
        sb.AppendLine($"- **Type**: {EscapeMarkdown(edge.Type)}");
        sb.AppendLine($"- **Source**: `{edge.SourceId}`");
        sb.AppendLine($"- **Target**: `{edge.TargetId}`");

        return sb.ToString();
    }

    /// <summary>
    /// Generates a YAML string representation of a graph edge.
    /// </summary>
    /// <param name="edge">The graph edge to serialize to YAML format.</param>
    /// <returns>A YAML string containing the edge's id, type, source_id, and target_id properties.</returns>
    private static string GenerateYaml(GraphEdge edge)
    {
        var sb = new StringBuilder();

        sb.AppendLine("edge:");
        sb.AppendLine($"  id: \"{EscapeYaml(edge.Id.ToString())}\"");
        sb.AppendLine($"  type: \"{EscapeYaml(edge.Type)}\"");
        sb.AppendLine($"  source_id: \"{EscapeYaml(edge.SourceId.ToString())}\"");
        sb.AppendLine($"  target_id: \"{EscapeYaml(edge.TargetId.ToString())}\"");

        return sb.ToString();
    }

    /// <summary>
    /// Échappe les caractères spéciaux Markdown en les préfixant avec un backslash.
    /// </summary>
    /// <param name="text">Le texte à échapper.</param>
    /// <returns>Le texte avec les caractères Markdown échappés, ou la valeur d'origine si elle est null ou vide.</returns>
    private static string EscapeMarkdown(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return text
            .Replace("\\", "\\\\")
            .Replace("`", "\\`")
            .Replace("*", "\\*")
            .Replace("_", "\\_")
            .Replace("[", "\\[")
            .Replace("]", "\\]");
    }

    /// <summary>
    /// Escape les caractères spéciaux pour le format YAML afin d'éviter les problèmes de parsing.
    /// </summary>
    /// <param name="text">Le texte à échapper.</param>
    /// <returns>Le texte échappé pour le format YAML.</returns>
    private static string EscapeYaml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }
}
