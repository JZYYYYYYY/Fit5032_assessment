
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/28/2023 14:18:21
-- Generated from EDMX file: D:\Projects\Visual Studio\WebApplication1\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Database1];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[CashierSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CashierSet];
GO
IF OBJECT_ID(N'[dbo].[DoctorSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DoctorSet];
GO
IF OBJECT_ID(N'[dbo].[PatientSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PatientSet];
GO
IF OBJECT_ID(N'[dbo].[ProcessSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProcessSet];
GO
IF OBJECT_ID(N'[dbo].[RoomSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoomSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'CashierSet'
CREATE TABLE [dbo].[CashierSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [pwd] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'DoctorSet'
CREATE TABLE [dbo].[DoctorSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [pwd] nvarchar(max)  NOT NULL,
    [endTime] datetime  NOT NULL
);
GO

-- Creating table 'PatientSet'
CREATE TABLE [dbo].[PatientSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [pwd] nvarchar(max)  NOT NULL,
    [confirm_pwd] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ProcessSet'
CREATE TABLE [dbo].[ProcessSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Patient] nvarchar(max)  NOT NULL,
    [Doctor] nvarchar(max)  NULL,
    [Room] nvarchar(max)  NULL,
    [startTime] datetime  NULL,
    [payment] nvarchar(max)  NOT NULL,
    [bodyPart] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RoomSet'
CREATE TABLE [dbo].[RoomSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [number] nvarchar(max)  NOT NULL,
    [endTime] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'CashierSet'
ALTER TABLE [dbo].[CashierSet]
ADD CONSTRAINT [PK_CashierSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DoctorSet'
ALTER TABLE [dbo].[DoctorSet]
ADD CONSTRAINT [PK_DoctorSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PatientSet'
ALTER TABLE [dbo].[PatientSet]
ADD CONSTRAINT [PK_PatientSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProcessSet'
ALTER TABLE [dbo].[ProcessSet]
ADD CONSTRAINT [PK_ProcessSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RoomSet'
ALTER TABLE [dbo].[RoomSet]
ADD CONSTRAINT [PK_RoomSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------