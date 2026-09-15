IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [CompetencyQuestion] (
        [Id] uniqueidentifier NOT NULL,
        [Domain] nvarchar(64) NOT NULL,
        [Question] nvarchar(1000) NOT NULL,
        [Rationale] nvarchar(2000) NULL,
        [Difficulty] nvarchar(16) NOT NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_CompetencyQuestion] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [Concept] (
        [Id] uniqueidentifier NOT NULL,
        [Kind] nvarchar(32) NOT NULL,
        [Domain] nvarchar(64) NOT NULL,
        [ParentConceptId] uniqueidentifier NULL,
        [SynonymsInline] nvarchar(2000) NULL,
        [IsDeprecated] bit NOT NULL,
        [DeprecationNote] nvarchar(max) NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Concept] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Concept_Concept_ParentConceptId] FOREIGN KEY ([ParentConceptId]) REFERENCES [Concept] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [DataAssetRef] (
        [Id] uniqueidentifier NOT NULL,
        [Kind] nvarchar(16) NOT NULL,
        [FullyQualifiedName] nvarchar(512) NOT NULL,
        [DatabaseName] nvarchar(128) NULL,
        [SchemaName] nvarchar(128) NULL,
        [TableName] nvarchar(128) NULL,
        [ColumnName] nvarchar(128) NULL,
        [DataType] nvarchar(64) NULL,
        [Dialect] nvarchar(24) NULL,
        [Grain] nvarchar(max) NULL,
        [IsNullable] bit NOT NULL,
        [IsPrimaryKey] bit NOT NULL,
        [IsDeprecated] bit NOT NULL,
        [ApproximateRowCount] bigint NULL,
        [ParentAssetId] uniqueidentifier NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_DataAssetRef] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DataAssetRef_DataAssetRef_ParentAssetId] FOREIGN KEY ([ParentAssetId]) REFERENCES [DataAssetRef] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [SemanticQuerySample] (
        [Id] uniqueidentifier NOT NULL,
        [NaturalLanguageQuestion] nvarchar(1000) NOT NULL,
        [Intent] nvarchar(24) NOT NULL,
        [TargetConceptUri] nvarchar(256) NOT NULL,
        [SemanticQueryJson] nvarchar(max) NOT NULL,
        [ExpectedSql] nvarchar(max) NULL,
        [CompetencyQuestionId] uniqueidentifier NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_SemanticQuerySample] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SemanticQuerySample_CompetencyQuestion_CompetencyQuestionId] FOREIGN KEY ([CompetencyQuestionId]) REFERENCES [CompetencyQuestion] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [BusinessMetricDefinition] (
        [Id] uniqueidentifier NOT NULL,
        [ConceptId] uniqueidentifier NOT NULL,
        [Aggregation] nvarchar(24) NOT NULL,
        [PreferredDefinition] nvarchar(1000) NULL,
        [Formula] nvarchar(1000) NULL,
        [Grain] nvarchar(128) NULL,
        [QualifyingPopulation] nvarchar(128) NULL,
        [TimeBasis] nvarchar(16) NOT NULL,
        [Unit] nvarchar(32) NULL,
        [BusinessCalendarConceptId] uniqueidentifier NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_BusinessMetricDefinition] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BusinessMetricDefinition_Concept_BusinessCalendarConceptId] FOREIGN KEY ([BusinessCalendarConceptId]) REFERENCES [Concept] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BusinessMetricDefinition_Concept_ConceptId] FOREIGN KEY ([ConceptId]) REFERENCES [Concept] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [ConceptRelation] (
        [Id] uniqueidentifier NOT NULL,
        [SourceConceptId] uniqueidentifier NOT NULL,
        [TargetConceptId] uniqueidentifier NOT NULL,
        [Predicate] nvarchar(64) NOT NULL,
        [Cardinality] nvarchar(16) NOT NULL,
        [IsInverseFunctional] bit NOT NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_ConceptRelation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConceptRelation_Concept_SourceConceptId] FOREIGN KEY ([SourceConceptId]) REFERENCES [Concept] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ConceptRelation_Concept_TargetConceptId] FOREIGN KEY ([TargetConceptId]) REFERENCES [Concept] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [Term] (
        [Id] uniqueidentifier NOT NULL,
        [SurfaceForm] nvarchar(256) NOT NULL,
        [ConceptId] uniqueidentifier NOT NULL,
        [Kind] nvarchar(24) NOT NULL,
        [Language] nvarchar(8) NULL,
        [Context] nvarchar(128) NULL,
        [Embedding] varbinary(max) NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Term] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Term_Concept_ConceptId] FOREIGN KEY ([ConceptId]) REFERENCES [Concept] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [AccessPolicyRule] (
        [Id] uniqueidentifier NOT NULL,
        [AssetId] uniqueidentifier NOT NULL,
        [AllowedRolesCsv] nvarchar(1000) NULL,
        [DeniedRolesCsv] nvarchar(1000) NULL,
        [AllowedAggregationsCsv] nvarchar(500) NULL,
        [RequiresApproval] bit NOT NULL,
        [PolicyNotes] nvarchar(max) NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_AccessPolicyRule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AccessPolicyRule_DataAssetRef_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [DataAssetRef] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [ConceptRepresentation] (
        [Id] uniqueidentifier NOT NULL,
        [ConceptId] uniqueidentifier NOT NULL,
        [AssetId] uniqueidentifier NOT NULL,
        [Kind] nvarchar(24) NOT NULL,
        [FilterExpression] nvarchar(2000) NULL,
        [Rationale] nvarchar(1000) NULL,
        [IsPreferred] bit NOT NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_ConceptRepresentation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConceptRepresentation_Concept_ConceptId] FOREIGN KEY ([ConceptId]) REFERENCES [Concept] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ConceptRepresentation_DataAssetRef_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [DataAssetRef] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [DataClassificationRule] (
        [Id] uniqueidentifier NOT NULL,
        [AssetId] uniqueidentifier NOT NULL,
        [Sensitivity] nvarchar(16) NOT NULL,
        [ComplianceTags] nvarchar(256) NULL,
        [Justification] nvarchar(max) NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_DataClassificationRule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DataClassificationRule_DataAssetRef_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [DataAssetRef] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [DataOwnerRecord] (
        [Id] uniqueidentifier NOT NULL,
        [AssetId] uniqueidentifier NOT NULL,
        [OwnerTeam] nvarchar(128) NOT NULL,
        [Steward] nvarchar(128) NULL,
        [DataDomain] nvarchar(64) NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_DataOwnerRecord] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DataOwnerRecord_DataAssetRef_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [DataAssetRef] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [JoinRelationship] (
        [Id] uniqueidentifier NOT NULL,
        [LeftTableId] uniqueidentifier NOT NULL,
        [RightTableId] uniqueidentifier NOT NULL,
        [JoinCondition] nvarchar(1000) NOT NULL,
        [Cardinality] nvarchar(16) NOT NULL,
        [IsPreferred] bit NOT NULL,
        [BackedByForeignKey] bit NOT NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_JoinRelationship] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JoinRelationship_DataAssetRef_LeftTableId] FOREIGN KEY ([LeftTableId]) REFERENCES [DataAssetRef] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_JoinRelationship_DataAssetRef_RightTableId] FOREIGN KEY ([RightTableId]) REFERENCES [DataAssetRef] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [QualityConstraint] (
        [Id] uniqueidentifier NOT NULL,
        [AssetId] uniqueidentifier NOT NULL,
        [MaxAcceptableLag] time NULL,
        [MinCompletenessPercent] float NULL,
        [QualityLevel] nvarchar(32) NULL,
        [IsDeprecated] bit NOT NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_QualityConstraint] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QualityConstraint_DataAssetRef_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [DataAssetRef] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE TABLE [MetricImplementation] (
        [Id] uniqueidentifier NOT NULL,
        [MetricDefinitionId] uniqueidentifier NOT NULL,
        [Dialect] nvarchar(24) NOT NULL,
        [Expression] nvarchar(4000) NOT NULL,
        [RequiredTables] nvarchar(1000) NULL,
        [Uri] nvarchar(256) NOT NULL,
        [PreferredLabel] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [OntologyVersion] nvarchar(32) NULL,
        [Source] nvarchar(128) NULL,
        [Confidence] float NULL,
        [AssertedBy] nvarchar(128) NULL,
        [AssertedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_MetricImplementation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MetricImplementation_BusinessMetricDefinition_MetricDefinitionId] FOREIGN KEY ([MetricDefinitionId]) REFERENCES [BusinessMetricDefinition] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AccessPolicyRule_AssetId] ON [AccessPolicyRule] ([AssetId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BusinessMetricDefinition_BusinessCalendarConceptId] ON [BusinessMetricDefinition] ([BusinessCalendarConceptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BusinessMetricDefinition_ConceptId] ON [BusinessMetricDefinition] ([ConceptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CompetencyQuestion_Domain] ON [CompetencyQuestion] ([Domain]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Concept_Domain_Kind] ON [Concept] ([Domain], [Kind]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Concept_ParentConceptId] ON [Concept] ([ParentConceptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Concept_Uri] ON [Concept] ([Uri]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ConceptRelation_SourceConceptId_Predicate_TargetConceptId] ON [ConceptRelation] ([SourceConceptId], [Predicate], [TargetConceptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ConceptRelation_TargetConceptId] ON [ConceptRelation] ([TargetConceptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ConceptRepresentation_AssetId] ON [ConceptRepresentation] ([AssetId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ConceptRepresentation_ConceptId_AssetId_Kind] ON [ConceptRepresentation] ([ConceptId], [AssetId], [Kind]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DataAssetRef_DatabaseName_SchemaName_TableName] ON [DataAssetRef] ([DatabaseName], [SchemaName], [TableName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DataAssetRef_FullyQualifiedName] ON [DataAssetRef] ([FullyQualifiedName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DataAssetRef_ParentAssetId] ON [DataAssetRef] ([ParentAssetId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DataClassificationRule_AssetId_Sensitivity] ON [DataClassificationRule] ([AssetId], [Sensitivity]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DataOwnerRecord_AssetId] ON [DataOwnerRecord] ([AssetId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JoinRelationship_LeftTableId_RightTableId] ON [JoinRelationship] ([LeftTableId], [RightTableId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JoinRelationship_RightTableId] ON [JoinRelationship] ([RightTableId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MetricImplementation_MetricDefinitionId_Dialect] ON [MetricImplementation] ([MetricDefinitionId], [Dialect]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_QualityConstraint_AssetId] ON [QualityConstraint] ([AssetId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SemanticQuerySample_CompetencyQuestionId] ON [SemanticQuerySample] ([CompetencyQuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SemanticQuerySample_TargetConceptUri] ON [SemanticQuerySample] ([TargetConceptUri]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Term_ConceptId] ON [Term] ([ConceptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Term_SurfaceForm] ON [Term] ([SurfaceForm]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Term_SurfaceForm_Language_Context] ON [Term] ([SurfaceForm], [Language], [Context]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915081053_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260915081053_InitialCreate', N'10.0.12');
END;

COMMIT;
GO

