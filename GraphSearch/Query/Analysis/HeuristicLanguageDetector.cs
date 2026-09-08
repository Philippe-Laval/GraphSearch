using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis;

/// <summary>
/// Tiny stopword-based language detector.
/// Ships with English &amp; French out of the box; extend via
/// <see cref="AddLanguage"/> for more.
///
/// For anything beyond a smoke test, swap this with a real detector
/// (CLD3, fastText lid.176, Azure AI Language).
/// </summary>
public sealed class HeuristicLanguageDetector : ILanguageDetector
{
    private static readonly Regex TokenRegex = new(
        @"[\p{L}]+",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private readonly Dictionary<string, HashSet<string>> _stopwords =
        new(StringComparer.OrdinalIgnoreCase);

    public HeuristicLanguageDetector()
    {
        AddLanguage("en", [
            "the", "a", "an", "is", "are", "was", "were", "what", "who", "which",
            "and", "or", "of", "in", "on", "to", "for", "with", "by", "how",
            "does", "do", "did", "why",
        ]);
        AddLanguage("fr", [
            "le", "la", "les", "un", "une", "des", "est", "sont", "quel", "quelle",
            "qui", "que", "quoi", "et", "ou", "de", "du", "dans", "sur", "pour",
            "avec", "par", "comment", "pourquoi", "où",
        ]);
    }

    public void AddLanguage(string bcp47, IEnumerable<string> stopwords)
    {
        _stopwords[bcp47] = new HashSet<string>(stopwords, StringComparer.OrdinalIgnoreCase);
    }

    public string? Detect(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var tokens = TokenRegex.Matches(text);
        if (tokens.Count == 0)
        {
            return null;
        }

        string? bestLang = null;
        var bestScore = 0;

        foreach (var (lang, words) in _stopwords)
        {
            var score = 0;
            foreach (Match t in tokens)
            {
                if (words.Contains(t.Value))
                {
                    score++;
                }
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestLang = lang;
            }
        }

        return bestScore > 0 ? bestLang : null;
    }
}
