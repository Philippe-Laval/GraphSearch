namespace GraphRag.Ontology.Itsm.Business;

public sealed class ChangeRequest : Ticket
{
    public ChangeRiskLevel? RiskLevel { get; init; }

    public ChangeType? ChangeType { get; init; }
}
