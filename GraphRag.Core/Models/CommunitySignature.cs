using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

public sealed record CommunitySignature
{
    public required FrozenSet<Guid> NodeIds { get; init; }

    public required FrozenDictionary<string, int> NodeTypes { get; init; }

    public required FrozenDictionary<string, int> RelationTypes { get; init; }

    public required FrozenSet<string> Keywords { get; init; }

    public required CommunityFingerprint Fingerprint { get; init; }
}

/*
Le score de stabilité serait alors calculé comme une combinaison de mesures de similarité :

Jaccard sur les identifiants de nœuds (NodeIds) ;
Cosine similarity sur les distributions de types de nœuds et de relations ;
Jaccard sur les mots-clés ;
Égalité des empreintes (Fingerprint) pour détecter les cas strictement identiques.

Par exemple :

CSI =
0.50 × Jaccard(NodeIds)
+ 0.20 × Cosine(NodeTypes)
+ 0.15 × Cosine(RelationTypes)
+ 0.10 × Jaccard(Keywords)
+ 0.05 × FingerprintEquality

Cette approche est beaucoup plus stable lorsqu'une communauté évolue progressivement (ajout ou suppression de quelques nœuds) et évite de considérer qu'elle est entièrement nouvelle à cause d'un simple changement mineur. Pour un GraphRAG de grande taille, c'est cette méthode que je privilégierais, car elle réduit fortement le nombre d'embeddings à régénérer tout en conservant des identifiants logiques stables.
 */