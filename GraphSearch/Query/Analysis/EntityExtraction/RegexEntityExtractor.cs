using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis.EntityExtraction;

/// <summary>
/// A single regex-driven extraction rule.
/// </summary>
/// <param name="Type">Entity type assigned to matches (e.g. "Date", "Version", "URL").</param>
/// <param name="Pattern">.NET regex pattern.</param>
/// <param name="Confidence">Fixed confidence assigned to matches (default 0.9).</param>
/// <param name="Group">
/// Optional named or numbered capture group. When null, the whole match is used.
/// Handy for anchored patterns like <c>\bversion\s+(\d+(?:\.\d+)+)\b</c> where you
/// only want the version number, not the anchor.
/// </param>
public sealed record RegexEntityRule(
    string Type,
    string Pattern,
    double Confidence = 0.9,
    string? Group = null);

/// <summary>
/// Pattern/rule-based extractor. Ideal for structured surface forms that a NER
/// model handles poorly: version numbers, ISO dates, URLs, emails, tickers,
/// SKUs, ISBNs, hashes, phone numbers, custom identifiers.
///
/// <para>
/// Best practice: use this for <b>closed-form</b> entities where you can enumerate
/// syntax. For open vocabulary (people, orgs, products) prefer
/// <see cref="NerEntityExtractor"/> or <see cref="LlmEntityExtractor"/> and combine
/// with regex via <see cref="HybridEntityExtractor"/>.
/// </para>
/// </summary>
public sealed class RegexEntityExtractor : IEntityExtractor
{
    private readonly IReadOnlyList<(Regex Regex, RegexEntityRule Rule)> _compiled;

    public RegexEntityExtractor(IEnumerable<RegexEntityRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        // Pre-compile regexes for performance. Ignore case and culture for most entity types.
        _compiled = rules
            .Select(r => (
                new Regex(
                    r.Pattern,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled),
                r))
            .ToList();
    }

    /// <summary>
    /// Convenience factory populated with a small but useful default rule set
    /// (semantic version, ISO date, URL, email). Extend or replace as needed.
    /// </summary>
    public static RegexEntityExtractor CreateDefault() => new(
    [
        new("Version", @"\b\d+(?:\.\d+){1,3}(?:-[A-Za-z0-9.]+)?\b", 0.85),
        new("Date",    @"\b\d{4}-\d{2}-\d{2}\b",                    0.95),
        new("Url",     @"\bhttps?://[^\s""'<>()]+",                 0.99),
        new("Email",   @"\b[\w.+-]+@[\w-]+(?:\.[\w-]+)+\b",          0.99),
    ]);

    public Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        cancellationToken.ThrowIfCancellationRequested();

        var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

        foreach (var (regex, rule) in _compiled)
        {
            foreach (Match match in regex.Matches(query))
            {
                var group = rule.Group is null
                    ? match
                    : (Group)match.Groups[rule.Group];

                if (!group.Success || group.Length == 0)
                {
                    continue;
                }

                var entity = new ExtractedEntity(
                    Text: group.Value,
                    Type: rule.Type,
                    Start: group.Index,
                    Length: group.Length,
                    Confidence: rule.Confidence);

                EntitySpanMerger.AddOrReplace(accumulator, entity, priority: 0);
            }
        }

        return Task.FromResult(EntitySpanMerger.Finalize(accumulator));
    }
}
