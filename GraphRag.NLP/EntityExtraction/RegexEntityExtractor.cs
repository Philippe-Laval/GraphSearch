using System.Text.RegularExpressions;
using GraphRag.NLP.Models;

namespace GraphRag.NLP.EntityExtraction
{
    /// <summary>
    /// A single regex-driven extraction rule.
    /// </summary>
    /// <param name="Type">Entity type assigned to matches.</param>
    /// <param name="Pattern">.NET regex pattern.</param>
    /// <param name="Confidence">Fixed confidence assigned to matches.</param>
    /// <param name="Group">Optional named or numbered capture group.</param>
    public sealed record RegexEntityRule(
        string Type,
        string Pattern,
        double Confidence = 0.9,
        string? Group = null);

    /// <summary>
    /// Pattern-based extractor for structured surface forms such as versions, dates,
    /// URLs, emails, SKUs, or custom identifiers.
    /// </summary>
    public sealed class RegexEntityExtractor : IEntityExtractor
    {
        private readonly IReadOnlyList<(Regex Regex, RegexEntityRule Rule)> _compiled;

        public RegexEntityExtractor(IEnumerable<RegexEntityRule> rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            _compiled = rules
                .Select(r => (
                    new Regex(
                        r.Pattern,
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled),
                    r))
                .ToList();
        }

        /// <summary>
        /// Convenience factory populated with a small default rule set.
        /// </summary>
        public static RegexEntityExtractor CreateDefault() => new(
        [
            new("Version", @"\b\d+(?:\.\d+){1,3}(?:-[A-Za-z0-9.]+)?\b", 0.85),
            new("Date",    @"\b\d{4}-\d{2}-\d{2}\b",                    0.95),
            new("Url",     @"\bhttps?://[^\s""'<>()]+",                   0.99),
            new("Email",   @"\b[\w.+-]+@[\w-]+(?:\.[\w-]+)+\b",          0.99),
        ]);

        public Task<IReadOnlyList<ExtractedEntity>> ExtractAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);
            cancellationToken.ThrowIfCancellationRequested();

            var accumulator = new List<(ExtractedEntity Entity, int Priority)>();

            foreach (var (regex, rule) in _compiled)
            {
                foreach (Match match in regex.Matches(text))
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
}
