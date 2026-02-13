USE [TamilHoroscope];

PRINT '======================================';
PRINT 'PASSWORD CHECKER';
PRINT '======================================';
PRINT '';

-- Get current password hash
DECLARE @CurrentHash NVARCHAR(MAX);
SELECT @CurrentHash = PasswordHash FROM Users WHERE UserId = 1;

PRINT 'Current password hash in database:';
PRINT @CurrentHash;
PRINT '';

-- Check against known passwords
PRINT 'Checking against common passwords:';
PRINT '';

IF @CurrentHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
    PRINT '? Password is: Test@4321';
ELSE IF @CurrentHash = 'oKWRFTtOX8HwB6qXqwUQlWEZQbA1OfFmWrALdpqNl0k='
    PRINT '? Password is: Test@123';
ELSE IF @CurrentHash = '3EqE+3UL3pKPsQYDr7VvQYlV6Y1LmDEP8F4pD0nXPx4='
    PRINT '? Password is: Admin@123';
ELSE IF @CurrentHash = 'gvCCrXXnmT7TGwRLhXEKmBvE5OhLc8cX5lGd8Lf5Wao='
    PRINT '? Password is: Password@123';
ELSE
    PRINT '? Password is NOT one of the common test passwords';

PRINT '';
PRINT '======================================';
PRINT 'QUICK FIX';
PRINT '======================================';
PRINT '';

PRINT 'To set password to Test@4321, run:';
PRINT '';
PRINT 'UPDATE Users';
PRINT 'SET PasswordHash = ''tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=''';
PRINT 'WHERE UserId = 1;';
PRINT '';

-- Show password hash mapping
PRINT '======================================';
PRINT 'PASSWORD HASH REFERENCE';
PRINT '======================================';
PRINT '';
PRINT 'Test@123     ? oKWRFTtOX8HwB6qXqwUQlWEZQbA1OfFmWrALdpqNl0k=';
PRINT 'Test@4321    ? tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=';
PRINT 'Admin@123    ? 3EqE+3UL3pKPsQYDr7VvQYlV6Y1LmDEP8F4pD0nXPx4=';
PRINT 'Password@123 ? gvCCrXXnmT7TGwRLhXEKmBvE5OhLc8cX5lGd8Lf5Wao=';
PRINT '';
