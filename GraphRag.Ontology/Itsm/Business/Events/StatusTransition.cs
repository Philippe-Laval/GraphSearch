namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Représente un événement opérationnel où un élément passe d’un statut à un autre.
/// </summary>
/// <remarks>Inclut le statut d’origine, le statut cible et, lorsqu’elle est connue, la durée passée dans le
/// statut d’origine.</remarks>
public sealed class StatusTransition : OperationalEvent
{
    /// <summary>
    /// Statut d’origine.
    /// </summary>
    public required Classification.Status FromStatus { get; init; }
    /// <summary>
    /// Statut cible.
    /// </summary>
    public required Classification.Status ToStatus { get; init; }
    /// <summary>
    /// Durée passée dans le statut d’origine.
    /// </summary>
    public TimeSpan? TimeInFromStatus { get; init; }
}
