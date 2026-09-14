namespace GraphRag.Ontology.Itsm.Core;

/// <summary>
/// Provenance metadata for an ontology assertion.
/// </summary>
public sealed class Provenance
{
    public required string Source { get; init; }

    public double? Confidence { get; init; }

    public DateTimeOffset? AssertedAt { get; init; }

    public string? AssertedBy { get; init; }
}
