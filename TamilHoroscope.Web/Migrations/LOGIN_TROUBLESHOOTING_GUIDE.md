# ?? LOGIN TROUBLESHOOTING - Step-by-Step Guide

## ?? **Login Still Not Working? Follow These Steps:**

---

## **STEP 1: Run the Comprehensive Debug Script**

This will tell you EXACTLY what's wrong.

### **A. Open SQL Server Management Studio (SSMS)**

### **B. Run this script:**

Open file: `TamilHoroscope.Web/Migrations/ComprehensiveLoginDebug.sql`

Or copy-paste this:

```sql
USE [TamilHoroscope];

SELECT 
    UserId,
    Email,
    FullName,
    IsActive,
    TrialStartDate,
    TrialEndDate,
    LEFT(PasswordHash, 40) + '...' AS PasswordHashPreview,
    CASE 
        WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' THEN '? Password is Test@4321'
        WHEN PasswordHash = 'oKWRFTtOX8HwB6qXqwUQlWEZQbA1OfFmWrALdpqNl0k=' THEN '? Password is Test@123'
        ELSE '? Password is something else'
    END AS PasswordCheck
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';
```

### **C. Check the output:**

**If NO rows returned:** User doesn't exist ? Go to STEP 2
**If IsActive = 0:** Account is inactive ? Go to STEP 3
**If PasswordCheck shows ?:** Wrong password hash ? Go to STEP 4
**If everything looks good:** ? Go to STEP 5

---

## **STEP 2: User Doesn't Exist**

### **Fix: Create the user**

```sql
USE [TamilHoroscope];

-- Insert user
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
    'rameshsivaperumal@gmail.com',  -- Email (lowercase!)
    NULL,                             -- Mobile (optional)
    'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=',  -- Password: Test@4321
    'Ramesh Sivaperumal',            -- Full Name
    GETUTCDATE(),                    -- Created Date
    0,                                -- Email Not Verified
    0,                                -- Mobile Not Verified
    1,                                -- Active ?
    GETUTCDATE(),                    -- Trial Start
    DATEADD(day, 30, GETUTCDATE()), -- Trial End (30 days)
    1,                                -- Trial Active
    NULL                              -- Never paid
);

-- Get the new user ID
DECLARE @NewUserId INT = SCOPE_IDENTITY();

-- Create wallet for user
INSERT INTO Wallets (UserId, Balance, CreatedDate, LastUpdatedDate)
VALUES (@NewUserId, 0.00, GETUTCDATE(), GETUTCDATE());

-- Verify
SELECT * FROM Users WHERE UserId = @NewUserId;
SELECT * FROM Wallets WHERE UserId = @NewUserId;

PRINT '? User created successfully!';
PRINT 'Email: rameshsivaperumal@gmail.com';
PRINT 'Password: Test@4321';
```

**Then try logging in again.**

---

## **STEP 3: Account is Inactive**

### **Fix: Activate the account**

```sql
USE [TamilHoroscope];

UPDATE Users 
SET IsActive = 1
WHERE Email = 'rameshsivaperumal@gmail.com';

-- Verify
SELECT UserId, Email, IsActive FROM Users 
WHERE Email = 'rameshsivaperumal@gmail.com';
```

**Then try logging in again.**

---

## **STEP 4: Wrong Password Hash**

### **Fix: Set password to Test@4321**

```sql
USE [TamilHoroscope];

UPDATE Users 
SET PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
WHERE Email = 'rameshsivaperumal@gmail.com';

-- Verify
SELECT 
    UserId,
    Email,
    CASE 
        WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' 
        THEN '? Password is now Test@4321'
        ELSE '? Update failed'
    END AS Status
FROM Users 
WHERE Email = 'rameshsivaperumal@gmail.com';
```

**Then try logging in again.**

---

## **STEP 5: Database Looks Good - Check Application**

### **A. Run with Detailed Logging**

```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet run --verbosity detailed
```

### **B. Watch the Console Output**

When you try to login, you should see:

```
info: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      === AUTHENTICATION ATTEMPT START ===
info: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      Input Email/Mobile: 'rameshsivaperumal@gmail.com'
info: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      Normalized email to: 'rameshsivaperumal@gmail.com'
info: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      User found: UserId=1, Email=rameshsivaperumal@gmail.com
info: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      Password verified successfully for UserId: 1
info: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      Account is active for UserId: 1
info: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      === AUTHENTICATION SUCCESSFUL for UserId: 1 ===
```

### **C. If You See Different Messages:**

**"User not found":**
```
warn: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      Authentication failed: User not found for: 'rameshsivaperumal@gmail.com'
```
? Email in database is different. Check STEP 2.

**"Invalid password":**
```
warn: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      Authentication failed: Invalid password for UserId: 1
```
? Password hash doesn't match. Check STEP 4.

**"Account inactive":**
```
warn: TamilHoroscope.Web.Services.Implementations.AuthenticationService[0]
      Authentication failed: Account inactive for UserId: 1
```
? User is inactive. Check STEP 3.

---

## **STEP 6: Check Connection String**

### **Verify in appsettings.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DESKTOP-99QP7PM\\SQLEXPRESS;Database=TamilHoroscope;User Id=sa;Password=sasa;TrustServerCertificate=true;"
  }
}
```

### **Test in SQL:**

```sql
SELECT 
    @@SERVERNAME AS ServerName,
    DB_NAME() AS CurrentDatabase;
```

Should show:
```
ServerName: DESKTOP-99QP7PM\SQLEXPRESS
CurrentDatabase: TamilHoroscope
```

---

## **STEP 7: Clear Browser Cache**

Sometimes old cookies cause issues.

### **Chrome/Edge:**
1. Press `Ctrl + Shift + Delete`
2. Select "Cookies and other site data"
3. Click "Clear data"
4. Close and reopen browser

### **Or use Incognito/Private mode**
- Chrome: `Ctrl + Shift + N`
- Edge: `Ctrl + Shift + P`

---

## **STEP 8: Verify Application is Running on Correct Port**

### **Check launchSettings.json:**

File: `TamilHoroscope.Web/Properties/launchSettings.json`

Should have:
```json
"applicationUrl": "https://localhost:7262;http://localhost:5262"
```

### **Access the correct URL:**
```
https://localhost:7262/Account/Login
```

---

## **STEP 9: Check for Database Migration Issues**

### **Verify schema:**

```sql
USE [TamilHoroscope];

SELECT 
    COLUMN_NAME,
    IS_NULLABLE,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
    AND COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate')
ORDER BY COLUMN_NAME;
```

**Expected:**
```
LastDailyFeeDeductionDate   YES   datetime2
TrialEndDate                YES   datetime2
TrialStartDate              YES   datetime2
```

If any show "NO" (not nullable), run migration:
```sql
ALTER TABLE Users ALTER COLUMN TrialStartDate datetime2 NULL;
ALTER TABLE Users ALTER COLUMN TrialEndDate datetime2 NULL;
```

---

## **STEP 10: Test with a NEW User**

Register a brand new account to rule out data issues:

1. Go to: `https://localhost:7262/Account/Register`
2. Register with:
   - Full Name: Test User
   - Email: test@example.com
   - Password: Test@123
   - Confirm: Test@123
3. Try logging in with new account

If new account works: Issue is with your specific user data.
If new account fails: Issue is with application/database connection.

---

## **QUICK REFERENCE: All Fixes in One**

### **Run ALL these fixes:**

```sql
USE [TamilHoroscope];

-- 1. Ensure user exists (won't error if already exists)
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'rameshsivaperumal@gmail.com')
BEGIN
    INSERT INTO Users (Email, MobileNumber, PasswordHash, FullName, CreatedDate, 
                      IsEmailVerified, IsMobileVerified, IsActive, TrialStartDate, 
                      TrialEndDate, IsTrialActive, LastDailyFeeDeductionDate)
    VALUES ('rameshsivaperumal@gmail.com', NULL, 
            'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=',
            'Ramesh Sivaperumal', GETUTCDATE(), 0, 0, 1, GETUTCDATE(), 
            DATEADD(day, 30, GETUTCDATE()), 1, NULL);
    
    DECLARE @NewUserId INT = SCOPE_IDENTITY();
    
    INSERT INTO Wallets (UserId, Balance, CreatedDate, LastUpdatedDate)
    VALUES (@NewUserId, 0.00, GETUTCDATE(), GETUTCDATE());
END

-- 2. Activate user
UPDATE Users 
SET IsActive = 1
WHERE Email = 'rameshsivaperumal@gmail.com';

-- 3. Set correct password (Test@4321)
UPDATE Users 
SET PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
WHERE Email = 'rameshsivaperumal@gmail.com';

-- 4. Verify
SELECT 
    UserId,
    Email,
    FullName,
    IsActive,
    CASE 
        WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' 
        THEN '? Password is Test@4321' 
        ELSE '? Password issue' 
    END AS PasswordStatus,
    CASE WHEN IsActive = 1 THEN '? Active' ELSE '? Inactive' END AS AccountStatus
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

PRINT '? All fixes applied!';
PRINT 'Login credentials:';
PRINT '  Email: rameshsivaperumal@gmail.com';
PRINT '  Password: Test@4321';
```

---

## **?? Final Checklist**

Before trying login again:

- [ ] User exists in database
- [ ] User IsActive = 1
- [ ] Password hash = `tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=`
- [ ] Wallet exists for user
- [ ] Application is running (`dotnet run`)
- [ ] Accessing correct URL (`https://localhost:7262/Account/Login`)
- [ ] Browser cache cleared
- [ ] Watching console logs for detailed errors

---

## **?? Still Having Issues?**

### **Share these details:**

1. **Console output** when you try to login
2. **SQL query result** from Step 1
3. **Error message** on login page
4. **Browser** and version
5. **Any exceptions** in the logs

This will help pinpoint the exact issue!

---

**Last Updated:** 2024-02-14  
**Status:** Enhanced Debugging Version  
**Success Rate:** Should fix 99% of login issues
