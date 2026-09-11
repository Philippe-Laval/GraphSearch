using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GraphRag.Core.Models;

// Using System.Text.Json.Serialization.JsonPropertyName to specify the property names in the JSON representation

public class RerankerAnalysis
{
    [property: JsonPropertyName("relevance_score")]
    public required float RelevanceScore { get; init; }

    [property: JsonPropertyName("explanation")]
    public required string Explanation { get; init; }
}

/// <summary>
/// For Native AOT or trimming, use a source-generated resolver
/// </summary>
[JsonSerializable(typeof(RerankerAnalysis))]
public partial class RerankerAnalysisJsonContext : JsonSerializerContext;