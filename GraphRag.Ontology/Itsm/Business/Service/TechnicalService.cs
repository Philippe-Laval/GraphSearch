namespace GraphRag.Ontology.Itsm.Business.Service;

/// <summary>
/// An IT-facing service (e.g. "SAP ERP Production", "Corporate DNS").
/// </summary>
public class TechnicalService : Service
{
    public IReadOnlyList<Configuration.ConfigurationItem> SupportedByCIs { get; init; } = [];
    public IReadOnlyList<BusinessService> Enables { get; init; } = [];
}
