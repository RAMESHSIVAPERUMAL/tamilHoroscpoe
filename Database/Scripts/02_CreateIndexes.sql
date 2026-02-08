-- =============================================
-- TamilHoroscope Web Application - Database Indexes
-- Version: 1.0
-- Description: Creates performance indexes for frequently queried tables
-- =============================================

USE [TamilHoroscopeDB];
GO

PRINT 'Creating indexes for performance optimization...';
PRINT '';

-- =============================================
-- Indexes for Users table
-- =============================================

-- Index for email login lookup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Email]
    ON [dbo].[Users] ([Email] ASC)
    INCLUDE ([UserId],[PasswordHash],[IsActive])
    WHERE [Email] IS NOT NULL;

    
    PRINT 'Index [IX_Users_Email] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_Users_Email] already exists.';
END

-- Index for mobile number login lookup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_MobileNumber' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_MobileNumber]
    ON [dbo].[Users] ([MobileNumber] ASC)
    INCLUDE ([UserId], [PasswordHash], [IsActive])
    WHERE [MobileNumber] IS NOT NULL;

    
    PRINT 'Index [IX_Users_MobileNumber] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_Users_MobileNumber] already exists.';
END

-- Index for trial period queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_TrialStatus' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_TrialStatus]
    ON [dbo].[Users] ([UserId] ASC)
    INCLUDE ([TrialStartDate], [TrialEndDate], [IsTrialActive]);
    
    PRINT 'Index [IX_Users_TrialStatus] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_Users_TrialStatus] already exists.';
END

-- =============================================
-- Indexes for Wallets table
-- =============================================

-- Index for user wallet lookup (covered by unique constraint, but adding covering index)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Wallets_UserId_Balance' AND object_id = OBJECT_ID('dbo.Wallets'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Wallets_UserId_Balance]
    ON [dbo].[Wallets] ([UserId] ASC)
    INCLUDE ([Balance], [LastUpdatedDate]);
    
    PRINT 'Index [IX_Wallets_UserId_Balance] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_Wallets_UserId_Balance] already exists.';
END

-- =============================================
-- Indexes for Transactions table
-- =============================================

-- Index for user transaction history queries (ordered by date)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_UserId_Date' AND object_id = OBJECT_ID('dbo.Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_UserId_Date]
    ON [dbo].[Transactions] ([UserId] ASC, [TransactionDate] DESC)
    INCLUDE ([TransactionType], [Amount], [BalanceBefore], [BalanceAfter], [Description]);
    
    PRINT 'Index [IX_Transactions_UserId_Date] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_Transactions_UserId_Date] already exists.';
END

-- Index for wallet transaction history
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_WalletId_Date' AND object_id = OBJECT_ID('dbo.Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_WalletId_Date]
    ON [dbo].[Transactions] ([WalletId] ASC, [TransactionDate] DESC);
    
    PRINT 'Index [IX_Transactions_WalletId_Date] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_Transactions_WalletId_Date] already exists.';
END

-- Index for transaction type filtering
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_TransactionType' AND object_id = OBJECT_ID('dbo.Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_TransactionType]
    ON [dbo].[Transactions] ([TransactionType] ASC, [TransactionDate] DESC);
    
    PRINT 'Index [IX_Transactions_TransactionType] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_Transactions_TransactionType] already exists.';
END

-- =============================================
-- Indexes for HoroscopeGenerations table
-- =============================================

-- CRITICAL INDEX: For daily deduction logic - check if user generated horoscope today
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HoroscopeGenerations_UserId_Date' AND object_id = OBJECT_ID('dbo.HoroscopeGenerations'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_HoroscopeGenerations_UserId_Date]
    ON [dbo].[HoroscopeGenerations] ([UserId] ASC, [GenerationDate] DESC)
    INCLUDE ([AmountDeducted], [WasTrialPeriod], [BirthDateTime], [PlaceName]);
    
    PRINT 'Index [IX_HoroscopeGenerations_UserId_Date] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_HoroscopeGenerations_UserId_Date] already exists.';
END

-- Index for horoscope history queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HoroscopeGenerations_CreatedDateTime' AND object_id = OBJECT_ID('dbo.HoroscopeGenerations'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_HoroscopeGenerations_CreatedDateTime]
    ON [dbo].[HoroscopeGenerations] ([CreatedDateTime] DESC);
    
    PRINT 'Index [IX_HoroscopeGenerations_CreatedDateTime] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_HoroscopeGenerations_CreatedDateTime] already exists.';
END

-- =============================================
-- Indexes for SystemConfig table
-- =============================================

-- Index for active config lookups (covered by unique constraint on ConfigKey)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SystemConfig_IsActive' AND object_id = OBJECT_ID('dbo.SystemConfig'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SystemConfig_IsActive]
    ON [dbo].[SystemConfig] ([IsActive] ASC)
    INCLUDE ([ConfigKey], [ConfigValue], [DataType]);
    
    PRINT 'Index [IX_SystemConfig_IsActive] created successfully.';
END
ELSE
BEGIN
    PRINT 'Index [IX_SystemConfig_IsActive] already exists.';
END

PRINT '';
PRINT '=============================================';
PRINT 'All indexes created successfully!';
PRINT 'Database is now optimized for performance.';
PRINT 'Next step: Run 03_SeedData.sql';
PRINT '=============================================';
GO
