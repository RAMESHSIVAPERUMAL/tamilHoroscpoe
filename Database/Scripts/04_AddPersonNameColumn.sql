-- Add PersonName column to HoroscopeGenerations table
-- Migration Script - Add PersonName Column

USE [TamilHoroscope]
GO

-- Check if column already exists
IF NOT EXISTS (SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[HoroscopeGenerations]') 
    AND name = 'PersonName')
BEGIN
    ALTER TABLE [dbo].[HoroscopeGenerations]
    ADD [PersonName] NVARCHAR(100) NULL;
    
    PRINT 'PersonName column added successfully';
END
ELSE
BEGIN
    PRINT 'PersonName column already exists';
END
GO

-- Add index for searching by PersonName
IF NOT EXISTS (SELECT * FROM sys.indexes 
    WHERE name = 'IX_HoroscopeGenerations_PersonName' 
    AND object_id = OBJECT_ID('[dbo].[HoroscopeGenerations]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_HoroscopeGenerations_PersonName]
    ON [dbo].[HoroscopeGenerations] ([PersonName])
    INCLUDE ([UserId], [GenerationDate], [PlaceName], [BirthDateTime]);
    
    PRINT 'Index IX_HoroscopeGenerations_PersonName created successfully';
END
ELSE
BEGIN
    PRINT 'Index IX_HoroscopeGenerations_PersonName already exists';
END
GO
