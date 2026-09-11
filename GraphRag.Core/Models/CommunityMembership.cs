using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/// <summary>
/// Represents the membership of a node in a community within a graph.
/// </summary>
/// <param name="NodeId">The unique identifier of the node.</param>
/// <param name="CommunityId">The identifier of the community.</param>
public sealed record CommunityMembership(
    Guid NodeId,
    int CommunityId);
