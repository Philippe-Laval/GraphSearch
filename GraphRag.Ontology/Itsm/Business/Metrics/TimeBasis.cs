namespace GraphRag.Ontology.Itsm.Business.Metrics;

/// <summary>
/// Représente la base temporelle utilisée pour interpréter les durées et les échéances.
/// </summary>
/// <remarks>Utiliser <see cref="TimeBasis.WallClock"/> pour le temps calendaire continu et <see
/// cref="TimeBasis.BusinessHours"/> pour le temps limité aux heures ouvrées.</remarks>
public enum TimeBasis
{ 
    /// <summary>
    /// Represents wall-clock time measured from the system clock.
    /// </summary>
    /// <remarks>Uses real elapsed time rather than simulated or process CPU time.</remarks>
    WallClock, 
    /// <summary>
    /// Représente les heures d’ouverture d’une activité.
    /// </summary>
    BusinessHours 
}
