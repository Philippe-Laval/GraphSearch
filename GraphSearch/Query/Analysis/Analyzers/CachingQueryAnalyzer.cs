using System.Collections.Concurrent;

namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Decorator that caches analyzed queries keyed by the raw input string.
/// Useful when the inner analyzer is expensive (LLM, remote NER, embedding model).
/// </summary>
/// <remarks>
/// The cache is unbounded. For production workloads with unbounded query space,
/// wrap this with an eviction policy (e.g. <c>IMemoryCache</c>) instead.
/// </remarks>
public sealed class CachingQueryAnalyzer : IQueryAnalyzer
{
    private readonly IQueryAnalyzer _inner;
    private readonly ConcurrentDictionary<string, AnalyzedQuery> _cache;

    /// <summary>
    /// Initialise une nouvelle instance de la classe <c>CachingQueryAnalyzer</c> avec un analyseur sous-jacent et un
    /// comparateur de clés de cache optionnel.
    /// </summary>
    /// <param name="inner">Analyseur de requêtes sous-jacent utilisé pour effectuer l’analyse.</param>
    /// <param name="comparer">Comparateur de chaînes utilisé pour les clés du cache. Si <see langword="null"/>, <see
    /// cref="StringComparer.Ordinal"/> est utilisé.</param>
    public CachingQueryAnalyzer(IQueryAnalyzer inner, StringComparer? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(inner);
        _inner = inner;
        _cache = new ConcurrentDictionary<string, AnalyzedQuery>(comparer ?? StringComparer.Ordinal);
    }

    /// <summary>
    /// Analyse une requête en utilisant l’analyseur sous-jacent et met en cache le résultat.
    /// </summary>
    /// <param name="query">La requête à analyser.</param>
    /// <param name="cancellationToken">Jeton d’annulation optionnel.</param>
    /// <returns>Une tâche représentant l’opération asynchrone, contenant le résultat de l’analyse.</returns>
    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // Vérifie si la requête a déjà été analysée et mise en cache.
        if (_cache.TryGetValue(query, out var cached))
        {
            return cached;
        }

        // Si la requête n’est pas dans le cache, l’analyse est effectuée par l’analyseur sous-jacent.
        var analyzed = await _inner.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);

        // Met en cache le résultat de l’analyse pour les futures requêtes identiques.
        _cache.TryAdd(query, analyzed);

        return analyzed;
    }
}
