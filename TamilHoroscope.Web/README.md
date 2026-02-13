# Tamil Horoscope Web Application

> **ASP.NET Core 8.0 Razor Pages** | Vedic Astrology Horoscope Generator with Subscription System

A complete web application for generating and managing Tamil/Vedic astrology horoscopes with wallet-based billing, trial periods, and visual South Indian style charts.

---

## ?? Quick Start

```bash
# 1. Create database
sqlcmd -S localhost -Q "CREATE DATABASE TamilHoroscope"

# 2. Run SQL scripts
cd Database/Scripts
sqlcmd -S localhost -d TamilHoroscope -i 01_CreateTables.sql
sqlcmd -S localhost -d TamilHoroscope -i 02_CreateIndexes.sql
sqlcmd -S localhost -d TamilHoroscope -i 03_SeedData.sql

# 3. Update connection string in appsettings.json

# 4. Build and run
cd TamilHoroscope.Web
dotnet restore
dotnet build
dotnet run
```

**Access**: https://localhost:5001

---

## ? Features

### Core Functionality
- ? **User Authentication** - Email or mobile number login
- ? **30-Day Trial Period** - Free limited features
- ? **Wallet System** - Top-up, automatic deductions, transaction history
- ? **Horoscope Generation** - Complete Vedic astrology calculations
- ? **Visual Charts** - South Indian style Rasi & Navamsa charts
- ? **Tamil Language Support** - All astrological terms in Tamil
- ? **Auto-Complete Cities** - 100+ pre-loaded cities worldwide
- ? **History** - View previously generated horoscopes

### Trial vs Paid

| Feature | Trial (30 days) | Paid (?5/day) |
|---------|----------------|---------------|
| Rasi Chart | ? | ? |
| Planetary Positions | ? | ? |
| Vimshottari Dasa | Main only | With Bhukti |
| Navamsa Chart | ? | ? |
| Planetary Strength | ? | ? |

---

## ?? Project Structure

```
TamilHoroscope.Web/
??? Data/
?   ??? Entities/              # User, Wallet, Transaction, etc.
?   ??? ApplicationDbContext.cs
??? Services/
?   ??? Interfaces/            # Service contracts
?   ??? Implementations/       # Business logic
??? Pages/
?   ??? Account/               # Login, Register, Logout, Profile
?   ??? Wallet/                # TopUp, History
?   ??? Horoscope/             # Generate, History
?   ??? Shared/                # _Layout.cshtml
??? ViewComponents/            # LowBalanceWarningViewComponent
??? wwwroot/                   # CSS, JS, images
```

---

## ?? Tech Stack

- **Framework**: ASP.NET Core 8.0 (Razor Pages)
- **ORM**: Entity Framework Core 8.0
- **Database**: MS SQL Server 2016+
- **Authentication**: ASP.NET Core Identity
- **UI**: Bootstrap 5, Bootstrap Icons
- **Calculations**: TamilHoroscope.Core (Swiss Ephemeris)

---

## ?? Documentation

### Essential Documentation
- **[MASTER_DOCUMENTATION.md](MASTER_DOCUMENTATION.md)** - Complete project guide (start here!)
- **[QUICK_START.md](QUICK_START.md)** - Quick reference and common commands
- **[FIX_PERSONNAME_COLUMN.md](FIX_PERSONNAME_COLUMN.md)** - Current fix guide

All documentation is written in simple English and is both human-friendly and AI-friendly.

---

## ?? Key Business Logic

### Daily Deduction System

```
User generates horoscope:
?? Already generated today? ? Return cached (no charge)
?? In trial period?
?  ?? Yes ? Limited features, no charge
?  ?? No ? Check balance, deduct ?5, full features
?? Record in HoroscopeGenerations table
```

**File**: `Services/Implementations/HoroscopeService.cs`

---

## ??? Development

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2016+ or SQL Server Express
- Visual Studio 2022 / VS Code / Rider

### Database Schema

**5 Core Tables**:
1. `Users` - ASP.NET Identity users with trial end date
2. `Wallets` - User wallet balances
3. `Transactions` - Credit/debit history
4. `HoroscopeGenerations` - Generated horoscopes tracking
5. `SystemConfig` - App configuration (pricing, limits)

**See**: [MASTER_DOCUMENTATION.md#database-schema](MASTER_DOCUMENTATION.md#database) for details

### Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TamilHoroscope;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

System config in `SystemConfig` table:
- `PerDayCost`: 5.00 (?5 per day)
- `TrialPeriodDays`: 30
- `MinimumWalletPurchase`: 100.00
- `LowBalanceWarningDays`: 10
- `MaxHoroscopesPerDay`: 10

---

## ?? Testing

### Test User Flow

1. **Register** - Email/mobile, auto-creates wallet, 30-day trial
2. **Generate Horoscope** (Trial) - Limited features, no charge
3. **Top-Up Wallet** - Add ?100+ (mock payment)
4. **Generate Horoscope** (Paid) - Full features, ?5 deducted
5. **View History** - See previous horoscopes
6. **View Again** - Re-display historical horoscope (free)

---

## ?? Troubleshooting

### Build Issues
```bash
dotnet clean
dotnet restore
dotnet build
```

### Database Issues
- Check SQL Server running: `sqlcmd -S localhost -E`
- Verify connection string in appsettings.json
- Re-run SQL scripts if tables missing

### Authentication Issues
- Clear browser cookies
- Check password requirements (8+ chars, uppercase, lowercase, digit)
- Verify Identity tables exist

**See**: [MASTER_DOCUMENTATION.md#troubleshooting](MASTER_DOCUMENTATION.md#troubleshooting)

---

## ?? Recent Updates

### February 2024
- ? Fixed PersonName column missing error
- ? Consolidated documentation (4 clean files)
- ? Removed 32+ redundant documentation files

### January 2024
- ? Fixed logout from Generate page
- ? Added PersonName field (required)
- ? Implemented auto-complete for 100+ cities
- ? Fixed "View Again" from history
- ? Added South Indian style Rasi & Navamsa charts
- ? Wallet and balance management complete

**See**: [MASTER_DOCUMENTATION.md](MASTER_DOCUMENTATION.md) for detailed information

---

## ?? Support

- **Documentation**: Start with [MASTER_DOCUMENTATION.md](MASTER_DOCUMENTATION.md)
- **Quick Reference**: See [QUICK_START.md](QUICK_START.md)
- **Current Fix**: See [FIX_PERSONNAME_COLUMN.md](FIX_PERSONNAME_COLUMN.md)
- **Database Setup**: Review `Database/Scripts/README.md`

---

**Version**: 1.0  
**Framework**: ASP.NET Core 8.0  
**License**: Proprietary  
**Last Updated**: 2024
