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
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    CREATE TABLE [CommunityMessages] (
        [Id] int NOT NULL IDENTITY,
        [Message] nvarchar(max) NULL,
        [EmployeeId] int NOT NULL,
        [EmployeeName] nvarchar(max) NULL,
        [RecieverId] int NULL,
        [DepartmentId] int NOT NULL,
        [UserType] nvarchar(max) NULL,
        [IsSeen] bit NOT NULL,
        [MessageDate] datetime2 NOT NULL,
        CONSTRAINT [PK_CommunityMessages] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    CREATE TABLE [Departments] (
        [Id] int NOT NULL IDENTITY,
        [DepartmentName] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [EmployeeType] int NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [City] nvarchar(max) NOT NULL,
        [Country] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [DepartmentID] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_Departments_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [Departments] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    CREATE TABLE [Logins] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [EmployeeID] int NOT NULL,
        CONSTRAINT [PK_Logins] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Logins_Employees_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    CREATE TABLE [Todos] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [IsCompleted] bit NOT NULL,
        [AssignBy] int NULL,
        [EmployeeId] int NULL,
        [DepartmentId] int NOT NULL,
        CONSTRAINT [PK_Todos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Todos_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    CREATE INDEX [IX_Employees_DepartmentID] ON [Employees] ([DepartmentID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    CREATE INDEX [IX_Logins_EmployeeID] ON [Logins] ([EmployeeID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    CREATE INDEX [IX_Todos_EmployeeId] ON [Todos] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129130321_UpdateEmpDB'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240129130321_UpdateEmpDB', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129140021_UpdateTodo'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Todos]') AND [c].[name] = N'DepartmentId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Todos] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Todos] ALTER COLUMN [DepartmentId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129140021_UpdateTodo'
)
BEGIN
    ALTER TABLE [Todos] ADD [AssignById] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240129140021_UpdateTodo'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240129140021_UpdateTodo', N'8.0.0');
END;
GO

COMMIT;
GO

