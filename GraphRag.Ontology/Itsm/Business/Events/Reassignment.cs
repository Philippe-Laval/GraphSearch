namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Représente un événement opérationnel indiquant qu’une affectation a été réattribuée.
/// </summary>
/// <remarks>Peut inclure une justification de la réattribution via la propriété <c>Reason</c>.</remarks>
public sealed class Reassignment : OperationalEvent
{
    /// <summary>
    /// Justification de la réattribution.
    /// </summary>
    public string? Reason { get; init; }
}
