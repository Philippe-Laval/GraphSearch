namespace GraphRag.Conceptual.Entities;

/// <summary>Broad classification of a <see cref="Concept"/>.</summary>
public enum ConceptKind
{
    // Core taxonomy from the ontology document.
    Entity,
    Attribute,
    Metric,
    Dimension,
    Event,

    // ITSM specializations.
    WorkItem,
    Ticket,
    Party,
    Service,
    ConfigurationItem,
    Classification,
    TimeConcept,
    Sla,
}

/// <summary>
/// A business/conceptual concept in the ontology (e.g. "Incident", "Priority", "MTTR").
/// Stored as a single flat table with a <see cref="Kind"/> discriminator because
/// a conceptual metamodel needs to remain open and extensible.
/// </summary>
public class Concept : ConceptualEntity
{
    public ConceptKind Kind { get; set; }

    /// <summary>Domain grouping (e.g. "ITSM", "CRM", "Finance"). Enables per-domain vocabularies.</summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>Optional parent for a subclassOf-style hierarchy (e.g. Incident subclassOf Ticket).</summary>
    public Guid? ParentConceptId { get; set; }
    public Concept? ParentConcept { get; set; }
    public ICollection<Concept> Children { get; set; } = [];

    /// <summary>Human-friendly comma-separated synonyms (durable, quick to full-text search).</summary>
    public string? SynonymsInline { get; set; }

    public bool IsDeprecated { get; set; }
    public string? DeprecationNote { get; set; }

    public ICollection<Term> Terms { get; set; } = [];
    public ICollection<ConceptRepresentation> Representations { get; set; } = [];
    public ICollection<ConceptRelation> OutgoingRelations { get; set; } = [];
    public ICollection<ConceptRelation> IncomingRelations { get; set; } = [];
}

/// <summary>Directed relationship between concepts (e.g. Ticket--affects-->ConfigurationItem).</summary>
public class ConceptRelation : ConceptualEntity
{
    public Guid SourceConceptId { get; set; }
    public Concept SourceConcept { get; set; } = null!;

    public Guid TargetConceptId { get; set; }
    public Concept TargetConcept { get; set; } = null!;

    /// <summary>Predicate name — e.g. "affects", "reportedBy", "governedBy", "derivedFrom".</summary>
    public string Predicate { get; set; } = string.Empty;

    public RelationCardinality Cardinality { get; set; }
    public bool IsInverseFunctional { get; set; }
}

public enum RelationCardinality { OneToOne, OneToMany, ManyToOne, ManyToMany }
