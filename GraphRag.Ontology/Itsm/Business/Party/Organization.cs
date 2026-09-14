namespace GraphRag.Ontology.Itsm.Business.Party;

public class Organization : Party
{
    public string? BusinessUnit { get; init; }
    public Organization? Parent { get; init; }
}
