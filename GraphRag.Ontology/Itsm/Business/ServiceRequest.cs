namespace GraphRag.Ontology.Itsm.Business;

/// <summary>
/// Represents a service request
/// </summary>
public sealed class ServiceRequest : Ticket
{
    /// <summary>
    /// The catalog item requested.
    /// </summary>
    public Service.ServiceCatalogItem? Requests { get; init; }
}
