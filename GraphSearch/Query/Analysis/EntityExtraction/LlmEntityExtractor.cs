using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// LLM-backed entity extractor. Prompts an <see cref="IChatCompletionClient"/>
/// to return a JSON array of the shape:
///
///   [
///     { "text": "Microsoft", "type": "Organization",       "confidence": 0.99 },
///     { "text": ".NET 10",   "type": "Technology",         "confidence": 0.95 },
///     { "text": "Linux",     "type": "OperatingSystem",    "confidence": 0.97 }
///   ]
///
/// <para>
/// Char offsets returned by LLMs are notoriously unreliable — the analyzer
/// re-locates each entity in the original text via <see cref="string.IndexOf(string, StringComparison)"/>
/// (first occurrence, case-insensitive). Entities not found are discarded rather
/// than reporting invalid spans.
/// </para>
///
/// <para>
/// Best used as the <b>last</b> stage of a <see cref="HybridEntityExtractor"/>
/// chain, and always wrapped in <see cref="CachingEntityExtractor"/> in production.
/// </para>
/// </summary>
public sealed class LlmEntityExtractor : IEntityExtractor
{
    private const string DefaultPromptTemplate =
        """
        Extract named entities from the user query. Reply with a JSON array only,
        no prose. Each element must have:
          "text"       — the entity surface form as it appears in the query,
          "type"       — a short label (Person, Organization, Location, Product,
                         Technology, ProgrammingLanguage, OperatingSystem, Date,
                         Version, ...),
          "confidence" — a float in [0, 1].

        Query:
        {{QUERY}}
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IChatCompletionClient _client;
    private readonly string _promptTemplate;
    private readonly IReadOnlyCollection<string>? _typeAllowList;

    public LlmEntityExtractor(
        IChatCompletionClient client,
        string? promptTemplate = null,
        IEnumerable<string>? typeAllowList = null)
    {
        ArgumentNullException.ThrowIfNull(client);
        _client = client;
        _promptTemplate = promptTemplate ?? DefaultPromptTemplate;
        _typeAllowList = typeAllowList is null
            ? null
            : new HashSet<string>(typeAllowList, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var prompt = _promptTemplate.Replace("{{QUERY}}", query, StringComparison.Ordinal);
        var raw = await _client.CompleteAsync(prompt, cancellationToken).ConfigureAwait(false);

        var payload = ExtractJsonArray(raw);
        List<LlmEntity>? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<List<LlmEntity>>(payload, JsonOptions);
        }
        catch (JsonException)
        {
            parsed = null;
        }

        if (parsed is null || parsed.Count == 0)
        {
            return Array.Empty<ExtractedEntity>();
        }

        var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

        foreach (var e in parsed)
        {
            if (string.IsNullOrWhiteSpace(e.Text))
            {
                continue;
            }

            if (_typeAllowList is not null
                && (e.Type is null || !_typeAllowList.Contains(e.Type)))
            {
                continue;
            }

            // Re-locate in the original text — LLM offsets are not trustworthy.
            var idx = query.IndexOf(e.Text, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
            {
                continue;
            }

            var entity = new ExtractedEntity(
                Text: query.Substring(idx, e.Text.Length),
                Type: e.Type,
                Start: idx,
                Length: e.Text.Length,
                Confidence: Math.Clamp(e.Confidence, 0.0, 1.0));

            EntitySpanMerger.AddOrReplace(accumulator, entity, priority: 0);
        }

        return EntitySpanMerger.Finalize(accumulator);
    }

    // LLMs may wrap the array in prose or ```json ... ```. Trim to the first [...]
    private static string ExtractJsonArray(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return "[]";
        }
        var start = raw.IndexOf('[');
        var end = raw.LastIndexOf(']');
        if (start < 0 || end < 0 || end <= start)
        {
            return "[]";
        }
        return raw.Substring(start, end - start + 1);
    }

    private sealed class LlmEntity
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; } = 0.8;
    }
}
