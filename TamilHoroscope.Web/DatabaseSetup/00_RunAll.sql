-- =====================================================
-- MASTER SCRIPT: RUN ALL DATABASE SETUP
-- Executes all setup scripts in sequence
-- =====================================================

USE master;
GO

PRINT '======================================';
PRINT 'TAMILHOROSCOPE DATABASE SETUP';
PRINT 'Complete Fresh Installation';
PRINT '======================================';
PRINT '';
PRINT 'Starting setup process...';
PRINT '';

-- Step 1: Create Database
:r "01_CreateDatabase.sql"

-- Wait for database to be ready
WAITFOR DELAY '00:00:02';

-- Step 2: Create Tables
:r "02_CreateTables.sql"

-- Step 3: Create Indexes
:r "03_CreateIndexes.sql"

-- Step 4: Insert Initial Data
:r "04_InsertInitialData.sql"

-- Step 5: Verify Setup
:r "05_VerifySetup.sql"

PRINT '';
PRINT '======================================';
PRINT '? SETUP COMPLETE!';
PRINT '======================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Update connection string in appsettings.json';
PRINT '2. Run the application: dotnet run';
PRINT '3. Login with:';
PRINT '   Email: rameshsivaperumal@gmail.com';
PRINT '   Password: Test@4321';
PRINT '';
GO
