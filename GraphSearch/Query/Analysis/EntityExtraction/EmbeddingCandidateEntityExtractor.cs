using GraphSearch.Library.Embeddings;

namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// A pre-embedded entry in the semantic entity index consumed by
/// <see cref="EmbeddingCandidateEntityExtractor"/>.
/// </summary>
public sealed record EmbeddedEntity(
    EntityDefinition Definition,
    ReadOnlyMemory<float> Embedding);

/// <summary>
/// Embedding-based extractor for <b>semantic aliases</b>. A candidate span
/// produced by an <see cref="IPhraseChunkService"/> is embedded via
/// <see cref="IEmbeddingService"/> and cosine-matched against a precomputed
/// entity index. Above the similarity threshold, the closest known entity
/// wins.
///
/// <para>
/// Handles cases dictionaries and NER miss:
/// </para>
/// <list type="bullet">
///   <item>"the Redmond company" → <c>Microsoft</c></item>
///   <item>"the iPhone maker" → <c>Apple</c></item>
///   <item>"the .NET framework" → <c>.NET</c></item>
/// </list>
///
/// <para>
/// Cost is O(candidates × entities) for cosine (all in-memory). For &gt;100k
/// entities, back this with an ANN index (HNSW, Chroma, LanceDB) and inject
/// a custom nearest-neighbour delegate.
/// </para>
/// </summary>
public sealed class EmbeddingCandidateEntityExtractor : IEntityExtractor
{
    private readonly IPhraseChunkService _chunker;
    private readonly IEmbeddingService _embedder;
    private readonly IReadOnlyList<EmbeddedEntity> _index;
    private readonly double _similarityThreshold;

    public EmbeddingCandidateEntityExtractor(
        IPhraseChunkService chunker,
        IEmbeddingService embedder,
        IReadOnlyList<EmbeddedEntity> index,
        double similarityThreshold = 0.75)
    {
        ArgumentNullException.ThrowIfNull(chunker);
        ArgumentNullException.ThrowIfNull(embedder);
        ArgumentNullException.ThrowIfNull(index);
        if (similarityThreshold is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(similarityThreshold), "Must be in [0, 1].");
        }
        _chunker = chunker;
        _embedder = embedder;
        _index = index;
        _similarityThreshold = similarityThreshold;
    }

    /// <summary>
    /// Convenience helper: pre-embeds a set of <see cref="EntityDefinition"/>s
    /// so callers do not have to manage the embedding cache themselves.
    /// </summary>
    public static async Task<IReadOnlyList<EmbeddedEntity>> BuildIndexAsync(
        IEnumerable<EntityDefinition> entities,
        IEmbeddingService embedder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        ArgumentNullException.ThrowIfNull(embedder);

        var list = new List<EmbeddedEntity>();
        foreach (var e in entities)
        {
            var embedding = await embedder.EmbedAsync(e.Name, cancellationToken).ConfigureAwait(false);
            list.Add(new EmbeddedEntity(e, embedding));
        }
        return list;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        if (_index.Count == 0)
        {
            return Array.Empty<ExtractedEntity>();
        }

        var chunks = await _chunker.ExtractChunksAsync(query, cancellationToken).ConfigureAwait(false);
        if (chunks.Count == 0)
        {
            return Array.Empty<ExtractedEntity>();
        }

        var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

        foreach (var chunk in chunks)
        {
            var embedding = await _embedder
                .EmbedAsync(chunk.Text, cancellationToken)
                .ConfigureAwait(false);

            EmbeddedEntity? best = null;
            var bestScore = double.NegativeInfinity;

            foreach (var candidate in _index)
            {
                var score = CosineSimilarity(embedding.Span, candidate.Embedding.Span);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }

            if (best is null || bestScore < _similarityThreshold)
            {
                continue;
            }

            var entity = new ExtractedEntity(
                Text: chunk.Text,
                Type: best.Definition.Type,
                Start: chunk.Start,
                Length: chunk.Length,
                Confidence: Math.Round(bestScore, 3));

            EntitySpanMerger.AddOrReplace(accumulator, entity, priority: 0);
        }

        return EntitySpanMerger.Finalize(accumulator);
    }

    /// <summary>
    /// Calcule la similarité cosinus entre deux vecteurs numériques.
    /// </summary>
    /// <param name="a">Premier vecteur en lecture seule.</param>
    /// <param name="b">Second vecteur en lecture seule.</param>
    /// <returns>Valeur de similarité cosinus comprise entre -1 et 1 ; retourne 0 si les vecteurs n'ont pas la même longueur,
    /// sont vides, ou si la norme de l'un des vecteurs est nulle.</returns>
    private static double CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
    {
        if (a.Length != b.Length || a.Length == 0)
        {
            return 0.0;
        }

        double dot = 0, magA = 0, magB = 0;
        for (var i = 0; i < a.Length; i++)
        {
            double av = a[i], bv = b[i];
            dot += av * bv;
            magA += av * av;
            magB += bv * bv;
        }

        if (magA == 0 || magB == 0)
        {
            return 0.0;
        }
        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    }
}
