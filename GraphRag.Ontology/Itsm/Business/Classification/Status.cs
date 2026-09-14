namespace GraphRag.Ontology.Itsm.Business.Classification;

public sealed class Status : ClassificationConcept
{
    public StatusKind Kind { get; init; }
    public bool IsTerminal { get; init; }
}
