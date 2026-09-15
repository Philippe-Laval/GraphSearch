namespace GraphRag.Ontology.Itsm.Business.Party;

/// <summary>
/// Représente une organisation, avec une unité opérationnelle et une relation hiérarchique optionnelle.
/// </summary>
/// <remarks>Hérite de Party. La propriété Parent permet de modéliser une hiérarchie d’organisations.</remarks>
public class Organization : Party
{
    /// <summary>
    /// Business unit
    /// </summary>
    public string? BusinessUnit { get; init; }

    /// <summary>
    /// Gets the parent organization.
    /// </summary>
    public Organization? Parent { get; init; }
}
