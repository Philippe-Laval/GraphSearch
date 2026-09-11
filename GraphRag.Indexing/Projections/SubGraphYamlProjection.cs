using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GraphRag.Indexing.Projections;

public sealed class SubGraphYamlProjection : IYamlProjection<SubGraph2>
{
    private readonly ISerializer _serializer =
        new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    public string Project(SubGraph2 graph)
    {
        // L'ordre est toujours identique.
        // C'est déjà un bon candidat pour un embedding.
        var dto = new
        {
            nodes =
                graph.Nodes
                    .OrderBy(n => n.Type)
                    .ThenBy(n => n.Label)
                    .Select(n => new
                    {
                        n.Id,
                        n.Type,
                        n.Label,
                        n.Description
                    }),

            edges =
                graph.Edges
                    .OrderBy(e => e.Source.Label)
                    .ThenBy(e => e.Edge.Type)
                    .ThenBy(e => e.Target.Label)
                    .Select(e => new
                    {
                        source = e.Source.Label,
                        relation = e.Edge.Type,
                        target = e.Target.Label
                    })
        };

        return _serializer.Serialize(dto);
    }
}