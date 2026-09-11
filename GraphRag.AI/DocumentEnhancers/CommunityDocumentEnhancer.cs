using GraphRag.Core.Interfaces;
using Microsoft.Extensions.AI;
using System.Threading;
using System.Threading.Tasks;

namespace GraphRag.AI.DocumentEnhancers;

/// <summary>
/// Enhances a YAML representation of a community by reformulating it in natural language, 
/// improving fluency, adding useful synonyms, and strictly preserving all information 
/// without inventing or modifying any proper names.
/// </summary>
public sealed class CommunityDocumentEnhancer : IEmbeddingDocumentEnhancer
{
    private readonly IChatClient _chat;

    public CommunityDocumentEnhancer(IChatClient chat)
    {
        _chat = chat;
    }

    /// <summary>
    /// Enhances a YAML representation of a community by reformulating it in natural language, 
    /// improving fluency, adding useful synonyms, and strictly preserving all information 
    /// without inventing or modifying any proper names.
    /// </summary>
    /// <param name="yaml">The YAML representation of the community to enhance.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The enhanced YAML representation as a string.</returns>
    public async Task<string> EnhanceAsync(string yaml, CancellationToken cancellationToken)
    {
        var prompt =
            $"""
            Tu reçois le résumé YAML d'une communauté de graphe.

            Explique en langage naturel :
            - les principaux sujets
            - les concepts dominants
            - les dépendances

            Ne modifie jamais les informations.

            YAML

            {yaml}
            """;

        var response = await _chat.GetResponseAsync(prompt, cancellationToken: cancellationToken);
        return response.Messages[0].Text;
    }
}
