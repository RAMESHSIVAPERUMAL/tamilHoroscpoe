-- =============================================
-- TamilHoroscope Web Application - Seed Data
-- Version: 1.0
-- Description: Inserts initial configuration data for the system
-- =============================================

USE [TamilHoroscopeDB];
GO

PRINT 'Seeding SystemConfig table with initial configuration...';
PRINT '';

-- =============================================
-- System Configuration - Wallet Settings
-- =============================================

-- Minimum wallet purchase amount
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'MinimumWalletPurchase')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('MinimumWalletPurchase', '100.00', 'Minimum wallet purchase amount in INR', 'decimal', 1);
    
    PRINT 'Config: MinimumWalletPurchase = 100.00 INR';
END
ELSE
BEGIN
    PRINT 'Config: MinimumWalletPurchase already exists.';
END
GO

-- Per-day cost for horoscope generation
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'PerDayCost')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('PerDayCost', '5.00', 'Cost per day for horoscope generation in INR', 'decimal', 1);
    
    PRINT 'Config: PerDayCost = 5.00 INR';
END
ELSE
BEGIN
    PRINT 'Config: PerDayCost already exists.';
END
GO

-- =============================================
-- System Configuration - Trial Period Settings
-- =============================================

-- Trial period duration in days
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'TrialPeriodDays')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('TrialPeriodDays', '30', 'Trial period duration in days', 'int', 1);
    
    PRINT 'Config: TrialPeriodDays = 30 days';
END
ELSE
BEGIN
    PRINT 'Config: TrialPeriodDays already exists.';
END
GO

-- =============================================
-- System Configuration - Warning Settings
-- =============================================

-- Low balance warning threshold
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'LowBalanceWarningDays')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('LowBalanceWarningDays', '10', 'Show warning when balance is below this many days', 'int', 1);
    
    PRINT 'Config: LowBalanceWarningDays = 10 days';
END
ELSE
BEGIN
    PRINT 'Config: LowBalanceWarningDays already exists.';
END
GO

-- =============================================
-- System Configuration - Application Settings
-- =============================================

-- Application name
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'ApplicationName')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('ApplicationName', 'Tamil Horoscope', 'Application display name', 'string', 1);
    
    PRINT 'Config: ApplicationName = Tamil Horoscope';
END
ELSE
BEGIN
    PRINT 'Config: ApplicationName already exists.';
END
GO

-- Support email
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'SupportEmail')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('SupportEmail', 'support@tamilhoroscope.com', 'Customer support email address', 'string', 1);
    
    PRINT 'Config: SupportEmail = support@tamilhoroscope.com';
END
ELSE
BEGIN
    PRINT 'Config: SupportEmail already exists.';
END
GO

-- Enable trial period (can be disabled globally)
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'EnableTrialPeriod')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('EnableTrialPeriod', 'true', 'Enable trial period for new users', 'bool', 1);
    
    PRINT 'Config: EnableTrialPeriod = true';
END
ELSE
BEGIN
    PRINT 'Config: EnableTrialPeriod already exists.';
END
GO

-- Maximum number of horoscopes per day (rate limiting)
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'MaxHoroscopesPerDay')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('MaxHoroscopesPerDay', '10', 'Maximum number of horoscope generations allowed per user per day', 'int', 1);
    
    PRINT 'Config: MaxHoroscopesPerDay = 10';
END
ELSE
BEGIN
    PRINT 'Config: MaxHoroscopesPerDay already exists.';
END
GO

-- Dasa years calculation
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigKey] = 'DasaYears')
BEGIN
    INSERT INTO [dbo].[SystemConfig] ([ConfigKey], [ConfigValue], [Description], [DataType], [IsActive])
    VALUES ('DasaYears', '120', 'Number of years to calculate for Vimshottari Dasa', 'int', 1);
    
    PRINT 'Config: DasaYears = 120 years';
END
ELSE
BEGIN
    PRINT 'Config: DasaYears already exists.';
END
GO

PRINT '';
PRINT '=============================================';
PRINT 'Seed data inserted successfully!';
PRINT '';
PRINT 'Configuration Summary:';
PRINT '  - Minimum Wallet Purchase: ₹100.00';
PRINT '  - Per-Day Cost: ₹5.00';
PRINT '  - Trial Period: 30 days';
PRINT '  - Low Balance Warning: 10 days';
PRINT '  - Max Horoscopes Per Day: 10';
PRINT '  - Dasa Calculation: 120 years';
PRINT '';
PRINT 'Database setup complete!';
PRINT 'Ready to run TamilHoroscope.Web application.';
PRINT '=============================================';
GO
