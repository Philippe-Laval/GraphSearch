namespace GraphSearch.EdgeScoring;

public sealed class RelationshipVocabulary
{
    private readonly Dictionary<string, HashSet<string>>
        _synonyms = new(StringComparer.OrdinalIgnoreCase);

    public void Add(
        string canonical,
        params string[] synonyms)
    {
        _synonyms[canonical] =
            new HashSet<string>(
                synonyms,
                StringComparer.OrdinalIgnoreCase);
    }

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