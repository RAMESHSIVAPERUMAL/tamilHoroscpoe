-- =====================================================
-- Migration: Make Trial Dates Nullable
-- Date: 2024-02-10
-- Description: Make TrialStartDate and TrialEndDate 
--              nullable and add LastDailyFeeDeductionDate
-- =====================================================

USE [TamilHoroscope]; -- Replace with your database name
GO

PRINT 'Starting migration: MakeTrialDatesNullable';
GO

-- Step 1: Make TrialStartDate nullable
PRINT 'Step 1: Making TrialStartDate nullable...';
ALTER TABLE Users
ALTER COLUMN TrialStartDate datetime2 NULL;
GO

PRINT 'TrialStartDate is now nullable ?';
GO

-- Step 2: Make TrialEndDate nullable
PRINT 'Step 2: Making TrialEndDate nullable...';
ALTER TABLE Users
ALTER COLUMN TrialEndDate datetime2 NULL;
GO

PRINT 'TrialEndDate is now nullable ?';
GO

-- Step 3: Add LastDailyFeeDeductionDate column (if not exists)
PRINT 'Step 3: Adding LastDailyFeeDeductionDate column...';
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'Users') 
    AND name = 'LastDailyFeeDeductionDate'
)
BEGIN
    ALTER TABLE Users
    ADD LastDailyFeeDeductionDate datetime2 NULL;
    
    PRINT 'LastDailyFeeDeductionDate column added ?';
END
ELSE
BEGIN
    PRINT 'LastDailyFeeDeductionDate column already exists ?';
END
GO

-- Step 4: Verify changes
PRINT 'Step 4: Verifying changes...';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CASE 
        WHEN IS_NULLABLE = 'YES' THEN '? NULLABLE'
        ELSE '? NOT NULL'
    END AS Status
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
    AND COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate')
ORDER BY COLUMN_NAME;
GO

-- Step 5: Check sample data
PRINT 'Step 5: Checking sample data...';
SELECT TOP 5
    UserId,
    FullName,
    TrialStartDate,
    TrialEndDate,
    IsTrialActive,
    LastDailyFeeDeductionDate,
    CreatedDate
FROM Users
ORDER BY CreatedDate DESC;
GO

PRINT 'Migration completed successfully! ?';
GO

-- =====================================================
-- Optional: Update existing users with NULL dates
-- (Uncomment if you want to clear dates for inactive trials)
-- =====================================================

/*
PRINT 'Optional: Updating existing users...';

-- Set TrialStartDate and TrialEndDate to NULL for inactive trials
UPDATE Users
SET 
    TrialStartDate = NULL,
    TrialEndDate = NULL
WHERE IsTrialActive = 0
    AND TrialEndDate < GETUTCDATE(); -- Only for expired trials

PRINT 'Existing users updated ?';
GO
*/

-- =====================================================
-- Rollback Script (Save this for emergency use)
-- =====================================================

/*
-- ROLLBACK: Make columns NOT NULL again
PRINT 'ROLLBACK: Making columns NOT NULL...';

-- First, update NULL values to valid dates
UPDATE Users
SET TrialStartDate = COALESCE(TrialStartDate, CreatedDate)
WHERE TrialStartDate IS NULL;

UPDATE Users
SET TrialEndDate = COALESCE(TrialEndDate, DATEADD(day, 30, COALESCE(TrialStartDate, CreatedDate)))
WHERE TrialEndDate IS NULL;

-- Then make columns NOT NULL
ALTER TABLE Users
ALTER COLUMN TrialStartDate datetime2 NOT NULL;

ALTER TABLE Users
ALTER COLUMN TrialEndDate datetime2 NOT NULL;

-- Optionally remove LastDailyFeeDeductionDate
-- ALTER TABLE Users
-- DROP COLUMN LastDailyFeeDeductionDate;

PRINT 'Rollback completed ?';
GO
*/
