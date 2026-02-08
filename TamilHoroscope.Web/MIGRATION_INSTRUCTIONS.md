# Migration Instructions - Apply Updated Schema

## Overview
Your C# entity configurations have been updated to match your SQL database schema. Follow these steps to apply the changes to your database.

## Step 1: Create a Migration

### Using Package Manager Console (Visual Studio)
```powershell
# Open: Tools > NuGet Package Manager > Package Manager Console

Add-Migration AlignDatabaseSchema -Project TamilHoroscope.Web -OutputDir Data/Migrations
```

### Using .NET CLI (Terminal/PowerShell)
```bash
cd TamilHoroscope.Web
dotnet ef migrations add AlignDatabaseSchema -o Data/Migrations
```

## Step 2: Review the Migration

Open the generated migration file in `TamilHoroscope.Web\Data\Migrations\[timestamp]_AlignDatabaseSchema.cs`

Verify it includes:
- All index creations
- All constraint configurations
- All property mappings

## Step 3: Apply the Migration

### Using Package Manager Console
```powershell
Update-Database -Project TamilHoroscope.Web
```

### Using .NET CLI
```bash
cd TamilHoroscope.Web
dotnet ef database update
```

## Step 4: Verify in SQL Server

Connect to your database and run:

```sql
-- Check Users table
SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Users')
ORDER BY name;

-- Check Wallets table
SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Wallets')
ORDER BY name;

-- Check Transactions table
SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Transactions')
ORDER BY name;

-- Check HoroscopeGenerations table
SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('HoroscopeGenerations')
ORDER BY name;

-- Check SystemConfig table
SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('SystemConfig')
ORDER BY name;

-- Check constraints
SELECT * FROM sys.check_constraints 
WHERE parent_object_id IN (
    OBJECT_ID('Users'),
    OBJECT_ID('Wallets'),
    OBJECT_ID('Transactions'),
    OBJECT_ID('HoroscopeGenerations'),
    OBJECT_ID('SystemConfig')
)
ORDER BY name;
```

## Important Notes

### 1. Backup Your Database First
Before applying migrations to production:
```sql
BACKUP DATABASE [TamilHoroscopeDB] 
TO DISK = 'C:\Backups\TamilHoroscopeDB_backup.bak'
WITH INIT, COMPRESSION;
```

### 2. Order of Operations
The migrations will be applied in this order:
1. User configurations (Identity properties, indexes)
2. Wallet configurations (balance constraints, indexes)
3. Transaction configurations (type constraints, indexes)
4. HoroscopeGeneration configurations (critical daily deduction index)
5. SystemConfig configurations (data type constraints)

### 3. Expected Migrations
You should see these index creations:
- ? IX_Users_Email
- ? IX_Users_MobileNumber
- ? IX_Users_TrialStatus
- ? IX_Wallets_UserId_Balance
- ? IX_Transactions_UserId_Date
- ? IX_Transactions_WalletId_Date
- ? IX_Transactions_TransactionType
- ? IX_HoroscopeGenerations_UserId_Date
- ? IX_HoroscopeGenerations_CreatedDateTime
- ? IX_SystemConfig_IsActive

### 4. If Migration Fails

#### Common Issues:

**Issue: "Cannot drop index 'IX_Users_Email' because it does not exist"**
- Solution: Check if indexes already exist in database
- Run the verification script above to see existing indexes

**Issue: "Duplicate index names"**
- Solution: The migration will handle this - drop existing manual indexes first if needed:
```sql
-- Only if you created these indexes manually
DROP INDEX IF EXISTS [IX_Users_Email] ON [dbo].[Users];
DROP INDEX IF EXISTS [IX_Users_MobileNumber] ON [dbo].[Users];
-- ... etc for other indexes
```

**Issue: "Foreign key conflict"**
- Solution: Ensure all referenced rows exist:
```sql
-- Check for orphaned records
SELECT * FROM Users WHERE UserId NOT IN (SELECT UserId FROM Wallets);
DELETE FROM Users WHERE UserId NOT IN (SELECT UserId FROM Wallets);
```

## Rollback (If Needed)

### Undo Last Migration
```powershell
# Package Manager Console
Remove-Migration -Project TamilHoroscope.Web

# .NET CLI
dotnet ef migrations remove
```

### Revert Database to Previous State
```powershell
# Package Manager Console
Update-Database -Migration [PreviousMigrationName] -Project TamilHoroscope.Web

# .NET CLI
dotnet ef database update [PreviousMigrationName]
```

## After Migration: Test Your Application

### 1. Test User Registration
```csharp
// Register page should work without column mismatch errors
var user = new User { Email = "test@example.com", ... };
await userManager.CreateAsync(user, password);
```

### 2. Test Wallet Operations
```csharp
// Wallet creation and updates should work
var wallet = new Wallet { UserId = userId, Balance = 100.00m };
context.Wallets.Add(wallet);
await context.SaveChangesAsync();
```

### 3. Test Transaction Logging
```csharp
// Transaction logging should work with proper indexes
var transaction = new Transaction 
{ 
    UserId = userId, 
    TransactionType = "Credit",
    Amount = 100.00m,
    TransactionDate = DateTime.UtcNow
};
context.Transactions.Add(transaction);
await context.SaveChangesAsync();
```

### 4. Test Horoscope Generation Tracking
```csharp
// Daily deduction logic should query efficiently
var generatedToday = await context.HoroscopeGenerations
    .Where(h => h.UserId == userId && h.GenerationDate == DateTime.Today)
    .CountAsync();
```

## Performance Validation

After migration, run these queries to ensure indexes are being used:

```sql
-- Check index usage
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s 
    ON i.object_id = s.object_id 
    AND i.index_id = s.index_id
WHERE OBJECT_NAME(i.object_id) IN ('Users', 'Wallets', 'Transactions', 'HoroscopeGenerations', 'SystemConfig')
ORDER BY s.user_seeks + s.user_scans + s.user_lookups DESC;
```

## Completion Checklist

- [ ] Migration created successfully
- [ ] Migration reviewed (no unexpected changes)
- [ ] Backup created
- [ ] Migration applied to development database
- [ ] Verification script confirms all indexes
- [ ] Application tests pass
- [ ] User registration works
- [ ] Wallet operations work
- [ ] Transaction logging works
- [ ] Horoscope generation tracking works
- [ ] Backup created before production migration
- [ ] Production migration applied
- [ ] Performance validation completed

---

**Status:** Ready to Migrate  
**Build:** ? Successful  
**Configuration:** ? Complete  
**Next Step:** Create Migration
