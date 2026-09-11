using GraphRag.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

public sealed record CommunityDocument(
    Metadata Metadata,
    CommunityProjection Community);


/*
 NodeDocument

EdgeDocument

SubGraphDocument

CommunityDocument
*/

/*
Résumé de communauté

Une communauté peut contenir
1200 nœuds
5400 relations

Impossible d'envoyer cela au LLM.

Je procéderais en deux temps :

Projection YAML

↓

Résumé automatique

↓

Résumé LLM

Le résumé automatique est déterministe.

Exemple

community:

    level: 2

    nodeTypes:

      Project : 52

      Person : 18

      Library : 33

    topRelations:

      Uses

      DependsOn

      CreatedBy

    keywords:

      Semantic Kernel

      Azure OpenAI

      Qdrant

      Agent Framework

Cette réduction peut être faite sans IA. 
 */