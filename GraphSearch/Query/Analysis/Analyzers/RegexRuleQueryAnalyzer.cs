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

    /// <summary>
    /// Initialise une nouvelle instance de la classe <c>RegexRuleQueryAnalyzer</c> à partir d’un ensemble de règles
    /// d’intention et d’un analyseur de normalisation optionnel.
    /// </summary>
    /// <param name="rules">Règles d’intention utilisées pour créer et ordonner les expressions régulières compilées par priorité
    /// décroissante.</param>
    /// <param name="normalizer">Analyseur utilisé pour normaliser la requête avant l’évaluation des règles. Si <see langword="null" />, un
    /// <c>RulesBasedQueryAnalyzer</c> par défaut est utilisé avec la détection d’intention désactivée.</param>
    public RegexRuleQueryAnalyzer(
        IEnumerable<IntentRule> rules,
        IQueryAnalyzer? normalizer = null)
    {
        ArgumentNullException.ThrowIfNull(rules);

        // Compile regexes once, ordered by priority descending (highest first).
        _compiled = rules
            .OrderByDescending(r => r.Priority)
            .Select(r => (
                new Regex(
                    r.Pattern,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled),
                r))
            .ToList();

        // Use a default normalizer if none is provided.
        _normalizer = normalizer ?? new RulesBasedQueryAnalyzer(QueryAnalyzerOptions.Default with
        {
            // Disable intent detection in the normalizer, since we will classify intent ourselves.
            DetectIntent = false,
        });
    }

    /// <summary>
    /// Loads rules from a JSON document (see class summary for schema).
    /// </summary>
    /// <param name="json"></param>
    /// <param name="normalizer"></param>
    /// <returns></returns>
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
    
    /// <inheritdoc/>
    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // Normalize the query first.
        var baseAnalysis = await _normalizer.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);
        
        // Apply regex rules in order of priority.
        foreach (var (regex, rule) in _compiled)
        {
            // If the regex matches, return a new AnalyzedQuery with the matched intent and confidence.
            if (regex.IsMatch(baseAnalysis.NormalizedQuery))
            {
                return baseAnalysis with
                {
                    Intent = rule.Intent,
                    IntentConfidence = 0.9,
                };
            }
        }

        // If no regex matches, return a new AnalyzedQuery with a general intent and lower confidence.
        return baseAnalysis with
        {
            Intent = QueryIntent.General,
            IntentConfidence = 0.3,
        };
    }
}
