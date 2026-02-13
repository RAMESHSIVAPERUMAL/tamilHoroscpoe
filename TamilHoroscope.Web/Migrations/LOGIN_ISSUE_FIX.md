# ?? LOGIN ISSUE - Quick Fix Guide

## Problem: "Invalid email/mobile or password" Error

After making trial dates nullable, login is failing even with correct credentials.

---

## ?? **Root Cause Analysis**

The issue is likely one of these:

### **1. Database Migration Not Applied** ?? MOST LIKELY
The database schema doesn't match the code changes. The columns `TrialStartDate` and `TrialEndDate` are still **NOT NULL** in the database, but the code expects them to be **NULLABLE**.

### **2. Existing Users Have NULL Values**
If you ran a partial migration, some users might have NULL trial dates causing query failures.

### **3. Case Sensitivity**
Email comparison might be case-sensitive.

---

## ? **Solution Steps**

### **STEP 1: Run Diagnostic Script**

1. Open **SQL Server Management Studio (SSMS)**
2. Open file: `TamilHoroscope.Web/Migrations/DiagnosticLoginIssues.sql`
3. Update database name (line 7):
   ```sql
   USE [TamilHoroscope]; -- YOUR database name
   ```
4. Execute the script (`F5`)
5. **READ THE OUTPUT CAREFULLY**

**The diagnostic will tell you:**
- ? If migration is applied
- ? If schema is mismatched
- ?? Any user data issues

---

### **STEP 2: Apply Migration (If Not Applied)**

If diagnostic shows "Migration NOT applied":

1. Open: `TamilHoroscope.Web/Migrations/MakeTrialDatesNullable.sql`
2. Update database name (line 7)
3. Execute in SSMS
4. Verify success

**OR run this quick fix:**
```sql
USE [TamilHoroscope];

-- Make columns nullable
ALTER TABLE Users ALTER COLUMN TrialStartDate datetime2 NULL;
ALTER TABLE Users ALTER COLUMN TrialEndDate datetime2 NULL;

-- Add new column if missing
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'Users') 
               AND name = 'LastDailyFeeDeductionDate')
BEGIN
    ALTER TABLE Users ADD LastDailyFeeDeductionDate datetime2 NULL;
END

-- Verify
SELECT 
    COLUMN_NAME, 
    IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Users' 
    AND COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate');
```

---

### **STEP 3: Fix Existing User Data (If Needed)**

If users have NULL trial dates causing issues:

```sql
USE [TamilHoroscope];

-- Update users with NULL trial dates
UPDATE Users
SET 
    TrialStartDate = COALESCE(TrialStartDate, CreatedDate),
    TrialEndDate = COALESCE(TrialEndDate, DATEADD(day, 30, COALESCE(TrialStartDate, CreatedDate)))
WHERE TrialStartDate IS NULL 
   OR TrialEndDate IS NULL;

-- Verify
SELECT UserId, Email, TrialStartDate, TrialEndDate, IsTrialActive
FROM Users;
```

---

### **STEP 4: Test Login**

1. **Rebuild the application**:
   ```cmd
   cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
   dotnet build
   ```

2. **Run the application**:
   ```cmd
   dotnet run
   ```

3. **Try logging in** with existing credentials

4. **Check application logs** for errors

---

## ?? **Quick Verification Queries**

### **Check Schema:**
```sql
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
    AND COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate')
ORDER BY COLUMN_NAME;
```

**Expected Result:**
```
LastDailyFeeDeductionDate  datetime2  YES
TrialEndDate               datetime2  YES
TrialStartDate             datetime2  YES
```

### **Check User Data:**
```sql
SELECT 
    UserId,
    Email,
    FullName,
    IsActive,
    TrialStartDate,
    TrialEndDate,
    IsTrialActive,
    CASE 
        WHEN LEN(PasswordHash) > 0 THEN 'Has Password'
        ELSE 'No Password'
    END AS PasswordStatus
FROM Users
ORDER BY CreatedDate DESC;
```

### **Test Login Query:**
```sql
-- Replace with your test user email
DECLARE @TestEmail NVARCHAR(256) = 'your.email@example.com';

SELECT 
    UserId,
    Email,
    FullName,
    IsActive,
    CASE 
        WHEN LEN(PasswordHash) > 0 THEN 'Password OK'
        ELSE 'Password MISSING'
    END AS PasswordCheck
FROM Users
WHERE Email = LOWER(@TestEmail)
   OR MobileNumber = @TestEmail;
```

---

## ?? **Common Issues & Solutions**

### **Issue 1: Schema Mismatch**
**Symptom:** Application throws exception when querying Users table

**Solution:**
```sql
-- Quick fix
ALTER TABLE Users ALTER COLUMN TrialStartDate datetime2 NULL;
ALTER TABLE Users ALTER COLUMN TrialEndDate datetime2 NULL;
```

### **Issue 2: NULL Trial Dates on Existing Users**
**Symptom:** Login works but dashboard crashes

**Solution:**
```sql
-- Fix NULL dates
UPDATE Users
SET 
    TrialStartDate = COALESCE(TrialStartDate, CreatedDate),
    TrialEndDate = DATEADD(day, 30, COALESCE(TrialStartDate, CreatedDate))
WHERE TrialStartDate IS NULL OR TrialEndDate IS NULL;
```

### **Issue 3: Inactive Users**
**Symptom:** "Invalid credentials" but password is correct

**Solution:**
```sql
-- Check and activate users
UPDATE Users
SET IsActive = 1
WHERE IsActive = 0;
```

### **Issue 4: Case Sensitivity**
**Symptom:** Login works with lowercase email but not original case

**Solution:** This is expected behavior. The system normalizes emails to lowercase.
- Email: `Test@Example.com` ? Login with: `test@example.com`

---

## ?? **Expected Database State After Fix**

### **Users Table Schema:**
```
Column Name                 Type        Nullable
-------------------------------------------------
UserId                     int          NO
Email                      nvarchar     YES
MobileNumber               nvarchar     YES
PasswordHash               nvarchar     NO
FullName                   nvarchar     NO
CreatedDate                datetime2    NO
IsEmailVerified            bit          NO
IsMobileVerified           bit          NO
IsActive                   bit          NO
LastLoginDate              datetime2    YES
TrialStartDate             datetime2    YES ? NULLABLE
TrialEndDate               datetime2    YES ? NULLABLE
IsTrialActive              bit          NO
LastDailyFeeDeductionDate  datetime2    YES ? NEW
```

### **Sample User Data:**
```
UserId  Email              IsActive  TrialStartDate  TrialEndDate  IsTrialActive
------------------------------------------------------------------------------------
1       test@example.com   1         2024-02-10      2024-03-11    1
2       user@test.com      1         2024-02-10      2024-03-11    1
```

---

## ?? **Still Not Working?**

### **Check Application Logs:**

**Windows:**
```
%USERPROFILE%\AppData\Local\Temp\TamilHoroscope\logs
```

**Or check console output** when running:
```cmd
dotnet run
```

Look for errors like:
- `Invalid column name 'LastDailyFeeDeductionDate'` ? Migration not applied
- `Cannot insert NULL into column 'TrialStartDate'` ? Schema issue
- `Authentication failed` ? Check user data

### **Enable Detailed Logging:**

In `Program.cs`, ensure:
```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

Then check logs for:
```
[WRN] Authentication failed: User not found for: test@example.com
[WRN] Authentication failed: Invalid password for UserId: 1
[WRN] Authentication failed: Account inactive for UserId: 1
```

---

## ?? **Quick Test User Creation**

If you need a fresh test user:

```sql
USE [TamilHoroscope];

-- Generate password hash for "Test@123" using C# (see below)
DECLARE @PasswordHash NVARCHAR(MAX);

-- You need to run this C# code to get the hash:
-- using System.Security.Cryptography;
-- using System.Text;
-- var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("Test@123")));
-- Console.WriteLine(hash);

SET @PasswordHash = 'oKWRFTtOX8HwB6qXqwUQlWEZQbA1OfFmWrALdpqNl0k='; -- Hash for "Test@123"

-- Check if user exists
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'testuser@example.com')
BEGIN
    INSERT INTO Users (
        Email, MobileNumber, PasswordHash, FullName, CreatedDate,
        IsEmailVerified, IsMobileVerified, IsActive,
        TrialStartDate, TrialEndDate, IsTrialActive, LastDailyFeeDeductionDate
    )
    VALUES (
        'testuser@example.com',
        '9876543210',
        @PasswordHash,
        'Test User',
        GETUTCDATE(),
        0, 0, 1,
        GETUTCDATE(),
        DATEADD(day, 30, GETUTCDATE()),
        1,
        NULL
    );
    
    DECLARE @NewUserId INT = SCOPE_IDENTITY();
    
    -- Create wallet
    INSERT INTO Wallets (UserId, Balance, CreatedDate, LastUpdatedDate)
    VALUES (@NewUserId, 0.00, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'Test user created successfully!';
    PRINT 'Email: testuser@example.com';
    PRINT 'Password: Test@123';
END
ELSE
BEGIN
    PRINT 'Test user already exists!';
END
```

---

## ? **Verification Checklist**

After applying fixes, verify:

- [ ] Run diagnostic script ? All checks pass ?
- [ ] Schema shows nullable columns ?
- [ ] Existing users have trial dates ?
- [ ] Application builds successfully ?
- [ ] Application runs without errors ?
- [ ] Login page loads ?
- [ ] Login with existing user succeeds ?
- [ ] Dashboard loads after login ?
- [ ] Registration works for new user ?

---

## ?? **Need More Help?**

1. **Run the diagnostic script first** - It will tell you exactly what's wrong
2. **Check the output carefully** - Look for ? and ?? symbols
3. **Follow the recommendations** - They're specific to your issue
4. **Check application logs** - They show the exact error

---

**Most Common Fix:** Run the migration SQL script!

```sql
ALTER TABLE Users ALTER COLUMN TrialStartDate datetime2 NULL;
ALTER TABLE Users ALTER COLUMN TrialEndDate datetime2 NULL;
ALTER TABLE Users ADD LastDailyFeeDeductionDate datetime2 NULL;
```

Then rebuild and run the app.

---

**Status:** Ready for Troubleshooting  
**Priority:** ?? HIGH - Blocking login functionality
