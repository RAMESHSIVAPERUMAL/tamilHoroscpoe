-- =====================================================
-- COMPLETE FIX: All Login Issues
-- Fixes NULL dates AND ensures user is ready
-- =====================================================

USE [TamilHoroscope];
GO

PRINT '======================================';
PRINT 'COMPLETE LOGIN FIX';
PRINT 'Email: rameshsivaperumal@gmail.com';
PRINT '======================================';
PRINT '';

-- Step 1: Fix NULL trial dates for ALL users
PRINT '1. Fixing NULL trial dates for all users...';

UPDATE Users
SET 
    TrialStartDate = COALESCE(TrialStartDate, CreatedDate, GETUTCDATE()),
    TrialEndDate = COALESCE(TrialEndDate, DATEADD(day, 30, COALESCE(TrialStartDate, CreatedDate, GETUTCDATE())))
WHERE TrialStartDate IS NULL OR TrialEndDate IS NULL;

DECLARE @FixedCount INT = @@ROWCOUNT;
PRINT '   Fixed ' + CAST(@FixedCount AS NVARCHAR(10)) + ' users with NULL dates';

-- Step 2: Ensure your specific user is ready
PRINT '';
PRINT '2. Preparing your user account...';

IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'rameshsivaperumal@gmail.com')
BEGIN
    PRINT '   Creating new user...';
    
    INSERT INTO Users (
        Email, MobileNumber, PasswordHash, FullName, CreatedDate,
        IsEmailVerified, IsMobileVerified, IsActive,
        TrialStartDate, TrialEndDate, IsTrialActive, LastDailyFeeDeductionDate
    )
    VALUES (
        'rameshsivaperumal@gmail.com',
        NULL,
        'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=',  -- Test@4321
        'Ramesh Sivaperumal',
        GETUTCDATE(),
        0, 0, 1,
        GETUTCDATE(),
        DATEADD(day, 30, GETUTCDATE()),
        1,
        NULL
    );
    
    DECLARE @NewUserId INT = SCOPE_IDENTITY();
    
    -- Create wallet
    INSERT INTO Wallets (UserId, Balance, CreatedDate, LastUpdatedDate)
    VALUES (@NewUserId, 0.00, GETUTCDATE(), GETUTCDATE());
    
    PRINT '   ? User created with wallet';
END
ELSE
BEGIN
    PRINT '   User exists, updating...';
    
    -- Fix all potential issues
    UPDATE Users
    SET 
        IsActive = 1,
        PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=',  -- Test@4321
        TrialStartDate = COALESCE(TrialStartDate, CreatedDate, GETUTCDATE()),
        TrialEndDate = COALESCE(TrialEndDate, DATEADD(day, 30, COALESCE(TrialStartDate, CreatedDate, GETUTCDATE()))),
        IsTrialActive = 1
    WHERE Email = 'rameshsivaperumal@gmail.com';
    
    PRINT '   ? User updated';
    
    -- Ensure wallet exists
    DECLARE @ExistingUserId INT;
    SELECT @ExistingUserId = UserId FROM Users WHERE Email = 'rameshsivaperumal@gmail.com';
    
    IF NOT EXISTS (SELECT * FROM Wallets WHERE UserId = @ExistingUserId)
    BEGIN
        INSERT INTO Wallets (UserId, Balance, CreatedDate, LastUpdatedDate)
        VALUES (@ExistingUserId, 0.00, GETUTCDATE(), GETUTCDATE());
        PRINT '   ? Wallet created';
    END
END

PRINT '';
PRINT '3. Final verification...';
PRINT '';

-- Show complete user details
SELECT 
    '=== USER DETAILS ===' AS Section,
    UserId,
    Email,
    FullName,
    IsActive AS IsActive,
    IsTrialActive AS IsTrialActive,
    FORMAT(TrialStartDate, 'yyyy-MM-dd HH:mm') AS TrialStart,
    FORMAT(TrialEndDate, 'yyyy-MM-dd HH:mm') AS TrialEnd,
    FORMAT(CreatedDate, 'yyyy-MM-dd HH:mm') AS Created,
    CASE 
        WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' 
        THEN '? Test@4321'
        WHEN PasswordHash = 'oKWRFTtOX8HwB6qXqwUQlWEZQbA1OfFmWrALdpqNl0k='
        THEN '? Test@123'
        ELSE '? Other'
    END AS Password,
    CASE 
        WHEN TrialStartDate IS NOT NULL AND TrialEndDate IS NOT NULL 
        THEN '? All dates set'
        ELSE '? HAS NULLs!'
    END AS DateStatus
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

-- Show wallet
SELECT 
    '=== WALLET ===' AS Section,
    w.WalletId,
    w.UserId,
    w.Balance,
    FORMAT(w.LastUpdatedDate, 'yyyy-MM-dd HH:mm') AS LastUpdated
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.UserId
WHERE u.Email = 'rameshsivaperumal@gmail.com';

PRINT '';
PRINT '======================================';
PRINT 'ALL FIXES APPLIED!';
PRINT '======================================';
PRINT '';
PRINT '? Ready to login with:';
PRINT '   Email: rameshsivaperumal@gmail.com';
PRINT '   Password: Test@4321';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Make sure application is running';
PRINT '2. Navigate to: https://localhost:7262/Account/Login';
PRINT '3. Enter credentials above';
PRINT '4. Login should work now!';
PRINT '';

GO
