-- =====================================================
-- VERIFY USER CREDENTIALS
-- Test if login will work for specific user
-- =====================================================

USE [TamilHoroscope];
GO

PRINT '======================================';
PRINT 'USER CREDENTIALS VERIFICATION';
PRINT '======================================';
PRINT '';

-- Check if user exists
DECLARE @Email NVARCHAR(256) = 'rameshsivaperumal@gmail.com';
DECLARE @UserId INT;
DECLARE @PasswordHash NVARCHAR(MAX);
DECLARE @IsActive BIT;
DECLARE @TrialActive BIT;

SELECT 
    @UserId = UserId,
    @PasswordHash = PasswordHash,
    @IsActive = IsActive,
    @TrialActive = IsTrialActive
FROM Users
WHERE Email = @Email;

IF @UserId IS NOT NULL
BEGIN
    PRINT '? User found!';
    PRINT '';
    PRINT 'User Details:';
    PRINT '  UserId: ' + CAST(@UserId AS NVARCHAR(10));
    PRINT '  Email: ' + @Email;
    PRINT '  IsActive: ' + CASE WHEN @IsActive = 1 THEN '? YES' ELSE '? NO (LOGIN WILL FAIL!)' END;
    PRINT '  IsTrialActive: ' + CASE WHEN @TrialActive = 1 THEN 'Yes' ELSE 'No' END;
    PRINT '  Password Hash: ' + LEFT(@PasswordHash, 20) + '... (exists)';
    PRINT '';
    
    -- Show full user details
    SELECT 
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
        LastDailyFeeDeductionDate
    FROM Users
    WHERE Email = @Email;
    
    PRINT '';
    
    -- Check wallet
    IF EXISTS (SELECT * FROM Wallets WHERE UserId = @UserId)
    BEGIN
        DECLARE @Balance DECIMAL(10,2);
        SELECT @Balance = Balance FROM Wallets WHERE UserId = @UserId;
        PRINT '? Wallet exists';
        PRINT '  Balance: ?' + CAST(@Balance AS NVARCHAR(20));
    END
    ELSE
    BEGIN
        PRINT '? Wallet NOT FOUND! This will cause issues.';
    END
    
    PRINT '';
    PRINT '======================================';
    PRINT 'PASSWORD VERIFICATION';
    PRINT '======================================';
    PRINT '';
    PRINT 'The password "Test@4321" should produce this hash:';
    PRINT '';
    PRINT 'To verify, run this C# code:';
    PRINT 'using System.Security.Cryptography;';
    PRINT 'using System.Text;';
    PRINT 'var password = "Test@4321";';
    PRINT 'var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));';
    PRINT 'Console.WriteLine(hash);';
    PRINT '';
    PRINT 'Expected hash for "Test@4321":';
    PRINT 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=';
    PRINT '';
    PRINT 'Current hash in database:';
    PRINT @PasswordHash;
    PRINT '';
    
    IF @PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
        PRINT '? Password hash MATCHES! Login should work.';
    ELSE
        PRINT '?? Password hash DIFFERENT! Login will fail unless password is correct.';
    
    PRINT '';
    PRINT '======================================';
    PRINT 'LOGIN READINESS CHECK';
    PRINT '======================================';
    PRINT '';
    
    IF @IsActive = 1
    BEGIN
        PRINT '? Account is active';
        PRINT '? User can log in';
        PRINT '';
        PRINT 'LOGIN INSTRUCTIONS:';
        PRINT '1. Run the application: dotnet run';
        PRINT '2. Navigate to: https://localhost:7262/Account/Login';
        PRINT '3. Enter:';
        PRINT '   Email: rameshsivaperumal@gmail.com';
        PRINT '   Password: Test@4321';
        PRINT '4. Click "Log in"';
        PRINT '';
        PRINT '? LOGIN SHOULD SUCCEED!';
    END
    ELSE
    BEGIN
        PRINT '? Account is INACTIVE!';
        PRINT '?? LOGIN WILL FAIL!';
        PRINT '';
        PRINT 'FIX: Run this command:';
        PRINT 'UPDATE Users SET IsActive = 1 WHERE Email = ''' + @Email + ''';';
    END
END
ELSE
BEGIN
    PRINT '? User NOT FOUND!';
    PRINT '';
    PRINT 'Email searched: rameshsivaperumal@gmail.com';
    PRINT '';
    PRINT 'This means the user does not exist in the database.';
    PRINT '';
    PRINT 'OPTIONS:';
    PRINT '1. Check if email is correct (case-sensitive check)';
    PRINT '2. Register a new account';
    PRINT '3. Check if user exists with different email';
    PRINT '';
    
    -- Show all users
    PRINT 'All users in database:';
    SELECT 
        UserId,
        Email,
        MobileNumber,
        FullName,
        IsActive
    FROM Users
    ORDER BY CreatedDate DESC;
END

GO
