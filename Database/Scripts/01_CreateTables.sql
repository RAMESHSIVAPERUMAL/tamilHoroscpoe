-- =============================================
-- TamilHoroscope Web Application - Database Tables
-- Version: 1.0
-- Description: Creates all required tables for user management, 
--              wallet system, and horoscope generation tracking
-- =============================================

USE [TamilHoroscopeDB];
GO

-- =============================================
-- Table: Users
-- Description: Stores user account information with trial period tracking
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] INT IDENTITY(1,1) NOT NULL,
        [Email] NVARCHAR(256) NULL,
        [MobileNumber] NVARCHAR(20) NULL,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [CreatedDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [IsEmailVerified] BIT NOT NULL DEFAULT 0,
        [IsMobileVerified] BIT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [LastLoginDate] DATETIME2(7) NULL,
        [TrialStartDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [TrialEndDate] DATETIME2(7) NOT NULL,
        [IsTrialActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC),
        CONSTRAINT [UQ_Users_Email] UNIQUE ([Email]),
        CONSTRAINT [UQ_Users_MobileNumber] UNIQUE ([MobileNumber]),
        CONSTRAINT [CK_Users_EmailOrMobile] CHECK (
            ([Email] IS NOT NULL AND [Email] <> '') OR 
            ([MobileNumber] IS NOT NULL AND [MobileNumber] <> '')
        )
    );
    
    PRINT 'Table [Users] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [Users] already exists.';
END
GO

-- =============================================
-- Table: Wallets
-- Description: Stores user wallet balance information
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wallets' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Wallets] (
        [WalletId] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [Balance] DECIMAL(10,2) NOT NULL DEFAULT 0.00,
        [LastUpdatedDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [CreatedDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Wallets] PRIMARY KEY CLUSTERED ([WalletId] ASC),
        CONSTRAINT [FK_Wallets_Users] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
        CONSTRAINT [UQ_Wallets_UserId] UNIQUE ([UserId]),
        CONSTRAINT [CK_Wallets_Balance] CHECK ([Balance] >= 0)
    );
    
    PRINT 'Table [Wallets] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [Wallets] already exists.';
END
GO

-- =============================================
-- Table: Transactions
-- Description: Stores all wallet transaction history
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Transactions] (
        [TransactionId] INT IDENTITY(1,1) NOT NULL,
        [WalletId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [TransactionType] NVARCHAR(50) NOT NULL,
        [Amount] DECIMAL(10,2) NOT NULL,
        [BalanceBefore] DECIMAL(10,2) NOT NULL,
        [BalanceAfter] DECIMAL(10,2) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [TransactionDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [ReferenceId] NVARCHAR(100) NULL,
        CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([TransactionId] ASC),
        CONSTRAINT [FK_Transactions_Wallets] FOREIGN KEY ([WalletId]) 
            REFERENCES [dbo].[Wallets]([WalletId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Transactions_Users] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[Users]([UserId]) ON DELETE NO ACTION,
        CONSTRAINT [CK_Transactions_Type] CHECK (
            [TransactionType] IN ('Credit', 'Debit', 'Refund')
        )
    );
    
    PRINT 'Table [Transactions] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [Transactions] already exists.';
END
GO

-- =============================================
-- Table: HoroscopeGenerations
-- Description: Tracks horoscope generation history for daily deduction logic
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HoroscopeGenerations' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[HoroscopeGenerations] (
        [GenerationId] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [GenerationDate] DATE NOT NULL,
        [BirthDateTime] DATETIME2(7) NOT NULL,
        [PlaceName] NVARCHAR(200) NULL,
        [Latitude] DECIMAL(10,6) NOT NULL,
        [Longitude] DECIMAL(10,6) NOT NULL,
        [AmountDeducted] DECIMAL(10,2) NOT NULL DEFAULT 0.00,
        [WasTrialPeriod] BIT NOT NULL DEFAULT 0,
        [CreatedDateTime] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_HoroscopeGenerations] PRIMARY KEY CLUSTERED ([GenerationId] ASC),
        CONSTRAINT [FK_HoroscopeGenerations_Users] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE
    );
    
    PRINT 'Table [HoroscopeGenerations] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [HoroscopeGenerations] already exists.';
END
GO

-- =============================================
-- Table: SystemConfig
-- Description: Stores system-wide configuration parameters
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemConfig' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[SystemConfig] (
        [ConfigId] INT IDENTITY(1,1) NOT NULL,
        [ConfigKey] NVARCHAR(100) NOT NULL,
        [ConfigValue] NVARCHAR(500) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [DataType] NVARCHAR(50) NOT NULL DEFAULT 'string',
        [LastModifiedDate] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_SystemConfig] PRIMARY KEY CLUSTERED ([ConfigId] ASC),
        CONSTRAINT [UQ_SystemConfig_ConfigKey] UNIQUE ([ConfigKey]),
        CONSTRAINT [CK_SystemConfig_DataType] CHECK (
            [DataType] IN ('decimal', 'int', 'string', 'bool')
        )
    );
    
    PRINT 'Table [SystemConfig] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [SystemConfig] already exists.';
END
GO

PRINT '';
PRINT '=============================================';
PRINT 'All tables created successfully!';
PRINT 'Next steps:';
PRINT '  1. Run 02_CreateIndexes.sql';
PRINT '  2. Run 03_SeedData.sql';
PRINT '=============================================';
GO
