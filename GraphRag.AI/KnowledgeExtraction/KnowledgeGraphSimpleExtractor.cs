using GraphRag.Core.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.AI.KnowledgeExtraction
{

    /// <summary>
    /// LLM-based extractor.
    /// This implementation is independent of OpenAI, Azure OpenAI or a local model. It only depends on IChatClient.
    /// </summary>
    /// <param name="chatClient"></param>
    /// <param name="logger"></param>
    public sealed class KnowledgeGraphSimpleExtractor(IChatClient chatClient, ILogger<KnowledgeGraphSimpleExtractor> logger) : IKnowledgeGraphSimpleExtractor
    {
        private const string SystemPrompt =
            """
                You extract a knowledge graph from supplied text.

                Rules:
                - Use only information explicitly supported by the text.
                - Do not use outside knowledge.
                - Extract only meaningful topics.
                - Prefer canonical names.
                - Merge abbreviations and aliases into one node.
                - Every edge must reference valid node identifiers.
                - Every node and edge must include supporting evidence.
                - Relationships are directed from SourceId to TargetId.
                - Use a concise uppercase relation name.
                - Prefer precise relations such as USES, PART_OF, DEPENDS_ON,
                  IMPLEMENTS, CAUSES, PRODUCES and LOCATED_IN.
                - Use RELATED_TO only when no more precise relation is justified.
                - Confidence must be between 0 and 1.
                - Importance must be between 0 and 1.
                - Do not create an edge only because two topics occur in the same text.
                """;

        public async Task<KnowledgeGraph> ExtractAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            var messages = new[]
            {
            new ChatMessage(ChatRole.System, SystemPrompt),
            new ChatMessage(
                ChatRole.User,
                $"""
                 Extract the main topics and their relationships from the following text.

                 <document>
                 {text}
                 </document>
                 """)
            };

            ChatResponse<KnowledgeGraph> response =
                await chatClient.GetResponseAsync<KnowledgeGraph>(
                    messages,
                    cancellationToken: cancellationToken);

            if (!response.TryGetResult(out KnowledgeGraph? graph) || graph is null)
            {
                throw new InvalidOperationException(
                    $"The model did not return a valid knowledge graph. " +
                    $"Raw response: {response.Text}");
            }

            Validate(graph);

            return graph;
        }

        private void Validate(KnowledgeGraph graph)
        {
            var nodeIds = graph.Nodes
                .Select(node => node.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var duplicateIds = graph.Nodes
                .GroupBy(node => node.Id, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();

            if (duplicateIds.Length > 0)
            {
                throw new InvalidOperationException(
                    $"Duplicate node identifiers: {string.Join(", ", duplicateIds)}");
            }

            foreach (KnowledgeNode node in graph.Nodes)
            {
                if (node.Importance is < 0 or > 1)
                {
                    throw new InvalidOperationException(
                        $"Invalid importance for node '{node.Id}'.");
                }
            }

            foreach (KnowledgeEdge edge in graph.Edges)
            {
                if (!nodeIds.Contains(edge.SourceId))
                {
                    throw new InvalidOperationException(
                        $"Unknown source node '{edge.SourceId}'.");
                }

                if (!nodeIds.Contains(edge.TargetId))
                {
                    throw new InvalidOperationException(
                        $"Unknown target node '{edge.TargetId}'.");
                }

                if (edge.Confidence is < 0 or > 1)
                {
                    throw new InvalidOperationException(
                        $"Invalid confidence for edge " +
                        $"'{edge.SourceId} -> {edge.TargetId}'.");
                }
            }
        }

    }

}
