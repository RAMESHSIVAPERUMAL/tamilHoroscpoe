# Database Migration Guide - Make Trial Dates Nullable

## ?? Quick Start

You have **3 options** to apply the migration. Choose the one that works best for you.

---

## ? **Option 1: SQL Server Management Studio (SSMS) - RECOMMENDED**

### **Steps:**

1. **Open SQL Server Management Studio (SSMS)**
   - Connect to your database server

2. **Open the SQL script**
   - File ? Open ? File
   - Navigate to: `TamilHoroscope.Web/Migrations/MakeTrialDatesNullable.sql`

3. **Update database name** (first line of script):
   ```sql
   USE [TamilHoroscope]; -- Replace with YOUR database name
   ```

4. **Execute the script**
   - Press `F5` or click "Execute" button
   - Watch the Messages tab for progress

5. **Verify success**
   - You should see output like:
   ```
   TrialStartDate is now nullable ?
   TrialEndDate is now nullable ?
   LastDailyFeeDeductionDate column added ?
   Migration completed successfully! ?
   ```

---

## ? **Option 2: Visual Studio SQL Server Object Explorer**

### **Steps:**

1. **Open SQL Server Object Explorer** in Visual Studio
   - View ? SQL Server Object Explorer

2. **Connect to your database**
   - Right-click on your database server ? New Query

3. **Copy-paste the SQL script**
   - Open: `TamilHoroscope.Web/Migrations/MakeTrialDatesNullable.sql`
   - Copy all content
   - Paste into the query window

4. **Update database name** (first line):
   ```sql
   USE [TamilHoroscope]; -- YOUR database name
   ```

5. **Execute**
   - Click the green "Execute" button
   - Check the Messages pane for success

---

## ? **Option 3: Command Line with sqlcmd**

### **Steps:**

1. **Open Command Prompt or PowerShell**

2. **Navigate to the migrations folder**:
   ```cmd
   cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web\Migrations
   ```

3. **Run sqlcmd**:
   ```cmd
   sqlcmd -S YOUR_SERVER_NAME -d TamilHoroscope -i MakeTrialDatesNullable.sql
   ```

   **For Windows Authentication**:
   ```cmd
   sqlcmd -S localhost -d TamilHoroscope -E -i MakeTrialDatesNullable.sql
   ```

   **For SQL Server Authentication**:
   ```cmd
   sqlcmd -S localhost -d TamilHoroscope -U sa -P YourPassword -i MakeTrialDatesNullable.sql
   ```

4. **Check output**
   - Should see success messages

---

## ? **Option 4: Entity Framework Core Tools (If Installed)**

### **Check if EF Core tools are installed:**
```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet ef --version
```

If installed, run:
```cmd
dotnet ef database update
```

If NOT installed, install it:
```cmd
dotnet tool install --global dotnet-ef
dotnet ef database update
```

---

## ?? **Verification Steps**

After running the migration, verify it worked:

### **Method 1: Query the schema**
```sql
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CASE 
        WHEN IS_NULLABLE = 'YES' THEN '? NULLABLE'
        ELSE '? NOT NULL'
    END AS Status
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
    AND COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate')
ORDER BY COLUMN_NAME;
```

**Expected Result:**
```
COLUMN_NAME                  DATA_TYPE  IS_NULLABLE  Status
LastDailyFeeDeductionDate   datetime2   YES         ? NULLABLE
TrialEndDate                datetime2   YES         ? NULLABLE
TrialStartDate              datetime2   YES         ? NULLABLE
```

### **Method 2: Check existing data**
```sql
SELECT TOP 5
    UserId,
    FullName,
    TrialStartDate,
    TrialEndDate,
    IsTrialActive,
    LastDailyFeeDeductionDate
FROM Users
ORDER BY CreatedDate DESC;
```

---

## ?? **Before You Start - IMPORTANT**

### **1. Backup Your Database**
```sql
-- Create a backup
BACKUP DATABASE TamilHoroscope
TO DISK = 'C:\Backups\TamilHoroscope_BeforeMigration.bak'
WITH FORMAT, NAME = 'Before Making Trial Dates Nullable';
```

### **2. Check Existing Data**
```sql
-- See current trial data
SELECT 
    COUNT(*) AS TotalUsers,
    SUM(CASE WHEN IsTrialActive = 1 THEN 1 ELSE 0 END) AS ActiveTrials,
    SUM(CASE WHEN IsTrialActive = 0 THEN 1 ELSE 0 END) AS InactiveTrials
FROM Users;
```

---

## ?? **Rollback Plan (If Something Goes Wrong)**

If you need to undo the changes:

```sql
-- ROLLBACK SCRIPT
USE [TamilHoroscope];
GO

-- Update NULL values first
UPDATE Users
SET TrialStartDate = COALESCE(TrialStartDate, CreatedDate)
WHERE TrialStartDate IS NULL;

UPDATE Users
SET TrialEndDate = COALESCE(TrialEndDate, DATEADD(day, 30, COALESCE(TrialStartDate, CreatedDate)))
WHERE TrialEndDate IS NULL;

-- Make columns NOT NULL again
ALTER TABLE Users
ALTER COLUMN TrialStartDate datetime2 NOT NULL;

ALTER TABLE Users
ALTER COLUMN TrialEndDate datetime2 NOT NULL;

-- Remove new column (optional)
-- ALTER TABLE Users DROP COLUMN LastDailyFeeDeductionDate;
```

---

## ?? **Post-Migration Checklist**

After successful migration:

- [ ] ? Run verification queries (see above)
- [ ] ? Check application builds successfully
- [ ] ? Test user registration (should set trial dates)
- [ ] ? Test login with existing user
- [ ] ? Test dashboard display
- [ ] ? Test trial activation logic
- [ ] ? Monitor application logs for errors

---

## ?? **Troubleshooting**

### **Error: Cannot alter column because it is used in a constraint**

**Solution:** Drop constraints first:
```sql
-- Find constraints
SELECT 
    name AS ConstraintName,
    OBJECT_NAME(parent_object_id) AS TableName
FROM sys.objects
WHERE type_desc = 'DEFAULT_CONSTRAINT'
    AND parent_object_id = OBJECT_ID('Users');

-- Drop default constraint (replace constraint name)
ALTER TABLE Users
DROP CONSTRAINT DF_Users_TrialStartDate;

-- Then run the migration
```

### **Error: Column already exists**

**Solution:** The migration is partially complete. Run this to check:
```sql
SELECT 
    COLUMN_NAME,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
    AND COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate');
```

Then manually run only the missing steps.

### **Error: Database is in use**

**Solution:** Close all connections first:
```sql
USE master;
GO

ALTER DATABASE TamilHoroscope
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Run migration

-- Then restore multi-user
ALTER DATABASE TamilHoroscope
SET MULTI_USER;
GO
```

---

## ?? **Expected Impact**

### **Before Migration:**
```
TrialStartDate: datetime2 NOT NULL (default: GETUTCDATE())
TrialEndDate: datetime2 NOT NULL
LastDailyFeeDeductionDate: (doesn't exist)
```

### **After Migration:**
```
TrialStartDate: datetime2 NULL
TrialEndDate: datetime2 NULL
LastDailyFeeDeductionDate: datetime2 NULL ? NEW
```

### **Data Impact:**
- ? Existing data remains unchanged (dates keep their values)
- ? New users will have dates set during registration
- ? Smart trial logic can now set/unset dates as needed

---

## ?? **Quick Command Reference**

### **SSMS Quick Commands:**
```sql
-- Use your database
USE [TamilHoroscope];

-- Run the migration script
-- (Copy-paste from MakeTrialDatesNullable.sql)

-- Verify
SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Users' 
AND COLUMN_NAME LIKE 'Trial%';
```

### **Visual Studio Package Manager Console:**
```powershell
# If EF Core tools work:
Update-Database
```

### **Command Line:**
```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet ef database update
```

---

## ?? **Need Help?**

If you encounter issues:

1. **Check the error message** - Most errors are self-explanatory
2. **Verify database connection** - Make sure you can connect
3. **Check permissions** - Ensure you have ALTER TABLE permissions
4. **Review the rollback script** - You can always undo changes
5. **Check application logs** - After migration, run the app and check for errors

---

## ? **Success Indicators**

You'll know the migration succeeded when:

1. ? SQL script executes without errors
2. ? Verification query shows columns are nullable
3. ? Application builds successfully
4. ? Application runs without database errors
5. ? Dashboard loads correctly
6. ? User registration works
7. ? Trial logic functions as expected

---

**Ready to proceed? Start with Option 1 (SSMS) - it's the safest and easiest!**

---

**Last Updated:** 2024-02-10  
**Version:** 1.0  
**Status:** ? Ready to Apply
