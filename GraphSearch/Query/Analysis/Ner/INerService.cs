namespace GraphSearch.Library.Query.Analysis.Ner;

/// <summary>
/// Result of Named-Entity-Recognition over a query.
/// </summary>
/// <param name="Lemma">Lemmatized / base form of the whole query, if provided.</param>
/// <param name="Entities">Recognized entities (people, org, product, etc.).</param>
/// <param name="RootVerbs">Main verbs (lemmatized) - useful for relationship intent.</param>
/// <param name="InterrogativeLemma">Wh-word lemma if present ("what", "who", "why", "how many"...).</param>
public sealed record NerAnalysis(
    string? Lemma,
    IReadOnlyList<ExtractedEntity> Entities,
    IReadOnlyList<string> RootVerbs,
    string? InterrogativeLemma);

/// <summary>
/// Abstraction over an external NER pipeline (spaCy, HuggingFace via ONNX,
/// Azure AI Language, etc.).
/// </summary>
public interface INerService
{
    Task<NerAnalysis> AnalyzeAsync(
        string text,
        CancellationToken cancellationToken = default);
}
