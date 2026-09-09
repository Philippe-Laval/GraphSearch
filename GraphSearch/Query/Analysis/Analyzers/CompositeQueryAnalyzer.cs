namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Chains analyzers, using the first result whose intent passes the acceptance
/// predicate. Default acceptance: any intent other than
/// <see cref="QueryIntent.Unknown"/> or <see cref="QueryIntent.General"/>.
///
/// Classic use case: cheap rules first, LLM fallback for hard queries.
///
///   new CompositeQueryAnalyzer(
///       new RulesBasedQueryAnalyzer(),                       // rules
///       new EmbeddingQueryAnalyzer(embed, classifier),       // paraphrase
///       new LlmQueryAnalyzer(chat));                         // last resort
///
/// Set <c>continueOnException = true</c> to skip inner analyzers that throw
/// (network errors, auth failures, etc.) — useful when composing a managed
/// cloud service (Azure AI Language) with a self-hosted fallback (spaCy).
/// </summary>
public sealed class CompositeQueryAnalyzer : IQueryAnalyzer
{
    private readonly IReadOnlyList<IQueryAnalyzer> _analyzers;
    private readonly Func<AnalyzedQuery, bool> _shouldAccept;
    private readonly bool _continueOnException;
    private readonly Action<Exception, IQueryAnalyzer>? _onException;

    /// <summary>
    /// Initialise une nouvelle instance de la classe en configurant une chaîne d’analyseurs de requête et la stratégie
    /// d’acceptation des résultats.
    /// </summary>
    /// <remarks>La collection <paramref name="analyzers"/> ne peut pas être null et doit contenir au moins un
    /// élément.</remarks>
    /// <param name="analyzers">Collection des analyseurs à exécuter.</param>
    /// <param name="shouldAccept">Fonction qui détermine si un résultat d’analyse est accepté. Si la valeur est null, la stratégie par défaut est
    /// utilisée.</param>
    /// <param name="continueOnException">Indique s’il faut poursuivre l’exécution des analyseurs restants lorsqu’une exception est levée.</param>
    /// <param name="onException">Action appelée lorsqu’un analyseur lève une exception.</param>
    /// <exception cref="ArgumentException">Levée lorsque la collection <paramref name="analyzers"/> est vide.</exception>
    public CompositeQueryAnalyzer(
        IEnumerable<IQueryAnalyzer> analyzers,
        Func<AnalyzedQuery, bool>? shouldAccept = null,
        bool continueOnException = false,
        Action<Exception, IQueryAnalyzer>? onException = null)
    {
        ArgumentNullException.ThrowIfNull(analyzers);
        _analyzers = analyzers.ToList();
        if (_analyzers.Count == 0)
        {
            throw new ArgumentException("At least one analyzer is required.", nameof(analyzers));
        }
        _shouldAccept = shouldAccept ?? DefaultShouldAccept;
        _continueOnException = continueOnException;
        _onException = onException;
    }

    public CompositeQueryAnalyzer(params IQueryAnalyzer[] analyzers)
        : this((IEnumerable<IQueryAnalyzer>)analyzers)
    {
    }

    /// <inheritdoc/>
    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        AnalyzedQuery? last = null;
        Exception? lastException = null;

        foreach (var analyzer in _analyzers)
        {
            AnalyzedQuery result;
            try
            {
                result = await analyzer.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (_continueOnException)
            {
                lastException = ex;
                _onException?.Invoke(ex, analyzer);
                continue;
            }

            last = result;
            if (_shouldAccept(result))
            {
                return result;
            }
        }

        if (last is not null)
        {
            return last;
        }

        // Every inner analyzer threw and we were told to keep going.
        throw new InvalidOperationException(
            "All analyzers in the composite failed.",
            lastException);
    }

    /// <summary>
    /// Détermine si une requête analysée doit être acceptée en fonction de son intention.
    /// </summary>
    /// <param name="q">Requête analysée à évaluer.</param>
    /// <returns>true si l’intention n’est ni Unknown ni General ; sinon, false.</returns>
    private static bool DefaultShouldAccept(AnalyzedQuery q) =>
        q.Intent is not (QueryIntent.Unknown or QueryIntent.General);
}

