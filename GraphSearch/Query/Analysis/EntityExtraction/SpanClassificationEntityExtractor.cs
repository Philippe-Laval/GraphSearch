namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Adapter that surfaces a <b>zero-shot / universal</b> NER model (e.g.
/// <c>numind/NuNER</c>, <c>gliner</c>, <c>UniversalNER</c>) as an
/// <see cref="IEntityExtractor"/>.
///
/// <para>
/// The wrapped <see cref="INerService"/> is expected to be an ONNX (or other)
/// runtime hosting a universal-NER model whose label space is broader (or
/// arbitrary, in the zero-shot case) than a classical PER/ORG/LOC tagger.
/// This class provides the plumbing that turns those raw spans into the
/// canonical <see cref="ExtractedEntity"/> shape:
/// </para>
/// <list type="bullet">
///   <item>Optional <b>type remapping</b> from raw model labels
///         (<c>LABEL_0</c>, <c>company</c>, <c>tech_product</c>, ...) to the
///         vocabulary the rest of the system uses
///         (<c>Organization</c>, <c>Technology</c>, ...).</item>
///   <item>Optional <b>type allow-list</b>: drop spans whose remapped type is
///         not in the set.</item>
///   <item><b>Minimum confidence</b> threshold to trim low-quality zero-shot
///         predictions before they enter fusion.</item>
/// </list>
///
/// <para>
/// Complements — rather than replaces — a task-specific
/// <see cref="NerEntityExtractor"/>. Chain both in a
/// <see cref="HybridEntityExtractor"/>: task-specific labels win on overlap,
/// universal spans cover long-tail types.
/// </para>
/// </summary>
public sealed class SpanClassificationEntityExtractor : IEntityExtractor
{
    private readonly INerService _ner;
    private readonly IReadOnlyDictionary<string, string>? _typeMap;
    private readonly HashSet<string>? _typeAllowList;
    private readonly double _minConfidence;

    public SpanClassificationEntityExtractor(
        INerService ner,
        IReadOnlyDictionary<string, string>? typeMap = null,
        IEnumerable<string>? typeAllowList = null,
        double minConfidence = 0.5)
    {
        ArgumentNullException.ThrowIfNull(ner);
        if (minConfidence is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minConfidence), "Must be in [0, 1].");
        }
        _ner = ner;
        _typeMap = typeMap;
        _typeAllowList = typeAllowList is null
            ? null
            : new HashSet<string>(typeAllowList, StringComparer.OrdinalIgnoreCase);
        _minConfidence = minConfidence;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var analysis = await _ner.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);
        if (analysis.Entities.Count == 0)
        {
            return Array.Empty<ExtractedEntity>();
        }

        var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

        foreach (var raw in analysis.Entities)
        {
            if (raw.Confidence < _minConfidence)
            {
                continue;
            }

            var mappedType = raw.Type;
            if (mappedType is not null && _typeMap is not null
                && _typeMap.TryGetValue(mappedType, out var mapped))
            {
                mappedType = mapped;
            }

            if (_typeAllowList is not null
                && (mappedType is null || !_typeAllowList.Contains(mappedType)))
            {
                continue;
            }

            var entity = raw with { Type = mappedType };
            EntitySpanMerger.AddOrReplace(accumulator, entity, priority: 0);
        }

        return EntitySpanMerger.Finalize(accumulator);
    }
}
