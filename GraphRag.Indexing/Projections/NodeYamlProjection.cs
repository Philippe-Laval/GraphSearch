using System;
using System.Collections.Generic;
using System.Text;
using GraphRag.Core.Interfaces;
using GraphRag.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GraphRag.Indexing.Projections;

public sealed class NodeYamlProjection : IYamlProjection<GraphNode>
{
    private readonly ISerializer _serializer =
        new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    public string Project(GraphNode node)
    {
        var dto = new
        {
            id = node.Id,
            type = node.Type,
            label = node.Label,
            description = node.Description,
            // Sort the properties by key and normalize their values
            // Les embeddings changeraient alors que l'information est identique. Il faut donc toujours trier.
            properties =
                node.Properties
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