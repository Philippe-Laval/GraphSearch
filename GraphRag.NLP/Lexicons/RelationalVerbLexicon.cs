using System.Text.RegularExpressions;

namespace GraphRag.NLP.Lexicons
{
    /// <summary>
    /// Dictionary of common relational verbs in English and French, mapped to a canonical English lemma.
    /// </summary>
    public static class RelationalVerbLexicon
    {
        private static readonly Dictionary<string, string> EnVerbs =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["develop"] = "develop", ["develops"] = "develop", ["developed"] = "develop", ["developing"] = "develop",
                ["create"] = "create", ["creates"] = "create", ["created"] = "create", ["creating"] = "create",
                ["make"] = "make", ["makes"] = "make", ["made"] = "make", ["making"] = "make",
                ["build"] = "build", ["builds"] = "build", ["built"] = "build", ["building"] = "build",
                ["found"] = "found", ["founds"] = "found", ["founded"] = "found", ["founding"] = "found",
                ["design"] = "design", ["designs"] = "design", ["designed"] = "design", ["designing"] = "design", ["write"] = "write", ["writes"] = "write", ["wrote"] = "write", ["written"] = "write",
                ["own"] = "own", ["owns"] = "own", ["owned"] = "own", ["owning"] = "own",
                ["acquire"] = "acquire", ["acquires"] = "acquire", ["acquired"] = "acquire",
                ["manage"] = "manage", ["manages"] = "manage", ["managed"] = "manage",
                ["maintain"] = "maintain", ["maintains"] = "maintain", ["maintained"] = "maintain",
                ["belong"] = "belong", ["belongs"] = "belong", ["belonged"] = "belong",
                ["use"] = "use", ["uses"] = "use", ["used"] = "use", ["using"] = "use",
                ["run"] = "run", ["runs"] = "run", ["ran"] = "run", ["running"] = "run",
                ["work"] = "work", ["works"] = "work", ["worked"] = "work", ["working"] = "work",
                ["depend"] = "depend", ["depends"] = "depend", ["depended"] = "depend",
                ["locate"] = "locate", ["located"] = "locate",
                ["base"] = "base", ["based"] = "base",
            };

        private static readonly Dictionary<string, string> FrVerbs =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["développer"] = "develop", ["développe"] = "develop", ["développent"] = "develop",
                ["développé"] = "develop", ["développée"] = "develop", ["développés"] = "develop", ["développées"] = "develop",
                ["developper"] = "develop",
                ["créer"] = "create", ["crée"] = "create", ["créent"] = "create",
                ["créé"] = "create", ["créée"] = "create", ["créés"] = "create", ["créées"] = "create",
                ["creer"] = "create",
                ["fabriquer"] = "make", ["fabrique"] = "make", ["fabriqué"] = "make",
                ["faire"] = "make", ["fait"] = "make", ["faits"] = "make", ["faite"] = "make", ["faites"] = "make",
                ["construire"] = "build", ["construit"] = "build", ["construits"] = "build",
                ["construite"] = "build", ["construites"] = "build",
                ["fonder"] = "found", ["fonde"] = "found", ["fondé"] = "found", ["fondée"] = "found",
                ["concevoir"] = "design", ["conçoit"] = "design", ["conçu"] = "design", ["conçue"] = "design",
                ["écrire"] = "write", ["écrit"] = "write", ["écrite"] = "write", ["écrits"] = "write",
                ["ecrire"] = "write", ["ecrit"] = "write",
                ["posséder"] = "own", ["possède"] = "own", ["possèdent"] = "own", ["possédé"] = "own",
                ["posseder"] = "own", ["possede"] = "own",
                ["appartenir"] = "belong", ["appartient"] = "belong", ["appartiennent"] = "belong",
                ["acquérir"] = "acquire", ["acquiert"] = "acquire", ["acquis"] = "acquire", ["acquise"] = "acquire",
                ["acquerir"] = "acquire",
                ["gérer"] = "manage", ["gère"] = "manage", ["gèrent"] = "manage", ["géré"] = "manage",
                ["gerer"] = "manage", ["gere"] = "manage",
                ["maintenir"] = "maintain", ["maintient"] = "maintain", ["maintiennent"] = "maintain", ["maintenu"] = "maintain",
                ["utiliser"] = "use", ["utilise"] = "use", ["utilisent"] = "use", ["utilisé"] = "use", ["utilisée"] = "use",
                ["employer"] = "use", ["emploie"] = "use", ["employé"] = "use",
                ["fonctionner"] = "run", ["fonctionne"] = "run", ["fonctionnent"] = "run",
                ["tourner"] = "run", ["tourne"] = "run", ["tournent"] = "run",
                ["travailler"] = "work", ["travaille"] = "work", ["travaillent"] = "work",
                ["dépendre"] = "depend", ["dépend"] = "depend", ["dépendent"] = "depend",
                ["dependre"] = "depend", ["depend"] = "depend",
                ["situer"] = "locate", ["situé"] = "locate", ["située"] = "locate",
                ["localiser"] = "locate", ["localisé"] = "locate",
                ["baser"] = "base", ["basé"] = "base", ["basée"] = "base", ["bases"] = "base",
            };

        private static readonly Regex TokenRegex = new(
            @"[\p{L}]+",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static IReadOnlyList<string> Extract(string text, string? language)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<string>();
            }

            var lang = string.IsNullOrWhiteSpace(language) ? "en" : language.ToLowerInvariant();
            var table = lang.StartsWith("fr", StringComparison.OrdinalIgnoreCase) ? FrVerbs : EnVerbs;

            var result = new List<string>();
            foreach (Match match in TokenRegex.Matches(text))
            {
                if (table.TryGetValue(match.Value, out var canonical)
                    && !result.Contains(canonical, StringComparer.OrdinalIgnoreCase))
                {
                    result.Add(canonical);
                }
            }

            return result;
        }
    }
}
