namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// Représente un fournisseur, c’est-à-dire une partie qui fournit des biens ou des services.   
/// </summary>
/// <remarks>Peut inclure une référence de contrat associée au fournisseur.</remarks>
public sealed class Vendor : Party
{
    /// <summary>
    /// Référence du contrat associé au fournisseur.
    /// </summary>
    public string? ContractReference { get; init; }
}
