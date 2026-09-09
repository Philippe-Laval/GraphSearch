namespace GraphSearch.Library.Query.Analysis.Ner;

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
