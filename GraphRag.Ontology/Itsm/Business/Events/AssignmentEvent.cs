namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Represents an operational event that records assignment changes between groups and agents.
/// </summary>
/// <remarks>Captures the previous and new assignment group and agent when available.</remarks>
public sealed class AssignmentEvent : OperationalEvent
{
    /// <summary>
    /// Groupe d’affectation d’origine.
    /// </summary>
    public Party.AssignmentGroup? FromGroup { get; init; }
    
    /// <summary>
    /// Groupe d’affectation destinataire.
    /// </summary>
    public Party.AssignmentGroup? ToGroup { get; init; }
    
    /// <summary>
    /// Agent d’origine.
    /// </summary>
    public Party.Agent? FromAgent { get; init; }

    /// <summary>
    /// Agent destinataire.
    /// </summary>
    public Party.Agent? ToAgent { get; init; }
}
