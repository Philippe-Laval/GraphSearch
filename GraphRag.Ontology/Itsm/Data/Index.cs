namespace GraphRag.Ontology.Itsm.Data;

public sealed class Index : DataAsset
{
    public required string Name { get; init; }
    public required IReadOnlyList<Column> Columns { get; init; }
    public bool IsUnique { get; init; }
}
