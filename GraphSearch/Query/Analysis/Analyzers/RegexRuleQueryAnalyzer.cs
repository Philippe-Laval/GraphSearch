using System.Text.Json;
using System.Text.RegularExpressions;

namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// A single intent rule: if <see cref="Pattern"/> matches the normalized query,
/// classify as <see cref="Intent"/>.
/// </summary>
public sealed record IntentRule(QueryIntent Intent, string Pattern, int Priority = 0);

/// <summary>
/// Configuration-driven analyzer. Rules can be authored in JSON so domain teams
/// can tune intent classification without recompiling.
///
/// JSON schema:
///   [
///     { "intent": "Aggregation",  "pattern": "\\b(how many|count of)\\b", "priority": 10 },
///     { "intent": "Relationship", "pattern": "\\brelated to\\b",           "priority": 5  }
///   ]
///
/// Highest-priority match wins; ties broken by declaration order.
/// </summary>
public sealed class RegexRuleQueryAnalyzer : IQueryAnalyzer
{
    private readonly IReadOnlyList<(Regex Regex, IntentRule Rule)> _compiled;
    private readonly IQueryAnalyzer _normalizer;

    public RegexRuleQueryAnalyzer(
        IEnumerable<IntentRule> rules,
        IQueryAnalyzer? normalizer = null)
    {
        ArgumentNullException.ThrowIfNull(rules);

        _compiled = rules
            .OrderByDescending(r => r.Priority)
            .Select(r => (
                new Regex(
                    r.Pattern,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled),
                r))
            .ToList();

        _normalizer = normalizer ?? new RulesBasedQueryAnalyzer(QueryAnalyzerOptions.Default with
        {
            DetectIntent = false,
        });
    }

    /// <summary>
    /// Loads rules from a JSON document (see class summary for schema).
    /// </summary>
    public static RegexRuleQueryAnalyzer FromJson(string json, IQueryAnalyzer? normalizer = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var rules = JsonSerializer.Deserialize<List<IntentRule>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
            }) ?? new List<IntentRule>();

        return new RegexRuleQueryAnalyzer(rules, normalizer);
    }

    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var baseAnalysis = await _normalizer.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);

        foreach (var (regex, rule) in _compiled)
        {
            if (regex.IsMatch(baseAnalysis.NormalizedQuery))
            {
                return baseAnalysis with
                {
                    Intent = rule.Intent,
                    IntentConfidence = 0.9,
                };
            }
        }

        return baseAnalysis with
        {
            Intent = QueryIntent.General,
            IntentConfidence = 0.3,
        };
    }
}
