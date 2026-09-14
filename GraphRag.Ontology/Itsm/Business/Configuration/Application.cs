namespace GraphRag.Ontology.Itsm.Business.Configuration;

public sealed class Application : ConfigurationItem
{
    public string? Vendor { get; init; }
    public string? VersionNumber { get; init; }
}
