# PersonName Column Fix

## Problem
Error: `Invalid column name 'PersonName'`

The entity model has `PersonName` property, but the database table is missing this column.

## Quick Fix

### Method 1: EF Migration (Recommended)
```bash
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet ef database update
```

### Method 2: SQL Script
```sql
USE [TamilHoroscopeDB];
ALTER TABLE [HoroscopeGenerations] ADD [PersonName] NVARCHAR(100) NULL;
```

### Method 3: PowerShell
```powershell
.\TamilHoroscope.Web\Migrations\ApplyPersonNameFix.ps1
```

## Verify Fix
```sql
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'HoroscopeGenerations' AND COLUMN_NAME = 'PersonName';
```
Should return one row.

## Test
1. Run application
2. Go to Horoscope ? Generate
3. Enter person name and generate horoscope
4. Should work without errors

## Files Changed
- `HoroscopeGenerationConfiguration.cs` - Added PersonName config
- `ApplicationDbContextModelSnapshot.cs` - Updated snapshot
- `20260214000000_AddPersonNameToHoroscopeGenerations.cs` - New migration
- `AddPersonNameColumn.sql` - Manual SQL script

## Troubleshooting
- **Column exists error**: Already applied, restart app
- **Still failing**: Clear bin/obj and rebuild
- **Wrong database**: Check connection string

---
**Status**: ? Build successful | Ready to apply
