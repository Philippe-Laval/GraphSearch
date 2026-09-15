namespace GraphRag.Conceptual.Entities;

/// <summary>
/// Abstract class : Common shape for every persisted ontology element: identity, provenance, audit, versioning.
/// </summary>
public abstract class ConceptualEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Stable IRI-like key ("itsm:Incident", "metric:MTTR", "col:crm.ticket.id").</summary>
    public string Uri { get; set; } = string.Empty;

    public string? PreferredLabel { get; set; }
    public string? Description { get; set; }

    /// <summary>Semantic version of this assertion (e.g. "1.2.0").</summary>
    public string? OntologyVersion { get; set; }

    // Provenance.
    public string? Source { get; set; }
    public double? Confidence { get; set; }
    public string? AssertedBy { get; set; }
    public DateTimeOffset? AssertedAt { get; set; }

    // Audit.
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }
}
