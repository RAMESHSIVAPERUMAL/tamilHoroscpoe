-- =============================================
-- Add PersonName Column to HoroscopeGenerations Table
-- Description: Adds PersonName column to track the person's name
--              for whom the horoscope was generated
-- =============================================

USE [TamilHoroscope];
GO

-- Check if the column already exists
IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[HoroscopeGenerations]') 
    AND name = 'PersonName'
)
BEGIN
    -- Add the PersonName column
    ALTER TABLE [dbo].[HoroscopeGenerations]
    ADD [PersonName] NVARCHAR(100) NULL;
    
    PRINT 'Column [PersonName] added successfully to [HoroscopeGenerations] table.';
END
ELSE
BEGIN
    PRINT 'Column [PersonName] already exists in [HoroscopeGenerations] table.';
END
GO

PRINT '';
PRINT '=============================================';
PRINT 'Migration completed successfully!';
PRINT 'You can now generate horoscopes with PersonName field.';
PRINT '=============================================';
GO
