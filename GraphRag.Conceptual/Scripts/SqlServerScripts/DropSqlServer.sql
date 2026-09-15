BEGIN TRANSACTION;
DROP TABLE [AccessPolicyRule];

DROP TABLE [ConceptRelation];

DROP TABLE [ConceptRepresentation];

DROP TABLE [DataClassificationRule];

DROP TABLE [DataOwnerRecord];

DROP TABLE [JoinRelationship];

DROP TABLE [MetricImplementation];

DROP TABLE [QualityConstraint];

DROP TABLE [SemanticQuerySample];

DROP TABLE [Term];

DROP TABLE [BusinessMetricDefinition];

DROP TABLE [DataAssetRef];

DROP TABLE [CompetencyQuestion];

DROP TABLE [Concept];

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20260915081053_InitialCreate';

COMMIT;
GO

