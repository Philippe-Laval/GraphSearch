namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Représente un événement opérationnel décrivant la résolution appliquée, y compris le code de résolution et les notes
/// associées.
/// </summary>
public sealed class Resolution : OperationalEvent
{
    /// <summary>
    /// Gets the resolution code.
    /// </summary>
    public string? ResolutionCode { get; init; }

    /// <summary>
    /// Notes décrivant la résolution apportée.
    /// </summary>
    public string? ResolutionNotes { get; init; }
}
