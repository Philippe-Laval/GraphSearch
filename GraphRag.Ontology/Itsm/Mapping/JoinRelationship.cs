using GraphRag.Ontology.Itsm.Core;
using GraphRag.Ontology.Itsm.Data;

namespace GraphRag.Ontology.Itsm.Mapping;

/// <summary>
/// Join semantics between two tables. Goes beyond FK constraints:
/// captures preferred join paths and cases with no declared FK.
/// </summary>
public sealed class JoinRelationship : OntologyEntity
{
    /// <summary>
    /// Table de gauche utilisée par l’opération.
    /// </summary>
    /// <remarks>Valeur obligatoire lors de l’initialisation.</remarks>
    public required Table LeftTable { get; init; }
    /// <summary>
    /// Gets the table on the right side of the operation.
    /// </summary>
    /// <remarks>Required member that must be initialized during object creation.</remarks>
    public required Table RightTable { get; init; }
    /// <summary>
    /// Expression that defines how records are matched between joined data sources.
    /// </summary>
    public required string JoinCondition { get; init; }
    /// <summary>
    /// Obtient la cardinalité de la jointure.
    /// </summary>
    public JoinCardinality Cardinality { get; init; }
    /// <summary>
    /// Indique si l’élément est défini comme préféré.
    /// </summary>
    public bool IsPreferred { get; init; }
    /// <summary>
    /// Indique si une clé étrangère est requise.
    /// </summary>
    public bool RequiresForeignKey { get; init; }
}
