using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/*
Dans un système GraphRAG industriel, j'ajouterais un en-tête de métadonnées afin de rendre 
chaque document traçable et de faciliter l'invalidation du cache des embeddings.

Grâce à ces métadonnées, tu peux :
- invalider automatiquement les embeddings si le schéma évolue (schema ou generatorVersion) ;
- comparer deux versions d'un document sans recalculer inutilement les embeddings ;
- utiliser le même pipeline pour des nœuds, des arêtes ou des sous-graphes avec une structure homogène.

C'est une pratique courante dans les pipelines d'indexation de grande taille, car elle améliore la reproductibilité et simplifie considérablement la maintenance.
 */
public sealed record EmbeddingDocument
{
    /// <summary>
    /// Métadonnées du document, incluant des informations sur le schéma et la version du générateur.
    /// </summary>
    public required Metadata Metadata { get; init; }

    /// <summary>
    /// Contenu du document.
    /// </summary>
    public required object Content { get; init; }
}

// La logique métier ne connaît jamais YAML, Markdown ou JSON.
public sealed record EmbeddingDocument<T>(
    Metadata Metadata,
    T Content);
