namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// Predicate over the knowledge-graph vocabulary consumed by
/// <see cref="GraphResolvableEntityFilter"/>. Return <c>true</c> when the
/// entity text (case-insensitive) or type is present in the graph.
/// </summary>
public interface IGraphVocabulary
{
    ValueTask<bool> ContainsAsync(
        ExtractedEntity entity,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Simple set-backed <see cref="IGraphVocabulary"/>. Useful when the graph
/// vocabulary fits in memory; swap for a Sqlite/Lucene-backed lookup for
/// larger graphs.
/// </summary>
public sealed class SetGraphVocabulary : IGraphVocabulary
{
    private readonly HashSet<string> _names;

    public SetGraphVocabulary(
        IEnumerable<string> names,
        StringComparer? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(names);
        _names = new HashSet<string>(names, comparer ?? StringComparer.OrdinalIgnoreCase);
    }

    public ValueTask<bool> ContainsAsync(
        ExtractedEntity entity,
        CancellationToken cancellationToken = default)
        => new(_names.Contains(entity.Text));
}

/// <summary>
/// Decorator that drops entities absent from the knowledge-graph vocabulary.
///
/// <para>
/// Place this <b>at the very end</b> of your composite pipeline so cheap
/// pre-filtering doesn't shadow candidates that a downstream resolver could
/// still normalize (e.g. "MS" → "Microsoft"). It is meant as a hard guardrail
/// against off-graph noise fed into the retrieval stage, not as a substitute
/// for the <c>EntityResolver</c>.
/// </para>
/// </summary>
public sealed class GraphResolvableEntityFilter : IEntityExtractor
{
    private readonly IEntityExtractor _inner;
    private readonly IGraphVocabulary _vocabulary;

    public GraphResolvableEntityFilter(
        IEntityExtractor inner,
        IGraphVocabulary vocabulary)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(vocabulary);
        _inner = inner;
        _vocabulary = vocabulary;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var extracted = await _inner.ExtractAsync(query, cancellationToken).ConfigureAwait(false);
        if (extracted.Count == 0)
        {
            return extracted;
        }

        var kept = new List<ExtractedEntity>(extracted.Count);
        foreach (var entity in extracted)
        {
            if (await _vocabulary.ContainsAsync(entity, cancellationToken).ConfigureAwait(false))
            {
                kept.Add(entity);
            }
        }
        return kept;
    }
}
