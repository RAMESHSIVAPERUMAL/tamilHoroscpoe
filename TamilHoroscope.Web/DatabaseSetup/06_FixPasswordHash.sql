-- =====================================================
-- FIX: Update Password Hash to Correct Value
-- The initial setup had wrong hash for Test@4321
-- =====================================================

USE [TamilHoroscope];
GO

PRINT '======================================';
PRINT 'FIXING PASSWORD HASH';
PRINT '======================================';
PRINT '';

-- The CORRECT hash for Test@4321 (verified by C# code)
DECLARE @CorrectHash NVARCHAR(MAX) = 'gnwYw8P15xaIZSOoPbJ69DAEzItMIyZnxgHOgx9/1dg=';

UPDATE Users
SET PasswordHash = @CorrectHash
WHERE Email = 'rameshsivaperumal@gmail.com';

PRINT '? Password hash updated to correct value';
PRINT '';
PRINT 'Verification:';

SELECT 
    Email,
    CASE 
        WHEN PasswordHash = 'gnwYw8P15xaIZSOoPbJ69DAEzItMIyZnxgHOgx9/1dg=' 
        THEN '? Correct - Test@4321 will work'
        ELSE '? Still wrong'
    END AS PasswordStatus,
    LEFT(PasswordHash, 50) + '...' AS HashPreview
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

PRINT '';
PRINT '======================================';
PRINT 'LOGIN CREDENTIALS:';
PRINT '======================================';
PRINT 'Email: rameshsivaperumal@gmail.com';
PRINT 'Password: Test@4321';
PRINT '';
PRINT '? Login should work now!';
GO
