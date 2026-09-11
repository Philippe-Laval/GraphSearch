using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Graph.Algorithms;

public sealed class LouvainCommunityDetector
    : ICommunityDetector
{
    public async Task<IReadOnlyCollection<GraphCommunity>>
        DetectAsync(
            IKnowledgeGraph graph,
            CancellationToken ct)
    {
        // Adaptation vers la bibliothèque choisie
        // (QuikGraph, Neo4j GDS, etc.)

        throw new NotImplementedException();
    }
}

/*
LouvainCommunityDetector
LeidenCommunityDetector
Neo4jGdsCommunityDetector
MemgraphCommunityDetector
 */
