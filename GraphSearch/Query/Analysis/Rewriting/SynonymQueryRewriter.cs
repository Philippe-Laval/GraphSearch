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

    public Task<IReadOnlyList<string>> RewriteAsync(
        string normalizedQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedQuery);
        cancellationToken.ThrowIfCancellationRequested();

        var result = new List<string>(capacity: _maxRewrites);
        foreach (var (pattern, replacement) in _rules)
        {
            if (!pattern.IsMatch(normalizedQuery))
            {
                continue;
            }

            var rewritten = pattern.Replace(normalizedQuery, replacement);
            if (!string.Equals(rewritten, normalizedQuery, StringComparison.Ordinal)
                && !result.Contains(rewritten, StringComparer.OrdinalIgnoreCase))
            {
                result.Add(rewritten);
                if (result.Count >= _maxRewrites)
                {
                    break;
                }
            }
        }

        return Task.FromResult<IReadOnlyList<string>>(result);
    }
}
