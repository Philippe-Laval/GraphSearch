namespace GraphRag.Ontology.Itsm.Business.Events;

public sealed class Reopen : OperationalEvent
{
    public string? Reason { get; init; }
}
