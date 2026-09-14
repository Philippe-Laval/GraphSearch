namespace GraphRag.Ontology.Itsm.Business;

public sealed class ServiceRequest : Ticket
{
    public Service.ServiceCatalogItem? Requests { get; init; }
}
