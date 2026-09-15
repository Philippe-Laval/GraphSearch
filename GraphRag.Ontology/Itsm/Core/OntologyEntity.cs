namespace GraphRag.Ontology.Itsm.Core;

/// <summary>
/// Base type for every element of the ontology (concept, relation, individual, mapping...).
/// Carries identity, human-readable labels and provenance so that assertions can be traced
/// back to their source (DBA, documentation, inferred, ...).
/// </summary>
public abstract class OntologyEntity
{
    /// <summary>
    /// Identifier of the ontology entity
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Preferred Label
    /// </summary>
    public string? PreferredLabel { get; init; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Alternative Labels
    /// </summary>
    public IReadOnlyList<string> AlternativeLabels { get; init; } = [];

    /// <summary>
    /// Provenance metadata for an ontology assertion.
    /// </summary>
    public Provenance? Provenance { get; init; }

    /// <summary>
    /// Version of the entity
    /// </summary>
    public string? Version { get; init; }
}
