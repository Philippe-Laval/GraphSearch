using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Graph.Communities;

public sealed class CommunitySignatureBuilder : ICommunitySignatureBuilder
{
    public CommunityFingerprint Build(GraphCommunity community)
    {
        return new()
        {
            CanonicalHash =
                ComputeCanonicalHash(community),

            CoreHash =
                ComputeCoreHash(community),

            StructureHash =
                ComputeStructureHash(community),

            KeywordHash =
                ComputeKeywordHash(community),

            NodeCount =
                community.Nodes.Count,

            EdgeCount =
                community.Edges.Count
        };
    }

    private string ComputeKeywordHash(GraphCommunity community)
    {
        throw new NotImplementedException();
    }

    private string ComputeStructureHash(GraphCommunity community)
    {
        throw new NotImplementedException();
    }

    private string ComputeCoreHash(GraphCommunity community)
    {
        throw new NotImplementedException();
    }

    private string ComputeCanonicalHash(GraphCommunity community)
    {
        throw new NotImplementedException();
    }


}