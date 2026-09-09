namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Options controlling how <see cref="RulesBasedQueryAnalyzer"/> normalizes and analyzes queries.
/// </summary>
public sealed record QueryAnalyzerOptions
{
    /// <summary>Apply Unicode NFKC normalization (recommended).</summary>
    public bool NormalizeUnicode { get; init; } = true;

    /// <summary>Convert curly quotes/dashes to ASCII equivalents.</summary>
    public bool UnifyQuotesAndDashes { get; init; } = true;

    /// <summary>Collapse runs of whitespace (including line endings) into a single space.</summary>
    public bool CollapseWhitespace { get; init; } = true;

    /// <summary>Lowercase the query. Recommended for BM25 / lexical retrieval.</summary>
    public bool ToLowerCase { get; init; } = true;

    /// <summary>Remove combining diacritics (e.g. "café" -> "cafe").</summary>
    public bool RemoveDiacritics { get; init; } = false;

    /// <summary>Trim leading/trailing punctuation (?, !, ., ,, ;, :, quotes).</summary>
    public bool StripSurroundingPunctuation { get; init; } = true;

    /// <summary>Attempt to infer <see cref="QueryIntent"/> from lexical cues.</summary>
    public bool DetectIntent { get; init; } = true;

    public static QueryAnalyzerOptions Default { get; } = new();
}
