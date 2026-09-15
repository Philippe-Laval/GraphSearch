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
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [AssignmentGroups] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Department] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [SupportTier] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_AssignmentGroups] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [BusinessCalendars] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [TimeZoneId] nvarchar(max) NOT NULL,
        [WorkingDaysMask] int NOT NULL,
        [WorkingHoursStart] time NULL,
        [WorkingHoursEnd] time NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_BusinessCalendars] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Category] (
        [Id] uniqueidentifier NOT NULL,
        [ParentCategoryId] uniqueidentifier NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        [Code] nvarchar(32) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Category_Category_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Category] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Impact] (
        [Id] uniqueidentifier NOT NULL,
        [Level] nvarchar(16) NOT NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        [Code] nvarchar(32) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Impact] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Organizations] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [BusinessUnit] nvarchar(max) NULL,
        [ParentOrganizationId] uniqueidentifier NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Organizations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Organizations_Organizations_ParentOrganizationId] FOREIGN KEY ([ParentOrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Severity] (
        [Id] uniqueidentifier NOT NULL,
        [Level] nvarchar(16) NOT NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        [Code] nvarchar(32) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Severity] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Status] (
        [Id] uniqueidentifier NOT NULL,
        [Kind] nvarchar(32) NOT NULL,
        [IsTerminal] bit NOT NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        [Code] nvarchar(32) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Status] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Urgency] (
        [Id] uniqueidentifier NOT NULL,
        [Level] nvarchar(16) NOT NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        [Code] nvarchar(32) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Urgency] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Vendors] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [ContractReference] nvarchar(max) NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Vendors] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [CalendarHolidays] (
        [Id] uniqueidentifier NOT NULL,
        [BusinessCalendarId] uniqueidentifier NOT NULL,
        [Date] date NOT NULL,
        [Name] nvarchar(max) NULL,
        CONSTRAINT [PK_CalendarHolidays] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CalendarHolidays_BusinessCalendars_BusinessCalendarId] FOREIGN KEY ([BusinessCalendarId]) REFERENCES [BusinessCalendars] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Person] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [Email] nvarchar(320) NULL,
        [Department] nvarchar(max) NULL,
        [Location] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [OrganizationId] uniqueidentifier NULL,
        [PersonType] nvarchar(5) NOT NULL,
        [SupportTier] nvarchar(max) NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Person] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Person_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Priority] (
        [Id] uniqueidentifier NOT NULL,
        [Level] nvarchar(16) NOT NULL,
        [DerivedFromImpactId] uniqueidentifier NULL,
        [DerivedFromUrgencyId] uniqueidentifier NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        [Code] nvarchar(32) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Priority] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Priority_Impact_DerivedFromImpactId] FOREIGN KEY ([DerivedFromImpactId]) REFERENCES [Impact] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Priority_Urgency_DerivedFromUrgencyId] FOREIGN KEY ([DerivedFromUrgencyId]) REFERENCES [Urgency] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [AgentGroupMemberships] (
        [AgentId] uniqueidentifier NOT NULL,
        [AssignmentGroupId] uniqueidentifier NOT NULL,
        [AssignedAt] datetimeoffset NOT NULL,
        [RemovedAt] datetimeoffset NULL,
        [RoleInGroup] nvarchar(max) NULL,
        CONSTRAINT [PK_AgentGroupMemberships] PRIMARY KEY ([AgentId], [AssignmentGroupId], [AssignedAt]),
        CONSTRAINT [FK_AgentGroupMemberships_AssignmentGroups_AssignmentGroupId] FOREIGN KEY ([AssignmentGroupId]) REFERENCES [AssignmentGroups] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AgentGroupMemberships_Person_AgentId] FOREIGN KEY ([AgentId]) REFERENCES [Person] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [ConfigurationItem] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [AssetTag] nvarchar(450) NULL,
        [OperationalStatus] nvarchar(32) NOT NULL,
        [Environment] nvarchar(max) NULL,
        [OwnerId] uniqueidentifier NULL,
        [CIType] nvarchar(21) NOT NULL,
        [Vendor] nvarchar(max) NULL,
        [VersionNumber] nvarchar(max) NULL,
        [Provider] nvarchar(max) NULL,
        [Region] nvarchar(max) NULL,
        [ResourceType] nvarchar(max) NULL,
        [Engine] nvarchar(max) NULL,
        [InstanceName] nvarchar(max) NULL,
        [DeviceType] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        [Hostname] nvarchar(max) NULL,
        [OperatingSystem] nvarchar(max) NULL,
        [Server_IpAddress] nvarchar(max) NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_ConfigurationItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConfigurationItem_Person_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Person] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Service] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [OwnerId] uniqueidentifier NULL,
        [ServiceType] nvarchar(13) NOT NULL,
        [BusinessCriticality] nvarchar(max) NULL,
        [CatalogCategory] nvarchar(max) NULL,
        [StandardFulfillmentTime] time NULL,
        [SupportTier] nvarchar(max) NULL,
        [BusinessServiceId] uniqueidentifier NULL,
        [EnvironmentName] nvarchar(max) NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Service] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Service_Person_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Person] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Service_Service_BusinessServiceId] FOREIGN KEY ([BusinessServiceId]) REFERENCES [Service] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [ConfigurationItemDependencies] (
        [SourceId] uniqueidentifier NOT NULL,
        [TargetId] uniqueidentifier NOT NULL,
        [DependencyType] nvarchar(max) NULL,
        CONSTRAINT [PK_ConfigurationItemDependencies] PRIMARY KEY ([SourceId], [TargetId]),
        CONSTRAINT [FK_ConfigurationItemDependencies_ConfigurationItem_SourceId] FOREIGN KEY ([SourceId]) REFERENCES [ConfigurationItem] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ConfigurationItemDependencies_ConfigurationItem_TargetId] FOREIGN KEY ([TargetId]) REFERENCES [ConfigurationItem] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [BusinessServiceConsumers] (
        [BusinessServiceId] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BusinessServiceConsumers] PRIMARY KEY ([BusinessServiceId], [OrganizationId]),
        CONSTRAINT [FK_BusinessServiceConsumers_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BusinessServiceConsumers_Service_BusinessServiceId] FOREIGN KEY ([BusinessServiceId]) REFERENCES [Service] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [ConfigurationItemSupports] (
        [ConfigurationItemId] uniqueidentifier NOT NULL,
        [TechnicalServiceId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ConfigurationItemSupports] PRIMARY KEY ([ConfigurationItemId], [TechnicalServiceId]),
        CONSTRAINT [FK_ConfigurationItemSupports_ConfigurationItem_ConfigurationItemId] FOREIGN KEY ([ConfigurationItemId]) REFERENCES [ConfigurationItem] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ConfigurationItemSupports_Service_TechnicalServiceId] FOREIGN KEY ([TechnicalServiceId]) REFERENCES [Service] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceLevelAgreements] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Kind] nvarchar(16) NOT NULL,
        [TargetDuration] time NOT NULL,
        [TargetAvailabilityPercent] float NULL,
        [AppliesToPriorityId] uniqueidentifier NULL,
        [AppliesToServiceId] uniqueidentifier NULL,
        [BusinessCalendarId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_ServiceLevelAgreements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServiceLevelAgreements_BusinessCalendars_BusinessCalendarId] FOREIGN KEY ([BusinessCalendarId]) REFERENCES [BusinessCalendars] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ServiceLevelAgreements_Priority_AppliesToPriorityId] FOREIGN KEY ([AppliesToPriorityId]) REFERENCES [Priority] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ServiceLevelAgreements_Service_AppliesToServiceId] FOREIGN KEY ([AppliesToServiceId]) REFERENCES [Service] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceSupportGroups] (
        [ServiceId] uniqueidentifier NOT NULL,
        [AssignmentGroupId] uniqueidentifier NOT NULL,
        [IsPrimary] bit NOT NULL,
        CONSTRAINT [PK_ServiceSupportGroups] PRIMARY KEY ([ServiceId], [AssignmentGroupId]),
        CONSTRAINT [FK_ServiceSupportGroups_AssignmentGroups_AssignmentGroupId] FOREIGN KEY ([AssignmentGroupId]) REFERENCES [AssignmentGroups] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ServiceSupportGroups_Service_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Service] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [TechnicalServiceEnablements] (
        [TechnicalServiceId] uniqueidentifier NOT NULL,
        [BusinessServiceId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TechnicalServiceEnablements] PRIMARY KEY ([TechnicalServiceId], [BusinessServiceId]),
        CONSTRAINT [FK_TechnicalServiceEnablements_Service_BusinessServiceId] FOREIGN KEY ([BusinessServiceId]) REFERENCES [Service] ([Id]),
        CONSTRAINT [FK_TechnicalServiceEnablements_Service_TechnicalServiceId] FOREIGN KEY ([TechnicalServiceId]) REFERENCES [Service] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [Ticket] (
        [Id] uniqueidentifier NOT NULL,
        [Number] nvarchar(32) NOT NULL,
        [ShortDescription] nvarchar(500) NOT NULL,
        [Description] nvarchar(max) NULL,
        [RequesterId] uniqueidentifier NULL,
        [AffectedUserId] uniqueidentifier NULL,
        [AssignedAgentId] uniqueidentifier NULL,
        [AssignedGroupId] uniqueidentifier NULL,
        [ResolverId] uniqueidentifier NULL,
        [ResolutionGroupId] uniqueidentifier NULL,
        [CurrentStatusId] uniqueidentifier NOT NULL,
        [PriorityId] uniqueidentifier NULL,
        [ImpactId] uniqueidentifier NULL,
        [UrgencyId] uniqueidentifier NULL,
        [SeverityId] uniqueidentifier NULL,
        [CategoryId] uniqueidentifier NULL,
        [SubcategoryId] uniqueidentifier NULL,
        [OpenedAt] datetimeoffset NOT NULL,
        [AcknowledgedAt] datetimeoffset NULL,
        [AssignedAt] datetimeoffset NULL,
        [FirstResponseAt] datetimeoffset NULL,
        [PendingAt] datetimeoffset NULL,
        [ResolvedAt] datetimeoffset NULL,
        [ClosedAt] datetimeoffset NULL,
        [DueAt] datetimeoffset NULL,
        [SlaResponseDeadline] datetimeoffset NULL,
        [SlaResolutionDeadline] datetimeoffset NULL,
        [ReopenCount] int NOT NULL,
        [WasEscalated] bit NOT NULL,
        [TicketType] nvarchar(21) NOT NULL,
        [RiskLevel] nvarchar(16) NULL,
        [ChangeType] nvarchar(16) NULL,
        [PlannedStart] datetimeoffset NULL,
        [PlannedEnd] datetimeoffset NULL,
        [ActualStart] datetimeoffset NULL,
        [ActualEnd] datetimeoffset NULL,
        [CausedByProblemId] uniqueidentifier NULL,
        [ResolvedByChangeId] uniqueidentifier NULL,
        [KnownErrorArticleRef] nvarchar(max) NULL,
        [RootCause] nvarchar(max) NULL,
        [Workaround] nvarchar(max) NULL,
        [RequestedCatalogItemId] uniqueidentifier NULL,
        [ParentTicketId] uniqueidentifier NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Ticket] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Ticket_AssignmentGroups_AssignedGroupId] FOREIGN KEY ([AssignedGroupId]) REFERENCES [AssignmentGroups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_AssignmentGroups_ResolutionGroupId] FOREIGN KEY ([ResolutionGroupId]) REFERENCES [AssignmentGroups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Category_SubcategoryId] FOREIGN KEY ([SubcategoryId]) REFERENCES [Category] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Impact_ImpactId] FOREIGN KEY ([ImpactId]) REFERENCES [Impact] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Person_AffectedUserId] FOREIGN KEY ([AffectedUserId]) REFERENCES [Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Person_AssignedAgentId] FOREIGN KEY ([AssignedAgentId]) REFERENCES [Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Person_RequesterId] FOREIGN KEY ([RequesterId]) REFERENCES [Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Person_ResolverId] FOREIGN KEY ([ResolverId]) REFERENCES [Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Priority_PriorityId] FOREIGN KEY ([PriorityId]) REFERENCES [Priority] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Service_RequestedCatalogItemId] FOREIGN KEY ([RequestedCatalogItemId]) REFERENCES [Service] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Severity_SeverityId] FOREIGN KEY ([SeverityId]) REFERENCES [Severity] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Status_CurrentStatusId] FOREIGN KEY ([CurrentStatusId]) REFERENCES [Status] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Ticket_CausedByProblemId] FOREIGN KEY ([CausedByProblemId]) REFERENCES [Ticket] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Ticket_ParentTicketId] FOREIGN KEY ([ParentTicketId]) REFERENCES [Ticket] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Ticket_ResolvedByChangeId] FOREIGN KEY ([ResolvedByChangeId]) REFERENCES [Ticket] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ticket_Urgency_UrgencyId] FOREIGN KEY ([UrgencyId]) REFERENCES [Urgency] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [OperationalEvent] (
        [Id] uniqueidentifier NOT NULL,
        [TicketId] uniqueidentifier NOT NULL,
        [EventTimestamp] datetimeoffset NOT NULL,
        [PerformedById] uniqueidentifier NULL,
        [Comment] nvarchar(max) NULL,
        [EventType] nvarchar(21) NOT NULL,
        [FromGroupId] uniqueidentifier NULL,
        [ToGroupId] uniqueidentifier NULL,
        [FromAgentId] uniqueidentifier NULL,
        [ToAgentId] uniqueidentifier NULL,
        [ClosureCode] nvarchar(max) NULL,
        [Reason] nvarchar(max) NULL,
        [NewTier] nvarchar(max) NULL,
        [ReassignmentEvent_Reason] nvarchar(max) NULL,
        [ReopenEvent_Reason] nvarchar(max) NULL,
        [ResolutionCode] nvarchar(max) NULL,
        [ResolutionNotes] nvarchar(max) NULL,
        [FromStatusId] uniqueidentifier NULL,
        [ToStatusId] uniqueidentifier NULL,
        [TimeInFromStatus] time NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_OperationalEvent] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OperationalEvent_AssignmentGroups_FromGroupId] FOREIGN KEY ([FromGroupId]) REFERENCES [AssignmentGroups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationalEvent_AssignmentGroups_ToGroupId] FOREIGN KEY ([ToGroupId]) REFERENCES [AssignmentGroups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationalEvent_Person_FromAgentId] FOREIGN KEY ([FromAgentId]) REFERENCES [Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationalEvent_Person_PerformedById] FOREIGN KEY ([PerformedById]) REFERENCES [Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationalEvent_Person_ToAgentId] FOREIGN KEY ([ToAgentId]) REFERENCES [Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationalEvent_Status_FromStatusId] FOREIGN KEY ([FromStatusId]) REFERENCES [Status] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationalEvent_Status_ToStatusId] FOREIGN KEY ([ToStatusId]) REFERENCES [Status] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationalEvent_Ticket_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [SlaMeasurements] (
        [Id] uniqueidentifier NOT NULL,
        [TicketId] uniqueidentifier NOT NULL,
        [ServiceLevelAgreementId] uniqueidentifier NOT NULL,
        [StartTime] datetimeoffset NOT NULL,
        [StopTime] datetimeoffset NULL,
        [PauseDuration] time NOT NULL,
        [TargetDuration] time NOT NULL,
        [ElapsedDuration] time NULL,
        [BreachStatus] nvarchar(16) NOT NULL,
        [ExternalId] nvarchar(64) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_SlaMeasurements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SlaMeasurements_ServiceLevelAgreements_ServiceLevelAgreementId] FOREIGN KEY ([ServiceLevelAgreementId]) REFERENCES [ServiceLevelAgreements] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SlaMeasurements_Ticket_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [TicketAffectedCIs] (
        [TicketId] uniqueidentifier NOT NULL,
        [ConfigurationItemId] uniqueidentifier NOT NULL,
        [IsPrimary] bit NOT NULL,
        CONSTRAINT [PK_TicketAffectedCIs] PRIMARY KEY ([TicketId], [ConfigurationItemId]),
        CONSTRAINT [FK_TicketAffectedCIs_ConfigurationItem_ConfigurationItemId] FOREIGN KEY ([ConfigurationItemId]) REFERENCES [ConfigurationItem] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TicketAffectedCIs_Ticket_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [TicketAffectedServices] (
        [TicketId] uniqueidentifier NOT NULL,
        [ServiceId] uniqueidentifier NOT NULL,
        [IsPrimary] bit NOT NULL,
        CONSTRAINT [PK_TicketAffectedServices] PRIMARY KEY ([TicketId], [ServiceId]),
        CONSTRAINT [FK_TicketAffectedServices_Service_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Service] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TicketAffectedServices_Ticket_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE TABLE [TicketRelations] (
        [Id] uniqueidentifier NOT NULL,
        [SourceTicketId] uniqueidentifier NOT NULL,
        [TargetTicketId] uniqueidentifier NOT NULL,
        [RelationKind] nvarchar(32) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_TicketRelations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TicketRelations_Ticket_SourceTicketId] FOREIGN KEY ([SourceTicketId]) REFERENCES [Ticket] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TicketRelations_Ticket_TargetTicketId] FOREIGN KEY ([TargetTicketId]) REFERENCES [Ticket] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AgentGroupMemberships_AssignmentGroupId] ON [AgentGroupMemberships] ([AssignmentGroupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AssignmentGroups_ExternalId] ON [AssignmentGroups] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AssignmentGroups_Name] ON [AssignmentGroups] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BusinessCalendars_ExternalId] ON [BusinessCalendars] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BusinessCalendars_Name] ON [BusinessCalendars] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BusinessServiceConsumers_OrganizationId] ON [BusinessServiceConsumers] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CalendarHolidays_BusinessCalendarId_Date] ON [CalendarHolidays] ([BusinessCalendarId], [Date]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Category_Code] ON [Category] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Category_ExternalId] ON [Category] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Category_ParentCategoryId] ON [Category] ([ParentCategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ConfigurationItem_AssetTag] ON [ConfigurationItem] ([AssetTag]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ConfigurationItem_ExternalId] ON [ConfigurationItem] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ConfigurationItem_Name] ON [ConfigurationItem] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ConfigurationItem_OwnerId] ON [ConfigurationItem] ([OwnerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ConfigurationItemDependencies_TargetId] ON [ConfigurationItemDependencies] ([TargetId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ConfigurationItemSupports_TechnicalServiceId] ON [ConfigurationItemSupports] ([TechnicalServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Impact_Code] ON [Impact] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Impact_ExternalId] ON [Impact] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_ExternalId] ON [OperationalEvent] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_FromAgentId] ON [OperationalEvent] ([FromAgentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_FromGroupId] ON [OperationalEvent] ([FromGroupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_FromStatusId] ON [OperationalEvent] ([FromStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_PerformedById] ON [OperationalEvent] ([PerformedById]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_TicketId_EventTimestamp] ON [OperationalEvent] ([TicketId], [EventTimestamp]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_ToAgentId] ON [OperationalEvent] ([ToAgentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_ToGroupId] ON [OperationalEvent] ([ToGroupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OperationalEvent_ToStatusId] ON [OperationalEvent] ([ToStatusId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Organizations_ExternalId] ON [Organizations] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Organizations_ParentOrganizationId] ON [Organizations] ([ParentOrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Person_Email] ON [Person] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Person_ExternalId] ON [Person] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Person_OrganizationId] ON [Person] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Priority_Code] ON [Priority] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Priority_DerivedFromImpactId] ON [Priority] ([DerivedFromImpactId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Priority_DerivedFromUrgencyId] ON [Priority] ([DerivedFromUrgencyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Priority_ExternalId] ON [Priority] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Service_BusinessServiceId] ON [Service] ([BusinessServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Service_ExternalId] ON [Service] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Service_Name] ON [Service] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Service_OwnerId] ON [Service] ([OwnerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceLevelAgreements_AppliesToPriorityId] ON [ServiceLevelAgreements] ([AppliesToPriorityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceLevelAgreements_AppliesToServiceId] ON [ServiceLevelAgreements] ([AppliesToServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceLevelAgreements_BusinessCalendarId] ON [ServiceLevelAgreements] ([BusinessCalendarId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceLevelAgreements_ExternalId] ON [ServiceLevelAgreements] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceLevelAgreements_Name] ON [ServiceLevelAgreements] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceSupportGroups_AssignmentGroupId] ON [ServiceSupportGroups] ([AssignmentGroupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Severity_Code] ON [Severity] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Severity_ExternalId] ON [Severity] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SlaMeasurements_ExternalId] ON [SlaMeasurements] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SlaMeasurements_ServiceLevelAgreementId] ON [SlaMeasurements] ([ServiceLevelAgreementId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SlaMeasurements_TicketId_ServiceLevelAgreementId] ON [SlaMeasurements] ([TicketId], [ServiceLevelAgreementId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Status_Code] ON [Status] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Status_ExternalId] ON [Status] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TechnicalServiceEnablements_BusinessServiceId] ON [TechnicalServiceEnablements] ([BusinessServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_AffectedUserId] ON [Ticket] ([AffectedUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_AssignedAgentId] ON [Ticket] ([AssignedAgentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_AssignedGroupId] ON [Ticket] ([AssignedGroupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_CategoryId] ON [Ticket] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_CausedByProblemId] ON [Ticket] ([CausedByProblemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_ClosedAt] ON [Ticket] ([ClosedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_CurrentStatusId_PriorityId] ON [Ticket] ([CurrentStatusId], [PriorityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_ExternalId] ON [Ticket] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_ImpactId] ON [Ticket] ([ImpactId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Ticket_Number] ON [Ticket] ([Number]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_OpenedAt] ON [Ticket] ([OpenedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_ParentTicketId] ON [Ticket] ([ParentTicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_PriorityId] ON [Ticket] ([PriorityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_RequestedCatalogItemId] ON [Ticket] ([RequestedCatalogItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_RequesterId] ON [Ticket] ([RequesterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_ResolutionGroupId] ON [Ticket] ([ResolutionGroupId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_ResolvedAt] ON [Ticket] ([ResolvedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_ResolvedByChangeId] ON [Ticket] ([ResolvedByChangeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_ResolverId] ON [Ticket] ([ResolverId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_SeverityId] ON [Ticket] ([SeverityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_SubcategoryId] ON [Ticket] ([SubcategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ticket_UrgencyId] ON [Ticket] ([UrgencyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TicketAffectedCIs_ConfigurationItemId] ON [TicketAffectedCIs] ([ConfigurationItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TicketAffectedServices_ServiceId] ON [TicketAffectedServices] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TicketRelations_SourceTicketId_TargetTicketId_RelationKind] ON [TicketRelations] ([SourceTicketId], [TargetTicketId], [RelationKind]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TicketRelations_TargetTicketId] ON [TicketRelations] ([TargetTicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Urgency_Code] ON [Urgency] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Urgency_ExternalId] ON [Urgency] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Vendors_ExternalId] ON [Vendors] ([ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915075845_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260915075845_InitialCreate', N'10.0.12');
END;

COMMIT;
GO

