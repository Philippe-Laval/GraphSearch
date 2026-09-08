namespace GraphSearch.Library.EdgeScoring;

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