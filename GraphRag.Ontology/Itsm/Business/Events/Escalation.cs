namespace GraphRag.Ontology.Itsm.Business.Events;

public sealed class Escalation : OperationalEvent
{
    public string? EscalationReason { get; init; }
    public string? NewTier { get; init; }
}
