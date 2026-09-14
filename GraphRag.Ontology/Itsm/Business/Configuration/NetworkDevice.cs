namespace GraphRag.Ontology.Itsm.Business.Configuration;

public sealed class NetworkDevice : ConfigurationItem
{
    public string? DeviceType { get; init; }
    public string? IpAddress { get; init; }
}
