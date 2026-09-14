namespace GraphRag.Ontology.Itsm.Core;

/// <summary>
/// Root of the ITSM Text-to-SQL business ontology.
/// Everything a user might naturally refer to (Incident, User, Service, Priority, MTTR...)
/// derives from <see cref="BusinessConcept"/>.
/// </summary>
public abstract class BusinessConcept : OntologyEntity
{
    /// <summary>
    /// Physical data assets that represent this concept in one or more databases.
    /// A single concept may be represented by several tables/columns/views.
    /// </summary>
    public IReadOnlyList<Mapping.ConceptRepresentation> RepresentedBy { get; init; } = [];
}
