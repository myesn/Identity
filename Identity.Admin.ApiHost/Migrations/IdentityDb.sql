IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Organizations] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [Order] int NOT NULL,
    [CreateTime] datetime NOT NULL,
    [ParentId] uniqueidentifier NULL,
    CONSTRAINT [PK_Organizations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Organizations_Organizations_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [AccountName] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Mobile] nvarchar(max) NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Avatar] varbinary(max) NULL,
    [IsEnabled] nvarchar(1) NOT NULL,
    [CreateTime] datetime NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [OrganizationUsers] (
    [OrganizationId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Order] int NOT NULL,
    [IsPrimary] nvarchar(1) NOT NULL,
    CONSTRAINT [PK_OrganizationUsers] PRIMARY KEY ([OrganizationId], [UserId]),
    CONSTRAINT [FK_OrganizationUsers_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrganizationUsers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Tokens] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [IssueTime] datetime NOT NULL,
    [ExpiredTime] datetime NOT NULL,
    CONSTRAINT [PK_Tokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_Organizations_ParentId] ON [Organizations] ([ParentId]);

GO

CREATE INDEX [IX_OrganizationUsers_UserId] ON [OrganizationUsers] ([UserId]);

GO

CREATE INDEX [IX_Tokens_UserId] ON [Tokens] ([UserId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190429065448_InitialIdentityDbContextMigration', N'2.2.4-servicing-10062');

GO

