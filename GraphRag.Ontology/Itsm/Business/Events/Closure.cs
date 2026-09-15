namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Représente un événement opérationnel de fermeture.
/// </summary>
/// <remarks>Inclut un code de fermeture optionnel pour qualifier la fermeture.</remarks>
public sealed class Closure : OperationalEvent
{
    /// <summary>
    /// Code de clôture.
    /// </summary>
    public string? ClosureCode { get; init; }
}
