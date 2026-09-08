namespace GraphSearch.Library.Query.Analysis.Ner;

/// <summary>
/// Language-routing pool for <see cref="HuggingFaceOnnxNerService"/> instances.
/// Wire one model per supported language (typically an EN BERT-NER and a FR
/// CamemBERT-NER), and let this class dispatch based on
/// <see cref="ILanguageDetector"/> output.
/// </summary>
public sealed class HuggingFaceOnnxNerServicePool : INerService, IDisposable
{
    private readonly IReadOnlyDictionary<string, INerService> _byLanguage;
    private readonly ILanguageDetector? _languageDetector;
    private readonly string _defaultLanguage;
    private readonly bool _ownsServices;

    public HuggingFaceOnnxNerServicePool(
        IReadOnlyDictionary<string, INerService> byLanguage,
        ILanguageDetector? languageDetector = null,
        string defaultLanguage = "en",
        bool disposeServices = false)
    {
        ArgumentNullException.ThrowIfNull(byLanguage);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultLanguage);
        if (byLanguage.Count == 0)
        {
            throw new ArgumentException("At least one NER service must be registered.", nameof(byLanguage));
        }

        _byLanguage = new Dictionary<string, INerService>(byLanguage, StringComparer.OrdinalIgnoreCase);
        _languageDetector = languageDetector;
        _defaultLanguage = defaultLanguage;
        _ownsServices = disposeServices;
    }

    public Task<NerAnalysis> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        var language = _languageDetector?.Detect(text) ?? _defaultLanguage;
        return AnalyzeAsync(text, language, cancellationToken);
    }

    public Task<NerAnalysis> AnalyzeAsync(string text, string language, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        var normalizedLang = NormalizeLanguage(language);
        if (!_byLanguage.TryGetValue(normalizedLang, out var service))
        {
            if (!_byLanguage.TryGetValue(_defaultLanguage, out service))
            {
                throw new InvalidOperationException(
                    $"No NER model registered for language '{normalizedLang}' and no fallback for '{_defaultLanguage}'.");
            }
        }

        return service.AnalyzeAsync(text, cancellationToken);
    }

    private static string NormalizeLanguage(string language)
    {
        var idx = language.IndexOfAny(['-', '_']);
        return (idx > 0 ? language[..idx] : language).ToLowerInvariant();
    }

    public void Dispose()
    {
        if (!_ownsServices)
        {
            return;
        }
        foreach (var svc in _byLanguage.Values)
        {
            (svc as IDisposable)?.Dispose();
        }
    }
}
