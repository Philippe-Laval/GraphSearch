namespace GraphRag.Ontology.Itsm.Business;

public sealed class Incident : Ticket
{
    /// <summary>Underlying root-cause problem, if identified.</summary>
    public Problem? CausedBy { get; init; }

    /// <summary>Change request that resolved this incident, if any.</summary>
    public ChangeRequest? ResolvedByChange { get; init; }
}
