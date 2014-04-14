
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/03/2014 04:45:25
-- Generated from EDMX file: E:\Dropbox\.NET Prosjekt\ServerWBSCKTest\ServerWBSCKTest\ServerWBSCKTest\Model\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [souls];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[fk_ability_]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[db_Player_Type] DROP CONSTRAINT [fk_ability_];
GO
IF OBJECT_ID(N'[dbo].[fk_card_type]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[db_Card] DROP CONSTRAINT [fk_card_type];
GO
IF OBJECT_ID(N'[dbo].[fk_play_id]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[db_Player_Login] DROP CONSTRAINT [fk_play_id];
GO
IF OBJECT_ID(N'[dbo].[fk_player_type]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[db_Player] DROP CONSTRAINT [fk_player_type];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[db_Ability]', 'U') IS NOT NULL
    DROP TABLE [dbo].[db_Ability];
GO
IF OBJECT_ID(N'[dbo].[db_Card]', 'U') IS NOT NULL
    DROP TABLE [dbo].[db_Card];
GO
IF OBJECT_ID(N'[dbo].[db_Card_Type]', 'U') IS NOT NULL
    DROP TABLE [dbo].[db_Card_Type];
GO
IF OBJECT_ID(N'[dbo].[db_Game]', 'U') IS NOT NULL
    DROP TABLE [dbo].[db_Game];
GO
IF OBJECT_ID(N'[dbo].[db_Player]', 'U') IS NOT NULL
    DROP TABLE [dbo].[db_Player];
GO
IF OBJECT_ID(N'[dbo].[db_Player_Login]', 'U') IS NOT NULL
    DROP TABLE [dbo].[db_Player_Login];
GO
IF OBJECT_ID(N'[dbo].[db_Player_Type]', 'U') IS NOT NULL
    DROP TABLE [dbo].[db_Player_Type];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'db_Ability'
CREATE TABLE [dbo].[db_Ability] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] varchar(255)  NOT NULL,
    [parameter] varchar(255)  NULL
);
GO

-- Creating table 'db_Card'
CREATE TABLE [dbo].[db_Card] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] varchar(255)  NOT NULL,
    [attack] int  NOT NULL,
    [health] int  NOT NULL,
    [armor] int  NOT NULL,
    [fk_ability] int  NOT NULL,
    [fk_type] int  NOT NULL,
    [cost] int  NOT NULL
);
GO

-- Creating table 'db_Game'
CREATE TABLE [dbo].[db_Game] (
    [id] int IDENTITY(1,1) NOT NULL,
    [fk_player1] int  NOT NULL,
    [fk_player2] int  NOT NULL
);
GO

-- Creating table 'db_Player'
CREATE TABLE [dbo].[db_Player] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] varchar(255)  NOT NULL,
    [password] varchar(255)  NOT NULL,
    [rank] int  NOT NULL,
    [timestamp] binary(8)  NULL,
    [fk_type] int  NULL
);
GO

-- Creating table 'db_Player_Login'
CREATE TABLE [dbo].[db_Player_Login] (
    [fk_player_id] int  NOT NULL,
    [ip] varchar(255)  NOT NULL,
    [logged_in_at] bigint  NOT NULL,
    [id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'db_Player_Type'
CREATE TABLE [dbo].[db_Player_Type] (
    [id] int  NOT NULL,
    [name] varchar(255)  NOT NULL,
    [attack] int  NOT NULL,
    [armor] int  NOT NULL,
    [health] int  NOT NULL,
    [fk_ability] int  NOT NULL
);
GO

-- Creating table 'db_Card_Type'
CREATE TABLE [dbo].[db_Card_Type] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] varchar(255)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'db_Ability'
ALTER TABLE [dbo].[db_Ability]
ADD CONSTRAINT [PK_db_Ability]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'db_Card'
ALTER TABLE [dbo].[db_Card]
ADD CONSTRAINT [PK_db_Card]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'db_Game'
ALTER TABLE [dbo].[db_Game]
ADD CONSTRAINT [PK_db_Game]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'db_Player'
ALTER TABLE [dbo].[db_Player]
ADD CONSTRAINT [PK_db_Player]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'db_Player_Login'
ALTER TABLE [dbo].[db_Player_Login]
ADD CONSTRAINT [PK_db_Player_Login]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'db_Player_Type'
ALTER TABLE [dbo].[db_Player_Type]
ADD CONSTRAINT [PK_db_Player_Type]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'db_Card_Type'
ALTER TABLE [dbo].[db_Card_Type]
ADD CONSTRAINT [PK_db_Card_Type]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [fk_ability] in table 'db_Player_Type'
ALTER TABLE [dbo].[db_Player_Type]
ADD CONSTRAINT [fk_ability_]
    FOREIGN KEY ([fk_ability])
    REFERENCES [dbo].[db_Ability]
        ([id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_ability_'
CREATE INDEX [IX_fk_ability_]
ON [dbo].[db_Player_Type]
    ([fk_ability]);
GO

-- Creating foreign key on [fk_player_id] in table 'db_Player_Login'
ALTER TABLE [dbo].[db_Player_Login]
ADD CONSTRAINT [fk_play_id]
    FOREIGN KEY ([fk_player_id])
    REFERENCES [dbo].[db_Player]
        ([id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_play_id'
CREATE INDEX [IX_fk_play_id]
ON [dbo].[db_Player_Login]
    ([fk_player_id]);
GO

-- Creating foreign key on [fk_type] in table 'db_Player'
ALTER TABLE [dbo].[db_Player]
ADD CONSTRAINT [fk_player_type]
    FOREIGN KEY ([fk_type])
    REFERENCES [dbo].[db_Player_Type]
        ([id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_player_type'
CREATE INDEX [IX_fk_player_type]
ON [dbo].[db_Player]
    ([fk_type]);
GO

-- Creating foreign key on [fk_type] in table 'db_Card'
ALTER TABLE [dbo].[db_Card]
ADD CONSTRAINT [fk_card_type]
    FOREIGN KEY ([fk_type])
    REFERENCES [dbo].[db_Card_Type]
        ([id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_card_type'
CREATE INDEX [IX_fk_card_type]
ON [dbo].[db_Card]
    ([fk_type]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------