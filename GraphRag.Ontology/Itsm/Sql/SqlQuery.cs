namespace GraphRag.Ontology.Itsm.Sql;

/// <summary>
/// Concrete SQL implementation of a semantic <see cref="SemanticQuery"/>.
/// </summary>
public sealed class SqlQuery : SqlConcept
{
    public required string Sql { get; init; }
    public IReadOnlyList<string> ReferencedTables { get; init; } = [];
    public IReadOnlyList<string> ReferencedColumns { get; init; } = [];
    public string? ExplainPlan { get; init; }
}
