# Tamil Horoscope Web - Complete Documentation

> **AI-Friendly Documentation** | Last Updated: 2024  
> Framework: ASP.NET Core 8.0 | Database: SQL Server

---

## ?? Table of Contents

1. [Quick Reference](#quick-reference)
2. [Architecture Overview](#architecture-overview)
3. [Recent Fixes & Implementations](#recent-fixes--implementations)
4. [Database Schema](#database-schema)
5. [Service Layer](#service-layer)
6. [Key Features](#key-features)
7. [Troubleshooting Guide](#troubleshooting-guide)

---

## Quick Reference

### ?? Running the Application

```bash
cd TamilHoroscope.Web
dotnet restore
dotnet build
dotnet run
```

**URLs:**
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

### ?? Key Files

| File | Purpose |
|------|---------|
| `Program.cs` | App configuration, DI setup, middleware |
| `ApplicationDbContext.cs` | EF Core database context |
| `Services/Implementations/HoroscopeService.cs` | **Core business logic** for horoscope generation |
| `Pages/Horoscope/Generate.cshtml` | Main horoscope generation page |
| `Pages/Account/Login.cshtml` | User authentication |

---

## Architecture Overview

### Tech Stack
- **Framework**: ASP.NET Core 8.0 (Razor Pages)
- **ORM**: Entity Framework Core 8.0
- **Database**: MS SQL Server 2016+
- **Auth**: ASP.NET Core Identity
- **UI**: Bootstrap 5, Bootstrap Icons

### Project Structure

```
TamilHoroscope.Web/
??? Data/
?   ??? Entities/              # User, Wallet, Transaction, HoroscopeGeneration, SystemConfig
?   ??? Configurations/        # EF fluent configs
?   ??? ApplicationDbContext.cs
??? Services/
?   ??? Interfaces/           # IHoroscopeService, IWalletService, etc.
?   ??? Implementations/      # Service implementations
??? Pages/
?   ??? Account/              # Login, Logout, Register, Profile
?   ??? Wallet/               # TopUp, History
?   ??? Horoscope/            # Generate, History
?   ??? Shared/               # _Layout.cshtml
??? ViewComponents/           # LowBalanceWarningViewComponent
??? wwwroot/                  # Static assets
```

---

## Recent Fixes & Implementations

### ? 1. Authentication & Logout (Jan 2024)
**Issue**: Logout from Generate page showed validation errors  
**Fix**: Added `Logout.cshtml` view and `OnGet()` handler

**Files**:
- `Pages/Account/Logout.cshtml` (Created)
- `Pages/Account/Logout.cshtml.cs` (Added OnGet)

### ? 2. Person Name Field (Jan 2024)
**Issue**: No field to enter person's name  
**Fix**: Added required PersonName field to Generate form

**Files**:
- `Pages/Horoscope/Generate.cshtml.cs` (Added PersonName property)
- `Pages/Horoscope/Generate.cshtml` (Added input field)

### ? 3. Birth Place Auto-Complete (Jan 2024)
**Issue**: No auto-complete when typing city names  
**Fix**: Implemented dropdown with 100+ cities, keyboard navigation

**Features**:
- 100+ pre-loaded cities (Tamil Nadu, India, International)
- Real-time filtering
- Keyboard navigation (?? Enter Esc)
- Auto-fills coordinates and timezone

**Files**:
- `Pages/Horoscope/Generate.cshtml` (JavaScript + CSS)

### ? 4. History "View Again" Fix (Jan 2024)
**Issue**: Clicking "View Again" showed error page  
**Fix**: Added `OnGetAsync` to handle regenerated horoscopes from TempData

**Flow**:
```
History Page ? Click "View Again" ? Regenerate horoscope
? Store in TempData ? Redirect to Generate?regenerated=true
? OnGetAsync reads TempData ? Display horoscope
```

**Files**:
- `Pages/Horoscope/Generate.cshtml.cs` (Enhanced OnGetAsync)
- `Pages/Horoscope/History.cshtml.cs` (Added TempData fields)

### ? 5. Rasi & Navamsa Charts (Jan 2024)
**Issue**: No visual birth chart display  
**Fix**: Implemented South Indian style charts in HTML/CSS/JS

**Features**:
- **Rasi Chart**: 4x4 grid, Lagna marker, Tamil planet names
- **Navamsa Chart**: Same style, paid users only
- Responsive design (400px desktop, 320px mobile)
- Auto-renders on page load

**Files**:
- `Pages/Horoscope/Generate.cshtml` (CSS + JavaScript)

### ? 6. Wallet & Balance Management
**Features**:
- Top-up wallet (min ?100)
- Transaction history with pagination
- Low balance warning component
- Automatic daily deduction

**Files**:
- `Pages/Wallet/TopUp.cshtml/.cs`
- `Pages/Wallet/History.cshtml/.cs`
- `ViewComponents/LowBalanceWarningViewComponent.cs`

**See**: `BALANCE_FUNCTIONALITY_IMPLEMENTATION.md` for details

---

## Database Schema

### Tables

#### 1. Users (ASP.NET Identity)
```sql
Users (
    UserId INT PRIMARY KEY IDENTITY,
    UserName NVARCHAR(256),
    NormalizedUserName NVARCHAR(256),
    Email NVARCHAR(256),
    NormalizedEmail NVARCHAR(256),
    EmailConfirmed BIT,
    PasswordHash NVARCHAR(MAX),
    PhoneNumber NVARCHAR(MAX),
    PhoneNumberConfirmed BIT,
    TwoFactorEnabled BIT,
    LockoutEnd DATETIMEOFFSET,
    LockoutEnabled BIT,
    AccessFailedCount INT,
    RegistrationDate DATETIME2,
    TrialEndDate DATETIME2
)
```

#### 2. Wallets
```sql
Wallets (
    WalletId INT PRIMARY KEY IDENTITY,
    UserId INT UNIQUE,
    Balance DECIMAL(10,2) DEFAULT 0,
    CreatedDateTime DATETIME2,
    UpdatedDateTime DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
)
```

#### 3. Transactions
```sql
Transactions (
    TransactionId INT PRIMARY KEY IDENTITY,
    UserId INT,
    WalletId INT,
    Amount DECIMAL(10,2),
    TransactionType NVARCHAR(20), -- 'Credit' or 'Debit'
    Description NVARCHAR(500),
    TransactionDate DATETIME2,
    CreatedDateTime DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (WalletId) REFERENCES Wallets(WalletId)
)
```

#### 4. HoroscopeGenerations
```sql
HoroscopeGenerations (
    GenerationId INT PRIMARY KEY IDENTITY,
    UserId INT,
    GenerationDate DATE,           -- Date only (for daily deduction check)
    BirthDateTime DATETIME2,       -- Birth date & time
    PlaceName NVARCHAR(200),
    Latitude DECIMAL(9,6),
    Longitude DECIMAL(9,6),
    AmountDeducted DECIMAL(10,2),
    WasTrialPeriod BIT,
    CreatedDateTime DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
)
```

#### 5. SystemConfig
```sql
SystemConfig (
    ConfigId INT PRIMARY KEY IDENTITY,
    ConfigKey NVARCHAR(100) UNIQUE,
    ConfigValue NVARCHAR(500),
    Description NVARCHAR(500),
    UpdatedDate DATETIME2
)
```

### Key Configuration Values

| ConfigKey | Default | Description |
|-----------|---------|-------------|
| `MinimumWalletPurchase` | 100.00 | Min top-up amount (?) |
| `PerDayCost` | 5.00 | Daily horoscope cost (?) |
| `TrialPeriodDays` | 30 | Trial duration (days) |
| `LowBalanceWarningDays` | 10 | Show warning when ? this many days |
| `MaxHoroscopesPerDay` | 10 | Rate limit per user |
| `DasaYears` | 120 | Vimshottari Dasa calculation years |

---

## Service Layer

### IHoroscopeService (Core Business Logic)

**Critical Method**: `GenerateHoroscopeAsync()`

```csharp
public async Task<(HoroscopeData? horoscope, HoroscopeGeneration? generation, string? errorMessage)> 
    GenerateHoroscopeAsync(
        int userId,
        DateTime birthDateTime,
        double latitude,
        double longitude,
        double timeZoneOffset,
        string? placeName = null)
```

**Logic Flow**:
```
1. Check if horoscope already generated TODAY
   ? YES: Return cached (no charge)
   ? NO: Continue

2. Check if user in trial period
   ? Trial: Limited features, no charge
   ? Paid: Check balance, deduct, full features

3. Rate limiting check (max 10/day)

4. Calculate horoscope using TamilHoroscope.Core

5. Record in HoroscopeGenerations table

6. Return horoscope data
```

### Other Services

| Service | Purpose |
|---------|---------|
| `IWalletService` | Add funds, deduct, check balance, transaction history |
| `ISubscriptionService` | Check trial status, days remaining, balance warnings |
| `IConfigService` | Get/set system configuration values |
| `IAuthenticationService` | User registration, login |

---

## Key Features

### Trial vs Paid Comparison

| Feature | Trial (30 days) | Paid (?5/day) |
|---------|----------------|---------------|
| Rasi Chart | ? | ? |
| Planetary Positions | ? | ? |
| Dasa (Main Periods) | ? | ? |
| **Dasa Bhukti** | ? | ? |
| **Navamsa Chart** | ? | ? |
| **Planetary Strength** | ? | ? |
| Daily Cost | Free | ?5 |

### Auto-Complete Cities (100+)

**Tamil Nadu**: Chennai, Coimbatore, Madurai, Trichy, Salem, Vellore, Tirunelveli, etc.  
**India**: Mumbai, Delhi, Bangalore, Hyderabad, Kolkata, Pune, Ahmedabad, etc.  
**International**: New York, London, Paris, Dubai, Singapore, Tokyo, Sydney, etc.

### Visual Charts

**Rasi Chart** (South Indian Style):
```
?????????????????????????????????????
?   12   ?   1    ?   2    ?   3    ?
?        ? ??? ?????        ?        ?
?????????????????????????????????????
?   11   ?        ????     ?   4    ?
??????????                 ??????????
?   10   ?                 ?   5    ?
?????????????????????????????????????
?   9    ?   8    ?   7    ?   6    ?
?????????????????????????????????????
```

- Lagna marker (???) in red
- Tamil planet abbreviations
- 4x4 grid layout
- Responsive design

---

## Troubleshooting Guide

### Common Issues

#### 1. Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

#### 2. Database Connection
- Check SQL Server is running
- Verify connection string in `appsettings.json`
- Test connection: `sqlcmd -S localhost -E`

#### 3. Missing Dependencies
```bash
# Restore NuGet packages
dotnet restore

# Check .NET version
dotnet --version  # Should be 8.0.x
```

#### 4. Authentication Issues
- Clear browser cookies
- Check Identity tables exist in database
- Verify password meets requirements (8+ chars, uppercase, lowercase, digit)

#### 5. Horoscope Generation Errors
- Check `TamilHoroscope.Core` library is referenced
- Verify Swiss Ephemeris files in `TamilHoroscope.Core/ephe/`
- Check birth date/time is valid

### Debug Checklist

- [ ] .NET 8.0 SDK installed
- [ ] SQL Server running
- [ ] Database created and scripts executed
- [ ] Connection string updated
- [ ] NuGet packages restored
- [ ] Build successful
- [ ] Identity tables exist
- [ ] SystemConfig seeded

### Logs & Diagnostics

```csharp
// Enable detailed logging in appsettings.json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.EntityFrameworkCore": "Information"
  }
}
```

---

## Quick Implementation Checklist

### For New Developers

- [ ] Clone repository
- [ ] Install .NET 8.0 SDK
- [ ] Setup SQL Server
- [ ] Create database
- [ ] Run SQL scripts (01_CreateTables, 02_CreateIndexes, 03_SeedData)
- [ ] Update connection string
- [ ] Build and run
- [ ] Register test user
- [ ] Test trial period features
- [ ] Top-up wallet (mock payment)
- [ ] Test paid features
- [ ] View history
- [ ] Test charts rendering

### For AI/LLM Context

**When asked about**:
- **Authentication**: See `Pages/Account/` + `Services/Implementations/AuthenticationService.cs`
- **Horoscope Generation**: See `Services/Implementations/HoroscopeService.cs`
- **Wallet/Billing**: See `Services/Implementations/WalletService.cs`
- **Visual Charts**: See `Pages/Horoscope/Generate.cshtml` (JavaScript section)
- **Database Schema**: See this document's Database Schema section
- **Recent Fixes**: See Recent Fixes & Implementations section above

---

## Related Documentation

- `FIXES_SUMMARY.md` - Detailed fix history (logout, person name, auto-complete, click selection)
- `HISTORY_VIEW_FIX.md` - View Again functionality implementation
- `RASI_CHART_IMPLEMENTATION.md` - Visual charts implementation
- `BALANCE_FUNCTIONALITY_IMPLEMENTATION.md` - Wallet & balance features

---

**Version**: 1.0  
**Framework**: ASP.NET Core 8.0  
**Database**: SQL Server 2016+  
**Last Updated**: 2024
