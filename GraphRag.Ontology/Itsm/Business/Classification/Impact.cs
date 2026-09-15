namespace GraphRag.Ontology.Itsm.Business.Classification;

/// <summary>
/// Impact modelled as a first-class concept rather than a simple enumeration.
/// </summary>
public sealed class Impact : ClassificationConcept
{
    /// <summary>
    /// Impact level
    /// </summary>
    public ImpactLevel Level { get; init; }
}
