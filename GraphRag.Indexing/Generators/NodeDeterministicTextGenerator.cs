using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System.Text;

namespace GraphRag.Indexing.Generators;

/// <summary>
/// Deterministic text generator for graph nodes.
/// Produces a structured output (Markdown or YAML) with all fields in a consistent order.
/// </summary>
public sealed class NodeDeterministicTextGenerator : ITextGenerator<GraphNode>
{
    private readonly DeterministicTextFormat _format;

    public NodeDeterministicTextGenerator(DeterministicTextFormat format = DeterministicTextFormat.Markdown)
    {
        _format = format;
    }

    /// <summary>
    /// Generate the deterministic text representation of a graph node in the specified format (Markdown or YAML).
    /// </summary>
    /// <param name="node">The graph node to generate text for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated text.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified format is not supported.</exception>
    public Task<string> GenerateAsync(GraphNode node, CancellationToken cancellationToken = default)
    {
        var result = _format switch
        {
            DeterministicTextFormat.Markdown => GenerateMarkdown(node),
            DeterministicTextFormat.Yaml => GenerateYaml(node),
            _ => throw new ArgumentOutOfRangeException(nameof(_format), _format, "Format non supporté")
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Generates a Markdown representation of a graph node, including its ID, type, label, and description in a structured format.
    /// </summary>
    /// <param name="node">The graph node to format.</param>
    /// <returns>A string in Markdown format containing the node's information.</returns>
    private static string GenerateMarkdown(GraphNode node)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {EscapeMarkdown(node.Label)}");
        sb.AppendLine();
        sb.AppendLine("## Information");
        sb.AppendLine();
        sb.AppendLine($"- **ID**: `{node.Id}`");
        sb.AppendLine($"- **Type**: {EscapeMarkdown(node.Type)}");
        sb.AppendLine($"- **Label**: {EscapeMarkdown(node.Label)}");
        sb.AppendLine();
        sb.AppendLine("## Description");
        sb.AppendLine();
        sb.AppendLine(EscapeMarkdown(node.Description));

        return sb.ToString();
    }

    /// <summary>
    /// Generates a YAML string representation of a graph node.
    /// </summary>
    /// <param name="node">The graph node to serialize to YAML format.</param>
    /// <returns>A YAML string containing the node's id, type, label, and description properties.</returns>
    private static string GenerateYaml(GraphNode node)
    {
        var sb = new StringBuilder();

        sb.AppendLine("node:");
        sb.AppendLine($"  id: \"{EscapeYaml(node.Id.ToString())}\"");
        sb.AppendLine($"  type: \"{EscapeYaml(node.Type)}\"");
        sb.AppendLine($"  label: \"{EscapeYaml(node.Label)}\"");
        sb.AppendLine($"  description: \"{EscapeYaml(node.Description)}\"");

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
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
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
