using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Models;

/// <summary>
/// Options for the community stabilizer, which merges or splits communities based on their size.
/// </summary>
public sealed class CommunityStabilizerOptions
{
    /// <summary>
    /// A community with fewer nodes than this number
    /// will be merged with the nearest one.
    /// </summary>
    public int MinimumNodes { get; init; } = 5;

    /// <summary>
    /// A community with more nodes than this number
    /// will be split into smaller ones.
    /// </summary>
    public int MaximumNodes { get; init; } = 500;

    /// <summary>
    /// The maximum number of split iterations.
    /// </summary>
    public int MaxSplitIterations { get; init; } = 5;
}
