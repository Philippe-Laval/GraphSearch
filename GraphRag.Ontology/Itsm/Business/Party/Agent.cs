namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// Support employee working on tickets.
/// </summary>
public class Agent : User
{
    /// <summary>
    /// Obtient la collection en lecture seule des groupes d’affectation.
    /// </summary>
    /// <remarks>Initialisée à une liste vide.</remarks>
    public IReadOnlyList<AssignmentGroup> Groups { get; init; } = [];
}
