namespace GraphRag.Ontology.Itsm.Business.Configuration;

public sealed class Server : ConfigurationItem
{
    public string? Hostname { get; init; }
    public string? OperatingSystem { get; init; }
    public string? Environment { get; init; }
}
