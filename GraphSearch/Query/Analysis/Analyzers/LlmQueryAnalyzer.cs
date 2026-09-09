using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphSearch.Library.Query.Analysis.Analyzers;

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

    /// <summary>
    /// Analyse asynchroniquement une requête utilisateur pour produire une forme normalisée et un intent.
    /// </summary>
    /// <param name="query">Requête à analyser.</param>
    /// <param name="cancellationToken">Jeton utilisé pour annuler l’opération asynchrone.</param>
    /// <returns>Une instance de <c>AnalyzedQuery</c> contenant la requête d’origine, la requête normalisée, l’intent détecté,
    /// les reformulations éventuelles et un score de confiance borné entre 0 et 1. Retourne un résultat avec
    /// <c>QueryIntent.Unknown</c> et une confiance de 0 si l’analyse ne peut pas être interprétée.</returns>
    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // Remplacer le placeholder {{QUERY}} dans le prompt par la requête utilisateur.
        var prompt = _promptTemplate.Replace("{{QUERY}}", query, StringComparison.Ordinal);

        // Envoyer le prompt à l’API LLM et récupérer la réponse brute.
        var raw = await _client.CompleteAsync(prompt, cancellationToken).ConfigureAwait(false);

        // Extraire le bloc JSON de la réponse brute.
        var payload = ExtractJson(raw);
        
        // Tenter de désérialiser le JSON en objet LlmResponse.
        LlmResponse? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<LlmResponse>(payload, JsonOptions);
        }
        catch (JsonException)
        {
            parsed = null;
        }

        // Si la désérialisation échoue ou si la requête normalisée est vide, retourner un résultat par défaut.
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
            // Clamp the confidence score to the range [0.0, 1.0].
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
