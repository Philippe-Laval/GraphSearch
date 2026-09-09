namespace GraphSearch.Library.EdgeScoring;

/// <summary>
/// Fournit une fabrique pour créer une instance préconfigurée de <c>RelationshipVocabulary</c> avec des relations
/// courantes et leurs variantes lexicales.
/// </summary>
/// <remarks>Centralise l’initialisation du vocabulaire de relations afin d’assurer une terminologie cohérente et
/// réutilisable dans l’application.</remarks>
public class RelationshipVocabularyFactory
{
    public RelationshipVocabulary Create()
    {
        var vocabulary =
            new RelationshipVocabulary();

        vocabulary.Add(
            "develop",
            "develops",
            "developed-by",
            "created",
            "created-by",
            "builds",
            "built-by");

        vocabulary.Add(
            "use",
            "uses",
            "used-by",
            "utilizes",
            "depends-on");

        vocabulary.Add(
            "run-on",
            "runs-on",
            "supports",
            "supported-by",
            "compatible-with");

        vocabulary.Add(
            "own",
            "owns",
            "owned-by");

        vocabulary.Add(
            "work-for",
            "works-for",
            "employed-by",
            "employee-of");
        
        return vocabulary;
    }
}