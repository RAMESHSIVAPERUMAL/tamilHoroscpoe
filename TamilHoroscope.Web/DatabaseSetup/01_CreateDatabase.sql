-- =====================================================
-- STEP 1: DROP AND CREATE DATABASE
-- Complete fresh start for TamilHoroscope
-- =====================================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'TamilHoroscope')
BEGIN
    PRINT 'Dropping existing TamilHoroscope database...';
    
    -- Set database to single user to close all connections
    ALTER DATABASE TamilHoroscope SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    -- Drop the database
    DROP DATABASE TamilHoroscope;
    
    PRINT '? Database dropped successfully';
END
ELSE
BEGIN
    PRINT 'Database does not exist, creating new...';
END
GO

-- Create new database
CREATE DATABASE TamilHoroscope
ON PRIMARY 
(
    NAME = N'TamilHoroscope_Data',
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\TamilHoroscope.mdf',
    SIZE = 100MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10MB
)
LOG ON 
(
    NAME = N'TamilHoroscope_Log',
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\TamilHoroscope_Log.ldf',
    SIZE = 50MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10MB
);
GO

PRINT '? Database created successfully';
PRINT '';
PRINT '======================================';
PRINT 'DATABASE: TamilHoroscope';
PRINT 'STATUS: Created and Ready';
PRINT '======================================';
GO
