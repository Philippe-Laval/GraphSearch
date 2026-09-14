using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// Any human or organizational actor involved in ITSM operations.
/// </summary>
public abstract class Party : BusinessConcept
{
    public Organization? Organization { get; init; }
}
