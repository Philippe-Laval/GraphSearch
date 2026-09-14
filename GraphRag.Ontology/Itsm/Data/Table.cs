namespace GraphRag.Ontology.Itsm.Data;

public class Table : DataAsset
{
    public required string Name { get; init; }
    public string? Grain { get; init; }
    public IReadOnlyList<Column> Columns { get; init; } = [];
    public IReadOnlyList<Column> PrimaryKey { get; init; } = [];
    public IReadOnlyList<ForeignKey> ForeignKeys { get; init; } = [];
    public IReadOnlyList<Index> Indexes { get; init; } = [];
    public long? ApproximateRowCount { get; init; }
}