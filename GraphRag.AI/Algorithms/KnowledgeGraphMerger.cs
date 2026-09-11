using GraphRag.Core.Models;
using GraphRag.Graph.Algorithms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.AI.Algorithms;

/// <summary>
/// 
/// </summary>
public sealed class KnowledgeGraphMerger(ILogger<KnowledgeGraphMerger> logger)
{
    /// <summary>
    /// This first version merges nodes by normalized canonical name or alias.
    /// </summary>
    /// <param name="graphs"></param>
    /// <returns></returns>
    public KnowledgeGraph Merge(
        IEnumerable<KnowledgeGraph> graphs)
    {
        var mergedNodes =
            new Dictionary<string, MutableNode>(
                StringComparer.OrdinalIgnoreCase);

        var idMappings =
            new List<Dictionary<string, string>>();

        foreach (KnowledgeGraph graph in graphs)
        {
            var localMapping =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (KnowledgeNode node in graph.Nodes)
            {
                string canonicalKey =
                    FindCanonicalKey(node, mergedNodes);

                if (!mergedNodes.TryGetValue(
                        canonicalKey,
                        out MutableNode? existing))
                {
                    existing = MutableNode.From(node);
                    mergedNodes.Add(canonicalKey, existing);
                }
                else
                {
                    existing.Merge(node);
                }

                localMapping[node.Id] = canonicalKey;
            }

            idMappings.Add(localMapping);
        }

        var mergedEdges =
            new Dictionary<EdgeKey, MutableEdge>();

        int graphIndex = 0;

        foreach (KnowledgeGraph graph in graphs)
        {
            Dictionary<string, string> mapping =
                idMappings[graphIndex++];

            foreach (KnowledgeEdge edge in graph.Edges)
            {
                if (!mapping.TryGetValue(
                        edge.SourceId,
                        out string? sourceKey) ||
                    !mapping.TryGetValue(
                        edge.TargetId,
                        out string? targetKey))
                {
                    continue;
                }

                if (sourceKey.Equals(
                        targetKey,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var key = new EdgeKey(
                    sourceKey,
                    targetKey,
                    edge.Relation.Trim().ToUpperInvariant());

                if (!mergedEdges.TryGetValue(
                        key,
                        out MutableEdge? existingEdge))
                {
                    mergedEdges[key] =
                        MutableEdge.From(key, edge);
                }
                else
                {
                    existingEdge.Merge(edge);
                }
            }
        }

        var finalNodes = mergedNodes
            .Select(pair => pair.Value.ToNode(pair.Key))
            .ToArray();

        var finalEdges = mergedEdges.Values
            .Select(edge => edge.ToEdge())
            .ToArray();

        return new KnowledgeGraph
        {
            Nodes = finalNodes,
            Edges = finalEdges
        };
    }

    private static string FindCanonicalKey(
        KnowledgeNode node,
        IReadOnlyDictionary<string, MutableNode> existingNodes)
    {
        var candidateNames = node.Aliases
            .Append(node.Name)
            .Select(EntityNameNormalizer.Normalize)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach ((string key, MutableNode existing) in existingNodes)
        {
            if (candidateNames.Contains(key) ||
                existing.NormalizedNames.Overlaps(candidateNames))
            {
                return key;
            }
        }

        return EntityNameNormalizer.Normalize(node.Name);
    }

    private sealed class MutableNode
    {
        private readonly HashSet<string> _aliases =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> _evidence =
            new(StringComparer.Ordinal);

        public required string Name { get; init; }

        public required string Type { get; init; }

        public required string Description { get; set; }

        public double Importance { get; set; }

        public HashSet<string> NormalizedNames { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        public static MutableNode From(KnowledgeNode node)
        {
            var result = new MutableNode
            {
                Name = node.Name,
                Type = node.Type,
                Description = node.Description,
                Importance = node.Importance
            };

            result.AddNames(node);
            result._evidence.Add(node.Evidence);

            return result;
        }

        public void Merge(KnowledgeNode node)
        {
            Importance = Math.Max(
                Importance,
                node.Importance);

            if (node.Description.Length > Description.Length)
            {
                Description = node.Description;
            }

            AddNames(node);
            _evidence.Add(node.Evidence);
        }

        public KnowledgeNode ToNode(string id)
        {
            return new KnowledgeNode
            {
                Id = id,
                Name = Name,
                Type = Type,
                Description = Description,
                Aliases = _aliases.ToArray(),
                Importance = Importance,
                Evidence = string.Join(
                    Environment.NewLine,
                    _evidence)
            };
        }

        private void AddNames(KnowledgeNode node)
        {
            NormalizedNames.Add(
                EntityNameNormalizer.Normalize(node.Name));

            foreach (string alias in node.Aliases)
            {
                _aliases.Add(alias);
                NormalizedNames.Add(
                    EntityNameNormalizer.Normalize(alias));
            }
        }
    }

    private sealed record EdgeKey(
        string SourceId,
        string TargetId,
        string Relation);

    private sealed class MutableEdge
    {
        private readonly HashSet<string> _evidence =
            new(StringComparer.Ordinal);

        public required EdgeKey Key { get; init; }

        public required string Description { get; set; }

        public double Confidence { get; set; }

        public static MutableEdge From(
            EdgeKey key,
            KnowledgeEdge edge)
        {
            var result = new MutableEdge
            {
                Key = key,
                Description = edge.Description,
                Confidence = edge.Confidence
            };

            result._evidence.Add(edge.Evidence);

            return result;
        }

        public void Merge(KnowledgeEdge edge)
        {
            Confidence = Math.Max(
                Confidence,
                edge.Confidence);

            if (edge.Description.Length > Description.Length)
            {
                Description = edge.Description;
            }

            _evidence.Add(edge.Evidence);
        }

        public KnowledgeEdge ToEdge()
        {
            return new KnowledgeEdge
            {
                SourceId = Key.SourceId,
                TargetId = Key.TargetId,
                Relation = Key.Relation,
                Description = Description,
                Confidence = Confidence,
                Evidence = string.Join(
                    Environment.NewLine,
                    _evidence)
            };
        }
    }
}
