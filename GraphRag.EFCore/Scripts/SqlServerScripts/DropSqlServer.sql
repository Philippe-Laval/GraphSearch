BEGIN TRANSACTION;
DROP TABLE [GraphEntityAlias];

DROP TABLE [GraphEntity];

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20260910154016_InitialCreate';

COMMIT;
GO

