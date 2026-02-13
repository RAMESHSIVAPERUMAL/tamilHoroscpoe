-- =====================================================
-- COMPREHENSIVE LOGIN DEBUG
-- Find out EXACTLY why login is failing
-- =====================================================

USE [TamilHoroscope];
GO

PRINT '======================================';
PRINT 'COMPREHENSIVE LOGIN DEBUG';
PRINT 'For: rameshsivaperumal@gmail.com';
PRINT '======================================';
PRINT '';

-- Step 1: Check exact email in database
PRINT '1. Checking email variations...';
PRINT '';

SELECT 
    UserId,
    Email,
    LOWER(Email) AS NormalizedEmail,
    FullName,
    IsActive,
    CASE 
        WHEN Email = 'rameshsivaperumal@gmail.com' THEN '? Exact match'
        WHEN LOWER(Email) = 'rameshsivaperumal@gmail.com' THEN '? Case-insensitive match'
        ELSE '? No match'
    END AS EmailMatch
FROM Users
WHERE Email LIKE '%ramesh%' OR Email LIKE '%sivaperumal%';

PRINT '';

-- Step 2: Get the ACTUAL user record
DECLARE @UserId INT;
DECLARE @Email NVARCHAR(256);
DECLARE @PasswordHash NVARCHAR(MAX);
DECLARE @IsActive BIT;

-- Try to find user with lowercase email (how system searches)
SELECT 
    @UserId = UserId,
    @Email = Email,
    @PasswordHash = PasswordHash,
    @IsActive = IsActive
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

IF @UserId IS NULL
BEGIN
    PRINT '? User NOT found with email: rameshsivaperumal@gmail.com';
    PRINT '';
    PRINT 'Searching all users...';
    SELECT UserId, Email, FullName, IsActive FROM Users;
    RETURN;
END

PRINT '? User found!';
PRINT '';
PRINT 'User ID: ' + CAST(@UserId AS NVARCHAR(10));
PRINT 'Email: ' + @Email;
PRINT 'IsActive: ' + CASE WHEN @IsActive = 1 THEN 'TRUE ?' ELSE 'FALSE ? (LOGIN WILL FAIL!)' END;
PRINT '';

-- Step 3: Show password hash
PRINT '======================================';
PRINT '2. PASSWORD HASH ANALYSIS';
PRINT '======================================';
PRINT '';
PRINT 'Current password hash in database:';
PRINT @PasswordHash;
PRINT '';

-- Common password hashes
PRINT 'Expected hashes for common passwords:';
PRINT '';
PRINT 'Test@123     : oKWRFTtOX8HwB6qXqwUQlWEZQbA1OfFmWrALdpqNl0k=';
PRINT 'Test@4321    : tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=';
PRINT 'Admin@123    : 3EqE+3UL3pKPsQYDr7VvQYlV6Y1LmDEP8F4pD0nXPx4=';
PRINT 'Password@123 : gvCCrXXnmT7TGwRLhXEKmBvE5OhLc8cX5lGd8Lf5Wao=';
PRINT '';

-- Check if it matches any known password
IF @PasswordHash = 'oKWRFTtOX8HwB6qXqwUQlWEZQbA1OfFmWrALdpqNl0k='
    PRINT '? Password is: Test@123';
ELSE IF @PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
    PRINT '? Password is: Test@4321';
ELSE IF @PasswordHash = '3EqE+3UL3pKPsQYDr7VvQYlV6Y1LmDEP8F4pD0nXPx4='
    PRINT '? Password is: Admin@123';
ELSE IF @PasswordHash = 'gvCCrXXnmT7TGwRLhXEKmBvE5OhLc8cX5lGd8Lf5Wao='
    PRINT '? Password is: Password@123';
ELSE
    PRINT '??  Password hash does NOT match any common password!';

PRINT '';

-- Step 4: Show full user details
PRINT '======================================';
PRINT '3. COMPLETE USER DETAILS';
PRINT '======================================';
PRINT '';

SELECT 
    UserId,
    Email,
    MobileNumber,
    FullName,
    IsActive,
    IsEmailVerified,
    IsMobileVerified,
    IsTrialActive,
    TrialStartDate,
    TrialEndDate,
    LastLoginDate,
    LastDailyFeeDeductionDate,
    CreatedDate,
    LEFT(PasswordHash, 30) + '...' AS PasswordHashPreview
FROM Users
WHERE UserId = @UserId;

PRINT '';

-- Step 5: Check wallet
PRINT '======================================';
PRINT '4. WALLET STATUS';
PRINT '======================================';
PRINT '';

IF EXISTS (SELECT * FROM Wallets WHERE UserId = @UserId)
BEGIN
    SELECT 
        WalletId,
        UserId,
        Balance,
        LastUpdatedDate
    FROM Wallets
    WHERE UserId = @UserId;
    
    PRINT '? Wallet exists';
END
ELSE
BEGIN
    PRINT '? Wallet NOT FOUND!';
END

PRINT '';

-- Step 6: Authentication simulation
PRINT '======================================';
PRINT '5. AUTHENTICATION SIMULATION';
PRINT '======================================';
PRINT '';

PRINT 'When you login with:';
PRINT '  Email: rameshsivaperumal@gmail.com';
PRINT '  Password: Test@4321';
PRINT '';
PRINT 'The system will:';
PRINT '1. Convert email to lowercase: rameshsivaperumal@gmail.com';
PRINT '2. Search for user: ' + CASE WHEN @UserId IS NOT NULL THEN '? Found' ELSE '? Not Found' END;
PRINT '3. Hash the password: tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=';
PRINT '4. Compare with database: ' + @PasswordHash;
PRINT '5. Match? ' + CASE WHEN @PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' THEN '? YES - Login will succeed' ELSE '? NO - Login will fail' END;
PRINT '6. Check IsActive: ' + CASE WHEN @IsActive = 1 THEN '? YES - Login will succeed' ELSE '? NO - Login will fail' END;
PRINT '';

-- Step 7: Provide fix if needed
PRINT '======================================';
PRINT '6. RECOMMENDED FIX';
PRINT '======================================';
PRINT '';

IF @IsActive = 0
BEGIN
    PRINT '??  ISSUE: Account is INACTIVE';
    PRINT '';
    PRINT 'RUN THIS FIX:';
    PRINT 'UPDATE Users SET IsActive = 1 WHERE UserId = ' + CAST(@UserId AS NVARCHAR(10)) + ';';
    PRINT '';
END

IF @PasswordHash <> 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
BEGIN
    PRINT '??  ISSUE: Password hash does NOT match Test@4321';
    PRINT '';
    PRINT 'Current hash: ' + @PasswordHash;
    PRINT 'Expected:     tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=';
    PRINT '';
    PRINT 'RUN THIS FIX to set password to Test@4321:';
    PRINT 'UPDATE Users SET PasswordHash = ''tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='' WHERE UserId = ' + CAST(@UserId AS NVARCHAR(10)) + ';';
    PRINT '';
END

IF @IsActive = 1 AND @PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
BEGIN
    PRINT '? EVERYTHING LOOKS GOOD!';
    PRINT '';
    PRINT 'Login should work with:';
    PRINT '  Email: rameshsivaperumal@gmail.com';
    PRINT '  Password: Test@4321';
    PRINT '';
    PRINT 'If login still fails:';
    PRINT '1. Check application logs for errors';
    PRINT '2. Verify connection string is correct';
    PRINT '3. Make sure application is running';
    PRINT '4. Try clearing browser cache/cookies';
END

PRINT '';
PRINT '======================================';
PRINT 'DEBUG COMPLETE';
PRINT '======================================';

GO
