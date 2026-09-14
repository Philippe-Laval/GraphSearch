namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// A team that owns tickets (aka support group / resolution group).
/// </summary>
public class AssignmentGroup : Party
{
    public string? Department { get; init; }
    public IReadOnlyList<Service.Service> SupportedServices { get; init; } = [];
}
