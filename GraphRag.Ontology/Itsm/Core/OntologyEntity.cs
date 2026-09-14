namespace GraphRag.Ontology.Itsm.Core;

/// <summary>
/// Base type for every element of the ontology (concept, relation, individual, mapping...).
/// Carries identity, human-readable labels and provenance so that assertions can be traced
/// back to their source (DBA, documentation, inferred, ...).
/// </summary>
public abstract class OntologyEntity
{
    public required string Id { get; init; }

    public string? PreferredLabel { get; init; }

    public string? Description { get; init; }

    public IReadOnlyList<string> AlternativeLabels { get; init; } = [];

    public Provenance? Provenance { get; init; }

    public string? Version { get; init; }
}
