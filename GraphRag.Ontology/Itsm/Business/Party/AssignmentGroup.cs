namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// A team that owns tickets (aka support group / resolution group).
/// </summary>
public class AssignmentGroup : Party
{
    /// <summary>
    /// Department of the assignment group
    /// </summary>
    public string? Department { get; init; }

    /// <summary>
    /// List of supported services
    /// </summary>
    public IReadOnlyList<Service.Service> SupportedServices { get; init; } = [];
}
