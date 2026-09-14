namespace GraphRag.Ontology.Itsm.Business.Events;

public sealed class StatusTransition : OperationalEvent
{
    public required Classification.Status FromStatus { get; init; }
    public required Classification.Status ToStatus { get; init; }
    public TimeSpan? TimeInFromStatus { get; init; }
}
