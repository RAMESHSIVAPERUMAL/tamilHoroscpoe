-- =====================================================
-- QUICK FIX: Make Login Work NOW
-- Run this to fix all common login issues
-- =====================================================

USE [TamilHoroscope];
GO

PRINT '======================================';
PRINT 'QUICK FIX FOR LOGIN';
PRINT 'Email: rameshsivaperumal@gmail.com';
PRINT 'Password: Test@4321';
PRINT '======================================';
PRINT '';

-- Check if user exists
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'rameshsivaperumal@gmail.com')
BEGIN
    PRINT '1. Creating user...';
    
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
        'rameshsivaperumal@gmail.com',
        NULL,
        'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=',  -- Test@4321
        'Ramesh Sivaperumal',
        GETUTCDATE(),
        0,
        0,
        1,  -- Active
        GETUTCDATE(),
        DATEADD(day, 30, GETUTCDATE()),
        1,
        NULL
    );
    
    DECLARE @NewUserId INT = SCOPE_IDENTITY();
    PRINT '   ? User created with ID: ' + CAST(@NewUserId AS NVARCHAR(10));
    
    -- Create wallet
    INSERT INTO Wallets (UserId, Balance, CreatedDate, LastUpdatedDate)
    VALUES (@NewUserId, 0.00, GETUTCDATE(), GETUTCDATE());
    
    PRINT '   ? Wallet created';
END
ELSE
BEGIN
    PRINT '1. User already exists';
    
    -- Ensure user is active
    UPDATE Users 
    SET IsActive = 1
    WHERE Email = 'rameshsivaperumal@gmail.com' AND IsActive = 0;
    
    IF @@ROWCOUNT > 0
        PRINT '   ? Activated user account';
    
    -- Update password to Test@4321
    UPDATE Users 
    SET PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
    WHERE Email = 'rameshsivaperumal@gmail.com'
        AND PasswordHash <> 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=';
    
    IF @@ROWCOUNT > 0
        PRINT '   ? Updated password to Test@4321';
    
    -- Ensure wallet exists
    DECLARE @ExistingUserId INT;
    SELECT @ExistingUserId = UserId FROM Users WHERE Email = 'rameshsivaperumal@gmail.com';
    
    IF NOT EXISTS (SELECT * FROM Wallets WHERE UserId = @ExistingUserId)
    BEGIN
        INSERT INTO Wallets (UserId, Balance, CreatedDate, LastUpdatedDate)
        VALUES (@ExistingUserId, 0.00, GETUTCDATE(), GETUTCDATE());
        
        PRINT '   ? Created missing wallet';
    END
END

PRINT '';
PRINT '======================================';
PRINT 'VERIFICATION';
PRINT '======================================';
PRINT '';

-- Show user details
SELECT 
    'User Details' AS Section,
    UserId,
    Email,
    FullName,
    CASE WHEN IsActive = 1 THEN '? Active' ELSE '? Inactive' END AS Status,
    CASE 
        WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' 
        THEN '? Password is Test@4321' 
        ELSE '? Password does NOT match' 
    END AS PasswordCheck,
    IsTrialActive,
    TrialEndDate
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

-- Show wallet
SELECT 
    'Wallet Details' AS Section,
    w.WalletId,
    w.UserId,
    w.Balance,
    w.LastUpdatedDate
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.UserId
WHERE u.Email = 'rameshsivaperumal@gmail.com';

PRINT '';
PRINT '======================================';
PRINT 'READY TO LOGIN!';
PRINT '======================================';
PRINT '';
PRINT 'Credentials:';
PRINT '  Email: rameshsivaperumal@gmail.com';
PRINT '  Password: Test@4321';
PRINT '';
PRINT 'Steps:';
PRINT '1. Run application: dotnet run';
PRINT '2. Open: https://localhost:7262/Account/Login';
PRINT '3. Enter credentials above';
PRINT '4. Click "Log in"';
PRINT '';
PRINT '? Login should work now!';
PRINT '';

GO
