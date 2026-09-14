namespace GraphRag.Ontology.Itsm.Business.Events;

public sealed class AssignmentEvent : OperationalEvent
{
    public Party.AssignmentGroup? FromGroup { get; init; }
    public Party.AssignmentGroup? ToGroup { get; init; }
    public Party.Agent? FromAgent { get; init; }
    public Party.Agent? ToAgent { get; init; }
}
