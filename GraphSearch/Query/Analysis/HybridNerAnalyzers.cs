namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// Convenience factories that wire two NER-backed analyzers
/// (<see cref="AzureLanguageNerService"/> and <see cref="SpacyNerService"/>)
/// into a resilient <see cref="CompositeQueryAnalyzer"/>.
///
/// The composite falls back to the secondary analyzer when the primary either:
///   - returns <see cref="QueryIntent.Unknown"/> / <see cref="QueryIntent.General"/>, or
///   - throws (network, auth, quota, ...).
///
/// The primary result is cached implicitly when you wrap the returned analyzer
/// with <see cref="CachingQueryAnalyzer"/>.
/// </summary>
public static class HybridNerAnalyzers
{
    /// <summary>
    /// Azure AI Language primary, spaCy fallback.
    ///
    /// Preferred when:
    ///   - your workload is mostly online and Azure quotas are healthy,
    ///   - you want the higher-quality Azure entity recognizer by default,
    ///   - the local spaCy service is a safety net for outages / rate limits.
    /// </summary>
    public static IQueryAnalyzer AzurePrimary(
        INerService azure,
        INerService spacy,
        IQueryAnalyzer? normalizer = null,
        Action<Exception, IQueryAnalyzer>? onException = null)
    {
        ArgumentNullException.ThrowIfNull(azure);
        ArgumentNullException.ThrowIfNull(spacy);

        return new CompositeQueryAnalyzer(
            analyzers: new IQueryAnalyzer[]
            {
                new NerQueryAnalyzer(azure, normalizer),
                new NerQueryAnalyzer(spacy, normalizer),
            },
            shouldAccept: null,             // default: reject Unknown/General
            continueOnException: true,      // Azure failure -> try spaCy
            onException: onException);
    }

    /// <summary>
    /// spaCy primary, Azure AI Language fallback.
    ///
    /// Preferred when:
    ///   - you want deterministic, local-first analysis (privacy, cost),
    ///   - Azure is only invoked for the queries spaCy can't classify.
    /// </summary>
    public static IQueryAnalyzer SpacyPrimary(
        INerService spacy,
        INerService azure,
        IQueryAnalyzer? normalizer = null,
        Action<Exception, IQueryAnalyzer>? onException = null)
    {
        ArgumentNullException.ThrowIfNull(spacy);
        ArgumentNullException.ThrowIfNull(azure);

        return new CompositeQueryAnalyzer(
            analyzers: new IQueryAnalyzer[]
            {
                new NerQueryAnalyzer(spacy, normalizer),
                new NerQueryAnalyzer(azure, normalizer),
            },
            shouldAccept: null,
            continueOnException: true,
            onException: onException);
    }
}
