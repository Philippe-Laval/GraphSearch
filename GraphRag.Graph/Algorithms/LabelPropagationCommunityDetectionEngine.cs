using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Graph.Algorithms;

/// <summary>
/// LPA algorithm implementation for community detection in graphs.
/// Le résultat doit être déterministe, ce qui est essentiel pour éviter de régénérer les embeddings inutilement.
/// </summary>
public sealed class LabelPropagationCommunityDetectionEngine : ICommunityDetectionEngine
{
    private readonly IRelationWeightProvider _weights;

    public LabelPropagationCommunityDetectionEngine(
        IRelationWeightProvider weights)
    {
        _weights = weights;
    }

    public Task<IReadOnlyCollection<CommunityMembership>>
        DetectAsync(
            GraphSnapshot snapshot,
            CancellationToken cancellationToken = default)
    {
        var labels =
            snapshot.Nodes
                .Select((n, i) => (n.Id, Label: i))
                .ToDictionary(x => x.Id, x => x.Label);

        bool changed;

        int iteration = 0;

        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            changed = false;

            iteration++;

            // OrderBy garantit le même résultat à chaque exécution pour un graphe identique.
            // C'est indispensable pour conserver des embeddings stables.
            foreach (var node in snapshot.Nodes.OrderBy(x => x.Id))
            {
                var votes = new Dictionary<int, float>();

                foreach (var edge in snapshot.GetEdges(node.Id))
                {
                    var neighbour =
                        edge.SourceId == node.Id
                            ? edge.TargetId
                            : edge.SourceId;

                    var label = labels[neighbour];

                    votes.TryAdd(label, 0);

                    votes[label] +=
                        _weights.GetWeight(edge.Type);
                }

                if (votes.Count == 0)
                    continue;

                var best = votes
                        .OrderByDescending(x => x.Value)
                        .ThenBy(x => x.Key)
                        .First();

                if (labels[node.Id] != best.Key)
                {
                    labels[node.Id] = best.Key;
                    changed = true;
                }
            }

        } while (changed && iteration < 50);

        IReadOnlyCollection<CommunityMembership> result =
            labels
                .Select(x =>
                    new CommunityMembership(
                        x.Key,
                        x.Value))
                .ToList();

        return Task.FromResult(result);
    }
}