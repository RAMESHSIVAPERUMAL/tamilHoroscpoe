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
    ('PerDayCost', '10.00', 'Daily subscription cost in INR', 'decimal', 1),
    ('LowBalanceWarningDays', '3', 'Days threshold for low balance warning', 'int', 1),
    ('TrialPeriodDays', '30', 'Trial period duration in days', 'int', 1),
    ('MinimumTopUpAmount', '100.00', 'Minimum wallet top-up amount in INR', 'decimal', 1),
    ('ApplicationVersion', '1.0.0', 'Current application version', 'string', 1);

PRINT '? System configuration inserted';
GO