using GraphSearch.Library.Embeddings;

namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Analyzer that embeds the normalized query and uses cosine similarity against
/// intent prototypes to classify intent. Delegates normalization to an inner
/// analyzer (default: <see cref="RulesBasedQueryAnalyzer"/> with intent detection disabled).
///
/// Handles paraphrase well, no LLM cost.
/// </summary>
public sealed class EmbeddingQueryAnalyzer : IQueryAnalyzer
{
    private readonly IEmbeddingService _embeddingService;
    private readonly EmbeddingIntentClassifier _classifier;
    private readonly IQueryAnalyzer _normalizer;

    public EmbeddingQueryAnalyzer(
        IEmbeddingService embeddingService,
        EmbeddingIntentClassifier classifier,
        IQueryAnalyzer? normalizer = null)
    {
        ArgumentNullException.ThrowIfNull(embeddingService);
        ArgumentNullException.ThrowIfNull(classifier);

        _embeddingService = embeddingService;
        _classifier = classifier;
        _normalizer = normalizer ?? new RulesBasedQueryAnalyzer(QueryAnalyzerOptions.Default with
        {
            // Disable intent detection in the normalizer, since we will classify intent ourselves.
            DetectIntent = false,
        });
    }

    /// <inheritdoc />
    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // Normalize the query.
        var baseAnalysis = await _normalizer.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);

        // Embed the normalized query.
        var embedding = await _embeddingService.EmbedAsync(baseAnalysis.NormalizedQuery, cancellationToken).ConfigureAwait(false);

        // Classify the query intent based on the embedding.
        var (intent, similarity) = _classifier.Classify(embedding.Span);

        return baseAnalysis with
        {
            Intent = intent,
            IntentConfidence = similarity,
        };
    }
}
