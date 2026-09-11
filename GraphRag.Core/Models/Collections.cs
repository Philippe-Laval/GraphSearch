using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace GraphRag.Core.Models;

/*
Le pipeline de recherche deviendrait hiérarchique :

Question
    │
    ▼
Recherche Community
    │
    ▼
Recherche SubGraph
    │
    ▼
Recherche Node / Edge
    │
    ▼
Fusion + Re-ranking
    │
    ▼
Contexte du LLM 
 */

internal class Collections
{
    // Collection        Contenu                     Usage
    // GraphNodes        Une entité                  Recherche précise d'objets
    // GraphEdges        Une relation                Recherche de faits
    // GraphSubGraphs    Un voisinage local          Construction du contexte
    // GraphCommunities  Un domaine métier entier    Recherche à grande échelle

    public List<GraphNode> GraphNodes { get; set; } = new List<GraphNode>();
    public List<GraphEdge> GraphEdges { get; set; } = new List<GraphEdge>();
    public List<SubGraph2> GraphSubGraphs { get; set; } = new List<SubGraph2>();
    public List<GraphCommunity> GraphCommunities { get; set; } = new List<GraphCommunity>();
}
