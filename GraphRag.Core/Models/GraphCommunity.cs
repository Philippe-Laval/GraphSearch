using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/*
Why Level?
Because a good GraphRAG implementation is hierarchical.

For example

Level 0
Entreprise

-----------------

Level 1
Développement
Infrastructure
RH

-----------------

Level 2
.NET
IA
Azure
DevOps

Communities become a tree.     
 */

/// <summary>
/// Represents a community within a knowledge graph, consisting of nodes and edges, 
/// along with its level and modularity score.
/// </summary>
public sealed record GraphCommunity
{
    public required Guid Id { get; init; }

    public required IReadOnlyCollection<GraphNode> Nodes { get; init; }

    public required IReadOnlyCollection<GraphEdge> Edges { get; init; }

    public required int Level { get; init; }

    /// <summary>
    /// Gets the modularity score.
    /// </summary>
    public required float Modularity { get; init; }
}

/*
A **modularity score** measures how strongly a graph is divided into communities.

A community has high modularity when:

* many edges connect nodes **inside** the community, and
* relatively few edges connect those nodes to the **rest of the graph**.

For a complete partition of an undirected graph, modularity is commonly defined as

[
Q=\frac{1}{2m}\sum_{i,j}
\left(A_{ij}-\frac{k_i k_j}{2m}\right)
\mathbf{1}(c_i=c_j),
]

where:

* (A_{ij}) indicates whether nodes (i) and (j) are connected,
* (k_i) and (k_j) are their degrees,
* (m) is the total number of edges,
* (\mathbf{1}(c_i=c_j)) equals 1 when the nodes belong to the same community.

The expression compares the **actual number of within-community edges** with the number expected in a randomized graph having approximately the same node degrees.

Interpretation:

* **Higher positive (Q):** strong community structure.
* **(Q \approx 0):** about as much internal connectivity as expected by chance.
* **Negative (Q):** fewer internal connections than expected.

Modularity is usually a score for the **entire partition**, although each community’s contribution can be calculated separately:

[
Q_C=\frac{l_C}{m}-\left(\frac{d_C}{2m}\right)^2,
]

where (l_C) is the number of edges within community (C), and (d_C) is the sum of the degrees of its nodes. 
*/