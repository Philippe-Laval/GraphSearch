namespace GraphSearch.Library.Query.Analysis;

/*
For example:

   "What is .NET?"
          ↓
   EntityLookup

   "What is the relationship between Microsoft and .NET?"
          ↓
   Relationship

   "Which products developed by Microsoft run on Linux?"
          ↓
   MultiHopRelationship
 */

/// <summary>
/// AnalyzedQuery represents the result of analyzing a user's query,
/// including the original query, its normalized form, and the inferred intent.
///
/// Extended (optional) fields let richer analyzers (LLM, rewriter, multilingual,
/// graph-pattern) add data without breaking existing producers/consumers.
/// </summary>
/// <param name="OriginalQuery">The original query submitted by the user.</param>
/// <param name="NormalizedQuery">The normalized form of the query.</param>
/// <param name="Intent">The inferred intent of the query.</param>
public sealed record AnalyzedQuery(
    string OriginalQuery,
    string NormalizedQuery,
    QueryIntent Intent)
{
    /// <summary>Alternative phrasings for hybrid retrieval (added by rewriters).</summary>
    public IReadOnlyList<string> Rewrites { get; init; } = [];

    /// <summary>Detected language code (BCP-47, e.g. "en", "fr"). Null if unknown.</summary>
    public string? DetectedLanguage { get; init; }

    /// <summary>
    /// Logical graph pattern extracted from the query (added by
    /// <see cref="GraphPatternQueryAnalyzer"/>). Null when not produced.
    /// </summary>
    public GraphQueryPattern? GraphPattern { get; init; }

    /// <summary>Confidence in the inferred <see cref="Intent"/>, in [0, 1].</summary>
    public double IntentConfidence { get; init; } = 1.0;
}
