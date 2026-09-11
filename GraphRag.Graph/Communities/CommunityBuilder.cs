using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphRag.Graph.Communities;

public sealed class CommunityBuilder
{
    /// <summary>
    /// Construit des communautés de graphe à partir d'un instantané de graphe et des informations d'appartenance.
    /// </summary>
    /// <param name="graph">L'instantané du graphe contenant les nœuds et les arêtes.</param>
    /// <param name="memberships">Les informations d'appartenance des nœuds aux communautés.</param>
    /// <returns>Une collection de communautés de graphe construites.</returns>
    public static IReadOnlyCollection<GraphCommunity> Build(
        GraphSnapshot graph,
        IEnumerable<CommunityMembership> memberships)
    {
        var lookup =
            memberships.ToLookup(x => x.CommunityId);

        var result = new List<GraphCommunity>();

        foreach (var community in lookup)
        {
            var ids =
                community
                    .Select(x => x.NodeId)
                    .ToHashSet();

            var nodes =
                graph.Nodes
                    .Where(x => ids.Contains(x.Id))
                    .ToList();

            var edges =
                graph.Edges
                    .Where(e =>
                        ids.Contains(e.SourceId) &&
                        ids.Contains(e.TargetId))
                    .ToList();

            result.Add(
                new GraphCommunity
                {
                    Id = Guid.NewGuid(),
                    Nodes = nodes,
                    Edges = edges,
                    Level = 0,
                    Modularity = 0
                });
        }

        return result;
    }
}
