# Tamil Horoscope Web - Documentation

> ASP.NET Core 8.0 Razor Pages | SQL Server | Entity Framework Core 8.0

## Quick Start

```bash
cd TamilHoroscope.Web
dotnet restore && dotnet build && dotnet run
```
Access: https://localhost:5001

## Project Structure

```
TamilHoroscope.Web/
??? Data/
?   ??? Entities/              # User, Wallet, Transaction, HoroscopeGeneration, SystemConfig
?   ??? Configurations/        # EF Core fluent configs
?   ??? ApplicationDbContext.cs
??? Services/
?   ??? Interfaces/           # Service contracts
?   ??? Implementations/      # Business logic
??? Pages/
?   ??? Account/              # Login, Register, Profile
?   ??? Wallet/               # TopUp, History
?   ??? Horoscope/            # Generate, History
??? ViewComponents/           # Reusable UI components
```

## Key Features

### User Management
- Email/Phone registration and login
- 30-day free trial for new users
- Session-based authentication

### Horoscope Generation
- Birth chart calculations (Rasi & Navamsa)
- Vimshottari Dasa predictions (120 years)
- Planetary positions and strengths
- South Indian style chart visualization

### Subscription & Billing
- ?5 per day charge after trial
- Wallet-based payment system
- Automatic daily deduction
- Transaction history

### Trial vs Paid Features

| Feature | Trial | Paid |
|---------|-------|------|
| Rasi Chart | ? | ? |
| Planetary Positions | ? | ? |
| Dasa (Main Periods) | ? | ? |
| Dasa Bhukti (Sub-periods) | ? | ? |
| Navamsa Chart | ? | ? |
| Planetary Strength | ? | ? |

## Database Schema

### Main Tables

**Users**: User accounts with trial tracking
- UserId, Email, MobileNumber, PasswordHash, FullName
- TrialStartDate, TrialEndDate, IsTrialActive
- LastDailyFeeDeductionDate

**Wallets**: User balance management
- WalletId, UserId, Balance
- CreatedDate, LastUpdatedDate

**Transactions**: Payment history
- TransactionId, UserId, WalletId
- Amount, TransactionType (Credit/Debit/Refund)
- BalanceBefore, BalanceAfter, Description

**HoroscopeGenerations**: Generation tracking
- GenerationId, UserId, GenerationDate
- BirthDateTime, PersonName, PlaceName
- Latitude, Longitude, AmountDeducted, WasTrialPeriod

**SystemConfig**: App configuration
- ConfigKey, ConfigValue, DataType, Description

### Key Configurations

```
MinimumWalletPurchase: 100.00 (?)
PerDayCost: 5.00 (?)
TrialPeriodDays: 30
MaxHoroscopesPerDay: 10
DasaYears: 120
```

## Core Services

### HoroscopeService
Main business logic for horoscope generation:
1. Check daily generation limit (10/day)
2. Verify trial status or check wallet balance
3. Deduct daily fee (?5) if needed
4. Calculate horoscope using TamilHoroscope.Core
5. Record generation in database
6. Return results with limited/full features based on subscription

### WalletService
- `AddFundsAsync()` - Top up wallet
- `DeductFundsAsync()` - Charge for horoscope
- `GetBalanceAsync()` - Check current balance
- `GetTransactionHistoryAsync()` - View past transactions

### SubscriptionService
- `IsUserInTrialAsync()` - Check trial status
- `GetTrialDaysRemainingAsync()` - Days left in trial
- `ShouldShowLowBalanceWarningAsync()` - Balance alerts

### ConfigService
- `GetPerDayCostAsync()` - Get daily charge amount
- `GetMaxHoroscopesPerDayAsync()` - Rate limit
- `GetDasaYearsAsync()` - Dasa calculation period

## Birth Place Features

### Auto-complete with 100+ Cities
- **Tamil Nadu**: Chennai, Coimbatore, Madurai, Trichy, Salem
- **India**: Mumbai, Delhi, Bangalore, Hyderabad, Kolkata
- **International**: New York, London, Paris, Dubai, Singapore

### Features
- Real-time filtering as you type
- Keyboard navigation (?? Enter Esc)
- Auto-fills latitude, longitude, timezone
- Click to select

## Recent Fixes

### PersonName Column Fix (Feb 2024)
**Issue**: `Invalid column name 'PersonName'` error  
**Fix**: Added PersonName column to HoroscopeGenerations table  
**See**: `FIX_PERSONNAME_COLUMN.md`

### View Again Fix (Jan 2024)
**Issue**: History "View Again" button showed errors  
**Fix**: Pass generationId via URL, fetch from database on Generate page

### Trial & Wallet Integration (Jan 2024)
**Issue**: Unclear trial expiry and balance warnings  
**Fix**: Added LowBalanceWarningViewComponent, trial countdown

## Troubleshooting

### Build Issues
```bash
dotnet clean
dotnet restore
dotnet build
```

### Database Connection
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Run migration: `dotnet ef database update`

### Authentication Problems
- Clear browser cookies/cache
- Check PasswordHash in database (should be BCrypt format)
- Verify session is enabled in Program.cs

### Horoscope Generation Errors
- Check TamilHoroscope.Core library is referenced
- Verify ephemeris files exist in Core/ephe/
- Ensure birth date/time are valid

## Development Checklist

- [ ] .NET 8.0 SDK installed
- [ ] SQL Server running
- [ ] Database created
- [ ] Connection string configured
- [ ] Migrations applied: `dotnet ef database update`
- [ ] Build successful: `dotnet build`
- [ ] SystemConfig seeded with default values

## Testing Flow

1. **Register** new user ? Trial starts automatically
2. **Generate** horoscope (free, limited features)
3. **View History** ? Click "View Again"
4. **Top-up Wallet** ? Add ?100+
5. **Generate** after trial ? ?5 deducted
6. **Check Transaction History** ? See deduction
7. **Generate Again** same day ? No additional charge

## For AI Agents

### Query Patterns
- **"How to add funds?"** ? WalletService.AddFundsAsync()
- **"Why is trial not working?"** ? Check TrialEndDate in Users table
- **"Horoscope generation fails"** ? Check HoroscopeService.cs line 139
- **"Missing column error"** ? Check migration files, run `dotnet ef database update`
- **"Authentication issues"** ? Check AuthenticationService.cs, session config

### Key Files
- Business Logic: `Services/Implementations/HoroscopeService.cs`
- Database Context: `Data/ApplicationDbContext.cs`
- Main Page: `Pages/Horoscope/Generate.cshtml.cs`
- Configuration: `appsettings.json`, SystemConfig table

---

**Version**: 1.0  
**Last Updated**: February 2026  
**Framework**: ASP.NET Core 8.0  
**Database**: SQL Server 2016+
