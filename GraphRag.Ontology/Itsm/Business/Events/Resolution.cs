namespace GraphRag.Ontology.Itsm.Business.Events;

public sealed class Resolution : OperationalEvent
{
    public string? ResolutionCode { get; init; }
    public string? ResolutionNotes { get; init; }
}
