namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Represents an operational event in which an issue is escalated to a higher support tier.
/// </summary>
/// <remarks>Includes optional details about the escalation reason and the new tier.</remarks>
public sealed class Escalation : OperationalEvent
{
    /// <summary>
    /// Escalation reason
    /// </summary>
    public string? EscalationReason { get; init; }
    /// <summary>
    /// New rier (L1,L2,L3)
    /// </summary>
    public string? NewTier { get; init; }
}
