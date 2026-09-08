using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis.Lexicons;

/// <summary>
/// Dictionary of common relational verbs in English and French, used to
/// approximate "root verbs" when no POS tagger is available (e.g. with
/// <see cref="AzureLanguageNerService"/>).
///
/// French verbs are mapped to their canonical English lemma so downstream
/// consumers see a unified vocabulary.
/// </summary>
public static class RelationalVerbLexicon
{
    // English verb -> canonical English lemma (usually itself).
    private static readonly Dictionary<string, string> EnVerbs =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // creation / development
            ["develop"] = "develop", ["develops"] = "develop", ["developed"] = "develop", ["developing"] = "develop",
            ["create"] = "create", ["creates"] = "create", ["created"] = "create", ["creating"] = "create",
            ["make"] = "make", ["makes"] = "make", ["made"] = "make", ["making"] = "make",
            ["build"] = "build", ["builds"] = "build", ["built"] = "build", ["building"] = "build",
            ["found"] = "found", ["founds"] = "found", ["founded"] = "found", ["founding"] = "found",
            ["design"] = "design", ["designs"] = "design", ["designed"] = "design", ["designing"] = "design",
            ["write"] = "write", ["writes"] = "write", ["wrote"] = "write", ["written"] = "write",

            // ownership / affiliation
            ["own"] = "own", ["owns"] = "own", ["owned"] = "own", ["owning"] = "own",
            ["acquire"] = "acquire", ["acquires"] = "acquire", ["acquired"] = "acquire",
            ["manage"] = "manage", ["manages"] = "manage", ["managed"] = "manage",
            ["maintain"] = "maintain", ["maintains"] = "maintain", ["maintained"] = "maintain",
            ["belong"] = "belong", ["belongs"] = "belong", ["belonged"] = "belong",

            // usage / operation
            ["use"] = "use", ["uses"] = "use", ["used"] = "use", ["using"] = "use",
            ["run"] = "run", ["runs"] = "run", ["ran"] = "run", ["running"] = "run",
            ["work"] = "work", ["works"] = "work", ["worked"] = "work", ["working"] = "work",
            ["depend"] = "depend", ["depends"] = "depend", ["depended"] = "depend",

            // location
            ["locate"] = "locate", ["located"] = "locate",
            ["base"] = "base", ["based"] = "base",
        };

    // French verb form -> canonical English lemma.
    private static readonly Dictionary<string, string> FrVerbs =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // développer
            ["développer"] = "develop", ["développe"] = "develop", ["développent"] = "develop",
            ["développé"] = "develop", ["développée"] = "develop", ["développés"] = "develop", ["développées"] = "develop",
            ["developper"] = "develop", // tolerate missing accent

            // créer
            ["créer"] = "create", ["crée"] = "create", ["créent"] = "create",
            ["créé"] = "create", ["créée"] = "create", ["créés"] = "create", ["créées"] = "create",
            ["creer"] = "create",

            // fabriquer / faire
            ["fabriquer"] = "make", ["fabrique"] = "make", ["fabriqué"] = "make",
            ["faire"] = "make", ["fait"] = "make", ["faits"] = "make", ["faite"] = "make", ["faites"] = "make",

            // construire
            ["construire"] = "build", ["construit"] = "build", ["construits"] = "build",
            ["construite"] = "build", ["construites"] = "build",

            // fonder
            ["fonder"] = "found", ["fonde"] = "found", ["fondé"] = "found", ["fondée"] = "found",

            // concevoir
            ["concevoir"] = "design", ["conçoit"] = "design", ["conçu"] = "design", ["conçue"] = "design",

            // écrire
            ["écrire"] = "write", ["écrit"] = "write", ["écrite"] = "write", ["écrits"] = "write",
            ["ecrire"] = "write", ["ecrit"] = "write",

            // posséder / avoir
            ["posséder"] = "own", ["possède"] = "own", ["possèdent"] = "own", ["possédé"] = "own",
            ["posseder"] = "own", ["possede"] = "own",
            ["appartenir"] = "belong", ["appartient"] = "belong", ["appartiennent"] = "belong",

            // acquérir
            ["acquérir"] = "acquire", ["acquiert"] = "acquire", ["acquis"] = "acquire", ["acquise"] = "acquire",
            ["acquerir"] = "acquire",

            // gérer / maintenir
            ["gérer"] = "manage", ["gère"] = "manage", ["gèrent"] = "manage", ["géré"] = "manage",
            ["gerer"] = "manage", ["gere"] = "manage",
            ["maintenir"] = "maintain", ["maintient"] = "maintain", ["maintiennent"] = "maintain", ["maintenu"] = "maintain",

            // utiliser / employer
            ["utiliser"] = "use", ["utilise"] = "use", ["utilisent"] = "use", ["utilisé"] = "use", ["utilisée"] = "use",
            ["employer"] = "use", ["emploie"] = "use", ["employé"] = "use",

            // fonctionner / tourner sur
            ["fonctionner"] = "run", ["fonctionne"] = "run", ["fonctionnent"] = "run",
            ["tourner"] = "run", ["tourne"] = "run", ["tournent"] = "run",

            // travailler
            ["travailler"] = "work", ["travaille"] = "work", ["travaillent"] = "work",

            // dépendre
            ["dépendre"] = "depend", ["dépend"] = "depend", ["dépendent"] = "depend",
            ["dependre"] = "depend", ["depend"] = "depend",

            // situer / localiser
            ["situer"] = "locate", ["situé"] = "locate", ["située"] = "locate",
            ["localiser"] = "locate", ["localisé"] = "locate",

            // baser
            ["baser"] = "base", ["basé"] = "base", ["basée"] = "base", ["bases"] = "base",
        };

    /// <summary>
    /// Returns the canonical English lemmas of relational verbs found in the text.
    /// Deduplicated, insertion-order preserved.
    /// </summary>
    public static IReadOnlyList<string> Extract(string text, string? language)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<string>();
        }

        var lang = string.IsNullOrWhiteSpace(language) ? "en" : language.ToLowerInvariant();
        var isFrench = lang.StartsWith("fr", StringComparison.OrdinalIgnoreCase);
        var table = isFrench ? FrVerbs : EnVerbs;

        var result = new List<string>();
        foreach (Match m in TokenRegex.Matches(text))
        {
            if (table.TryGetValue(m.Value, out var canonical)
                && !result.Contains(canonical, StringComparer.OrdinalIgnoreCase))
            {
                result.Add(canonical);
            }
        }

        return result;
    }

    /// <summary>
    /// Représente une expression régulière compilée qui correspond à une ou plusieurs lettres Unicode consécutives.
    /// </summary>
    /// <remarks>Utilise une correspondance indépendante de la culture pour garantir un comportement cohérent
    /// entre environnements.</remarks>
    private static readonly Regex TokenRegex = new(
        @"[\p{L}]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
}

/*
The pattern [\p{L}]+

Part	Meaning
[...]	Character class — match any one character listed inside.
\p{L}	Unicode property escape. Matches any character whose Unicode category is Letter (category L, 
        which covers Lu uppercase, Ll lowercase, Lt titlecase, Lm modifier, Lo other letters).
+	    One or more of the preceding — so we greedily gobble a whole run of letters.

Effectively: "a maximal run of Unicode letters" — i.e. one word, stripped of surrounding punctuation, 
whitespace, digits, symbols.

Why \p{L} and not \w or [a-zA-Z]?
•	[a-zA-Z] misses accented and non-Latin letters — développé, où, Zürich, 北京 — all excluded. Very bad for a French lexicon.
•	\w in .NET matches [A-Za-z0-9_] plus Unicode letters/digits/connectors — it would swallow digits and underscores, which we don't want when tokenizing words.
•	\p{L} matches only letters, in any script, including:
•	é ç à ü ñ ö (French, Spanish, German)
•	Ω π λ (Greek)
•	а б в (Cyrillic)
•	北 京 東 (CJK ideographs)
•	א ב ג (Hebrew)
•	etc.

That's exactly what a language-aware tokenizer needs for lexicon lookup.

The RegexOptions flags

Flag	Effect
RegexOptions.Compiled	Compiles the pattern to IL at construction time. Each match is faster; construction is slower. Worth it because this instance is static readonly — compiled once, reused forever.
RegexOptions.CultureInvariant	Case comparisons (and locale-sensitive character folding) ignore the current thread's CultureInfo. Guarantees the pattern behaves identically on every machine — no surprise Turkish-i vs Latin-i issues, no differences between servers in different regions.

Concretely, what it does in RelationalVerbLexicon 

foreach (Match m in TokenRegex.Matches(text))
{
    if (table.TryGetValue(m.Value, out var canonical)) ...
}

Given input "Microsoft a développé C# et .NET 10 en 2002 !", TokenRegex.Matches yields:
Microsoft, a, développé, C, et, NET, en

•	Punctuation (., #, !) → skipped.
•	Digits (10, 2002) → skipped.
•	Whitespace → skipped.
•	Accented letter é in développé → kept, so the lookup FrVerbs["développé"] → "develop" succeeds.

Note on the [...] wrapper
\p{L}+ alone would work identically here — the character class brackets are redundant 
when there's a single element inside. [\p{L}]+ is just a stylistic choice that makes 
it clear this is a class you might later extend to 
[\p{L}\p{M}]+ (letters + combining marks, e.g. to keep decomposed accents together) 
or [\p{L}\p{Nd}]+ (letters + decimal digits).
 */