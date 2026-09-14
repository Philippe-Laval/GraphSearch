namespace GraphRag.Ontology.Itsm.Data;

public sealed class ForeignKey : DataAsset
{
    public required IReadOnlyList<Column> Columns { get; init; }
    public required IReadOnlyList<Column> ReferencedColumns { get; init; }
    public string? ConstraintName { get; init; }
}
