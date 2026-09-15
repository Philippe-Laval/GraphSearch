namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Représente un événement opérationnel indiquant qu’un élément est rouvert.
/// </summary>
/// <remarks>Inclut éventuellement un motif de réouverture.</remarks>
public sealed class Reopen : OperationalEvent
{
    /// <summary>
    /// Motif de réouverture.
    /// </summary>
    public string? Reason { get; init; }
}
