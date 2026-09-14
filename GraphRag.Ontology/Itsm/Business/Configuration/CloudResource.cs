namespace GraphRag.Ontology.Itsm.Business.Configuration;

public sealed class CloudResource : ConfigurationItem
{
    public string? Provider { get; init; }
    public string? Region { get; init; }
    public string? ResourceType { get; init; }
}
