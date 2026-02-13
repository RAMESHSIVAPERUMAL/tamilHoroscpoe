-- Verification Script - Check if PersonName column was added
-- Run this in SSMS to verify the migration

USE [TamilHoroscope]
GO

-- Check if PersonName column exists
PRINT '========================================';
PRINT 'Checking PersonName Column...';
PRINT '========================================';
PRINT '';

SELECT 
    COLUMN_NAME as 'Column Name',
    DATA_TYPE as 'Data Type',
    CHARACTER_MAXIMUM_LENGTH as 'Max Length',
    IS_NULLABLE as 'Nullable'
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'HoroscopeGenerations' 
  AND COLUMN_NAME = 'PersonName';

PRINT '';
PRINT 'If you see a result above, PersonName column exists!';
PRINT '';

-- Check if index exists
PRINT '========================================';
PRINT 'Checking PersonName Index...';
PRINT '========================================';
PRINT '';

SELECT 
    i.name as 'Index Name',
    i.type_desc as 'Index Type',
    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) as 'Indexed Columns'
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('HoroscopeGenerations')
  AND i.name = 'IX_HoroscopeGenerations_PersonName'
GROUP BY i.name, i.type_desc;

PRINT '';
PRINT 'If you see a result above, the index exists!';
PRINT '';

-- Show sample data from HoroscopeGenerations
PRINT '========================================';
PRINT 'Current HoroscopeGenerations Structure:';
PRINT '========================================';
PRINT '';

-- Show column structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'HoroscopeGenerations'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '========================================';
PRINT 'Sample Data (Top 5 rows):';
PRINT '========================================';
PRINT '';

-- Show sample data
SELECT TOP 5
    GenerationId,
    UserId,
    PersonName,
    BirthDateTime,
    PlaceName,
    CreatedDateTime
FROM HoroscopeGenerations
ORDER BY CreatedDateTime DESC;

PRINT '';
PRINT 'PersonName values shown above (will be NULL for old records)';
PRINT '';
