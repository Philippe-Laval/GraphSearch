namespace GraphRag.Ontology.Itsm.Business;

/// <summary>
/// Représente un élément de travail spécialisé pour un cas de connaissance.
/// </summary>
/// <remarks>Type scellé dérivé de <see cref="WorkItem"/> pour modéliser un cas lié à une base de connaissances,
/// avec une référence d’article optionnelle.</remarks>
public sealed class KnowledgeCase : WorkItem
{
    /// <summary>
    /// The reference of the related knowledge article.
    /// </summary>
    public string? ArticleReference { get; init; }
}
