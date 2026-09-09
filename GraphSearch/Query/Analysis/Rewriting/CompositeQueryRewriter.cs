namespace GraphSearch.Library.Query.Analysis.Rewriting;

/// <summary>
/// Fans out to several inner <see cref="IQueryRewriter"/>s in parallel, unions
/// their outputs, deduplicates by rewrite text (case-insensitive), and caps
/// the total number of rewrites returned.
///
/// <para>
/// Best-practice notes:
/// </para>
/// <list type="bullet">
///   <item>Deterministic ordering: rewriters are queried in construction order;
///         ties are broken in favour of the earlier rewriter.</item>
///   <item>Fail-open: if <c>continueOnException</c> is true (default), a failing
///         rewriter is skipped rather than aborting the whole pipeline.</item>
///   <item>Fan-out is capped by <paramref name="maxRewrites"/> — retrieval cost
///         grows linearly with the number of variants, so keep this small
///         (typically 3–8).</item>
/// </list>
/// </summary>
public sealed class CompositeQueryRewriter : IQueryRewriter
{
    private readonly IReadOnlyList<IQueryRewriter> _rewriters;
    private readonly int _maxRewrites;
    private readonly bool _continueOnException;
    private readonly Action<Exception, IQueryRewriter>? _onException;

    public CompositeQueryRewriter(
        IEnumerable<IQueryRewriter> rewriters,
        int maxRewrites = 8,
        bool continueOnException = true,
        Action<Exception, IQueryRewriter>? onException = null)
    {
        ArgumentNullException.ThrowIfNull(rewriters);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRewrites);

        _rewriters = rewriters.ToList();
        if (_rewriters.Count == 0)
        {
            throw new ArgumentException("At least one rewriter is required.", nameof(rewriters));
        }

        _maxRewrites = maxRewrites;
        _continueOnException = continueOnException;
        _onException = onException;
    }

    public CompositeQueryRewriter(params IQueryRewriter[] rewriters)
        : this((IEnumerable<IQueryRewriter>)rewriters)
    {
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<QueryRewrite>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedQuery);
        
        // Invoke all rewriters in parallel, handling exceptions as configured.
        var tasks = _rewriters
            .Select(r => InvokeSafeAsync(r, normalizedQuery, cancellationToken))
            .ToArray();

        // Wait for all rewriters to complete and gather their results.
        var perRewriter = await Task.WhenAll(tasks).ConfigureAwait(false);

        // Deduplicate rewrites by text (case-insensitive) and merge them into a single list.
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            normalizedQuery,
        };
        var merged = new List<QueryRewrite>(capacity: _maxRewrites);

        foreach (var rewrites in perRewriter)
        {
            foreach (var rewrite in rewrites)
            {
                if (string.IsNullOrWhiteSpace(rewrite.Text))
                {
                    continue;
                }

                // Deduplicate by rewrite text (case-insensitive).
                if (!seen.Add(rewrite.Text))
                {
                    continue;
                }

                // Add the unique rewrite to the merged list.
                merged.Add(rewrite);

                if (merged.Count >= _maxRewrites)
                {
                    return merged;
                }
            }
        }

        // Return the merged list of unique rewrites.
        return merged;
    }

    /// <summary>
    /// Exécute le réécrivain de requête de manière sécurisée et retourne les réécritures produites.
    /// </summary>
    /// <remarks>Si l’opération est annulée via <paramref name="cancellationToken"/>, l’exception d’annulation
    /// est relancée. Si une exception se produit et que la poursuite sur exception est activée, l’exception est
    /// signalée puis une liste vide est retournée.</remarks>
    /// <param name="rewriter">Instance du réécrivain à invoquer.</param>
    /// <param name="normalizedQuery">Texte de requête normalisé à réécrire.</param>
    /// <param name="cancellationToken">Jeton utilisé pour propager l’annulation de l’opération.</param>
    /// <returns>Liste en lecture seule des réécritures retournées par le réécrivain, ou une liste vide si une exception est
    /// ignorée.</returns>
    private async Task<IReadOnlyList<QueryRewrite>> InvokeSafeAsync(
        IQueryRewriter rewriter,
        string normalizedQuery,
        CancellationToken cancellationToken)
    {
        try
        {
            // Call the rewriter to get the rewrites
            return await rewriter.RewriteAsync(normalizedQuery, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // If the operation was canceled, rethrow the exception to propagate cancellation.
            throw;
        }
        catch (Exception ex) when (_continueOnException)
        {
            // If the rewriter throws an exception, log it and continue with the next rewriter.
            _onException?.Invoke(ex, rewriter);

            // Return an empty list of rewrites for this rewriter.
            return Array.Empty<QueryRewrite>();
        }
    }
}
