namespace GraphRag.Ontology.Itsm.Business;

/// <summary>
/// Represents a change request
/// </summary>
public sealed class ChangeRequest : Ticket
{
    /// <summary>
    /// The risk level of the change request.
    /// </summary>
    public ChangeRiskLevel? RiskLevel { get; init; }

    /// <summary>
    /// The type of the change request.
    /// </summary>
    public ChangeType? ChangeType { get; init; }
}
