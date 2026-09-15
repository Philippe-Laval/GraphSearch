using GraphRag.Ontology.Itsm.Data;

namespace GraphRag.Ontology.Itsm.Governance;

/// <summary>
/// How sensitive is the data element from a compliance perspective.
/// </summary>
public sealed class DataClassification : GovernanceConcept
{
    public required DataAsset Asset { get; init; }
    public required Sensitivity Sensitivity { get; init; }
    public IReadOnlyList<string> ComplianceTags { get; init; } = [];
}
