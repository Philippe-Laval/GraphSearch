namespace GraphRag.Ontology.Itsm.Data;

/// <summary>
/// Sealed class : Represents a database schema.
/// </summary>
public sealed class Schema : DataAsset
{
    public required string Name { get; init; }
    public IReadOnlyList<Table> Tables { get; init; } = [];
    public IReadOnlyList<View> Views { get; init; } = [];
}
