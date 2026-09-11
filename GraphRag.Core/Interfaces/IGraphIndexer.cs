using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace GraphRag.Core.Interfaces;

public interface IGraphIndexer
{
    /// <summary>
    /// Index a knowledge node into the graph database.
    /// </summary>
    /// <param name="node">The knowledge node to index.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task IndexNodeAsync(KnowledgeNode node);

    /// <summary>
    /// Index a knowledge edge into the graph database.
    /// </summary>
    /// <param name="edge">The knowledge edge to index.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task IndexEdgeAsync(KnowledgeEdge edge);

    /// <summary>
    /// Index a subgraph into the graph database.
    /// </summary>
    /// <param name="chunk">The subgraph chunk to index.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>   
    Task IndexSubgraphAsync(GraphChunk chunk);
}
