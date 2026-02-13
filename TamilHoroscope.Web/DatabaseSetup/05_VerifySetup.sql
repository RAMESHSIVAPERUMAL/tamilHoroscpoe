-- =====================================================
-- STEP 5: VERIFY DATABASE SETUP
-- Check everything was created correctly
-- =====================================================

USE TamilHoroscope;
GO

PRINT '======================================';
PRINT 'DATABASE VERIFICATION';
PRINT '======================================';
PRINT '';

-- =====================================================
-- Check Tables
-- =====================================================
PRINT '1. Tables:';
SELECT 
    TABLE_NAME,
    '? Created' AS Status
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

PRINT '';

-- =====================================================
-- Check Indexes
-- =====================================================
PRINT '2. Indexes:';
SELECT 
    OBJECT_NAME(object_id) AS TableName,
    name AS IndexName,
    '? Created' AS Status
FROM sys.indexes
WHERE object_id IN (
    OBJECT_ID('Users'),
    OBJECT_ID('Wallets'),
    OBJECT_ID('Transactions'),
    OBJECT_ID('HoroscopeGenerations'),
    OBJECT_ID('SystemConfig')
)
AND name IS NOT NULL
AND is_primary_key = 0
ORDER BY TableName, IndexName;

PRINT '';

-- =====================================================
-- Check Users Table Schema
-- =====================================================
PRINT '3. Users Table Schema:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CASE 
        WHEN COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate') 
        THEN CASE WHEN IS_NULLABLE = 'YES' THEN '? Nullable (Correct)' ELSE '? Not Nullable' END
        ELSE 'OK'
    END AS NullableStatus
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- =====================================================
-- Check System Configuration
-- =====================================================
PRINT '4. System Configuration:';
SELECT 
    ConfigKey,
    ConfigValue,
    DataType,
    '? Active' AS Status
FROM SystemConfig
WHERE IsActive = 1
ORDER BY ConfigKey;

PRINT '';

-- =====================================================
-- Check Test User
-- =====================================================
PRINT '5. Test User Account:';
SELECT 
    UserId,
    Email,
    FullName,
    CASE WHEN IsActive = 1 THEN '? Active' ELSE '? Inactive' END AS AccountStatus,
    CASE WHEN IsTrialActive = 1 THEN '? Active' ELSE '? Inactive' END AS TrialStatus,
    FORMAT(TrialStartDate, 'yyyy-MM-dd HH:mm') AS TrialStart,
    FORMAT(TrialEndDate, 'yyyy-MM-dd HH:mm') AS TrialEnd,
    DATEDIFF(day, GETUTCDATE(), TrialEndDate) AS DaysRemaining,
    CASE 
        WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' 
        THEN '? Test@4321' 
        ELSE '? Different'
    END AS PasswordCheck
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

PRINT '';

-- =====================================================
-- Check Wallet
-- =====================================================
PRINT '6. Test User Wallet:';
SELECT 
    w.WalletId,
    w.UserId,
    u.Email,
    w.Balance,
    FORMAT(w.CreatedDate, 'yyyy-MM-dd HH:mm') AS Created,
    '? Ready' AS Status
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.UserId
WHERE u.Email = 'rameshsivaperumal@gmail.com';

PRINT '';

-- =====================================================
-- Summary
-- =====================================================
DECLARE @TableCount INT = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE');
DECLARE @IndexCount INT = (SELECT COUNT(*) FROM sys.indexes WHERE object_id IN (
    OBJECT_ID('Users'), OBJECT_ID('Wallets'), OBJECT_ID('Transactions'),
    OBJECT_ID('HoroscopeGenerations'), OBJECT_ID('SystemConfig')
) AND name IS NOT NULL AND is_primary_key = 0);
DECLARE @ConfigCount INT = (SELECT COUNT(*) FROM SystemConfig WHERE IsActive = 1);
DECLARE @UserCount INT = (SELECT COUNT(*) FROM Users);

PRINT '======================================';
PRINT 'VERIFICATION SUMMARY';
PRINT '======================================';
PRINT '';
PRINT '? Tables created: ' + CAST(@TableCount AS NVARCHAR(10));
PRINT '? Indexes created: ' + CAST(@IndexCount AS NVARCHAR(10));
PRINT '? System configs: ' + CAST(@ConfigCount AS NVARCHAR(10));
PRINT '? Users created: ' + CAST(@UserCount AS NVARCHAR(10));
PRINT '';
PRINT '======================================';
PRINT 'DATABASE SETUP COMPLETE!';
PRINT '======================================';
PRINT '';
PRINT 'Ready to use with:';
PRINT '  Email: rameshsivaperumal@gmail.com';
PRINT '  Password: Test@4321';
PRINT '';
GO
