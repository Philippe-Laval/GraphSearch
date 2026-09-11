using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GraphRag.Indexing.Projections;

public sealed class EdgeYamlProjection : IYamlProjection<GraphEdgeContext>
{
    private readonly ISerializer _serializer =
        new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    public string Project(GraphEdgeContext edge)
    {
        var dto = new
        {
            relation = edge.Edge.Type,

            source = new
            {
                edge.Source.Id,
                edge.Source.Type,
                edge.Source.Label
            },

            target = new
            {
                edge.Target.Id,
                edge.Target.Type,
                edge.Target.Label
            },

            properties =
                edge.Edge.Properties
                    .OrderBy(x => x.Key)
                    .ToDictionary(
                        x => x.Key,
                        x => Normalize(x.Value))
        };

        return _serializer.Serialize(dto);
    }

    private static object? Normalize(object? value)
    {
        return value switch
        {
            DateTime d => d.ToUniversalTime().ToString("O"),
            IEnumerable<string> list =>
                list.OrderBy(x => x).ToArray(),
            _ => value
        };
    }
}
