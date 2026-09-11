using GraphRag.Core.DTOs;
using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Indexing.Projections;

public sealed class CommunityProjectionBuilder
{
    public CommunityProjection Project(GraphCommunity community)
    {
        // Toujours le même ordre. Toujours le même YAML.
        return new()
        {
            Id = community.Id,
            Level = community.Level,
            Modularity = community.Modularity,
            Nodes =
                community.Nodes
                    .OrderBy(x => x.Type)
                    .ThenBy(x => x.Label)
                    .Select(x => new NodeProjection
                    {
                        Id = x.Id,
                        Label = x.Label,
                        Type = x.Type,
                        Description = x.Description,
                        Properties = x.Properties
                    })
                    .ToList(),

            Edges =
                community.Edges
                    .OrderBy(x => x.Type)
                    .ThenBy(x => x.SourceId)
                    .ThenBy(x => x.TargetId)
                    .Select(x => new EdgeProjection
                    {
                        Source = x.SourceId.ToString(),
                        Relation = x.Type,
                        Target = x.TargetId.ToString()
                    })
                    .ToList()
        };
    }
}
