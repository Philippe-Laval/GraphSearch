namespace GraphRag.Ontology.Itsm.Business.Classification;

/// <summary>
/// Priority Level (P1 | P2 | P3 | P4 | P5)
/// </summary>
public enum PriorityLevel {
    /// <summary>
    /// Critical - Critique
    /// Impact : Extensive
    /// </summary>
    P1,
    /// <summary>
    /// High - Haute
    /// Impact : Large
    /// </summary>
    P2,
    /// <summary>
    /// Medium - Moyenne
    /// Impact : Moderate
    /// </summary>
    P3,
    /// <summary>
    /// Low - Basse
    /// Impact : Minor
    /// </summary>
    P4,
    /// <summary>
    /// Planning - Planification
    /// Impact : Negligible
    /// </summary>
    P5
}
