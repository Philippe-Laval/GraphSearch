using GraphRag.NLP.Models;

namespace GraphRag.NLP.Ner
{
    /// <summary>
    /// Result of named-entity recognition over a text input.
    /// </summary>
    /// <param name="Lemma">Lemmatized or base form of the whole text, if provided.</param>
    /// <param name="Entities">Recognized entities.</param>
    /// <param name="RootVerbs">Main verbs, useful for relationship intent.</param>
    /// <param name="InterrogativeLemma">Detected wh-word lemma, if present.</param>
    public sealed record NerAnalysis(
        string? Lemma,
        IReadOnlyList<ExtractedEntity> Entities,
        IReadOnlyList<string> RootVerbs,
        string? InterrogativeLemma);
}
