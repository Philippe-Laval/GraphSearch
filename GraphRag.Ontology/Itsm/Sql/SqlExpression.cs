namespace GraphRag.Ontology.Itsm.Sql;

/// <summary>
/// A raw SQL expression fragment (e.g. "SUM(o.net_amount)").
/// </summary>
public sealed class SqlExpression : SqlConcept
{
    public required string Expression { get; init; }
}
