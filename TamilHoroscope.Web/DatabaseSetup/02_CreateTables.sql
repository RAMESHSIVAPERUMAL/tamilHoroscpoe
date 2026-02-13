-- =====================================================
-- STEP 2: CREATE TABLES
-- All tables for TamilHoroscope application
-- =====================================================

USE TamilHoroscope;
GO

PRINT 'Creating tables...';
PRINT '';

-- =====================================================
-- Table: Users
-- =====================================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(256) NULL,
    MobileNumber NVARCHAR(20) NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    IsMobileVerified BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    LastLoginDate DATETIME2 NULL,
    TrialStartDate DATETIME2 NULL,  -- Nullable for smart trial management
    TrialEndDate DATETIME2 NULL,    -- Nullable for smart trial management
    IsTrialActive BIT NOT NULL DEFAULT 1,
    LastDailyFeeDeductionDate DATETIME2 NULL,
    
    -- Constraints
    CONSTRAINT CK_Users_EmailOrMobile CHECK (Email IS NOT NULL OR MobileNumber IS NOT NULL),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT UQ_Users_Mobile UNIQUE (MobileNumber)
);

PRINT '? Table created: Users';

-- =====================================================
-- Table: Wallets
-- =====================================================
CREATE TABLE Wallets (
    WalletId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Balance DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    LastUpdatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    -- Constraints
    CONSTRAINT FK_Wallets_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT CK_Wallets_Balance CHECK (Balance >= 0),
    CONSTRAINT UQ_Wallets_UserId UNIQUE (UserId)
);

PRINT '? Table created: Wallets';

-- =====================================================
-- Table: Transactions
-- =====================================================
CREATE TABLE Transactions (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    WalletId INT NOT NULL,
    UserId INT NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    BalanceBefore DECIMAL(10,2) NOT NULL,
    BalanceAfter DECIMAL(10,2) NOT NULL,
    Description NVARCHAR(500) NULL,
    TransactionDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ReferenceId NVARCHAR(100) NULL,
    
    -- Constraints
    CONSTRAINT FK_Transactions_Wallets FOREIGN KEY (WalletId) REFERENCES Wallets(WalletId),
    CONSTRAINT FK_Transactions_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT CK_Transactions_Type CHECK (TransactionType IN ('Credit', 'Debit', 'Refund'))
);

PRINT '? Table created: Transactions';

-- =====================================================
-- Table: HoroscopeGenerations
-- =====================================================
CREATE TABLE HoroscopeGenerations (
    GenerationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    GenerationDate DATE NOT NULL,
    BirthDateTime DATETIME2 NOT NULL,
    PlaceName NVARCHAR(200) NULL,
    Latitude DECIMAL(10,6) NOT NULL,
    Longitude DECIMAL(10,6) NOT NULL,
    AmountDeducted DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    WasTrialPeriod BIT NOT NULL DEFAULT 0,
    CreatedDateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    -- Constraints
    CONSTRAINT FK_HoroscopeGenerations_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

PRINT '? Table created: HoroscopeGenerations';

-- =====================================================
-- Table: SystemConfig
-- =====================================================
CREATE TABLE SystemConfig (
    ConfigId INT IDENTITY(1,1) PRIMARY KEY,
    ConfigKey NVARCHAR(100) NOT NULL,
    ConfigValue NVARCHAR(500) NOT NULL,
    Description NVARCHAR(500) NULL,
    DataType NVARCHAR(50) NOT NULL DEFAULT 'string',
    LastModifiedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    
    -- Constraints
    CONSTRAINT UQ_SystemConfig_ConfigKey UNIQUE (ConfigKey),
    CONSTRAINT CK_SystemConfig_DataType CHECK (DataType IN ('decimal', 'int', 'string', 'bool'))
);

PRINT '? Table created: SystemConfig';

PRINT '';
PRINT '======================================';
PRINT 'ALL TABLES CREATED SUCCESSFULLY';
PRINT '======================================';
GO
