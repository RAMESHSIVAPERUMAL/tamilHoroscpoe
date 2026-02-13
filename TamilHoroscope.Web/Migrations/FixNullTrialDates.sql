-- =====================================================
-- EMERGENCY FIX: Fix NULL Trial Dates
-- This is causing the SqlNullValueException
-- =====================================================

USE [TamilHoroscope];
GO

PRINT '======================================';
PRINT 'EMERGENCY FIX: NULL TRIAL DATES';
PRINT '======================================';
PRINT '';

-- Check for NULL trial dates
PRINT '1. Checking for NULL trial dates...';
SELECT 
    UserId,
    Email,
    TrialStartDate,
    TrialEndDate,
    CASE 
        WHEN TrialStartDate IS NULL THEN '? NULL (PROBLEM!)'
        ELSE '? Has value'
    END AS TrialStartStatus,
    CASE 
        WHEN TrialEndDate IS NULL THEN '? NULL (PROBLEM!)'
        ELSE '? Has value'
    END AS TrialEndStatus
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

PRINT '';
PRINT '2. Fixing NULL trial dates...';

-- Update NULL trial dates
UPDATE Users
SET 
    TrialStartDate = COALESCE(TrialStartDate, CreatedDate, GETUTCDATE()),
    TrialEndDate = COALESCE(TrialEndDate, DATEADD(day, 30, COALESCE(TrialStartDate, CreatedDate, GETUTCDATE()))),
    IsTrialActive = 1
WHERE Email = 'rameshsivaperumal@gmail.com'
    AND (TrialStartDate IS NULL OR TrialEndDate IS NULL);

IF @@ROWCOUNT > 0
    PRINT '   ? Fixed NULL trial dates';
ELSE
    PRINT '   ? No NULL dates to fix';

PRINT '';
PRINT '3. Verifying fix...';

SELECT 
    UserId,
    Email,
    FullName,
    IsActive,
    IsTrialActive,
    TrialStartDate,
    TrialEndDate,
    CASE 
        WHEN TrialStartDate IS NOT NULL AND TrialEndDate IS NOT NULL 
        THEN '? ALL DATES SET'
        ELSE '? STILL HAS NULLS'
    END AS Status,
    CASE 
        WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' 
        THEN '? Password is Test@4321'
        ELSE '? Password is different'
    END AS PasswordCheck
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

PRINT '';
PRINT '======================================';
PRINT 'FIX COMPLETE';
PRINT '======================================';
PRINT '';
PRINT '? Your user should now be able to login!';
PRINT '';
PRINT 'Credentials:';
PRINT '  Email: rameshsivaperumal@gmail.com';
PRINT '  Password: Test@4321';
PRINT '';

GO
