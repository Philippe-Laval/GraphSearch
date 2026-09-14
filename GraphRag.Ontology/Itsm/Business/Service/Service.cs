using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Service;

/// <summary>Anything the IT organization offers, at any abstraction level.</summary>
public abstract class Service : BusinessConcept
{
    public Party.Party? Owner { get; init; }
    public IReadOnlyList<Party.AssignmentGroup> SupportedBy { get; init; } = [];
}
