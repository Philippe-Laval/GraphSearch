namespace GraphRag.Ontology.Itsm.Business.Service;

/// <summary>A commercial packaging of a service (e.g. "Gold VIP Support").</summary>
public sealed class ServiceOffering : Service
{
    public BusinessService? For { get; init; }
    public string? SupportTier { get; init; }
}
