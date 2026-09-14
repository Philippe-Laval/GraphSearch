namespace GraphRag.Ontology.Itsm.Business.Events;

public sealed class Reassignment : OperationalEvent
{
    public string? Reason { get; init; }
}
