namespace GraphRag.Ontology.Itsm.Business.Configuration;

public sealed class DatabaseCI : ConfigurationItem
{
    public string? Engine { get; init; }
    public string? InstanceName { get; init; }
}
