# TamilHoroscope Database Setup Guide

This folder contains SQL scripts to set up the database for the TamilHoroscope web application.

## Prerequisites

- Microsoft SQL Server 2016 or later (or SQL Server Express)
- SQL Server Management Studio (SSMS) or Azure Data Studio
- Appropriate permissions to create databases and tables

## Database Setup Instructions

### Step 1: Create the Database

First, create a new database named `TamilHoroscopeDB`:

```sql
CREATE DATABASE [TamilHoroscopeDB];
GO

USE [TamilHoroscopeDB];
GO
```

**Note:** You can use a different database name, but make sure to:
1. Update the `USE` statements in all scripts
2. Update the connection string in `appsettings.json` in the web application

### Step 2: Run Scripts in Order

Execute the SQL scripts in the following order:

#### 1. Create Tables (01_CreateTables.sql)

This script creates all required tables:
- **Users** - User accounts with trial period tracking
- **Wallets** - User wallet balances
- **Transactions** - Wallet transaction history
- **HoroscopeGenerations** - Horoscope generation tracking
- **SystemConfig** - System configuration parameters

```bash
# Using sqlcmd (Command Line)
sqlcmd -S localhost -d TamilHoroscopeDB -i 01_CreateTables.sql

# Or execute in SSMS/Azure Data Studio
```

#### 2. Create Indexes (02_CreateIndexes.sql)

This script creates performance indexes for frequently queried columns:
- Email and mobile number lookups (login)
- Trial period status checks
- Wallet balance queries
- Transaction history (ordered by date)
- Daily horoscope generation checks (critical for billing)

```bash
sqlcmd -S localhost -d TamilHoroscopeDB -i 02_CreateIndexes.sql
```

#### 3. Seed Initial Data (03_SeedData.sql)

This script inserts initial configuration data:

| Configuration Key | Default Value | Description |
|------------------|---------------|-------------|
| MinimumWalletPurchase | ₹100.00 | Minimum wallet top-up amount |
| PerDayCost | ₹5.00 | Daily cost for horoscope service |
| TrialPeriodDays | 30 | Trial period duration |
| LowBalanceWarningDays | 10 | Warning threshold for low balance |
| MaxHoroscopesPerDay | 10 | Rate limit per user |
| DasaYears | 120 | Vimshottari Dasa calculation period |

```bash
sqlcmd -S localhost -d TamilHoroscopeDB -i 03_SeedData.sql
```

### Step 3: Verify Installation

Run the following query to verify all tables were created:

```sql
SELECT 
    TABLE_NAME,
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) AS ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;
```

Expected output:
- HoroscopeGenerations (10 columns)
- SystemConfig (7 columns)
- Transactions (10 columns)
- Users (13 columns)
- Wallets (5 columns)

Verify seed data:

```sql
SELECT ConfigKey, ConfigValue, Description 
FROM SystemConfig 
WHERE IsActive = 1 
ORDER BY ConfigKey;
```

Should return 8 configuration records.

## Connection String Configuration

Update the connection string in your web application's `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TamilHoroscopeDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

For SQL Server Authentication:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TamilHoroscopeDB;User Id=your_username;Password=your_password;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

## Database Schema Overview

### Users Table
- Stores user accounts (email OR mobile required)
- Tracks trial period (start date, end date, active status)
- Supports email and mobile verification
- Stores password hash (never plain text)

### Wallets Table
- One wallet per user (1:1 relationship)
- Tracks current balance
- Balance cannot be negative (check constraint)

### Transactions Table
- Complete audit trail of all wallet operations
- Records balance before and after each transaction
- Transaction types: Credit, Debit, Refund
- Links to both User and Wallet

### HoroscopeGenerations Table
- **Critical for daily billing logic**
- Stores GenerationDate (date only, no time) for daily checks
- Tracks amount deducted and trial status
- Used to prevent multiple charges per day

### SystemConfig Table
- Flexible configuration system
- Supports different data types (decimal, int, string, bool)
- Can be modified through Admin panel
- Changes take effect immediately

## Common Queries

### Check User Trial Status
```sql
SELECT 
    UserId,
    FullName,
    TrialStartDate,
    TrialEndDate,
    IsTrialActive,
    DATEDIFF(day, GETUTCDATE(), TrialEndDate) AS DaysRemaining
FROM Users
WHERE UserId = @userId;
```

### Check Wallet Balance and Days Remaining
```sql
DECLARE @PerDayCost DECIMAL(10,2) = (SELECT CAST(ConfigValue AS DECIMAL(10,2)) FROM SystemConfig WHERE ConfigKey = 'PerDayCost');

SELECT 
    w.UserId,
    u.FullName,
    w.Balance,
    @PerDayCost AS PerDayCost,
    FLOOR(w.Balance / @PerDayCost) AS DaysRemaining
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.UserId
WHERE w.UserId = @userId;
```

### Check if User Generated Horoscope Today
```sql
SELECT TOP 1 * 
FROM HoroscopeGenerations
WHERE UserId = @userId 
  AND GenerationDate = CAST(GETUTCDATE() AS DATE)
ORDER BY CreatedDateTime DESC;
```

## Maintenance

### Update Configuration Values
```sql
UPDATE SystemConfig
SET ConfigValue = '150.00', 
    LastModifiedDate = GETUTCDATE()
WHERE ConfigKey = 'MinimumWalletPurchase';
```

### View Transaction History
```sql
SELECT 
    t.TransactionDate,
    t.TransactionType,
    t.Amount,
    t.BalanceBefore,
    t.BalanceAfter,
    t.Description
FROM Transactions t
WHERE t.UserId = @userId
ORDER BY t.TransactionDate DESC;
```

## Backup and Recovery

### Create Backup
```sql
BACKUP DATABASE [TamilHoroscopeDB]
TO DISK = 'C:\Backups\TamilHoroscopeDB.bak'
WITH FORMAT, COMPRESSION;
```

### Restore Backup
```sql
USE master;
GO

RESTORE DATABASE [TamilHoroscopeDB]
FROM DISK = 'C:\Backups\TamilHoroscopeDB.bak'
WITH REPLACE;
```

## Troubleshooting

### Issue: Scripts fail with "Database does not exist"
**Solution:** Ensure you created the database first (Step 1) and updated all `USE` statements if using a different name.

### Issue: Foreign key constraint errors
**Solution:** Run scripts in order. Table dependencies require Users to exist before Wallets, etc.

### Issue: Unique constraint violations on seed data
**Solution:** Normal if re-running 03_SeedData.sql. The script checks for existing records before inserting.

### Issue: Cannot connect from web application
**Solution:** 
1. Verify SQL Server is running and accepting TCP/IP connections
2. Check firewall settings (port 1433 for default instance)
3. Verify connection string format matches your SQL Server authentication method
4. Enable SQL Server Browser service for named instances

## Security Recommendations

1. **Never commit connection strings with passwords to source control**
2. Use User Secrets for development: `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"`
3. Use Azure Key Vault or similar for production secrets
4. Grant minimal required permissions to the database user
5. Enable SQL Server auditing for production environments
6. Regularly backup the database
7. Implement database encryption at rest for sensitive data

## Support

For issues or questions:
- Check the main repository README
- Review the web application documentation
- Contact: support@tamilhoroscope.com

---

**Last Updated:** 2026-02-08  
**Version:** 1.0  
**Compatible with:** TamilHoroscope.Web v1.0+
