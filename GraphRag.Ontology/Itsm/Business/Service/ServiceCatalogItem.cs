namespace GraphRag.Ontology.Itsm.Business.Service;

/// <summary>An item requestable through the service catalog (e.g. "New laptop", "VPN access").</summary>
public sealed class ServiceCatalogItem : Service
{
    public string? CatalogCategory { get; init; }
    public TimeSpan? StandardFulfillmentTime { get; init; }
}
