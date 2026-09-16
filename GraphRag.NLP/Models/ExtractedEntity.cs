namespace GraphRag.NLP.Models
{
    /// <summary>
    /// Represents an entity extracted from text, including its text, type, span, and confidence score.
    /// </summary>
    /// <param name="Text">The extracted entity text as it appears in the source text.</param>
    /// <param name="Type">The extracted entity type, if known.</param>
    /// <param name="Start">The zero-based starting position of the entity in the source text.</param>
    /// <param name="Length">The length of the extracted entity text.</param>
    /// <param name="Confidence">The confidence score of the extraction in the interval [0.0 - 1.0].</param>
    public sealed record ExtractedEntity(
        string Text,
        string? Type,
        int Start,
        int Length,
        double Confidence);
}
