BEGIN TRANSACTION;
DROP TABLE [AgentGroupMemberships];

DROP TABLE [BusinessServiceConsumers];

DROP TABLE [CalendarHolidays];

DROP TABLE [ConfigurationItemDependencies];

DROP TABLE [ConfigurationItemSupports];

DROP TABLE [OperationalEvent];

DROP TABLE [ServiceSupportGroups];

DROP TABLE [SlaMeasurements];

DROP TABLE [TechnicalServiceEnablements];

DROP TABLE [TicketAffectedCIs];

DROP TABLE [TicketAffectedServices];

DROP TABLE [TicketRelations];

DROP TABLE [Vendors];

DROP TABLE [ServiceLevelAgreements];

DROP TABLE [ConfigurationItem];

DROP TABLE [Ticket];

DROP TABLE [BusinessCalendars];

DROP TABLE [AssignmentGroups];

DROP TABLE [Category];

DROP TABLE [Priority];

DROP TABLE [Service];

DROP TABLE [Severity];

DROP TABLE [Status];

DROP TABLE [Impact];

DROP TABLE [Urgency];

DROP TABLE [Person];

DROP TABLE [Organizations];

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20260915075845_InitialCreate';

COMMIT;
GO

