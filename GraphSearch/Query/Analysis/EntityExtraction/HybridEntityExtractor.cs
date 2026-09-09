namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Composite extractor that runs several <see cref="IEntityExtractor"/>s in
/// priority order and merges the results with
/// <see cref="EntitySpanMerger"/>.
///
/// <para>
/// Recommended stack for GraphRAG (highest priority first):
/// </para>
/// <list type="number">
///   <item><b>Dictionary</b> — curated domain gazetteer (products, tenants, SKUs).</item>
///   <item><b>Regex</b> — structured tokens (versions, dates, URLs).</item>
///   <item><b>NER</b> — spaCy / Azure / HuggingFace for open-vocabulary entities.</item>
///   <item><b>LLM</b> — last-mile recall on hard/colloquial queries.</item>
/// </list>
///
/// <para>
/// Higher-priority extractors win on overlap, so a hand-curated
/// <see cref="DictionaryEntityExtractor"/> match will never be shadowed by a
/// lower-confidence NER or LLM span. Set <c>continueOnException = true</c>
/// (default) to keep the pipeline resilient when a remote extractor fails.
/// </para>
/// </summary>
public sealed class HybridEntityExtractor : IEntityExtractor
{
    private readonly IReadOnlyList<(IEntityExtractor Extractor, int Priority)> _tiers;
    private readonly bool _continueOnException;
    private readonly Action<Exception, IEntityExtractor>? _onException;

    /// <summary>
    /// Initialise une nouvelle instance de <c>HybridEntityExtractor</c> avec une collection d’extracteurs et une
    /// stratégie de gestion des exceptions.
    /// </summary>
    /// <remarks>L’ordre des extracteurs détermine leur priorité ; en cas d’égalité de score, les extracteurs
    /// placés en premier sont privilégiés.</remarks>
    /// <param name="extractors">Collection d’extracteurs à utiliser pour l’extraction hybride.</param>
    /// <param name="continueOnException">Indique s’il faut poursuivre le traitement avec les extracteurs restants lorsqu’une exception est levée.</param>
    /// <param name="onException">Action appelée quand un extracteur lève une exception, avec l’exception et l’extracteur concernés.</param>
    /// <exception cref="ArgumentException">Levée lorsque <paramref name="extractors"/> est vide.</exception>
    public HybridEntityExtractor(
        IEnumerable<IEntityExtractor> extractors,
        bool continueOnException = true,
        Action<Exception, IEntityExtractor>? onException = null)
    {
        ArgumentNullException.ThrowIfNull(extractors);
        var list = extractors.ToList();
        if (list.Count == 0)
        {
            throw new ArgumentException("At least one extractor is required.", nameof(extractors));
        }

        // Priority = position from the end so the first extractor wins ties.
        _tiers = list
            .Select((extractor, index) => (extractor, priority: list.Count - index))
            .ToList();

        _continueOnException = continueOnException;
        _onException = onException;
    }

    public HybridEntityExtractor(params IEntityExtractor[] extractors)
        : this((IEnumerable<IEntityExtractor>)extractors)
    {
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

        foreach (var (extractor, priority) in _tiers)
        {
            IReadOnlyList<ExtractedEntity> partial;
            try
            {
                // Run the extractor.
                partial = await extractor
                    .ExtractAsync(query, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (_continueOnException)
            {
                _onException?.Invoke(ex, extractor);
                continue;
            }

            // Merge the partial results into the accumulator, respecting priority.
            foreach (var entity in partial)
            {
                EntitySpanMerger.AddOrReplace(accumulator, entity, priority);
            }
        }

        // Finalize and return the merged results.
        return EntitySpanMerger.Finalize(accumulator);
    }
}
