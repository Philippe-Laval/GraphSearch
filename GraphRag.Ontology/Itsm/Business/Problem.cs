namespace GraphRag.Ontology.Itsm.Business;

public sealed class Problem : Ticket
{
    public IReadOnlyList<Incident> RelatedIncidents { get; init; } = [];

    public KnowledgeCase? KnownErrorDocumentedBy { get; init; }
}
