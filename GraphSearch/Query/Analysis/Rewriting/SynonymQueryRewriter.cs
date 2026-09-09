using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis.Rewriting;

/// <summary>
/// Simple dictionary-driven rewriter: replaces phrases with canonical
/// alternatives. Useful as a deterministic baseline before wiring an LLM
/// rewriter.
///
/// Example:
///   Add("microsoft's tech", "technologies used by microsoft");
///   Add("who made {x}",     "who developed {x}");   // literal, not templated
/// </summary>
public sealed class SynonymQueryRewriter : IQueryRewriter
{
    private readonly List<(Regex Pattern, string Replacement)> _rules = new();
    private readonly int _maxRewrites;

    public SynonymQueryRewriter(int maxRewrites = 3)
    {
        if (maxRewrites <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRewrites));
        }
        _maxRewrites = maxRewrites;
    }

    /// <summary>
    /// Ajoute une règle de réécriture qui remplace une expression par une valeur de substitution.
    /// </summary>
    /// <remarks>La correspondance utilise une expression régulière avec bornes de mot, sans distinction de
    /// casse, avec options invariantes de culture.</remarks>
    /// <param name="phrase">Expression à rechercher dans la requête.</param>
    /// <param name="replacement">Valeur utilisée pour remplacer l’expression correspondante.</param>
    /// <returns>L’instance actuelle de <see cref="SynonymQueryRewriter"/> pour permettre le chaînage d’appels.</returns>
    public SynonymQueryRewriter Add(string phrase, string replacement)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phrase);
        ArgumentNullException.ThrowIfNull(replacement);

        var pattern = new Regex(
            @"\b" + Regex.Escape(phrase) + @"\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        _rules.Add((pattern, replacement));

        return this;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<QueryRewrite>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedQuery);

        cancellationToken.ThrowIfCancellationRequested();

        var result = new List<QueryRewrite>(capacity: _maxRewrites);

        foreach (var (pattern, replacement) in _rules)
        {
            // Skip if the pattern doesn't match the query
            if (!pattern.IsMatch(normalizedQuery))
            {
                continue;
            }

            // Perform the replacement
            var rewritten = pattern.Replace(normalizedQuery, replacement);

            // Skip no-op rewrites
            if (string.Equals(rewritten, normalizedQuery, StringComparison.Ordinal))
            {
                continue;
            }

            // Deduplicate by rewrite text (case-insensitive)
            if (result.Any(r => string.Equals(r.Text, rewritten, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            result.Add(new QueryRewrite(
                Text: rewritten,
                Kind: RewriteKind.Synonym,
                Weight: 1.0));

            if (result.Count >= _maxRewrites)
            {
                break;
            }
        }

        return Task.FromResult<IReadOnlyList<QueryRewrite>>(result);
    }
}
