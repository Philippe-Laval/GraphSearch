namespace GraphSearch.Library.Query.Analysis.Rewriting;

/// <summary>
/// HyDE — Hypothetical Document Embeddings (Gao et al., 2022).
///
/// <para>
/// Instead of paraphrasing the query, HyDE asks the LLM to generate a
/// <b>plausible ideal answer paragraph</b>. That paragraph is much closer in
/// vocabulary/style to real corpus documents than the original short query,
/// so embedding it and running vector search recovers relevant passages that
/// a bare query would miss.
/// </para>
///
/// <para>
/// Important consumer contract:
/// </para>
/// <list type="bullet">
///   <item>HyDE rewrites are <b>not paraphrases</b>. They must be routed to the
///         <b>vector retriever only</b> — never fed to BM25/keyword search.
///         Downstream fusion should key on <see cref="RewriteKind.HyDE"/>.</item>
///   <item>The generated paragraph may contain fabricated facts. That is fine
///         because we only use it as an embedding target, not as evidence.</item>
///   <item>Wrap in <see cref="CachingQueryRewriter"/>; LLM calls are expensive
///         and HyDE outputs are stable for a given query.</item>
/// </list>
/// </summary>
public sealed class HydeQueryRewriter : IQueryRewriter
{
    private const string DefaultPromptTemplate =
        """
        Write a concise, factual-sounding paragraph (2–4 sentences) that could
        plausibly appear in a knowledge base and would directly answer the
        following question. Use the same language as the question. Do not
        include disclaimers, citations, or meta-commentary. Reply with the
        paragraph only.

        Question:
        {{QUERY}}
        """;

    private readonly IChatCompletionClient _client;
    private readonly string _promptTemplate;
    private readonly int _numHypotheses;
    private readonly double _weight;

    /// <param name="client">LLM client.</param>
    /// <param name="numHypotheses">
    /// Number of hypothetical documents to generate. The original HyDE paper
    /// suggests 1–8; more hypotheses generally help vector recall but multiply
    /// cost. Each hypothesis is a separate LLM call so results diverge.
    /// </param>
    /// <param name="weight">Fusion weight applied to HyDE rewrites (default 1.0).</param>
    /// <param name="promptTemplate">Optional custom prompt template.</param>
    public HydeQueryRewriter(
        IChatCompletionClient client,
        int numHypotheses = 1,
        double weight = 1.0,
        string? promptTemplate = null)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numHypotheses);

        _client = client;
        _numHypotheses = numHypotheses;
        _weight = weight;
        _promptTemplate = promptTemplate ?? DefaultPromptTemplate;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<QueryRewrite>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedQuery);

        var prompt = _promptTemplate.Replace("{{QUERY}}", normalizedQuery, StringComparison.Ordinal);

        // Fire N independent completions in parallel — HyDE benefits from diversity.
        var tasks = new Task<string?>[_numHypotheses];
        for (var i = 0; i < _numHypotheses; i++)
        {
            tasks[i] = SafeCompleteAsync(prompt, cancellationToken);
        }

        // Wait for all completions to finish
        var completions = await Task.WhenAll(tasks).ConfigureAwait(false);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var results = new List<QueryRewrite>(capacity: _numHypotheses);

        foreach (var raw in completions)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                continue;
            }

            // Trim the text and check for uniqueness
            var paragraph = raw.Trim();
            if (!seen.Add(paragraph))
            {
                continue;
            }

            // Add the unique paragraph as a HyDE rewrite
            results.Add(new QueryRewrite(
                Text: paragraph,
                Kind: RewriteKind.HyDE,
                Weight: _weight));
        }

        return results;
    }

    /// <summary>
    /// Exécute une requête de complétion de manière sécurisée en propageant l’annulation demandée 
    /// et en gérant les autres échecs.
    /// </summary>
    /// <remarks>Une <see cref="OperationCanceledException"/> est relancée lorsque l’annulation est demandée
    /// via <paramref name="cancellationToken"/>.</remarks>
    /// <param name="prompt">Texte d’entrée à soumettre au client de complétion.</param>
    /// <param name="cancellationToken">Jeton utilisé pour observer et propager une demande d’annulation.</param>
    /// <returns>Résultat de la complétion, ou <see langword="null"/> si une erreur autre qu’une annulation demandée se produit.</returns>
    private async Task<string?> SafeCompleteAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            // Call the LLM client to get the raw completion
            return await _client.CompleteAsync(prompt, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Propagate cancellation if it was requested
            throw;
        }
        catch
        {
            // Swallow other exceptions and return null to indicate failure
            return null;
        }
    }
}
