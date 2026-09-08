using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// LLM-backed analyzer. Sends the raw query to an <see cref="IChatCompletionClient"/>
/// and expects a JSON object of shape:
///
///   {
///     "normalizedQuery": "…",
///     "intent": "EntityLookup" | "Relationship" | "MultiHopRelationship" |
///               "Aggregation"  | "Comparison"   | "Explanation" | "General" | "Unknown",
///     "rewrites": ["…", "…"],
///     "confidence": 0.87
///   }
///
/// Combine with <see cref="CachingQueryAnalyzer"/> in production; LLM calls are
/// expensive and queries repeat a lot.
/// </summary>
public sealed class LlmQueryAnalyzer : IQueryAnalyzer
{
    private const string DefaultPromptTemplate =
        """
        You are a query analyzer for a GraphRAG system.
        Analyze the user query and reply with a JSON object matching this schema:
        {
          "normalizedQuery": "<canonical rewrite, single line, lowercase, no punctuation>",
          "intent": "EntityLookup | Relationship | MultiHopRelationship | Aggregation | Comparison | Explanation | General | Unknown",
          "rewrites": ["<alternate phrasings>", ...],
          "confidence": <float 0..1>
        }
        Reply with JSON only, no prose.

        User query:
        {{QUERY}}
        """;

    private readonly IChatCompletionClient _client;
    private readonly string _promptTemplate;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) },
    };

    public LlmQueryAnalyzer(IChatCompletionClient client, string? promptTemplate = null)
    {
        ArgumentNullException.ThrowIfNull(client);
        _client = client;
        _promptTemplate = promptTemplate ?? DefaultPromptTemplate;
    }

    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var prompt = _promptTemplate.Replace("{{QUERY}}", query, StringComparison.Ordinal);
        var raw = await _client.CompleteAsync(prompt, cancellationToken).ConfigureAwait(false);

        var payload = ExtractJson(raw);
        LlmResponse? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<LlmResponse>(payload, JsonOptions);
        }
        catch (JsonException)
        {
            parsed = null;
        }

        if (parsed is null || string.IsNullOrWhiteSpace(parsed.NormalizedQuery))
        {
            return new AnalyzedQuery(query, query.Trim(), QueryIntent.Unknown)
            {
                IntentConfidence = 0.0,
            };
        }

        return new AnalyzedQuery(
            OriginalQuery: query,
            NormalizedQuery: parsed.NormalizedQuery.Trim(),
            Intent: parsed.Intent)
        {
            Rewrites = (IReadOnlyList<string>?)parsed.Rewrites ?? Array.Empty<string>(),
            IntentConfidence = Math.Clamp(parsed.Confidence, 0.0, 1.0),
        };
    }

    /// <summary>
    /// Extracts the first {...} JSON block from the raw LLM response.
    /// </summary>
    /// <param name="raw">The raw response from the LLM</param>
    /// <returns>The extracted JSON string</returns>
    private static string ExtractJson(string raw)
    {
        // LLMs occasionally wrap JSON in ```json … ``` fences or add prose.
        // Trim to the first {...} block.

        if (string.IsNullOrWhiteSpace(raw))
        {
            return "{}";
        }

        var start = raw.IndexOf('{');
        var end = raw.LastIndexOf('}');
        if (start < 0 || end < 0 || end <= start)
        {
            return "{}";
        }
        
        return raw.Substring(start, end - start + 1);
    }

    private sealed class LlmResponse
    {
        public string? NormalizedQuery { get; set; }
        public QueryIntent Intent { get; set; } = QueryIntent.Unknown;
        public List<string>? Rewrites { get; set; }
        public double Confidence { get; set; } = 1.0;
    }
}
