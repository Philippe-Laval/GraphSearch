namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// <see cref="IEntityExtractor"/> backed by any <see cref="INerService"/>
/// implementation (spaCy microservice, Azure AI Language, HuggingFace ONNX,
/// or a hybrid). Returns the entities reported by the NER pipeline as-is.
///
/// <para>
/// Best used when your queries contain open-vocabulary named entities (people,
/// organizations, locations, products) that a static dictionary cannot cover.
/// Combine with <see cref="DictionaryEntityExtractor"/> (domain gazetteer) and
/// <see cref="RegexEntityExtractor"/> (structured tokens) through
/// <see cref="HybridEntityExtractor"/> for the best of all three worlds.
/// </para>
/// </summary>
public sealed class NerEntityExtractor : IEntityExtractor
{
    private readonly INerService _ner;
    private readonly HashSet<string>? _typeAllowList;

    /// <param name="ner">Underlying NER service.</param>
    /// <param name="typeAllowList">
    /// Optional set of entity types (e.g. "PER", "ORG", "PRODUCT") to keep.
    /// When null, all types produced by the NER service are returned.
    /// </param>
    public NerEntityExtractor(
        INerService ner,
        IEnumerable<string>? typeAllowList = null)
    {
        ArgumentNullException.ThrowIfNull(ner);
        _ner = ner;
        _typeAllowList = typeAllowList is null
            ? null
            : new HashSet<string>(typeAllowList, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var analysis = await _ner.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);

        if (_typeAllowList is null)
        {
            return analysis.Entities;
        }

        return analysis.Entities
            .Where(e => e.Type is not null && _typeAllowList.Contains(e.Type))
            .ToList();
    }
}
