-- =====================================================
-- DIAGNOSTIC SCRIPT: Troubleshoot Login Issues
-- After Making Trial Dates Nullable
-- =====================================================

USE [TamilHoroscope]; -- Replace with your database name
GO

PRINT '======================================';
PRINT 'DIAGNOSTIC: Login Issue Investigation';
PRINT '======================================';
PRINT '';

-- Step 1: Check if Users table exists
PRINT '1. Checking Users table...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    PRINT '   ? Users table exists';
END
ELSE
BEGIN
    PRINT '   ? Users table NOT FOUND!';
    PRINT '   ERROR: Database schema missing. Run InitialCreate migration first.';
    RETURN;
END
GO

-- Step 2: Check Users table schema
PRINT '';
PRINT '2. Checking Users table schema...';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
GO

-- Step 3: Check if TrialStartDate and TrialEndDate are nullable
PRINT '';
PRINT '3. Checking trial date columns...';
DECLARE @TrialStartNullable NVARCHAR(3);
DECLARE @TrialEndNullable NVARCHAR(3);
DECLARE @LastFeeExists BIT = 0;

SELECT @TrialStartNullable = IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TrialStartDate';

SELECT @TrialEndNullable = IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'TrialEndDate';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'LastDailyFeeDeductionDate')
    SET @LastFeeExists = 1;

PRINT '   TrialStartDate: ' + CASE WHEN @TrialStartNullable = 'YES' THEN '? NULLABLE' ELSE '? NOT NULL (Migration not applied!)' END;
PRINT '   TrialEndDate: ' + CASE WHEN @TrialEndNullable = 'YES' THEN '? NULLABLE' ELSE '? NOT NULL (Migration not applied!)' END;
PRINT '   LastDailyFeeDeductionDate: ' + CASE WHEN @LastFeeExists = 1 THEN '? EXISTS' ELSE '? MISSING (Migration not applied!)' END;

IF @TrialStartNullable = 'NO' OR @TrialEndNullable = 'NO' OR @LastFeeExists = 0
BEGIN
    PRINT '';
    PRINT '   ??  WARNING: Migration NOT applied!';
    PRINT '   The database schema does not match the code changes.';
    PRINT '   This will cause runtime errors.';
    PRINT '';
    PRINT '   ACTION REQUIRED: Run MakeTrialDatesNullable.sql migration!';
    PRINT '';
END
GO

-- Step 4: Check if any users exist
PRINT '';
PRINT '4. Checking user count...';
DECLARE @UserCount INT;
SELECT @UserCount = COUNT(*) FROM Users;
PRINT '   Total users: ' + CAST(@UserCount AS NVARCHAR(10));

IF @UserCount = 0
BEGIN
    PRINT '   ??  No users found. You need to register a new user.';
    PRINT '';
END
GO

-- Step 5: List sample users (without showing passwords)
PRINT '';
PRINT '5. Sample users in database:';
SELECT TOP 5
    UserId,
    Email,
    MobileNumber,
    FullName,
    IsActive,
    IsTrialActive,
    TrialStartDate,
    TrialEndDate,
    LastLoginDate,
    CreatedDate,
    CASE 
        WHEN LEN(PasswordHash) > 0 THEN '? Password exists'
        ELSE '? No password'
    END AS PasswordStatus
FROM Users
ORDER BY CreatedDate DESC;
GO

-- Step 6: Check for potential login issues
PRINT '';
PRINT '6. Checking for common login issues...';

-- Check for inactive users
DECLARE @InactiveCount INT;
SELECT @InactiveCount = COUNT(*) FROM Users WHERE IsActive = 0;
IF @InactiveCount > 0
    PRINT '   ??  ' + CAST(@InactiveCount AS NVARCHAR(10)) + ' inactive user(s) found';
ELSE
    PRINT '   ? All users are active';

-- Check for NULL email AND mobile
DECLARE @NoContactCount INT;
SELECT @NoContactCount = COUNT(*) FROM Users WHERE Email IS NULL AND MobileNumber IS NULL;
IF @NoContactCount > 0
    PRINT '   ??  ' + CAST(@NoContactCount AS NVARCHAR(10)) + ' user(s) with no email or mobile';
ELSE
    PRINT '   ? All users have contact info';

-- Check for empty password hashes
DECLARE @NoPasswordCount INT;
SELECT @NoPasswordCount = COUNT(*) FROM Users WHERE PasswordHash IS NULL OR PasswordHash = '';
IF @NoPasswordCount > 0
    PRINT '   ??  ' + CAST(@NoPasswordCount AS NVARCHAR(10)) + ' user(s) with no password hash';
ELSE
    PRINT '   ? All users have password hashes';
GO

-- Step 7: Check Wallets
PRINT '';
PRINT '7. Checking Wallets table...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Wallets')
BEGIN
    DECLARE @WalletCount INT;
    DECLARE @UserCountForWallet INT;
    
    SELECT @WalletCount = COUNT(*) FROM Wallets;
    SELECT @UserCountForWallet = COUNT(*) FROM Users;
    
    PRINT '   ? Wallets table exists';
    PRINT '   Total wallets: ' + CAST(@WalletCount AS NVARCHAR(10));
    PRINT '   Total users: ' + CAST(@UserCountForWallet AS NVARCHAR(10));
    
    IF @WalletCount < @UserCountForWallet
        PRINT '   ??  Some users are missing wallets!';
    ELSE
        PRINT '   ? All users have wallets';
END
ELSE
BEGIN
    PRINT '   ? Wallets table NOT FOUND!';
END
GO

-- Step 8: Test login query (without password check)
PRINT '';
PRINT '8. Testing login query for existing users...';
PRINT '';
PRINT '   Testing email login query:';
-- Show what the query would return for first user
DECLARE @TestEmail NVARCHAR(256);
SELECT TOP 1 @TestEmail = Email FROM Users WHERE Email IS NOT NULL;

IF @TestEmail IS NOT NULL
BEGIN
    PRINT '   Test email: ' + @TestEmail;
    
    IF EXISTS (SELECT * FROM Users WHERE Email = @TestEmail)
        PRINT '   ? User found by email';
    ELSE
        PRINT '   ? User NOT found (this should not happen)';
END
ELSE
    PRINT '   ??  No users with email addresses';
GO

-- Step 9: Check for case sensitivity issues
PRINT '';
PRINT '9. Checking for case sensitivity issues...';
SELECT TOP 3
    Email,
    LOWER(Email) AS LowerEmail,
    CASE 
        WHEN Email = LOWER(Email) THEN '? Already lowercase'
        ELSE '??  Mixed case'
    END AS CaseStatus
FROM Users
WHERE Email IS NOT NULL;
GO

-- Step 10: Recommendations
PRINT '';
PRINT '======================================';
PRINT 'DIAGNOSTIC COMPLETE';
PRINT '======================================';
PRINT '';
PRINT 'RECOMMENDATIONS:';
PRINT '1. If migration not applied: Run MakeTrialDatesNullable.sql';
PRINT '2. If no users exist: Register a new user';
PRINT '3. If inactive users: Check IsActive column';
PRINT '4. If password issues: Check PasswordHash column';
PRINT '5. Check application logs for detailed error messages';
PRINT '';
PRINT 'NEXT STEPS:';
PRINT '1. Review the output above';
PRINT '2. Fix any issues marked with ? or ??';
PRINT '3. Try logging in again';
PRINT '4. If still failing, check application logs';
PRINT '';
GO

-- Bonus: Create a test user query (commented out)
/*
PRINT '======================================';
PRINT 'BONUS: Create Test User';
PRINT '======================================';
PRINT '';
PRINT 'Creating test user...';

-- Hash for password "Test@123" using SHA256
-- You need to generate this using C# code
DECLARE @TestPasswordHash NVARCHAR(MAX) = 'YOUR_PASSWORD_HASH_HERE';

INSERT INTO Users (
    Email,
    MobileNumber,
    PasswordHash,
    FullName,
    CreatedDate,
    IsEmailVerified,
    IsMobileVerified,
    IsActive,
    TrialStartDate,
    TrialEndDate,
    IsTrialActive,
    LastDailyFeeDeductionDate
)
VALUES (
    'test@example.com',
    '9876543210',
    @TestPasswordHash,
    'Test User',
    GETUTCDATE(),
    0,
    0,
    1,
    GETUTCDATE(),
    DATEADD(day, 30, GETUTCDATE()),
    1,
    NULL
);

PRINT '? Test user created';
PRINT 'Email: test@example.com';
PRINT 'Password: Test@123';
PRINT '';
GO
*/
