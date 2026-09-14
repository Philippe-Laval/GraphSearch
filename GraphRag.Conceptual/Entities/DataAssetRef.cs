using GraphRag.Ontology.Itsm.Sql;

namespace GraphRag.Conceptual.Entities;

public enum DataAssetKind { Database, Schema, Table, View, Column, ForeignKey, Index }

/// <summary>
/// Reference to a physical data element known to the ontology (not the element itself —
/// that lives in the source database). Uses a fully-qualified name so it survives
/// disconnected environments and multi-database catalogs.
/// </summary>
public class DataAssetRef : ConceptualEntity
{
    public DataAssetKind Kind { get; set; }

    /// <summary>Fully-qualified name, e.g. "crm.dbo.ticket.customer_id".</summary>
    public string FullyQualifiedName { get; set; } = string.Empty;

    public string? DatabaseName { get; set; }
    public string? SchemaName { get; set; }
    public string? TableName { get; set; }
    public string? ColumnName { get; set; }
    public string? DataType { get; set; }

    public SqlDialect? Dialect { get; set; }

    /// <summary>Grain description for tables (e.g. "one row per Ticket").</summary>
    public string? Grain { get; set; }

    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsDeprecated { get; set; }
    public long? ApproximateRowCount { get; set; }

    public Guid? ParentAssetId { get; set; }
    public DataAssetRef? ParentAsset { get; set; }
    public ICollection<DataAssetRef> Children { get; set; } = [];
}
