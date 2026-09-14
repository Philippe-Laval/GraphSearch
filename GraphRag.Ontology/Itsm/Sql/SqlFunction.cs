namespace GraphRag.Ontology.Itsm.Sql;

/// <summary>
/// A dialect-specific SQL function name.
/// </summary>
public sealed class SqlFunction : SqlConcept
{
    public required string Name { get; init; }
    public IReadOnlyList<string> ArgumentTypes { get; init; } = [];
    public string? ReturnType { get; init; }
}
