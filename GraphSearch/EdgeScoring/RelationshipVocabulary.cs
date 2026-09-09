namespace GraphSearch.Library.EdgeScoring;

/// <summary>
/// Représente un vocabulaire de relations canoniques et de synonymes pour évaluer la similarité entre deux valeurs de
/// relation.
/// </summary>
/// <remarks>Les comparaisons sont insensibles à la casse. Le score de similarité est de 1.0 pour une
/// correspondance directe, 0.8 pour un synonyme et 0.0 en l’absence de correspondance.</remarks>
public sealed class RelationshipVocabulary
{
    private readonly Dictionary<string, HashSet<string>>
        _synonyms = new(StringComparer.OrdinalIgnoreCase);
    
    /// <summary>
    /// Ajoute une relation canonique et ses synonymes au vocabulaire.
    /// </summary>
    /// <param name="canonical">La forme canonique de la relation.</param>
    /// <param name="synonyms">Les synonymes de la relation.</param>
    public void Add(
        string canonical,
        params string[] synonyms)
    {
        _synonyms[canonical] =
            new HashSet<string>(
                synonyms,
                StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Calcule un score de similarité entre une valeur réelle et une valeur demandée à partir d’une correspondance
    /// directe et d’un dictionnaire de synonymes.
    /// </summary>
    /// <param name="actual">Valeur réelle à évaluer.</param>
    /// <param name="requested">Valeur demandée utilisée comme référence de comparaison et clé de recherche des synonymes.</param>
    /// <returns>1.0 pour une correspondance égale sans tenir compte de la casse, 0.8 si la valeur réelle est un synonyme de la
    /// valeur demandée, sinon 0.0.</returns>
    public double Similarity(
        string actual,
        string requested)
    {
        if (string.Equals(
                actual,
                requested,
                StringComparison.OrdinalIgnoreCase))
        {
            return 1.0;
        }

        if (!_synonyms.TryGetValue(
                requested,
                out var synonyms))
        {
            return 0.0;
        }

        return synonyms.Contains(actual)
            ? 0.8
            : 0.0;
    }
}