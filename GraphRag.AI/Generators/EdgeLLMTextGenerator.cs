using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using Microsoft.Extensions.AI;

namespace GraphRag.AI.Generators;

/*
Éviter les hallucinations
Je conseille de fixer une température très faible (voire nulle) pour ces générateurs. Ils ne doivent pas inventer des faits mais reformuler fidèlement les données du graphe.

Le prompt système peut rappeler explicitement :
Tu n'inventes jamais d'information.
Tu reformules uniquement les informations présentes.

Ainsi, les descriptions restent stables au fil des réindexations. 
 */


public sealed class EdgeLLMTextGenerator
    : ITextGenerator<GraphEdgeContext>
{
    private readonly IChatClient _chatClient;

    public EdgeLLMTextGenerator(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> GenerateAsync(
        GraphEdgeContext edge,
        CancellationToken ct = default)
    {
        var prompt =
            $$"""
            Décris la relation suivante.

            Nœud source :
            {{edge.Source.Label}}

            Relation :
            {{edge.Edge.Type}}

            Nœud cible :
            {{edge.Target.Label}}

            Explique le sens métier de cette relation.
            Le texte sera utilisé pour générer un embedding.
            """;

        var response = await _chatClient.GetResponseAsync(prompt, cancellationToken: ct);
        return response.Messages[0].Text;
    }
}
