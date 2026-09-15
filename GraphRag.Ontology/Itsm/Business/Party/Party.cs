using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// Abstract class : Any human or organizational actor involved in ITSM operations.
/// </summary>
public abstract class Party : BusinessConcept
{
    /// <summary>
    /// 
    /// </summary>
    public Organization? Organization { get; init; }
}
