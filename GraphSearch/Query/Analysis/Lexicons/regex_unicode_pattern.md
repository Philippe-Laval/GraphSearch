# The pattern [\p\{L\}]+

| Part    | Meaning  |
| :------ | :------  |
| [...]   | Character class — match any one character listed inside.  |
| \p\{L\}   | Unicode property escape. Matches any character whose Unicode category is Letter (category L, which covers Lu uppercase, Ll lowercase, Lt titlecase, Lm modifier, Lo other letters).  |
| +       | One or more of the preceding — so we greedily gobble a whole run of letters.  |


Effectively: "a maximal run of Unicode letters" — i.e. one word, stripped of surrounding punctuation, 
whitespace, digits, symbols.

Why \p\{L\} and not \w or [a-zA-Z]?
-	[a-zA-Z] misses accented and non-Latin letters — développé, où, Zürich, 北京 — all excluded. Very bad for a French lexicon.
-	\w in .NET matches [A-Za-z0-9_] plus Unicode letters/digits/connectors — it would swallow digits and underscores, which we don't want when tokenizing words.
-	\p\{L\} matches only letters, in any script, including:
-	é ç à ü ñ ö (French, Spanish, German)
-	Ω π λ (Greek)
-	а б в (Cyrillic)
-	北 京 東 (CJK ideographs)
-	א ב ג (Hebrew)
-	etc.

That's exactly what a language-aware tokenizer needs for lexicon lookup.

## The RegexOptions flags

| Flag    | Effect  |
| :------ | :------ |
| RegexOptions.Compiled  | Compiles the pattern to IL at construction time. Each match is faster; construction is slower. Worth it because this instance is static readonly — compiled once, reused forever.  |
| RegexOptions.CultureInvariant  | Case comparisons (and locale-sensitive character folding) ignore the current thread's CultureInfo. Guarantees the pattern behaves identically on every machine — no surprise Turkish-i vs Latin-i issues, no differences between servers in different regions.  |

Concretely, what it does in RelationalVerbLexicon 

```csharp
foreach (Match m in TokenRegex.Matches(text))
{
    if (table.TryGetValue(m.Value, out var canonical)) ...
}
```

Given input "Microsoft a développé C# et .NET 10 en 2002 !", TokenRegex.Matches yields:
Microsoft, a, développé, C, et, NET, en

-	Punctuation (., #, !) → skipped.
-	Digits (10, 2002) → skipped.
-	Whitespace → skipped.
-	Accented letter é in développé → kept, so the lookup FrVerbs["développé"] → "develop" succeeds.

## Note on the [...] wrapper

\p\{L\}+ alone would work identically here — the character class brackets are redundant when there's a single element inside. 

[\p\{L\}]+ is just a stylistic choice that makes it clear this is a class you might later extend to 
[\p\{L\}\p\{M\}]+ (letters + combining marks, e.g. to keep decomposed accents together) 
or [\p\{L\}\p\{Nd\}]+ (letters + decimal digits).