namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// Specialization of AssignmentGroup used specifically to denote L1/L2/L3 support teams.
/// </summary>
public sealed class SupportTeam : AssignmentGroup
{
    /// <summary>
    /// Niveau de support associé au support teams (L1/L2/L3).
    /// </summary>
    public string? SupportTier { get; init; }
}
