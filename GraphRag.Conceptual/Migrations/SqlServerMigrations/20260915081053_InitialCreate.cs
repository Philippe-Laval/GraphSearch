using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphRag.Conceptual.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompetencyQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Question = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Rationale = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Difficulty = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetencyQuestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Concept",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ParentConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SynonymsInline = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeprecated = table.Column<bool>(type: "bit", nullable: false),
                    DeprecationNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concept", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Concept_Concept_ParentConceptId",
                        column: x => x.ParentConceptId,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataAssetRef",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    FullyQualifiedName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SchemaName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ColumnName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Dialect = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    Grain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsNullable = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimaryKey = table.Column<bool>(type: "bit", nullable: false),
                    IsDeprecated = table.Column<bool>(type: "bit", nullable: false),
                    ApproximateRowCount = table.Column<long>(type: "bigint", nullable: true),
                    ParentAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataAssetRef", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataAssetRef_DataAssetRef_ParentAssetId",
                        column: x => x.ParentAssetId,
                        principalTable: "DataAssetRef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SemanticQuerySample",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NaturalLanguageQuestion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Intent = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    TargetConceptUri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SemanticQueryJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedSql = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompetencyQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemanticQuerySample", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemanticQuerySample_CompetencyQuestion_CompetencyQuestionId",
                        column: x => x.CompetencyQuestionId,
                        principalTable: "CompetencyQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BusinessMetricDefinition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Aggregation = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    PreferredDefinition = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Formula = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Grain = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    QualifyingPopulation = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TimeBasis = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    BusinessCalendarConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessMetricDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessMetricDefinition_Concept_BusinessCalendarConceptId",
                        column: x => x.BusinessCalendarConceptId,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessMetricDefinition_Concept_ConceptId",
                        column: x => x.ConceptId,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConceptRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Predicate = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Cardinality = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IsInverseFunctional = table.Column<bool>(type: "bit", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConceptRelation_Concept_SourceConceptId",
                        column: x => x.SourceConceptId,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConceptRelation_Concept_TargetConceptId",
                        column: x => x.TargetConceptId,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Term",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurfaceForm = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Context = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Embedding = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Term", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Term_Concept_ConceptId",
                        column: x => x.ConceptId,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessPolicyRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowedRolesCsv = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeniedRolesCsv = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AllowedAggregationsCsv = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    PolicyNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPolicyRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessPolicyRule_DataAssetRef_AssetId",
                        column: x => x.AssetId,
                        principalTable: "DataAssetRef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConceptRepresentation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    FilterExpression = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Rationale = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptRepresentation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConceptRepresentation_Concept_ConceptId",
                        column: x => x.ConceptId,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConceptRepresentation_DataAssetRef_AssetId",
                        column: x => x.AssetId,
                        principalTable: "DataAssetRef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataClassificationRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sensitivity = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ComplianceTags = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Justification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataClassificationRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataClassificationRule_DataAssetRef_AssetId",
                        column: x => x.AssetId,
                        principalTable: "DataAssetRef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataOwnerRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerTeam = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Steward = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DataDomain = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataOwnerRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataOwnerRecord_DataAssetRef_AssetId",
                        column: x => x.AssetId,
                        principalTable: "DataAssetRef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinRelationship",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeftTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RightTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinCondition = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Cardinality = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: false),
                    BackedByForeignKey = table.Column<bool>(type: "bit", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinRelationship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinRelationship_DataAssetRef_LeftTableId",
                        column: x => x.LeftTableId,
                        principalTable: "DataAssetRef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoinRelationship_DataAssetRef_RightTableId",
                        column: x => x.RightTableId,
                        principalTable: "DataAssetRef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QualityConstraint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxAcceptableLag = table.Column<TimeSpan>(type: "time", nullable: true),
                    MinCompletenessPercent = table.Column<double>(type: "float", nullable: true),
                    QualityLevel = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    IsDeprecated = table.Column<bool>(type: "bit", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityConstraint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityConstraint_DataAssetRef_AssetId",
                        column: x => x.AssetId,
                        principalTable: "DataAssetRef",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetricImplementation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetricDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Dialect = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Expression = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RequiredTables = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreferredLabel = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OntologyVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    AssertedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricImplementation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetricImplementation_BusinessMetricDefinition_MetricDefinitionId",
                        column: x => x.MetricDefinitionId,
                        principalTable: "BusinessMetricDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessPolicyRule_AssetId",
                table: "AccessPolicyRule",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMetricDefinition_BusinessCalendarConceptId",
                table: "BusinessMetricDefinition",
                column: "BusinessCalendarConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMetricDefinition_ConceptId",
                table: "BusinessMetricDefinition",
                column: "ConceptId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetencyQuestion_Domain",
                table: "CompetencyQuestion",
                column: "Domain");

            migrationBuilder.CreateIndex(
                name: "IX_Concept_Domain_Kind",
                table: "Concept",
                columns: new[] { "Domain", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_Concept_ParentConceptId",
                table: "Concept",
                column: "ParentConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_Concept_Uri",
                table: "Concept",
                column: "Uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConceptRelation_SourceConceptId_Predicate_TargetConceptId",
                table: "ConceptRelation",
                columns: new[] { "SourceConceptId", "Predicate", "TargetConceptId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConceptRelation_TargetConceptId",
                table: "ConceptRelation",
                column: "TargetConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptRepresentation_AssetId",
                table: "ConceptRepresentation",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptRepresentation_ConceptId_AssetId_Kind",
                table: "ConceptRepresentation",
                columns: new[] { "ConceptId", "AssetId", "Kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataAssetRef_DatabaseName_SchemaName_TableName",
                table: "DataAssetRef",
                columns: new[] { "DatabaseName", "SchemaName", "TableName" });

            migrationBuilder.CreateIndex(
                name: "IX_DataAssetRef_FullyQualifiedName",
                table: "DataAssetRef",
                column: "FullyQualifiedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataAssetRef_ParentAssetId",
                table: "DataAssetRef",
                column: "ParentAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DataClassificationRule_AssetId_Sensitivity",
                table: "DataClassificationRule",
                columns: new[] { "AssetId", "Sensitivity" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataOwnerRecord_AssetId",
                table: "DataOwnerRecord",
                column: "AssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JoinRelationship_LeftTableId_RightTableId",
                table: "JoinRelationship",
                columns: new[] { "LeftTableId", "RightTableId" });

            migrationBuilder.CreateIndex(
                name: "IX_JoinRelationship_RightTableId",
                table: "JoinRelationship",
                column: "RightTableId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricImplementation_MetricDefinitionId_Dialect",
                table: "MetricImplementation",
                columns: new[] { "MetricDefinitionId", "Dialect" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualityConstraint_AssetId",
                table: "QualityConstraint",
                column: "AssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SemanticQuerySample_CompetencyQuestionId",
                table: "SemanticQuerySample",
                column: "CompetencyQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticQuerySample_TargetConceptUri",
                table: "SemanticQuerySample",
                column: "TargetConceptUri");

            migrationBuilder.CreateIndex(
                name: "IX_Term_ConceptId",
                table: "Term",
                column: "ConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_Term_SurfaceForm",
                table: "Term",
                column: "SurfaceForm");

            migrationBuilder.CreateIndex(
                name: "IX_Term_SurfaceForm_Language_Context",
                table: "Term",
                columns: new[] { "SurfaceForm", "Language", "Context" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessPolicyRule");

            migrationBuilder.DropTable(
                name: "ConceptRelation");

            migrationBuilder.DropTable(
                name: "ConceptRepresentation");

            migrationBuilder.DropTable(
                name: "DataClassificationRule");

            migrationBuilder.DropTable(
                name: "DataOwnerRecord");

            migrationBuilder.DropTable(
                name: "JoinRelationship");

            migrationBuilder.DropTable(
                name: "MetricImplementation");

            migrationBuilder.DropTable(
                name: "QualityConstraint");

            migrationBuilder.DropTable(
                name: "SemanticQuerySample");

            migrationBuilder.DropTable(
                name: "Term");

            migrationBuilder.DropTable(
                name: "BusinessMetricDefinition");

            migrationBuilder.DropTable(
                name: "DataAssetRef");

            migrationBuilder.DropTable(
                name: "CompetencyQuestion");

            migrationBuilder.DropTable(
                name: "Concept");
        }
    }
}
