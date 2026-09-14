namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>End-user of a service (may or may not raise tickets themselves).</summary>
public class User : Party
{
    public string? Email { get; init; }
    public string? Department { get; init; }
    public string? Location { get; init; }
}
