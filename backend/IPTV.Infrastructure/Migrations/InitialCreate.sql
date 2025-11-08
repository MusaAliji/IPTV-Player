-- IPTV Database Initial Creation Script
-- This script creates all tables for the IPTV Player application
-- Run this script on your SQL Server instance if you prefer manual database setup

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'IPTVDb')
BEGIN
    CREATE DATABASE IPTVDb;
END
GO

USE IPTVDb;
GO

-- Create Channels table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Channels')
BEGIN
    CREATE TABLE Channels (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        StreamUrl NVARCHAR(500) NOT NULL,
        LogoUrl NVARCHAR(MAX) NULL,
        ChannelNumber INT NOT NULL,
        Category NVARCHAR(50) NULL,
        Language NVARCHAR(50) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Create Contents table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Contents')
BEGIN
    CREATE TABLE Contents (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        StreamUrl NVARCHAR(500) NOT NULL,
        ThumbnailUrl NVARCHAR(MAX) NULL,
        Type INT NOT NULL, -- 0=LiveTV, 1=VOD, 2=Series, 3=Movie
        Duration INT NULL,
        ReleaseDate DATETIME2 NULL,
        Genre NVARCHAR(50) NULL,
        Rating FLOAT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        FullName NVARCHAR(100) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        LastLoginAt DATETIME2 NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        Role INT NOT NULL DEFAULT 0, -- 0=User, 1=Premium, 2=Admin
        CONSTRAINT UQ_Users_Username UNIQUE (Username),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END
GO

-- Create EPGPrograms table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EPGPrograms')
BEGIN
    CREATE TABLE EPGPrograms (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ChannelId INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        StartTime DATETIME2 NOT NULL,
        EndTime DATETIME2 NOT NULL,
        Category NVARCHAR(50) NULL,
        Rating NVARCHAR(10) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_EPGPrograms_Channels FOREIGN KEY (ChannelId)
            REFERENCES Channels(Id) ON DELETE CASCADE
    );

    -- Create index on ChannelId for better query performance
    CREATE INDEX IX_EPGPrograms_ChannelId ON EPGPrograms(ChannelId);
END
GO

-- Create indexes on Users table if they don't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);
END
GO

PRINT 'Database IPTVDb created successfully!';
PRINT 'Tables created: Channels, Contents, Users, EPGPrograms';
GO
