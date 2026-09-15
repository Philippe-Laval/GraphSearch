using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Linguistic;

/// <summary>
/// Linguistic layer. A "term" is a surface form that a user might type,
/// and it maps to a concept with a confidence score. Context is captured
/// because synonyms are usually context-dependent ("sales" -> Revenue,
/// SalesOrder, SalesDepartment or UnitsSold).
/// </summary>
public sealed class Term : OntologyEntity
{
    /// <summary>
    /// Visible name
    /// </summary>
    public required string SurfaceForm { get; init; }

    /// <summary>
    /// The business concept mapped by the term
    /// </summary>
    public required BusinessConcept MapsTo { get; init; }

    /// <summary>
    /// Confidence
    /// </summary>
    public double Confidence { get; init; }

    /// <summary>
    /// Context of the term (like "ITSM")
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Kind of term
    /// </summary>
    public TermKind Kind { get; init; }

    /// <summary>
    /// Langage of the term ("en", "fr")
    /// </summary>
    public string? Language { get; init; }
}
