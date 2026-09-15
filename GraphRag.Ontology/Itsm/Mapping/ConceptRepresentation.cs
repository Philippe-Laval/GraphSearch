using GraphRag.Ontology.Itsm.Core;
using GraphRag.Ontology.Itsm.Data;

namespace GraphRag.Ontology.Itsm.Mapping;

/// <summary>
/// Maps a <see cref="BusinessConcept"/> to a concrete <see cref="DataAsset"/>
/// (table, view, column) with an optional filter clause.
/// Example: Incident represented by table "ticket" WHERE ticket_type = 'incident'.
/// </summary>
public sealed class ConceptRepresentation : OntologyEntity
{
    public required DataAsset Asset { get; init; }

    /// <summary>
    /// Optional SQL-agnostic filter that narrows the asset down to the concept.
    /// </summary>
    public string? FilterExpression { get; init; }

    public MappingKind Kind { get; init; }

    /// <summary>
    /// Confidence score for the mapping. Useful for inferred mappings.
    /// </summary>
    public double? Confidence { get; init; }
}