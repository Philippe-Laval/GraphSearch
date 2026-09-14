namespace GraphRag.Ontology.Itsm.Business.Classification;

/// <summary>
/// Priority is derived from impact and urgency according to a business-specific matrix.
/// Kept as a distinct concept so questions such as "critical incidents" can be grounded
/// on Priority = P1 or Severity = Critical depending on the organization's vocabulary.
/// </summary>
public sealed class Priority : ClassificationConcept
{
    public PriorityLevel Level { get; init; }
    public Impact? DerivedFromImpact { get; init; }
    public Urgency? DerivedFromUrgency { get; init; }
}
