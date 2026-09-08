using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// <see cref="INerService"/> implementation backed by the Python spaCy
/// microservice (see <c>python/spacy-ner</c>). Supports English (<c>en</c>)
/// and French (<c>fr</c>) out of the box.
///
/// Language selection:
///   1. Explicit language passed to the request (see overload).
///   2. Detected via the optional <see cref="ILanguageDetector"/>.
///   3. Fallback to <see cref="_defaultLanguage"/>.
///
/// The service normalizes French wh-words to their English canonical lemma
/// ("combien" -&gt; "how many", "pourquoi" -&gt; "why", ...), so
/// <see cref="NerQueryAnalyzer"/> works with a single intent table across
/// supported languages.
/// </summary>
public sealed class SpacyNerService : INerService
{
    private readonly HttpClient _httpClient;
    private readonly ILanguageDetector? _languageDetector;
    private readonly string _defaultLanguage;

    /// <param name="httpClient">
    /// Preconfigured <see cref="HttpClient"/> whose <see cref="HttpClient.BaseAddress"/>
    /// points at the spaCy service (e.g. <c>http://127.0.0.1:8080/</c>).
    /// </param>
    /// <param name="languageDetector">Optional detector for auto-routing per request.</param>
    /// <param name="defaultLanguage">BCP-47 code used when detection fails. Defaults to "en".</param>
    public SpacyNerService(
        HttpClient httpClient,
        ILanguageDetector? languageDetector = null,
        string defaultLanguage = "en")
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultLanguage);

        _httpClient = httpClient;
        _languageDetector = languageDetector;
        _defaultLanguage = defaultLanguage;
    }

    public Task<NerAnalysis> AnalyzeAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var language = _languageDetector?.Detect(text) ?? _defaultLanguage;
        return AnalyzeAsync(text, language, cancellationToken);
    }

    /// <summary>
    /// Analyzes with an explicit language, bypassing the detector.
    /// </summary>
    public async Task<NerAnalysis> AnalyzeAsync(
        string text,
        string language,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        var request = new AnalyzeRequest(text, language);

        using var response = await _httpClient
            .PostAsJsonAsync("analyze", request, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content
            .ReadFromJsonAsync<AnalyzeResponse>(cancellationToken)
            .ConfigureAwait(false);

        if (payload is null)
        {
            return new NerAnalysis(
                Lemma: null,
                Entities: Array.Empty<ExtractedEntity>(),
                RootVerbs: Array.Empty<string>(),
                InterrogativeLemma: null);
        }

        var entities = payload.Entities is null
            ? (IReadOnlyList<ExtractedEntity>)Array.Empty<ExtractedEntity>()
            : payload.Entities
                .Select(e => new ExtractedEntity(
                    Text: e.Text ?? string.Empty,
                    Type: e.Type,
                    Start: e.Start,
                    Length: e.Length,
                    Confidence: e.Confidence))
                .ToList();

        return new NerAnalysis(
            Lemma: string.IsNullOrWhiteSpace(payload.Lemma) ? null : payload.Lemma,
            Entities: entities,
            RootVerbs: (IReadOnlyList<string>?)payload.RootVerbs ?? Array.Empty<string>(),
            InterrogativeLemma: string.IsNullOrWhiteSpace(payload.InterrogativeLemma)
                ? null
                : payload.InterrogativeLemma);
    }

    private sealed record AnalyzeRequest(
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("language")] string Language);

    private sealed class AnalyzeResponse
    {
        [JsonPropertyName("lemma")]
        public string? Lemma { get; set; }

        [JsonPropertyName("entities")]
        public List<EntityDto>? Entities { get; set; }

        [JsonPropertyName("rootVerbs")]
        public List<string>? RootVerbs { get; set; }

        [JsonPropertyName("interrogativeLemma")]
        public string? InterrogativeLemma { get; set; }
    }

    private sealed class EntityDto
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; } = 1.0;
    }
}
