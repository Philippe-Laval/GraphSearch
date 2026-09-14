namespace GraphRag.Ontology.Itsm.Business.Service;

/// <summary>A business-facing service (e.g. "Finance ERP", "HR Portal").</summary>
public class BusinessService : Service
{
    public IReadOnlyList<Party.Organization> UsedBy { get; init; } = [];
    public IReadOnlyList<TechnicalService> EnabledBy { get; init; } = [];
}
