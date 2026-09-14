namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// Support employee working on tickets.
/// </summary>
public class Agent : User
{
    public IReadOnlyList<AssignmentGroup> Groups { get; init; } = [];
}
