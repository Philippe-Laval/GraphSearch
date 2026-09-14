using GraphRag.Ontology.Itsm.Linguistic;

namespace GraphRag.Conceptual.Entities;

/// <summary>
/// Linguistic layer: a natural-language surface form that maps to a concept.
/// Context is captured because synonyms are context-dependent
/// ("sales" → Revenue vs SalesOrder vs SalesDepartment vs UnitsSold).
/// </summary>
public class Term : ConceptualEntity
{
    public string SurfaceForm { get; set; } = string.Empty;

    public Guid ConceptId { get; set; }
    public Concept Concept { get; set; } = null!;

    public TermKind Kind { get; set; }

    /// <summary>ISO 639-1 language code (e.g. "en", "fr"). Nullable = language-agnostic.</summary>
    public string? Language { get; set; }

    /// <summary>Context in which this term applies (e.g. "Finance", "Support tier 1").</summary>
    public string? Context { get; set; }

    /// <summary>
    /// Optional pre-computed vector embedding, stored as bytes to remain provider-agnostic.
    /// Postgres can override with pgvector, others can store raw floats.
    /// </summary>
    public byte[]? Embedding { get; set; }
}
