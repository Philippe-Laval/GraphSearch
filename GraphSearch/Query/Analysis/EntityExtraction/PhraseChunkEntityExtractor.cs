using GraphSearch.Library.Query.Analysis.PhraseChunkService;

namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Emits raw noun-phrase candidates (typed as <c>"NounPhrase"</c>) using an
/// <see cref="IPhraseChunkService"/>.
///
/// <para>
/// Useful as a <b>recall booster</b> in a <see cref="HybridEntityExtractor"/>
/// chain when a NER model produces sparse or missing labels. Downstream you
/// can:
/// </para>
/// <list type="bullet">
///   <item>Resolve chunks against a knowledge graph via
///         <see cref="GraphResolvableEntityFilter"/>.</item>
///   <item>Type them with <see cref="EmbeddingCandidateEntityExtractor"/>
///         (cosine to known entities).</item>
///   <item>Feed them to a slot-filling LLM prompt as suspected entities.</item>
/// </list>
///
/// <para>
/// Because the type is <c>"NounPhrase"</c> (or your custom
/// <c>defaultType</c>) rather than a real NER label, place this extractor at
/// a <b>low priority</b> in your composite so it never shadows typed matches.
/// </para>
/// </summary>
public sealed class PhraseChunkEntityExtractor : IEntityExtractor
{
    private readonly IPhraseChunkService _chunker;
    private readonly string _defaultType;
    private readonly double _confidence;
    private readonly int _minLength;

    public PhraseChunkEntityExtractor(
        IPhraseChunkService chunker,
        string defaultType = "NounPhrase",
        double confidence = 0.5,
        int minLength = 3)
    {
        ArgumentNullException.ThrowIfNull(chunker);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultType);
        _chunker = chunker;
        _defaultType = defaultType;
        _confidence = confidence;
        _minLength = minLength;
    }

    public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var chunks = await _chunker.ExtractChunksAsync(query, cancellationToken).ConfigureAwait(false);
        if (chunks.Count == 0)
        {
            return Array.Empty<ExtractedEntity>();
        }

        var accumulator = new List<(ExtractedEntity Entity, int Priority)>();
        foreach (var chunk in chunks)
        {
            if (chunk.Length < _minLength)
            {
                continue;
            }
            var entity = new ExtractedEntity(
                Text: chunk.Text,
                Type: _defaultType,
                Start: chunk.Start,
                Length: chunk.Length,
                Confidence: _confidence);
            EntitySpanMerger.AddOrReplace(accumulator, entity, priority: 0);
        }

        return EntitySpanMerger.Finalize(accumulator);
    }
}
