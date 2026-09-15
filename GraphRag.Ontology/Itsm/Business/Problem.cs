namespace GraphRag.Ontology.Itsm.Business;

/// <summary>
/// Problem
/// </summary>
public sealed class Problem : Ticket
{
    /// <summary>
    /// The list of related incidents.
    /// </summary>
    public IReadOnlyList<Incident> RelatedIncidents { get; init; } = [];

    /// <summary>
    /// Obtient le dossier de connaissance qui documente l’erreur connue.
    /// </summary>
    public KnowledgeCase? KnownErrorDocumentedBy { get; init; }
}
