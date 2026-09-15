using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphRag.ITSM.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignmentGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportTier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingDaysMask = table.Column<int>(type: "int", nullable: false),
                    WorkingHoursStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHoursEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Category_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Impact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BusinessUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Organizations_ParentOrganizationId",
                        column: x => x.ParentOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Severity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Severity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IsTerminal = table.Column<bool>(type: "bit", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urgency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urgency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContractReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarHolidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessCalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarHolidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarHolidays_BusinessCalendars_BusinessCalendarId",
                        column: x => x.BusinessCalendarId,
                        principalTable: "BusinessCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersonType = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    SupportTier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Priority",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DerivedFromImpactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DerivedFromUrgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priority", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Priority_Impact_DerivedFromImpactId",
                        column: x => x.DerivedFromImpactId,
                        principalTable: "Impact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Priority_Urgency_DerivedFromUrgencyId",
                        column: x => x.DerivedFromUrgencyId,
                        principalTable: "Urgency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentGroupMemberships",
                columns: table => new
                {
                    AgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RemovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RoleInGroup = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentGroupMemberships", x => new { x.AgentId, x.AssignmentGroupId, x.AssignedAt });
                    table.ForeignKey(
                        name: "FK_AgentGroupMemberships_AssignmentGroups_AssignmentGroupId",
                        column: x => x.AssignmentGroupId,
                        principalTable: "AssignmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentGroupMemberships_Person_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OperationalStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CIType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VersionNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Engine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstanceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hostname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatingSystem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Server_IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationItem_Person_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Person",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    BusinessCriticality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CatalogCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardFulfillmentTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    SupportTier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EnvironmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Service_Person_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Service_Service_BusinessServiceId",
                        column: x => x.BusinessServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationItemDependencies",
                columns: table => new
                {
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DependencyType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationItemDependencies", x => new { x.SourceId, x.TargetId });
                    table.ForeignKey(
                        name: "FK_ConfigurationItemDependencies_ConfigurationItem_SourceId",
                        column: x => x.SourceId,
                        principalTable: "ConfigurationItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigurationItemDependencies_ConfigurationItem_TargetId",
                        column: x => x.TargetId,
                        principalTable: "ConfigurationItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusinessServiceConsumers",
                columns: table => new
                {
                    BusinessServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessServiceConsumers", x => new { x.BusinessServiceId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_BusinessServiceConsumers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessServiceConsumers_Service_BusinessServiceId",
                        column: x => x.BusinessServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationItemSupports",
                columns: table => new
                {
                    ConfigurationItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnicalServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationItemSupports", x => new { x.ConfigurationItemId, x.TechnicalServiceId });
                    table.ForeignKey(
                        name: "FK_ConfigurationItemSupports_ConfigurationItem_ConfigurationItemId",
                        column: x => x.ConfigurationItemId,
                        principalTable: "ConfigurationItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationItemSupports_Service_TechnicalServiceId",
                        column: x => x.TechnicalServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLevelAgreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    TargetDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    TargetAvailabilityPercent = table.Column<double>(type: "float", nullable: true),
                    AppliesToPriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppliesToServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BusinessCalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLevelAgreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceLevelAgreements_BusinessCalendars_BusinessCalendarId",
                        column: x => x.BusinessCalendarId,
                        principalTable: "BusinessCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceLevelAgreements_Priority_AppliesToPriorityId",
                        column: x => x.AppliesToPriorityId,
                        principalTable: "Priority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceLevelAgreements_Service_AppliesToServiceId",
                        column: x => x.AppliesToServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSupportGroups",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSupportGroups", x => new { x.ServiceId, x.AssignmentGroupId });
                    table.ForeignKey(
                        name: "FK_ServiceSupportGroups_AssignmentGroups_AssignmentGroupId",
                        column: x => x.AssignmentGroupId,
                        principalTable: "AssignmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceSupportGroups_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalServiceEnablements",
                columns: table => new
                {
                    TechnicalServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalServiceEnablements", x => new { x.TechnicalServiceId, x.BusinessServiceId });
                    table.ForeignKey(
                        name: "FK_TechnicalServiceEnablements_Service_BusinessServiceId",
                        column: x => x.BusinessServiceId,
                        principalTable: "Service",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechnicalServiceEnablements_Service_TechnicalServiceId",
                        column: x => x.TechnicalServiceId,
                        principalTable: "Service",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AffectedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolutionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImpactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UrgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SeverityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubcategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OpenedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AcknowledgedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    FirstResponseAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PendingAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ClosedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DueAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SlaResponseDeadline = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SlaResolutionDeadline = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReopenCount = table.Column<int>(type: "int", nullable: false),
                    WasEscalated = table.Column<bool>(type: "bit", nullable: false),
                    TicketType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ChangeType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    PlannedStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PlannedEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActualStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActualEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CausedByProblemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolvedByChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KnownErrorArticleRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Workaround = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedCatalogItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_AssignmentGroups_AssignedGroupId",
                        column: x => x.AssignedGroupId,
                        principalTable: "AssignmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_AssignmentGroups_ResolutionGroupId",
                        column: x => x.ResolutionGroupId,
                        principalTable: "AssignmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Category_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Impact_ImpactId",
                        column: x => x.ImpactId,
                        principalTable: "Impact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Person_AffectedUserId",
                        column: x => x.AffectedUserId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Person_AssignedAgentId",
                        column: x => x.AssignedAgentId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Person_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Person_ResolverId",
                        column: x => x.ResolverId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Priority_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "Priority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Service_RequestedCatalogItemId",
                        column: x => x.RequestedCatalogItemId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Severity_SeverityId",
                        column: x => x.SeverityId,
                        principalTable: "Severity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Status_CurrentStatusId",
                        column: x => x.CurrentStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Ticket_CausedByProblemId",
                        column: x => x.CausedByProblemId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Ticket_ParentTicketId",
                        column: x => x.ParentTicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Ticket_ResolvedByChangeId",
                        column: x => x.ResolvedByChangeId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Urgency_UrgencyId",
                        column: x => x.UrgencyId,
                        principalTable: "Urgency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperationalEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PerformedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    FromGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClosureCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewTier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReassignmentEvent_Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReopenEvent_Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolutionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeInFromStatus = table.Column<TimeSpan>(type: "time", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationalEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationalEvent_AssignmentGroups_FromGroupId",
                        column: x => x.FromGroupId,
                        principalTable: "AssignmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalEvent_AssignmentGroups_ToGroupId",
                        column: x => x.ToGroupId,
                        principalTable: "AssignmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalEvent_Person_FromAgentId",
                        column: x => x.FromAgentId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalEvent_Person_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalEvent_Person_ToAgentId",
                        column: x => x.ToAgentId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalEvent_Status_FromStatusId",
                        column: x => x.FromStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalEvent_Status_ToStatusId",
                        column: x => x.ToStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationalEvent_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlaMeasurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceLevelAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StopTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PauseDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    TargetDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    ElapsedDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    BreachStatus = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlaMeasurements_ServiceLevelAgreements_ServiceLevelAgreementId",
                        column: x => x.ServiceLevelAgreementId,
                        principalTable: "ServiceLevelAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SlaMeasurements_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketAffectedCIs",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfigurationItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAffectedCIs", x => new { x.TicketId, x.ConfigurationItemId });
                    table.ForeignKey(
                        name: "FK_TicketAffectedCIs_ConfigurationItem_ConfigurationItemId",
                        column: x => x.ConfigurationItemId,
                        principalTable: "ConfigurationItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketAffectedCIs_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketAffectedServices",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAffectedServices", x => new { x.TicketId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_TicketAffectedServices_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketAffectedServices_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationKind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketRelations_Ticket_SourceTicketId",
                        column: x => x.SourceTicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRelations_Ticket_TargetTicketId",
                        column: x => x.TargetTicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentGroupMemberships_AssignmentGroupId",
                table: "AgentGroupMemberships",
                column: "AssignmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentGroups_ExternalId",
                table: "AssignmentGroups",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentGroups_Name",
                table: "AssignmentGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCalendars_ExternalId",
                table: "BusinessCalendars",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCalendars_Name",
                table: "BusinessCalendars",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessServiceConsumers_OrganizationId",
                table: "BusinessServiceConsumers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarHolidays_BusinessCalendarId_Date",
                table: "CalendarHolidays",
                columns: new[] { "BusinessCalendarId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Code",
                table: "Category",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_ExternalId",
                table: "Category",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentCategoryId",
                table: "Category",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationItem_AssetTag",
                table: "ConfigurationItem",
                column: "AssetTag");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationItem_ExternalId",
                table: "ConfigurationItem",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationItem_Name",
                table: "ConfigurationItem",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationItem_OwnerId",
                table: "ConfigurationItem",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationItemDependencies_TargetId",
                table: "ConfigurationItemDependencies",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationItemSupports_TechnicalServiceId",
                table: "ConfigurationItemSupports",
                column: "TechnicalServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Impact_Code",
                table: "Impact",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Impact_ExternalId",
                table: "Impact",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_ExternalId",
                table: "OperationalEvent",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_FromAgentId",
                table: "OperationalEvent",
                column: "FromAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_FromGroupId",
                table: "OperationalEvent",
                column: "FromGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_FromStatusId",
                table: "OperationalEvent",
                column: "FromStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_PerformedById",
                table: "OperationalEvent",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_TicketId_EventTimestamp",
                table: "OperationalEvent",
                columns: new[] { "TicketId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_ToAgentId",
                table: "OperationalEvent",
                column: "ToAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_ToGroupId",
                table: "OperationalEvent",
                column: "ToGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalEvent_ToStatusId",
                table: "OperationalEvent",
                column: "ToStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ExternalId",
                table: "Organizations",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ParentOrganizationId",
                table: "Organizations",
                column: "ParentOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_Email",
                table: "Person",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Person_ExternalId",
                table: "Person",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_OrganizationId",
                table: "Person",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Priority_Code",
                table: "Priority",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Priority_DerivedFromImpactId",
                table: "Priority",
                column: "DerivedFromImpactId");

            migrationBuilder.CreateIndex(
                name: "IX_Priority_DerivedFromUrgencyId",
                table: "Priority",
                column: "DerivedFromUrgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Priority_ExternalId",
                table: "Priority",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_BusinessServiceId",
                table: "Service",
                column: "BusinessServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ExternalId",
                table: "Service",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Name",
                table: "Service",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Service_OwnerId",
                table: "Service",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLevelAgreements_AppliesToPriorityId",
                table: "ServiceLevelAgreements",
                column: "AppliesToPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLevelAgreements_AppliesToServiceId",
                table: "ServiceLevelAgreements",
                column: "AppliesToServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLevelAgreements_BusinessCalendarId",
                table: "ServiceLevelAgreements",
                column: "BusinessCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLevelAgreements_ExternalId",
                table: "ServiceLevelAgreements",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLevelAgreements_Name",
                table: "ServiceLevelAgreements",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSupportGroups_AssignmentGroupId",
                table: "ServiceSupportGroups",
                column: "AssignmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Severity_Code",
                table: "Severity",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Severity_ExternalId",
                table: "Severity",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_SlaMeasurements_ExternalId",
                table: "SlaMeasurements",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_SlaMeasurements_ServiceLevelAgreementId",
                table: "SlaMeasurements",
                column: "ServiceLevelAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_SlaMeasurements_TicketId_ServiceLevelAgreementId",
                table: "SlaMeasurements",
                columns: new[] { "TicketId", "ServiceLevelAgreementId" });

            migrationBuilder.CreateIndex(
                name: "IX_Status_Code",
                table: "Status",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Status_ExternalId",
                table: "Status",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalServiceEnablements_BusinessServiceId",
                table: "TechnicalServiceEnablements",
                column: "BusinessServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AffectedUserId",
                table: "Ticket",
                column: "AffectedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AssignedAgentId",
                table: "Ticket",
                column: "AssignedAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AssignedGroupId",
                table: "Ticket",
                column: "AssignedGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CategoryId",
                table: "Ticket",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CausedByProblemId",
                table: "Ticket",
                column: "CausedByProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ClosedAt",
                table: "Ticket",
                column: "ClosedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CurrentStatusId_PriorityId",
                table: "Ticket",
                columns: new[] { "CurrentStatusId", "PriorityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ExternalId",
                table: "Ticket",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ImpactId",
                table: "Ticket",
                column: "ImpactId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Number",
                table: "Ticket",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_OpenedAt",
                table: "Ticket",
                column: "OpenedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ParentTicketId",
                table: "Ticket",
                column: "ParentTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_PriorityId",
                table: "Ticket",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_RequestedCatalogItemId",
                table: "Ticket",
                column: "RequestedCatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_RequesterId",
                table: "Ticket",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ResolutionGroupId",
                table: "Ticket",
                column: "ResolutionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ResolvedAt",
                table: "Ticket",
                column: "ResolvedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ResolvedByChangeId",
                table: "Ticket",
                column: "ResolvedByChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ResolverId",
                table: "Ticket",
                column: "ResolverId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SeverityId",
                table: "Ticket",
                column: "SeverityId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SubcategoryId",
                table: "Ticket",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_UrgencyId",
                table: "Ticket",
                column: "UrgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAffectedCIs_ConfigurationItemId",
                table: "TicketAffectedCIs",
                column: "ConfigurationItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAffectedServices_ServiceId",
                table: "TicketAffectedServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRelations_SourceTicketId_TargetTicketId_RelationKind",
                table: "TicketRelations",
                columns: new[] { "SourceTicketId", "TargetTicketId", "RelationKind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketRelations_TargetTicketId",
                table: "TicketRelations",
                column: "TargetTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Urgency_Code",
                table: "Urgency",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Urgency_ExternalId",
                table: "Urgency",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_ExternalId",
                table: "Vendors",
                column: "ExternalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentGroupMemberships");

            migrationBuilder.DropTable(
                name: "BusinessServiceConsumers");

            migrationBuilder.DropTable(
                name: "CalendarHolidays");

            migrationBuilder.DropTable(
                name: "ConfigurationItemDependencies");

            migrationBuilder.DropTable(
                name: "ConfigurationItemSupports");

            migrationBuilder.DropTable(
                name: "OperationalEvent");

            migrationBuilder.DropTable(
                name: "ServiceSupportGroups");

            migrationBuilder.DropTable(
                name: "SlaMeasurements");

            migrationBuilder.DropTable(
                name: "TechnicalServiceEnablements");

            migrationBuilder.DropTable(
                name: "TicketAffectedCIs");

            migrationBuilder.DropTable(
                name: "TicketAffectedServices");

            migrationBuilder.DropTable(
                name: "TicketRelations");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "ServiceLevelAgreements");

            migrationBuilder.DropTable(
                name: "ConfigurationItem");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "BusinessCalendars");

            migrationBuilder.DropTable(
                name: "AssignmentGroups");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Priority");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Severity");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Impact");

            migrationBuilder.DropTable(
                name: "Urgency");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
