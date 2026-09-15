namespace GraphRag.Ontology.Itsm.Core;

/// <summary>
/// Provenance metadata for an ontology assertion.
/// </summary>
public sealed class Provenance
{
    /// <summary>
    /// Source
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// Confidence of the source
    /// </summary>
    public double? Confidence { get; init; }

    /// <summary>
    /// Date of the addition of the assertion in the ontology.
    /// This is not necessarily the date of the source, but rather when it was asserted in the ontology.
    /// </summary>
    public DateTimeOffset? AssertedAt { get; init; }

    /// <summary>
    /// Person who asserted the provenance
    /// </summary>
    public string? AssertedBy { get; init; }
}
