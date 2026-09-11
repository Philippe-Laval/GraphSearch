using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;

namespace GraphRag.AI.Generators;

/*
Éviter les hallucinations
Je conseille de fixer une température très faible (voire nulle) pour ces générateurs. Ils ne doivent pas inventer des faits mais reformuler fidèlement les données du graphe.

Le prompt système peut rappeler explicitement :
Tu n'inventes jamais d'information.
Tu reformules uniquement les informations présentes.

Ainsi, les descriptions restent stables au fil des réindexations. 
 */

public sealed class NodeLLMTextGenerator : ITextGenerator<GraphNode>
{
    private readonly IChatClient _chatClient;

    public NodeLLMTextGenerator(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> GenerateAsync(
        GraphNode node,
        CancellationToken ct = default)
    {
        // Pourquoi demander de ne pas résumer ?
        // Parce qu'un embedding est meilleur lorsqu'il contient le maximum d'information utile.
        var prompt =
            $$"""
            Tu es un expert en représentation de connaissances.
            Produis une description textuelle destinée à une recherche sémantique.

            Ne résume pas.

            Conserve :
            - le nom
            - le type
            - les propriétés importantes
            - les synonymes éventuels
            - les concepts métier

            Type :
            {{node.Type}}

            Nom :
            {{node.Label}}

            Description :
            {{node.Description}}
            """;

        var response = await _chatClient.GetResponseAsync(prompt, cancellationToken: ct);
        return response.Messages[0].Text;
    }
}
