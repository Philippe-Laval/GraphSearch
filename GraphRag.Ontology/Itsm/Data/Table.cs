namespace GraphRag.Ontology.Itsm.Data;

/// <summary>
/// Représente une table de données avec son nom, sa granularité et ses métadonnées relationnelles. 
/// </summary>
/// <remarks>Inclut la définition des colonnes, de la clé primaire, des clés étrangères, des index et une
/// estimation optionnelle du nombre de lignes.</remarks>
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