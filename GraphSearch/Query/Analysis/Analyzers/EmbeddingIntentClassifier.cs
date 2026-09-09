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

    /// <summary>
    /// 
    /// </summary>
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
    /// <param name="embeddingService">Service d’embedding utilisé pour générer les vecteurs d’exemples.</param>
    /// <param name="examples">Dictionnaire des intentions et de leurs exemples de requêtes.</param>
    /// <param name="cancellationToken">Jeton d’annulation.</param>
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
            foreach (var query in queries)
            {
                var e = await embeddingService.EmbedAsync(query, cancellationToken).ConfigureAwait(false);
                var span = e.Span;

                // Initialize the accumulator vector if needed.
                accum ??= new float[span.Length];

                // Check for consistent dimensionality.
                if (span.Length != accum.Length)
                {
                    throw new InvalidOperationException("Inconsistent embedding dimensionality.");
                }

                // Accumulate the embedding vector.
                for (var i = 0; i < span.Length; i++)
                {
                    accum[i] += span[i];
                }
            }

            // Average the accumulated vector.
            for (var i = 0; i < accum!.Length; i++)
            {
                accum[i] /= queries.Count;
            }

            // Normalize the prototype vector.
            Normalize(accum);

            // Store the prototype for this intent.
            _prototypes[intent] = accum;
        }
    }

    /// <summary>
    /// Sets prototypes directly (e.g. loaded from disk).
    /// </summary>
    /// <param name="prototypes"></param>
    public void SetPrototypes(IReadOnlyDictionary<QueryIntent, ReadOnlyMemory<float>> prototypes)
    {
        _prototypes.Clear();
        foreach (var (intent, vec) in prototypes)
        {
            var copy = vec.ToArray();

            // Normalize the prototype vector.
            Normalize(copy);

            // Store the prototype for this intent.
            _prototypes[intent] = copy;
        }
    }

    /// <summary>
    /// Classe le vecteur d’entrée en comparant sa similarité cosinus normalisée aux prototypes disponibles.
    /// </summary>
    /// <remarks>Les prototypes dont la dimension diffère de celle de <paramref name="embedding"/> sont
    /// ignorés.</remarks>
    /// <param name="embedding">Vecteur d’embedding à classer.</param>
    /// <returns>Un tuple contenant l’intention la plus proche et son score de similarité ; retourne <c>QueryIntent.Unknown</c>
    /// si aucun prototype n’est disponible ou si le meilleur score est inférieur au seuil configuré.</returns>
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
        
        // Normalize the embedding vector.
        Normalize(normalized);

        var best = QueryIntent.Unknown;
        var bestSim = double.NegativeInfinity;

        foreach (var (intent, proto) in _prototypes)
        {
            // Skip prototypes of different dimensionality.
            if (proto.Length != normalized.Length)
            {
                continue;
            }

            // Cosine similarity is the dot product of normalized vectors.
            var sim = Dot(normalized, proto);
            if (sim > bestSim)
            {
                // Found a better match.
                bestSim = sim;
                best = intent;
            }
        }

        return bestSim >= _threshold
            ? (best, bestSim)
            : (QueryIntent.Unknown, bestSim < 0 ? 0.0 : bestSim);
    }
    
    /// <summary>
    /// Calcule le produit scalaire de deux vecteurs.
    /// </summary>
    /// <param name="a">Premier vecteur.</param>
    /// <param name="b">Deuxième vecteur.</param>
    /// <returns>Le produit scalaire des deux vecteurs.</returns>
    private static double Dot(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
    {
        double s = 0;
        for (var i = 0; i < a.Length; i++)
        {
            s += a[i] * b[i];
        }
        return s;
    }

    /// <summary>
    /// Met à l’échelle le vecteur pour que sa norme euclidienne soit égale à 1.
    /// </summary>
    /// <remarks>Ne modifie pas le vecteur si sa norme est inférieure à 1e-12.</remarks>
    /// <param name="v">Vecteur à normaliser sur place.</param>
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
