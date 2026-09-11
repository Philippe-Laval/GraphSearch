using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

public sealed record CommunityFingerprint
{
    public required string CanonicalHash { get; init; }

    public required string CoreHash { get; init; }

    public required string StructureHash { get; init; }

    public required string KeywordHash { get; init; }

    public required int NodeCount { get; init; }

    public required int EdgeCount { get; init; }
}
