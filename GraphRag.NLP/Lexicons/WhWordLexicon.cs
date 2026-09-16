using System.Text.RegularExpressions;

namespace GraphRag.NLP.Lexicons
{
    /// <summary>
    /// Detects the interrogative word in English or French and returns a canonical English lemma.
    /// </summary>
    public static class WhWordLexicon
    {
        private static readonly HashSet<string> EnWhWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "what", "who", "whom", "whose", "which", "when", "where", "why", "how",
        };

        private static readonly Dictionary<(string, string), string> EnMulti = new()
        {
            [("how", "many")] = "how many",
            [("how", "much")] = "how much",
            [("how", "does")] = "how does",
            [("how", "do")] = "how do",
        };

        private static readonly Dictionary<string, string> FrSingle =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["qui"] = "who",
                ["que"] = "what",
                ["qu"] = "what",
                ["quoi"] = "what",
                ["quel"] = "which",
                ["quelle"] = "which",
                ["quels"] = "which",
                ["quelles"] = "which",
                ["lequel"] = "which",
                ["laquelle"] = "which",
                ["quand"] = "when",
                ["où"] = "where",
                ["ou"] = "where",
                ["pourquoi"] = "why",
                ["comment"] = "how",
                ["combien"] = "how many",
            };

        private static readonly Dictionary<(string, string), string> FrMulti = new()
        {
            [("combien", "de")] = "how many",
            [("comment", "est")] = "how does",
            [("comment", "sont")] = "how do",
        };

        private static readonly Regex TokenRegex = new(
            @"[\p{L}]+",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string? Detect(string text, string? language)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var lang = string.IsNullOrWhiteSpace(language) ? "en" : language.ToLowerInvariant();

            var tokens = new List<string>(3);
            foreach (Match match in TokenRegex.Matches(text))
            {
                tokens.Add(match.Value.ToLowerInvariant());
                if (tokens.Count == 3)
                {
                    break;
                }
            }

            if (tokens.Count == 0)
            {
                return null;
            }

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
                else if (EnMulti.TryGetValue(pair, out var enCanonical))
                {
                    return enCanonical;
                }
            }

            foreach (var token in tokens)
            {
                if (lang.StartsWith("fr", StringComparison.OrdinalIgnoreCase))
                {
                    if (FrSingle.TryGetValue(token, out var canonical))
                    {
                        return canonical;
                    }
                }
                else if (EnWhWords.Contains(token))
                {
                    return token;
                }
            }

            return null;
        }
    }
}
