-- =====================================================
-- STEP 4: INSERT INITIAL DATA
-- System configuration and test user
-- =====================================================

USE TamilHoroscope;
GO

PRINT 'Inserting initial data...';
PRINT '';

-- =====================================================
-- SystemConfig: Application Settings
-- =====================================================
INSERT INTO SystemConfig (ConfigKey, ConfigValue, Description, DataType, IsActive)
VALUES
    ('PerDayCost', '5.00', 'Daily subscription cost in INR', 'decimal', 1),
    ('LowBalanceWarningDays', '3', 'Days threshold for low balance warning', 'int', 1),
    ('TrialPeriodDays', '30', 'Trial period duration in days', 'int', 1),
    ('MinimumTopUpAmount', '100.00', 'Minimum wallet top-up amount in INR', 'decimal', 1),
    ('ApplicationVersion', '1.0.0', 'Current application version', 'string', 1);

PRINT '? System configuration inserted';

-- =====================================================
-- Test User: rameshsivaperumal@gmail.com
-- Password: Test@4321
-- =====================================================
DECLARE @TestUserId INT;

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
    1,
    GETUTCDATE(),
    DATEADD(day, 30, GETUTCDATE()),
    1,
    NULL
);

SET @TestUserId = SCOPE_IDENTITY();

PRINT '? Test user created: rameshsivaperumal@gmail.com';
PRINT '  Password: Test@4321';
PRINT '  UserId: ' + CAST(@TestUserId AS NVARCHAR(10));

-- =====================================================
-- Create Wallet for Test User
-- =====================================================
INSERT INTO Wallets (UserId, Balance, CreatedDate, LastUpdatedDate)
VALUES (@TestUserId, 0.00, GETUTCDATE(), GETUTCDATE());

PRINT '? Wallet created for test user';
PRINT '  Balance: ?0.00';

PRINT '';
PRINT '======================================';
PRINT 'INITIAL DATA INSERTED SUCCESSFULLY';
PRINT '======================================';
PRINT '';
PRINT 'Test Account Credentials:';
PRINT '  Email: rameshsivaperumal@gmail.com';
PRINT '  Password: Test@4321';
PRINT '  Trial: 30 days (Active)';
PRINT '  Wallet: ?0.00';
PRINT '';
GO
