using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using Microsoft.Extensions.AI;
using System.Text;

namespace GraphRag.AI.Generators;

/*
Éviter les hallucinations
Je conseille de fixer une température très faible (voire nulle) pour ces générateurs. Ils ne doivent pas inventer des faits mais reformuler fidèlement les données du graphe.

Le prompt système peut rappeler explicitement :
Tu n'inventes jamais d'information.
Tu reformules uniquement les informations présentes.

Ainsi, les descriptions restent stables au fil des réindexations. 
 */

public sealed class SubGraphLLMTextGenerator
    : ITextGenerator<SubGraph>
{
    private readonly IChatClient _chatClient;
    private readonly IKnowledgeGraph _graph;

    public SubGraphLLMTextGenerator(IChatClient chatClient, IKnowledgeGraph graph)
    {
        _chatClient = chatClient;
        _graph = graph;
    }

    public async Task<string> GenerateAsync(
        SubGraph graph,
        CancellationToken ct = default)
    {
        var builder = new StringBuilder();

        foreach (var edge in graph.Edges)
        {
            var sourceNode = _graph.GetNode(edge.SourceId);
            var targetNode = _graph.GetNode(edge.TargetId);

            builder.AppendLine($"{sourceNode.Label} --{edge.Type}--> {targetNode.Label}");
        }

        string graphText = builder.ToString();

        var prompt =
            $$"""
            Tu es chargé de transformer un sous-graphe en un document destiné à une recherche sémantique.

            Conserve :
            - toutes les entités
            - toutes les relations
            - les dépendances
            - les concepts

            Ne résume pas.

            Sous-graphe :

            {{graphText}}
            """;

        var response = await _chatClient.GetResponseAsync(prompt, cancellationToken: ct);
        return response.Messages[0].Text;
    }
}
