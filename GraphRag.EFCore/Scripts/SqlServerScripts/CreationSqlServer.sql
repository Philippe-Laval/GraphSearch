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
    WHERE [MigrationId] = N'20260910154016_InitialCreate'
)
BEGIN
    CREATE TABLE [GraphEntity] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(450) NOT NULL,
        [Type] nvarchar(450) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_GraphEntity] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910154016_InitialCreate'
)
BEGIN
    CREATE TABLE [GraphEntityAlias] (
        [EntityId] int NOT NULL,
        [Alias] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_GraphEntityAlias] PRIMARY KEY ([EntityId], [Alias]),
        CONSTRAINT [FK_GraphEntityAlias_GraphEntity_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [GraphEntity] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910154016_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_GraphEntity_Name] ON [GraphEntity] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910154016_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_GraphEntity_Type] ON [GraphEntity] ([Type]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910154016_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_GraphEntityAlias_Alias] ON [GraphEntityAlias] ([Alias]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910154016_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260910154016_InitialCreate', N'10.0.12');
END;

COMMIT;
GO

