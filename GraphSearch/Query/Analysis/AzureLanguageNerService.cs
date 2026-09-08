using Azure.AI.TextAnalytics;

namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// <see cref="INerService"/> implementation backed by Azure AI Language
/// (<c>Azure.AI.TextAnalytics</c>). Supports English (<c>en</c>) and French
/// (<c>fr</c>).
///
/// <para>
/// Azure AI Language provides Named Entity Recognition and language detection,
/// but does <b>not</b> expose POS tagging or lemmatization. To keep parity with
/// <see cref="INerService"/>, we approximate the missing signals locally:
/// </para>
/// <list type="bullet">
///   <item><b>Entities:</b> <c>RecognizeEntitiesAsync</c>, with per-entity confidence.</item>
///   <item><b>Wh-word:</b> <see cref="WhWordLexicon"/> — FR words normalized to canonical EN.</item>
///   <item><b>Root verbs:</b> <see cref="RelationalVerbLexicon"/> dictionary lookup.</item>
///   <item><b>Lemma:</b> not produced (<c>null</c>). <see cref="NerQueryAnalyzer"/>
///         falls back to the normalized query.</item>
/// </list>
///
/// Language selection order:
///   explicit argument &gt; <see cref="ILanguageDetector"/> &gt; configured default.
/// </summary>
public sealed class AzureLanguageNerService : INerService
{
    private readonly TextAnalyticsClient _client;
    private readonly ILanguageDetector? _languageDetector;
    private readonly string _defaultLanguage;

    /// <param name="client">Pre-built <see cref="TextAnalyticsClient"/>.</param>
    /// <param name="languageDetector">Optional detector for auto-routing.</param>
    /// <param name="defaultLanguage">BCP-47 fallback. Defaults to "en".</param>
    public AzureLanguageNerService(
        TextAnalyticsClient client,
        ILanguageDetector? languageDetector = null,
        string defaultLanguage = "en")
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultLanguage);

        _client = client;
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

        var azureLanguage = NormalizeLanguage(language);

        var response = await _client
            .RecognizeEntitiesAsync(text, azureLanguage, cancellationToken)
            .ConfigureAwait(false);

        var entities = new List<ExtractedEntity>(response.Value.Count);
        foreach (var entity in response.Value)
        {
            entities.Add(new ExtractedEntity(
                Text: entity.Text,
                Type: entity.Category.ToString(),
                Start: entity.Offset,
                Length: entity.Length,
                Confidence: entity.ConfidenceScore));
        }

        return new NerAnalysis(
            Lemma: null, // Azure AI Language does not lemmatize.
            Entities: entities,
            RootVerbs: RelationalVerbLexicon.Extract(text, azureLanguage),
            InterrogativeLemma: WhWordLexicon.Detect(text, azureLanguage));
    }

    // Azure expects the base code ("en", "fr"), not a regional variant.
    private static string NormalizeLanguage(string language)
    {
        var idx = language.IndexOfAny(['-', '_']);
        return (idx > 0 ? language[..idx] : language).ToLowerInvariant();
    }
}
