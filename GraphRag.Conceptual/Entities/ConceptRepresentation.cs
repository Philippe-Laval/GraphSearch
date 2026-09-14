using GraphRag.Ontology.Itsm.Mapping;

namespace GraphRag.Conceptual.Entities;

/// <summary>
/// The bridge: maps a <see cref="Concept"/> to a physical <see cref="DataAssetRef"/>
/// with an optional filter clause. This is the "Incident represented by
/// ticket WHERE ticket_type = 'incident'" case that motivated the whole library.
/// </summary>
public class ConceptRepresentation : ConceptualEntity
{
    public Guid ConceptId { get; set; }
    public Concept Concept { get; set; } = null!;

    public Guid AssetId { get; set; }
    public DataAssetRef Asset { get; set; } = null!;

    public MappingKind Kind { get; set; }

    /// <summary>Optional SQL-agnostic filter narrowing the asset to the concept.</summary>
    public string? FilterExpression { get; set; }

    /// <summary>Notes on why this mapping exists (e.g. "documented by CRM team").</summary>
    public string? Rationale { get; set; }

    public bool IsPreferred { get; set; }
}

/// <summary>
/// Join semantics between two tables — richer than FK metadata:
/// captures preferred join path, valid non-FK joins, and cardinality.
/// </summary>
public class JoinRelationship : ConceptualEntity
{
    public Guid LeftTableId { get; set; }
    public DataAssetRef LeftTable { get; set; } = null!;

    public Guid RightTableId { get; set; }
    public DataAssetRef RightTable { get; set; } = null!;

    /// <summary>SQL-like join condition (e.g. "orders.customer_id = customers.id").</summary>
    public string JoinCondition { get; set; } = string.Empty;

    public JoinCardinality Cardinality { get; set; }
    public bool IsPreferred { get; set; }

    /// <summary>False when no declared FK exists but the join is still semantically valid.</summary>
    public bool BackedByForeignKey { get; set; }
}
