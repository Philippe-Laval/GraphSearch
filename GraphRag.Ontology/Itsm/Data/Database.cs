namespace GraphRag.Ontology.Itsm.Data;

public sealed class Database : DataAsset
{
    public required string Name { get; init; }
    public Sql.SqlDialect? Dialect { get; init; }
    public IReadOnlyList<Schema> Schemas { get; init; } = [];
}
