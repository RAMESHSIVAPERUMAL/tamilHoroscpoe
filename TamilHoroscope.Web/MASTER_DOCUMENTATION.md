# Tamil Horoscope Web - Master Documentation

> **Human & AI-Friendly Guide**  
> ASP.NET Core 8.0 | Razor Pages | SQL Server | EF Core 8.0

---

## Table of Contents
1. [Quick Start](#quick-start)
2. [Architecture](#architecture)
3. [Features](#features)
4. [Database](#database)
5. [Services](#services)
6. [Common Fixes](#common-fixes)
7. [Troubleshooting](#troubleshooting)

---

## Quick Start

### Run Application
```bash
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet restore
dotnet build
dotnet run
```
Access: **https://localhost:5001**

### First Time Setup
```bash
# 1. Apply migrations
dotnet ef database update

# 2. Verify database
# Check that these tables exist:
# Users, Wallets, Transactions, HoroscopeGenerations, SystemConfig
```

---

## Architecture

### Tech Stack
- **Framework**: ASP.NET Core 8.0 (Razor Pages)
- **Database**: SQL Server 2016+
- **ORM**: Entity Framework Core 8.0
- **Auth**: Session-based (no Identity)
- **UI**: Bootstrap 5

### Project Structure
```
TamilHoroscope.Web/
??? Data/
?   ??? Entities/              # Database models
?   ??? Configurations/        # EF fluent API configs
?   ??? ApplicationDbContext.cs
??? Services/
?   ??? Interfaces/            # Service contracts
?   ??? Implementations/       # Business logic
??? Pages/
?   ??? Account/               # Login, Register, Profile
?   ??? Wallet/                # TopUp, History
?   ??? Horoscope/             # Generate, History
??? Migrations/                # Database migrations
```

---

## Features

### User Management
- Email or phone number registration
- Password-based login (BCrypt hashed)
- 30-day free trial for new users
- Session-based authentication

### Horoscope Generation
- Birth chart calculation (Rasi & Navamsa)
- Vimshottari Dasa predictions (120 years)
- Planetary positions and strengths
- Visual South Indian style charts

### Billing System
- **Trial**: Free for 30 days (limited features)
- **Paid**: ?5 per day (full features)
- Wallet-based payments
- Automatic daily deduction
- Transaction history

### Feature Matrix

| Feature | Trial (Free) | Paid (?5/day) |
|---------|--------------|---------------|
| Rasi Chart | ? | ? |
| Planetary Positions | ? | ? |
| Dasa Main Periods | ? | ? |
| Dasa Bhukti Sub-periods | ? | ? |
| Navamsa Chart | ? | ? |
| Planetary Strength | ? | ? |
| Generations per day | 10 | 10 |

---

## Database

### Tables

#### Users
Stores user accounts and trial information
```
UserId (PK), Email, MobileNumber, PasswordHash, FullName
TrialStartDate, TrialEndDate, IsTrialActive
LastDailyFeeDeductionDate, CreatedDate, LastLoginDate
```

#### Wallets
User wallet balance
```
WalletId (PK), UserId (FK), Balance
CreatedDate, LastUpdatedDate
```

#### Transactions
Payment history
```
TransactionId (PK), UserId (FK), WalletId (FK)
Amount, TransactionType (Credit/Debit/Refund)
BalanceBefore, BalanceAfter, Description
TransactionDate, ReferenceId
```

#### HoroscopeGenerations
Tracks horoscope generation for billing
```
GenerationId (PK), UserId (FK)
GenerationDate (date only - for daily tracking)
BirthDateTime, PersonName, PlaceName
Latitude, Longitude
AmountDeducted, WasTrialPeriod
CreatedDateTime
```

#### SystemConfig
Application settings
```
ConfigId (PK), ConfigKey (unique), ConfigValue
DataType (decimal/int/string/bool)
Description, LastModifiedDate, IsActive
```

### Configuration Values
```
MinimumWalletPurchase: 100.00
PerDayCost: 5.00
TrialPeriodDays: 30
LowBalanceWarningDays: 10
MaxHoroscopesPerDay: 10
DasaYears: 120
```

---

## Services

### HoroscopeService
**Main business logic for horoscope generation**

Key Method: `GenerateHoroscopeAsync()`

**Flow**:
1. Check rate limit (max 10/day)
2. Get user info and check last fee deduction
3. Determine if trial or paid
   - **Trial**: No charge, limited features
   - **Paid (first of day)**: Deduct ?5, full features
   - **Paid (already paid today)**: No charge, full features
4. Calculate horoscope using TamilHoroscope.Core
5. Record generation in database
6. Return results

**Important**: Daily fee is charged once per day, not per generation

### WalletService
- `AddFundsAsync()` - Add money to wallet
- `DeductFundsAsync()` - Charge for service
- `GetBalanceAsync()` - Check balance
- `HasSufficientBalanceAsync()` - Verify funds
- `GetTransactionHistoryAsync()` - View history

### SubscriptionService
- `IsUserInTrialAsync()` - Check if user is in trial
- `GetTrialDaysRemainingAsync()` - Days left in trial
- `ShouldShowLowBalanceWarningAsync()` - Show balance warning

### ConfigService
- `GetPerDayCostAsync()` - Get daily cost
- `GetMaxHoroscopesPerDayAsync()` - Get rate limit
- `GetDasaYearsAsync()` - Get Dasa period

---

## Common Fixes

### PersonName Column Missing (Feb 2024)
**Error**: `Invalid column name 'PersonName'`

**Quick Fix**:
```bash
# Option 1: EF Migration
dotnet ef database update

# Option 2: SQL Script
USE [TamilHoroscopeDB];
ALTER TABLE HoroscopeGenerations ADD PersonName NVARCHAR(100) NULL;
```

**Files**: See `FIX_PERSONNAME_COLUMN.md`

### View Again Not Working (Jan 2024)
**Issue**: Clicking "View Again" in history showed error

**Fix**: Pass generationId in URL, fetch from database on Generate page

**Flow**: History ? Click "View Again" ? Generate?generationId=123 ? Fetch & Display

### Trial Not Working
**Check**:
- TrialEndDate is set correctly (30 days from TrialStartDate)
- IsTrialActive = true
- Current date is before TrialEndDate

### Wallet Deduction Issues
**Check**:
- LastDailyFeeDeductionDate in Users table
- Only one charge per day (date only, not datetime)
- Balance >= PerDayCost

---

## Troubleshooting

### Build Errors
```bash
dotnet clean
dotnet restore
dotnet build
```

### Database Connection Failed
1. Check SQL Server is running
2. Verify connection string in `appsettings.json`
3. Test: `sqlcmd -S localhost -E`

### Migration Errors
```bash
# View migration status
dotnet ef migrations list

# Apply all migrations
dotnet ef database update

# Rollback to specific migration
dotnet ef database update <MigrationName>
```

### Authentication Not Working
- Clear browser cookies/cache
- Check password is BCrypt hashed in database
- Verify session is configured in Program.cs:
  ```csharp
  builder.Services.AddSession();
  app.UseSession();
  ```

### Horoscope Generation Fails
- Check TamilHoroscope.Core is referenced
- Verify ephemeris files in `TamilHoroscope.Core/ephe/`
- Check birth date/time are valid
- View logs in Output window

### Balance Not Deducting
- Check LastDailyFeeDeductionDate column exists
- Verify PerDayCost in SystemConfig
- Check wallet balance is sufficient
- Look at Transactions table for deduction record

---

## Development Workflow

### Testing Scenario
1. **Register** new user ? Auto-creates wallet with ?0
2. **Generate** horoscope ? Free (trial), limited features
3. **Wait** for trial to expire OR manually set TrialEndDate to past
4. **Top-up** wallet ? Add ?100 minimum
5. **Generate** horoscope ? Deducts ?5, full features
6. **Generate again** same day ? No charge, full features
7. **Next day** ? Deducts ?5 again

### Database Verification Queries

**Check user trial status**:
```sql
SELECT UserId, Email, TrialStartDate, TrialEndDate, IsTrialActive
FROM Users WHERE Email = 'test@example.com';
```

**Check wallet balance**:
```sql
SELECT u.Email, w.Balance
FROM Users u
INNER JOIN Wallets w ON u.UserId = w.UserId
WHERE u.Email = 'test@example.com';
```

**Check today's generations**:
```sql
SELECT * FROM HoroscopeGenerations
WHERE UserId = 1 AND GenerationDate = CAST(GETDATE() AS DATE);
```

**Check last deduction**:
```sql
SELECT UserId, LastDailyFeeDeductionDate
FROM Users WHERE UserId = 1;
```

---

## For AI Agents

### Common Questions & Answers

**Q: How to add PersonName column?**  
A: Run `dotnet ef database update` or execute SQL:  
`ALTER TABLE HoroscopeGenerations ADD PersonName NVARCHAR(100) NULL;`

**Q: Why is user charged multiple times per day?**  
A: Check GenerationDate uses DATE type, not DATETIME. Should only charge once per calendar day.

**Q: How to extend trial period?**  
A: Update `TrialEndDate` in Users table: `UPDATE Users SET TrialEndDate = DATEADD(day, 30, GETDATE()) WHERE UserId = 1;`

**Q: Where is billing logic?**  
A: `Services/Implementations/HoroscopeService.cs` ? `GenerateHoroscopeAsync()` method around line 50-120

**Q: How to check if column exists?**  
A: SQL: `SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HoroscopeGenerations' AND COLUMN_NAME = 'PersonName';`

### Key Code Locations
- **Login Logic**: `Pages/Account/Login.cshtml.cs`
- **Registration**: `Pages/Account/Register.cshtml.cs`
- **Horoscope Gen**: `Services/Implementations/HoroscopeService.cs`
- **Wallet Top-up**: `Pages/Wallet/TopUp.cshtml.cs`
- **DB Context**: `Data/ApplicationDbContext.cs`
- **Configurations**: `Data/Configurations/*.cs`

---

## Quick Commands

```bash
# Build
dotnet build

# Run
dotnet run

# Apply migrations
dotnet ef database update

# Create new migration
dotnet ef migrations add <MigrationName>

# List migrations
dotnet ef migrations list

# Remove last migration (if not applied)
dotnet ef migrations remove

# Clean
dotnet clean

# Restore packages
dotnet restore
```

---

**Version**: 1.0  
**Last Updated**: February 14, 2026  
**Status**: Production Ready ?

**Related Docs**:
- `FIX_PERSONNAME_COLUMN.md` - PersonName column fix guide
- `Database/Scripts/*.sql` - Database setup scripts
- `appsettings.json` - Configuration settings
