using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Sql;

/// <summary>Any SQL-level artifact.</summary>
public abstract class SqlConcept : OntologyEntity
{
    public SqlDialect Dialect { get; init; }
}
