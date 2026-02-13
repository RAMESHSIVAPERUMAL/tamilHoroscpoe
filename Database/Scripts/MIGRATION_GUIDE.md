# PersonName Migration - Quick Start Guide

## Your Database Connection Details
- **Server**: `DESKTOP-99QP7PM\SQLEXPRESS`
- **Database**: `TamilHoroscope`
- **Authentication**: SQL Server (Username: `sa`, Password: `sasa`)

---

## Option 1: Run Using Batch File (EASIEST - Recommended)

Just double-click these files in Windows Explorer:

### Step 1: Run Migration
1. Navigate to: `C:\GitWorkplace\tamilHoroscpoe\Database\Scripts\`
2. **Double-click**: `RunMigration.bat`
3. Wait for "Migration completed successfully!" message
4. Press any key to close

### Step 2: Verify Migration
1. **Double-click**: `VerifyMigration.bat`
2. Check the output shows PersonName column
3. Press any key to close

---

## Option 2: Run Using Command Prompt

Open Command Prompt and run:

```cmd
cd C:\GitWorkplace\tamilHoroscpoe\Database\Scripts

REM Run migration
sqlcmd -S "DESKTOP-99QP7PM\SQLEXPRESS" -d TamilHoroscope -U sa -P sasa -i "04_AddPersonNameColumn.sql"

REM Verify migration
sqlcmd -S "DESKTOP-99QP7PM\SQLEXPRESS" -d TamilHoroscope -U sa -P sasa -i "VerifyPersonNameMigration.sql" -W
```

---

## Option 3: Run Using PowerShell

Open PowerShell and run:

```powershell
cd C:\GitWorkplace\tamilHoroscpoe\Database\Scripts

# Run migration
.\RunMigration.ps1

# Verify migration
.\VerifyMigration.ps1
```

**Note**: If you get execution policy error, run PowerShell as Administrator and execute:
```powershell
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

---

## Option 4: Run Manually in SSMS (SQL Server Management Studio)

### Step 1: Open SSMS
1. Start SQL Server Management Studio
2. Connect to: `DESKTOP-99QP7PM\SQLEXPRESS`
3. Use SQL Server Authentication:
   - Login: `sa`
   - Password: `sasa`

### Step 2: Run Migration Script
1. Click **File ? Open ? File**
2. Navigate to: `C:\GitWorkplace\tamilHoroscpoe\Database\Scripts\04_AddPersonNameColumn.sql`
3. Make sure database dropdown shows: `TamilHoroscope`
4. Click **Execute** (or press F5)
5. Check Messages tab for:
   ```
   PersonName column added successfully
   Index IX_HoroscopeGenerations_PersonName created successfully
   ```

### Step 3: Verify
1. Open: `VerifyPersonNameMigration.sql`
2. Execute it (F5)
3. Check Results tab shows PersonName column

---

## Quick Verification Query

Run this in SSMS or command line to quickly check:

```sql
USE TamilHoroscope
GO

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'HoroscopeGenerations' 
  AND COLUMN_NAME = 'PersonName'
```

**Expected Result:**
```
COLUMN_NAME    DATA_TYPE    CHARACTER_MAXIMUM_LENGTH
PersonName     nvarchar     100
```

---

## Troubleshooting

### Error: "Login failed for user 'sa'"
**Solution**: 
1. Open SQL Server Configuration Manager
2. Enable SQL Server Authentication
3. Restart SQL Server service

### Error: "Cannot open database"
**Solution**: 
Check database name:
```sql
SELECT name FROM sys.databases WHERE name LIKE '%Tamil%'
```

### Error: "Named Pipes Provider"
**Solution**: 
1. Open SQL Server Configuration Manager
2. Enable TCP/IP protocol
3. Restart SQL Server service

### Error: "sqlcmd is not recognized"
**Solution**: 
Add SQL Server tools to PATH or use full path:
```cmd
"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\sqlcmd.exe"
```

---

## After Migration Success

### 1. Build the Application
```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet build
```

### 2. Run the Application
```cmd
dotnet run
```

### 3. Test the Features
1. Navigate to: https://localhost:5001
2. Login
3. Generate a horoscope with PersonName
4. Check History page - PersonName should appear
5. Use Search feature to find by PersonName

---

## Files Created/Updated

| File | Purpose |
|------|---------|
| `04_AddPersonNameColumn.sql` | ? Migration script (Updated) |
| `VerifyPersonNameMigration.sql` | ? Verification script (Updated) |
| `RunMigration.bat` | ? Easy migration runner |
| `VerifyMigration.bat` | ? Easy verification runner |
| `RunMigration.ps1` | ? PowerShell migration script (Updated) |
| `VerifyMigration.ps1` | ? PowerShell verification script |
| `MIGRATION_GUIDE.md` | ? This guide |

---

## Expected Output After Success

### Migration Output:
```
PersonName column added successfully
Index IX_HoroscopeGenerations_PersonName created successfully
```

### Verification Output:
```
Column Name    Data Type    Max Length    Nullable
PersonName     nvarchar     100          YES

Index Name                                Index Type
IX_HoroscopeGenerations_PersonName       NONCLUSTERED
```

---

## Summary

**Recommended**: Just double-click `RunMigration.bat` in Windows Explorer!

It's that simple! No PowerShell execution policy issues, no command line needed.

---

**Need Help?**
- Check the Troubleshooting section above
- Verify SQL Server Express is running
- Check Windows Services for "SQL Server (SQLEXPRESS)"
