using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Interfaces;

/*
Pour un GraphRAG, je ne m'arrêterais pas à LPA "pur". J'introduirais une phase de post-traitement qui améliore sensiblement la qualité des communautés :

Fusion des petites communautés : toute communauté contenant moins de N nœuds (par exemple 5) est fusionnée avec la communauté voisine ayant le plus grand poids de relations.
Scission des communautés trop grandes : si une communauté dépasse un seuil (par exemple 500 ou 1 000 nœuds), on relance LPA uniquement sur cette communauté ou on utilise un autre algorithme pour la subdiviser.
Calcul de statistiques : nombre de nœuds, nombre d'arêtes, distribution des types de nœuds, distribution des types de relations, principaux mots-clés et score de cohésion. Ces informations enrichissent ensuite le CommunityDocument projeté et peuvent servir au classement ou au résumé.
*/

public interface ICommunityStabilizer
{
    IReadOnlyCollection<GraphCommunity> Stabilize(
        GraphSnapshot graph,
        IReadOnlyCollection<GraphCommunity> communities);
}
