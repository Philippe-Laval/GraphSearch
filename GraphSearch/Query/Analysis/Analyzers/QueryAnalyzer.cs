using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis.Analyzers;

/*
This is deliberately kept as a separate component. Later you may want to:

   "What technologies does Microsoft use?"
                │
                ▼
   "What technologies are used/developed by Microsoft?"

   ...or infer richer query intent (Aggregation, MultiHopRelationship, etc.)
   without touching the rest of the pipeline.

Rules-based implementation:
   - Deterministic, cheap, no I/O.
   - Handles Unicode, quotes/dashes, casing, whitespace.
   - Heuristic intent classifier based on lexical cues.

For anything requiring semantic understanding (paraphrasing, coreference,
truly ambiguous intent), swap this with an LLM- or embedding-based analyzer
via the IQueryAnalyzer interface.
*/

/// <summary>
/// Rules-based, deterministic <see cref="IQueryAnalyzer"/> implementation.
/// Performs Unicode-safe normalization and heuristic intent classification.
/// </summary>
public sealed partial class QueryAnalyzer : IQueryAnalyzer
{
    private readonly QueryAnalyzerOptions _options;

    public QueryAnalyzer()
        : this(QueryAnalyzerOptions.Default)
    {
    }

    public QueryAnalyzer(QueryAnalyzerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    /// <inheritdoc />
    public Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        cancellationToken.ThrowIfCancellationRequested();

        var normalized = Normalize(query);

        var intent = _options.DetectIntent
            ? DetectIntent(normalized)
            : QueryIntent.Unknown;

        return Task.FromResult(new AnalyzedQuery(
            OriginalQuery: query,
            NormalizedQuery: normalized,
            Intent: intent));
    }

    private string Normalize(string query)
    {
        var s = query;

        if (_options.NormalizeUnicode)
        {
            s = s.Normalize(NormalizationForm.FormKC);
        }

        if (_options.UnifyQuotesAndDashes)
        {
            s = UnifyQuotesAndDashes(s);
        }

        // Always drop line endings so downstream tokenizers see one line.
        s = s.ReplaceLineEndings(" ");

        if (_options.RemoveDiacritics)
        {
            s = RemoveDiacritics(s);
        }

        if (_options.CollapseWhitespace)
        {
            s = WhitespaceRegex().Replace(s, " ");
        }

        s = s.Trim();

        if (_options.StripSurroundingPunctuation)
        {
            s = s.Trim(PunctuationTrimChars);
        }

        if (_options.ToLowerCase)
        {
            s = s.ToLowerInvariant();
        }

        return s;
    }

    private static readonly char[] PunctuationTrimChars =
    [
        '?', '!', '.', ',', ';', ':', '"', '\'', '(', ')', '[', ']', '{', '}', ' ', '\t',
    ];

    private static string UnifyQuotesAndDashes(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            var mapped = ch switch
            {
                '\u2018' or '\u2019' or '\u201A' or '\u2032' => '\'',
                '\u201C' or '\u201D' or '\u201E' or '\u2033' => '"',
                '\u2013' or '\u2014' or '\u2212' => '-',
                '\u00A0' or '\u2007' or '\u202F' => ' ',
                _ => ch,
            };
            sb.Append(mapped);
        }
        return sb.ToString();
    }

    private static string RemoveDiacritics(string s)
    {
        var decomposed = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);
        foreach (var ch in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Detect <see cref="QueryIntent"/> for the given query string.
    /// </summary>
    /// <param name="q">The query string to analyze.</param>
    /// <returns>The detected <see cref="QueryIntent"/>.</returns>
    private static QueryIntent DetectIntent(string q)
    {
        // Order matters: check more specific intents first, then fall through to General.

        if (string.IsNullOrWhiteSpace(q))
        {
            return QueryIntent.Unknown;
        }

        if (AggregationRegex().IsMatch(q))
        {
            return QueryIntent.Aggregation;
        }

        if (ComparisonRegex().IsMatch(q))
        {
            return QueryIntent.Comparison;
        }

        var relationshipVerbs = RelationshipVerbRegex().Matches(q).Count;

        // Multi-hop cue: 2+ relationship verbs, OR chained pattern like
        // "products developed by X and running on Y".
        if (relationshipVerbs >= 2 || MultiHopRegex().IsMatch(q))
        {
            return QueryIntent.MultiHopRelationship;
        }

        if (relationshipVerbs == 1 || RelationshipRegex().IsMatch(q))
        {
            return QueryIntent.Relationship;
        }

        if (ExplanationRegex().IsMatch(q))
        {
            return QueryIntent.Explanation;
        }

        if (EntityLookupRegex().IsMatch(q))
        {
            return QueryIntent.EntityLookup;
        }

        return QueryIntent.General;
    }

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(
        @"\b(how many|how much|count of|total number of|number of|average|mean|median|sum of|top\s*\d+|least|most)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex AggregationRegex();

    [GeneratedRegex(
        @"\b(compare|comparison|versus|vs\.?|difference between|compared to|better than|worse than)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ComparisonRegex();

    [GeneratedRegex(
        @"\b(why|explain|how does|how do|what causes|reason for|because)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ExplanationRegex();

    [GeneratedRegex(
        @"^\s*(what is|what's|who is|who's|define|definition of|tell me about)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex EntityLookupRegex();

    [GeneratedRegex(
        @"\b(relationship between|related to|connected to|link between|connection between|association between)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex RelationshipRegex();

    // Common relational verbs used in GraphRAG-style questions.
    [GeneratedRegex(
        @"\b(developed by|created by|made by|built by|owned by|acquired by|founded by|used by|written by|based on|runs on|works with|works on|depends on|part of|belongs to|located in|based in|managed by|maintained by)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex RelationshipVerbRegex();

    // "which X ... by ... that ...", "products ... and ... on ...": chained clauses.
    [GeneratedRegex(
        @"\b(which|what)\b.+\b(and|that|which)\b.+\b(by|on|in|with|for)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MultiHopRegex();
}
