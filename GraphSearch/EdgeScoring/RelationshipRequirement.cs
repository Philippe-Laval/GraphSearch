namespace GraphSearch.EdgeScoring;

/*
 new RelationshipRequirement(
   "develop",
   EdgeDirection.Outgoing);
 */

public sealed record RelationshipRequirement(
    string Type,
    EdgeDirection Direction);