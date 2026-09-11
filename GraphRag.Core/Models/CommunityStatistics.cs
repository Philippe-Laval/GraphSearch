using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/// <summary>
/// Contains statistics about a graph community, such as the number of nodes, 
/// the number of edges, density, average degree, node and edge types, 
/// and the most frequent keywords.
/// </summary>
public sealed record CommunityStatistics
{
    /// <summary>
    /// Number of nodes in the community.
    /// </summary>
    public required int NodeCount { get; init; }

    /// <summary>
    /// Number of edges connecting nodes within the community.
    /// </summary>
    public required int EdgeCount { get; init; }

    /// <summary>
    /// Density measures the ratio of existing edges to the maximum number of possible edges.
    /// The value is in [0 - 1.0], where 0 means no edges and 1 means a complete graph 
    /// where every node is connected to every other node.
    /// </summary>
    public required float Density { get; init; }

    /// <summary>
    /// The average degree of a graph community.
    /// The degree of a node in a graph is the number of edges connected to it.
    /// The average degree is therefore the sum of all degrees divided by the number of nodes.
    /// </summary>
    public required float AverageDegree { get; init; }

    /// <summary>
    /// Gets the node types and the associated count in the community.
    /// The key is the node type, and the value is the count of nodes of that type.
    /// </summary>
    public required IReadOnlyDictionary<string, int> NodeTypes { get; init; }

    /// <summary>
    /// Gets the relation types and the associated count in the community.
    /// The key is the relation type, and the value is the count of relations of that type.
    /// </summary>
    public required IReadOnlyDictionary<string, int> RelationTypes { get; init; }

    /// <summary>
    /// Gets the top keywords in the community.
    /// </summary>
    public required IReadOnlyList<string> TopKeywords { get; init; }
}
