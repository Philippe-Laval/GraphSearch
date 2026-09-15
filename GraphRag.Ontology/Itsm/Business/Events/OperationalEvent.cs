using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Events;

/// <summary>
/// Abstract class : Any event happening during a ticket's lifecycle.
/// Modelling these explicitly enables queries such as
/// "How many tickets were in Pending state during August?".
/// </summary>
public abstract class OperationalEvent : BusinessConcept
{
    public required DateTimeOffset EventTimestamp { get; init; }
    public Party.Agent? PerformedBy { get; init; }
    public string? Comment { get; init; }
}
