using GraphRag.Ontology.Itsm.Core;
using GraphRag.Ontology.Itsm.Data;

namespace GraphRag.Ontology.Itsm.Mapping;

/// <summary>
/// Join semantics between two tables. Goes beyond FK constraints:
/// captures preferred join paths and cases with no declared FK.
/// </summary>
public sealed class JoinRelationship : OntologyEntity
{
    public required Table LeftTable { get; init; }
    public required Table RightTable { get; init; }
    public required string JoinCondition { get; init; }
    public JoinCardinality Cardinality { get; init; }
    public bool IsPreferred { get; init; }
    public bool RequiresForeignKey { get; init; }
}
