namespace GraphRag.Ontology.Itsm.Business.Service;

/// <summary>
/// An item requestable through the service catalog (e.g. "New laptop", "VPN access").
/// </summary>
public sealed class ServiceCatalogItem : Service
{
    /// <summary>
    /// The category of the catalog item.
    /// </summary>
    public string? CatalogCategory { get; init; }

    /// <summary>
    /// The standard fulfillment time for the catalog item.
    /// </summary>
    public TimeSpan? StandardFulfillmentTime { get; init; }
}
