using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis.Language;

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
    /// <summary>
    /// Expression régulière qui correspond à une séquence d’un ou plusieurs caractères de lettre Unicode.
    /// </summary>
    /// <remarks>Configurée avec <c>RegexOptions.CultureInvariant</c> et <c>RegexOptions.Compiled</c> pour un
    /// comportement indépendant de la culture et de meilleures performances d’exécution.</remarks>
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

    /// <summary>
    /// Ajoute ou remplace la liste des mots vides associée à une langue.
    /// </summary>
    /// <remarks>Les mots sont stockés dans un ensemble insensible à la casse.</remarks>
    /// <param name="bcp47">Balise de langue BCP-47 utilisée comme clé de la langue.</param>
    /// <param name="stopwords">Collection des mots vides à enregistrer pour la langue.</param>
    public void AddLanguage(string bcp47, IEnumerable<string> stopwords)
    {
        _stopwords[bcp47] = new HashSet<string>(stopwords, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Détecte la langue la plus probable d’un texte en comparant ses tokens aux listes de mots vides configurées.
    /// </summary>
    /// <param name="text">Texte à analyser.</param>
    /// <returns>Identifiant de la langue ayant le score le plus élevé, ou <see langword="null"/> si le texte est vide, ne
    /// contient aucun token ou ne correspond à aucune liste de mots vides.</returns>
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
