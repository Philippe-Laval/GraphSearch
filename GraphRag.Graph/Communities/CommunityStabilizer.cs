using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Graph.Communities;

/// <summary>
/// Stabilizes graph communities by merging small communities with their neighbors 
/// and splitting large communities into smaller ones.
/// </summary>
public sealed class CommunityStabilizer : ICommunityStabilizer
{
    private readonly CommunityStabilizerOptions _options;
    private readonly ICommunityDetectionEngine _detector;

    public CommunityStabilizer(
        CommunityStabilizerOptions options,
        ICommunityDetectionEngine detector)
    {
        _options = options;
        _detector = detector;
    }

    /// <summary>
    /// Stabilizes graph communities by merging small communities and splitting large ones.
    /// </summary>
    /// <param name="graph">The graph snapshot used for stabilization operations.</param>
    /// <param name="communities">The communities to stabilize.</param>
    /// <returns>A collection of stabilized communities.</returns>
    public IReadOnlyCollection<GraphCommunity> Stabilize(
        GraphSnapshot graph,
        IReadOnlyCollection<GraphCommunity> communities)
    {
        var result = communities
            .Select(c => new MutableGraphCommunity
            {
                Id = c.Id,
                Nodes = new List<GraphNode>(c.Nodes),
                Edges = new List<GraphEdge>(c.Edges),
                Level = c.Level,
                Modularity = c.Modularity
            })
            .ToList();

        MergeSmallCommunities(graph, result);

        SplitLargeCommunities(graph, result);

        return result.Select(c => new GraphCommunity
        {
            Id = c.Id,
            Nodes = new List<GraphNode>(c.Nodes),
            Edges = new List<GraphEdge>(c.Edges),
            Level = c.Level,
            Modularity = c.Modularity
        }).ToList();
    }

    /// <summary>
    /// Merges small communities with their most connected neighboring community.
    /// </summary>
    /// <param name="graph">The graph snapshot used for stabilization operations.</param>
    /// <param name="communities">The communities to stabilize.</param>
    private void MergeSmallCommunities(
    GraphSnapshot graph,
    List<MutableGraphCommunity> communities)
    {
        foreach (var community in communities.ToList())
        {
            if (community.Nodes.Count >= _options.MinimumNodes)
                continue;

            var target =
                FindBestNeighbour(
                    graph,
                    community,
                    communities);

            if (target == null)
                continue;

            Merge(community, target);

            communities.Remove(community);
        }
    }

    /// <summary>
    /// Counts the number of edges between communities.
    /// </summary>
    /// <param name="graph">The graph snapshot used for stabilization operations.</param>
    /// <param name="community">The community for which to find the best neighbor.</param>
    /// <param name="all">All communities to consider as potential neighbors.</param>
    /// <returns>The most connected neighboring community, or null if none found.</returns>
    private MutableGraphCommunity? FindBestNeighbour(
    GraphSnapshot graph,
    MutableGraphCommunity community,
    IEnumerable<MutableGraphCommunity> all)
    {
        var ids =
            community.Nodes
                .Select(x => x.Id)
                .ToHashSet();

        return all
            .Where(x => x != community)
            .OrderByDescending(other =>
            {
                var otherIds =
                    other.Nodes
                        .Select(x => x.Id)
                        .ToHashSet();

                return graph.Edges.Count(e =>
                    ids.Contains(e.SourceId)
                        && otherIds.Contains(e.TargetId)
                    ||
                    ids.Contains(e.TargetId)
                        && otherIds.Contains(e.SourceId));
            })
            .FirstOrDefault();
    }

    private static void Merge(
        MutableGraphCommunity source,
        MutableGraphCommunity target)
    {
        target.Nodes.AddRange(source.Nodes);
        target.Edges.AddRange(source.Edges);
    }

    /// <summary>
    /// Splits large communities into smaller ones using the LPA algorithm.
    /// </summary>
    /// <param name="graph">The graph snapshot used for stabilization operations.</param>
    /// <param name="communities">The communities to stabilize.</param>
    private void SplitLargeCommunities(
    GraphSnapshot graph,
    List<MutableGraphCommunity> communities)
    {
        foreach (var community in communities.ToList())
        {
            if (community.Nodes.Count <= _options.MaximumNodes)
                continue;

            var subGraph =
                graph.Extract(community);

            var memberships =
                _detector
                    .DetectAsync(subGraph)
                    .Result;

            var split =
                CommunityBuilder.Build(
                    subGraph,
                    memberships);

            communities.Remove(community);

            // On transforme les communautés en MutableGraphCommunity pour les ajouter à la liste.
            var mutableSplit = split.Select(c => new MutableGraphCommunity
            {
                Id = c.Id,
                Nodes = new List<GraphNode>(c.Nodes),
                Edges = new List<GraphEdge>(c.Edges),
                Level = c.Level,
                Modularity = c.Modularity
            });

            communities.AddRange(mutableSplit);
        }
    }
}
