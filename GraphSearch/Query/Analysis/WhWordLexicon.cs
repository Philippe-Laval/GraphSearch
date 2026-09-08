using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// Detects the interrogative ("wh-") word in an English or French query and
/// returns it as the <b>canonical English lemma</b>.
///
/// This lets downstream code (<see cref="NerQueryAnalyzer"/>) keep a single
/// intent table for all supported languages.
/// </summary>
public static class WhWordLexicon
{
    private static readonly HashSet<string> EnWhWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "what", "who", "whom", "whose", "which", "when", "where", "why", "how",
    };

    // Multi-word English cues, matched on the first two tokens.
    private static readonly Dictionary<(string, string), string> EnMulti = new()
    {
        [("how", "many")] = "how many",
        [("how", "much")] = "how much",
        [("how", "does")] = "how does",
        [("how", "do")] = "how do",
    };

    // French single-word -> canonical English lemma.
    private static readonly Dictionary<string, string> FrSingle =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["qui"] = "who",
            ["que"] = "what",
            ["qu"] = "what",       // "qu'est-ce que"
            ["quoi"] = "what",
            ["quel"] = "which",
            ["quelle"] = "which",
            ["quels"] = "which",
            ["quelles"] = "which",
            ["lequel"] = "which",
            ["laquelle"] = "which",
            ["quand"] = "when",
            ["où"] = "where",
            ["ou"] = "where",       // tolerate missing accent
            ["pourquoi"] = "why",
            ["comment"] = "how",
            ["combien"] = "how many",
        };

    // Multi-word French cues.
    private static readonly Dictionary<(string, string), string> FrMulti = new()
    {
        [("combien", "de")] = "how many",
        [("comment", "est")] = "how does",
        [("comment", "sont")] = "how do",
    };

    /// <summary>
    /// Returns the canonical English wh-lemma detected in the given text,
    /// or <c>null</c> if none is found.
    /// </summary>
    /// <param name="text">Raw or normalized query text.</param>
    /// <param name="language">BCP-47 code ("en", "fr"). Case-insensitive. Defaults to "en".</param>
    public static string? Detect(string text, string? language)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var lang = string.IsNullOrWhiteSpace(language) ? "en" : language.ToLowerInvariant();

        // Take the first ~3 word tokens (ignoring punctuation/quotes).
        var tokens = new List<string>(3);
        foreach (Match m in TokenRegex.Matches(text))
        {
            tokens.Add(m.Value.ToLowerInvariant());
            if (tokens.Count == 3)
            {
                break;
            }
        }

        if (tokens.Count == 0)
        {
            return null;
        }

        // Multi-word first (only meaningful when we have 2+ tokens).
        if (tokens.Count >= 2)
        {
            var pair = (tokens[0], tokens[1]);
            if (lang.StartsWith("fr", StringComparison.OrdinalIgnoreCase))
            {
                if (FrMulti.TryGetValue(pair, out var frCanonical))
                {
                    return frCanonical;
                }
            }
            else
            {
                if (EnMulti.TryGetValue(pair, out var enCanonical))
                {
                    return enCanonical;
                }
            }
        }

        // Single-word.
        foreach (var tok in tokens)
        {
            if (lang.StartsWith("fr", StringComparison.OrdinalIgnoreCase))
            {
                if (FrSingle.TryGetValue(tok, out var canonical))
                {
                    return canonical;
                }
            }
            else
            {
                if (EnWhWords.Contains(tok))
                {
                    return tok;
                }
            }
        }

        return null;
    }

    private static readonly Regex TokenRegex = new(
        @"[\p{L}]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
}
