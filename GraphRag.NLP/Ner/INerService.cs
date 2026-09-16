namespace GraphRag.NLP.Ner
{
    /// <summary>
    /// Abstraction over a named-entity recognition pipeline.
    /// </summary>
    public interface INerService
    {
        Task<NerAnalysis> AnalyzeAsync(
            string text,
            CancellationToken cancellationToken = default);
    }
}
