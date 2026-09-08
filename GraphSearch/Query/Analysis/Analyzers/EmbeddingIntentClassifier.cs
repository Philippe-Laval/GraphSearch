namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Classifies an embedding against a set of intent prototype embeddings using
/// cosine similarity. Cheap, no LLM.
///
/// Seed with representative example queries per intent via
/// <see cref="TrainAsync"/>; prototypes are the mean of their example embeddings.
/// </summary>
public sealed class EmbeddingIntentClassifier
{
    private readonly Dictionary<QueryIntent, float[]> _prototypes = new();
    private double _threshold;

    /// <param name="threshold">
    /// Minimum cosine similarity to accept a classification. Below this, the
    /// classifier returns <see cref="QueryIntent.Unknown"/>. Default 0.55.
    /// </param>
    public EmbeddingIntentClassifier(double threshold = 0.55)
    {
        _threshold = threshold;
    }

    /// <summary>
    /// Builds one prototype per intent from labeled example queries.
    /// </summary>
    public async Task TrainAsync(
        Query.Embeddings.IEmbeddingService embeddingService,
        IReadOnlyDictionary<QueryIntent, IReadOnlyList<string>> examples,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(embeddingService);
        ArgumentNullException.ThrowIfNull(examples);

        _prototypes.Clear();
        foreach (var (intent, queries) in examples)
        {
            if (queries.Count == 0)
            {
                continue;
            }

            float[]? accum = null;
            foreach (var q in queries)
            {
                var e = await embeddingService.EmbedAsync(q, cancellationToken).ConfigureAwait(false);
                var span = e.Span;
                accum ??= new float[span.Length];
                if (span.Length != accum.Length)
                {
                    throw new InvalidOperationException("Inconsistent embedding dimensionality.");
                }
                for (var i = 0; i < span.Length; i++)
                {
                    accum[i] += span[i];
                }
            }

            for (var i = 0; i < accum!.Length; i++)
            {
                accum[i] /= queries.Count;
            }

            Normalize(accum);
            _prototypes[intent] = accum;
        }
    }

    /// <summary>
    /// Sets prototypes directly (e.g. loaded from disk).
    /// </summary>
    public void SetPrototypes(IReadOnlyDictionary<QueryIntent, ReadOnlyMemory<float>> prototypes)
    {
        _prototypes.Clear();
        foreach (var (intent, vec) in prototypes)
        {
            var copy = vec.ToArray();
            Normalize(copy);
            _prototypes[intent] = copy;
        }
    }

    public (QueryIntent Intent, double Similarity) Classify(ReadOnlySpan<float> embedding)
    {
        if (_prototypes.Count == 0)
        {
            return (QueryIntent.Unknown, 0.0);
        }

        Span<float> normalized = embedding.Length <= 4096
            ? stackalloc float[embedding.Length]
            : new float[embedding.Length];
        embedding.CopyTo(normalized);
        Normalize(normalized);

        var best = QueryIntent.Unknown;
        var bestSim = double.NegativeInfinity;

        foreach (var (intent, proto) in _prototypes)
        {
            if (proto.Length != normalized.Length)
            {
                continue;
            }
            var sim = Dot(normalized, proto);
            if (sim > bestSim)
            {
                bestSim = sim;
                best = intent;
            }
        }

        return bestSim >= _threshold
            ? (best, bestSim)
            : (QueryIntent.Unknown, bestSim < 0 ? 0.0 : bestSim);
    }

    private static double Dot(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
    {
        double s = 0;
        for (var i = 0; i < a.Length; i++)
        {
            s += a[i] * b[i];
        }
        return s;
    }

    private static void Normalize(Span<float> v)
    {
        double sq = 0;
        for (var i = 0; i < v.Length; i++)
        {
            sq += v[i] * v[i];
        }
        var norm = Math.Sqrt(sq);
        if (norm < 1e-12)
        {
            return;
        }
        var inv = (float)(1.0 / norm);
        for (var i = 0; i < v.Length; i++)
        {
            v[i] *= inv;
        }
    }
}
