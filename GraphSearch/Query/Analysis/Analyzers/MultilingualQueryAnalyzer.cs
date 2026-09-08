namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Detects the query language and routes to a language-specific analyzer.
/// Falls back to <see cref="_defaultAnalyzer"/> when the language is unknown
/// or not registered.
///
/// Sets <see cref="AnalyzedQuery.DetectedLanguage"/> on the result.
/// </summary>
public sealed class MultilingualQueryAnalyzer : IQueryAnalyzer
{
    private readonly ILanguageDetector _detector;
    private readonly IReadOnlyDictionary<string, IQueryAnalyzer> _byLanguage;
    private readonly IQueryAnalyzer _defaultAnalyzer;

    public MultilingualQueryAnalyzer(
        ILanguageDetector detector,
        IReadOnlyDictionary<string, IQueryAnalyzer> byLanguage,
        IQueryAnalyzer defaultAnalyzer)
    {
        ArgumentNullException.ThrowIfNull(detector);
        ArgumentNullException.ThrowIfNull(byLanguage);
        ArgumentNullException.ThrowIfNull(defaultAnalyzer);

        _detector = detector;
        _byLanguage = new Dictionary<string, IQueryAnalyzer>(byLanguage, StringComparer.OrdinalIgnoreCase);
        _defaultAnalyzer = defaultAnalyzer;
    }

    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // Detects the language of the query and routes to the appropriate analyzer.
        var language = _detector.Detect(query);
        var analyzer = language is not null && _byLanguage.TryGetValue(language, out var found)
            ? found
            : _defaultAnalyzer;


        var analysis = await analyzer.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);
        return analysis with { DetectedLanguage = language };
    }
}
