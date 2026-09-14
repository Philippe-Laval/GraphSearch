using GraphRag.Ontology.Itsm.Query;

namespace GraphRag.Conceptual.Entities;

/// <summary>
/// A stored competency question — a question the ontology must be able to answer.
/// The doc recommends 20–50 of these before designing OWL classes.
/// Also useful as an evaluation dataset for the Text-to-SQL pipeline.
/// </summary>
public class CompetencyQuestion : ConceptualEntity
{
    public string Domain { get; set; } = "ITSM";
    public string Question { get; set; } = string.Empty;
    public string? Rationale { get; set; }
    public QuestionDifficulty Difficulty { get; set; }

    public ICollection<SemanticQuerySample> Samples { get; set; } = [];
}

public enum QuestionDifficulty { Easy, Medium, Hard, Expert }

/// <summary>
/// Example Semantic Query for grounding / few-shot prompting.
/// The semantic-query body is stored as JSON to keep the schema stable while
/// the intermediate representation evolves.
/// </summary>
public class SemanticQuerySample : ConceptualEntity
{
    public string NaturalLanguageQuestion { get; set; } = string.Empty;

    public QueryIntent Intent { get; set; }

    /// <summary>Concept URI that the query targets (e.g. "itsm:Incident").</summary>
    public string TargetConceptUri { get; set; } = string.Empty;

    /// <summary>JSON representation of the full SemanticQuery tree.</summary>
    public string SemanticQueryJson { get; set; } = "{}";

    /// <summary>Expected SQL implementation.</summary>
    public string? ExpectedSql { get; set; }

    public Guid? CompetencyQuestionId { get; set; }
    public CompetencyQuestion? CompetencyQuestion { get; set; }
}
