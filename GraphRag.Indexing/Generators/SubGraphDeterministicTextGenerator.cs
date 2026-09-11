using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System.Text;

namespace GraphRag.Indexing.Generators;

/// <summary>
/// Deterministic text generator for subgraphs.
/// Produces a structured output (Markdown or YAML) with all fields in a consistent order.
/// </summary>
public sealed class SubGraphDeterministicTextGenerator : ITextGenerator<SubGraph>
{
    private readonly DeterministicTextFormat _format;

    public SubGraphDeterministicTextGenerator(DeterministicTextFormat format = DeterministicTextFormat.Markdown)
    {
        _format = format;
    }

    /// <summary>
    /// Generate the deterministic text representation of a subgraph in the specified format (Markdown or YAML).
    /// </summary>
    /// <param name="subGraph">The subgraph to generate the text for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The deterministic text representation of the subgraph.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Task<string> GenerateAsync(SubGraph subGraph, CancellationToken cancellationToken = default)
    {
        var result = _format switch
        {
            DeterministicTextFormat.Markdown => GenerateMarkdown(subGraph),
            DeterministicTextFormat.Yaml => GenerateYaml(subGraph),
            _ => throw new ArgumentOutOfRangeException(nameof(_format), _format, "Unsupported format")
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Generates a Markdown representation of a subgraph with deterministic ordering of nodes and edges.
    /// </summary>
    /// <param name="subGraph">The subgraph to generate the Markdown for.</param>
    /// <returns>The Markdown representation of the subgraph.</returns>
    private static string GenerateMarkdown(SubGraph subGraph)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# Subgraph");
        sb.AppendLine();

        // Node Section
        sb.AppendLine("## Nodes");
        sb.AppendLine();

        if (subGraph.Nodes.Count == 0)
        {
            sb.AppendLine("*No nodes*");
        }
        else
        {
            // Deterministic sort by ID to ensure a stable order
            var sortedNodes = subGraph.Nodes.OrderBy(n => n.Id).ToList();

            foreach (var node in sortedNodes)
            {
                sb.AppendLine($"### {EscapeMarkdown(node.Label)}");
                sb.AppendLine();
                sb.AppendLine($"- **ID**: `{node.Id}`");
                sb.AppendLine($"- **Type**: {EscapeMarkdown(node.Type)}");
                sb.AppendLine($"- **Description**: {EscapeMarkdown(node.Description)}");
                sb.AppendLine();
            }
        }

        // Edge Section
        sb.AppendLine("## Edges");
        sb.AppendLine();

        if (subGraph.Edges.Count == 0)
        {
            sb.AppendLine("*No edges*");
        }
        else
        {
            // Deterministic sort by ID to ensure a stable order
            var sortedEdges = subGraph.Edges.OrderBy(e => e.Id).ToList();

            foreach (var edge in sortedEdges)
            {
                sb.AppendLine($"- `{edge.SourceId}` → `{edge.TargetId}` ({EscapeMarkdown(edge.Type)})");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates a YAML string representation of a subgraph with deterministic ordering of nodes and edges.
    /// </summary>
    /// <param name="subGraph">The subgraph to generate the YAML for.</param>
    /// <returns>The YAML representation of the subgraph.</returns>
    private static string GenerateYaml(SubGraph subGraph)
    {
        var sb = new StringBuilder();

        sb.AppendLine("subgraph:");

        // Node Section
        sb.AppendLine("  nodes:");

        if (subGraph.Nodes.Count == 0)
        {
            sb.AppendLine("    []");
        }
        else
        {
            // Deterministic sort by ID to ensure a stable order
            var sortedNodes = subGraph.Nodes.OrderBy(n => n.Id).ToList();

            foreach (var node in sortedNodes)
            {
                sb.AppendLine($"    - id: \"{EscapeYaml(node.Id.ToString())}\"");
                sb.AppendLine($"      type: \"{EscapeYaml(node.Type)}\"");
                sb.AppendLine($"      label: \"{EscapeYaml(node.Label)}\"");
                sb.AppendLine($"      description: \"{EscapeYaml(node.Description)}\"");
            }
        }

        // Edge Section
        sb.AppendLine("  edges:");

        if (subGraph.Edges.Count == 0)
        {
            sb.AppendLine("    []");
        }
        else
        {
            // Deterministic sort by ID to ensure a stable order
            var sortedEdges = subGraph.Edges.OrderBy(e => e.Id).ToList();

            foreach (var edge in sortedEdges)
            {
                sb.AppendLine($"    - id: \"{EscapeYaml(edge.Id.ToString())}\"");
                sb.AppendLine($"      type: \"{EscapeYaml(edge.Type)}\"");
                sb.AppendLine($"      source_id: \"{EscapeYaml(edge.SourceId.ToString())}\"");
                sb.AppendLine($"      target_id: \"{EscapeYaml(edge.TargetId.ToString())}\"");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Escapes special characters in a string for use in Markdown.
    /// </summary>
    /// <param name="text">The text to escape.</param>
    /// <returns>The escaped text.</returns>
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
    /// Escapes special characters in a string for YAML representation. 
    /// </summary>
    /// <param name="text">The string to escape.</param>
    /// <returns>The escaped string with backslashes, quotes, newlines, and carriage returns replaced by their escape sequences,
    /// or the original string if null or empty.</returns>
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
