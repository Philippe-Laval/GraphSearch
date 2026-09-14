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
    public required string SurfaceForm { get; init; }
    public required BusinessConcept MapsTo { get; init; }
    public double Confidence { get; init; }
    public string? Context { get; init; }
    public TermKind Kind { get; init; }
    public string? Language { get; init; }
}
