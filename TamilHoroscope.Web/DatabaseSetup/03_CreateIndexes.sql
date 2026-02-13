-- =====================================================
-- STEP 3: CREATE INDEXES
-- Performance indexes for TamilHoroscope
-- =====================================================

USE TamilHoroscope;
GO

PRINT 'Creating indexes...';
PRINT '';

-- =====================================================
-- Users Table Indexes
-- =====================================================
CREATE NONCLUSTERED INDEX IX_Users_Email 
ON Users(Email) 
WHERE Email IS NOT NULL;

PRINT '? Index created: IX_Users_Email';

CREATE NONCLUSTERED INDEX IX_Users_MobileNumber 
ON Users(MobileNumber) 
WHERE MobileNumber IS NOT NULL;

PRINT '? Index created: IX_Users_MobileNumber';

CREATE NONCLUSTERED INDEX IX_Users_IsActive 
ON Users(IsActive) 
INCLUDE (UserId, Email, FullName);

PRINT '? Index created: IX_Users_IsActive';

-- =====================================================
-- Wallets Table Indexes
-- =====================================================
CREATE NONCLUSTERED INDEX IX_Wallets_UserId_Balance 
ON Wallets(UserId, Balance);

PRINT '? Index created: IX_Wallets_UserId_Balance';

-- =====================================================
-- Transactions Table Indexes
-- =====================================================
CREATE NONCLUSTERED INDEX IX_Transactions_UserId_Date 
ON Transactions(UserId, TransactionDate DESC);

PRINT '? Index created: IX_Transactions_UserId_Date';

CREATE NONCLUSTERED INDEX IX_Transactions_WalletId_Date 
ON Transactions(WalletId, TransactionDate DESC);

PRINT '? Index created: IX_Transactions_WalletId_Date';

CREATE NONCLUSTERED INDEX IX_Transactions_TransactionType 
ON Transactions(TransactionType, TransactionDate DESC);

PRINT '? Index created: IX_Transactions_TransactionType';

-- =====================================================
-- HoroscopeGenerations Table Indexes
-- =====================================================
CREATE NONCLUSTERED INDEX IX_HoroscopeGenerations_UserId_Date 
ON HoroscopeGenerations(UserId, GenerationDate DESC);

PRINT '? Index created: IX_HoroscopeGenerations_UserId_Date';

CREATE NONCLUSTERED INDEX IX_HoroscopeGenerations_CreatedDateTime 
ON HoroscopeGenerations(CreatedDateTime DESC);

PRINT '? Index created: IX_HoroscopeGenerations_CreatedDateTime';

-- =====================================================
-- SystemConfig Table Indexes
-- =====================================================
CREATE NONCLUSTERED INDEX IX_SystemConfig_IsActive 
ON SystemConfig(IsActive) 
INCLUDE (ConfigKey, ConfigValue);

PRINT '? Index created: IX_SystemConfig_IsActive';

PRINT '';
PRINT '======================================';
PRINT 'ALL INDEXES CREATED SUCCESSFULLY';
PRINT '======================================';
GO
