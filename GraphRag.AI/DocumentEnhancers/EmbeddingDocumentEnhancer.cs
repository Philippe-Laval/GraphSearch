using GraphRag.Core.Interfaces;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.AI.DocumentEnhancers;

/// <summary>
/// Enhances a YAML representation of a subgraph by reformulating it in natural language, 
/// improving fluency, adding useful synonyms, and strictly preserving all information 
/// without inventing or modifying any proper names.
/// </summary>
public sealed class EmbeddingDocumentEnhancer : IEmbeddingDocumentEnhancer
{
    private readonly IChatClient _chat;

    public EmbeddingDocumentEnhancer(IChatClient chat)
    {
        _chat = chat;
    }

    /// <summary>
    /// Enhances a YAML representation of a subgraph by reformulating it in natural language, 
    /// improving fluency, adding useful synonyms, and strictly preserving all information 
    /// without inventing or modifying any proper names.
    /// </summary>
    /// <param name="yaml">The YAML representation of the subgraph to enhance.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The enhanced YAML representation as a string.</returns>
    public async Task<string> EnhanceAsync(string yaml, CancellationToken cancellationToken)
    {
        var prompt =
            $"""
            Tu es un assistant spécialisé dans la préparation de documents destinés à une recherche sémantique.
            Le document suivant est une représentation YAML d'un sous-graphe.

            Ta mission :
            - reformuler en langage naturel
            - améliorer la fluidité
            - ajouter quelques synonymes utiles
            - conserver STRICTEMENT toutes les informations
            - ne rien inventer
            - ne supprimer aucune information
            - ne modifier aucun nom propre

            YAML

            {yaml}
            """;

        var response = await _chat.GetResponseAsync(prompt, cancellationToken: cancellationToken);
        return response.Messages[0].Text;
    }
}
