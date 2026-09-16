namespace GraphRag.NLP.Models
{
    /// <summary>
    /// Defines a known entity label and its type for dictionary- or gazetteer-based extraction.
    /// </summary>
    /// <param name="Name">The entity surface form.</param>
    /// <param name="Type">The entity type.</param>
    public sealed record EntityDefinition(
        string Name,
        string Type);
}
