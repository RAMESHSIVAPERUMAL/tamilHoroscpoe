# Tamil Horoscope Web Application

A complete ASP.NET Core 8.0 Razor Pages web application for generating and managing Vedic astrology horoscopes with subscription and wallet-based billing system.

## Features

### ✅ Implemented

#### Database Layer
- **5 Core Tables**: Users, Wallets, Transactions, HoroscopeGenerations, SystemConfig
- **Complete SQL Scripts**: Table creation, indexes, and seed data in `/Database/Scripts/`
- **Optimized Indexes**: For login queries, transaction history, and daily horoscope checks
- **Seed Configuration**: Default values for pricing, trial periods, and limits

#### Data Models & EF Core
- **Entity Models**: Complete data models with validation and relationships
- **DbContext Configuration**: ApplicationDbContext with Identity integration
- **Entity Configurations**: Fluent API configurations for all entities
- **ASP.NET Core Identity**: Custom User entity extending IdentityUser<int>

#### Service Layer (Business Logic)
- **IConfigService**: System configuration management with typed value retrieval
- **IWalletService**: Wallet operations (add funds, deduct, check balance, transaction history)
- **ISubscriptionService**: Trial period management and balance warning logic
- **IHoroscopeService**: **CRITICAL** - Horoscope generation with daily deduction logic:
  - ✅ Check if horoscope already generated today (single daily charge)
  - ✅ Trial user handling (limited features, no charge)
  - ✅ Paid user handling (full features, wallet deduction)
  - ✅ Feature restrictions based on subscription status
  - ✅ Rate limiting (max horoscopes per day)

#### User Authentication & Registration
- **Register Page**: Email OR mobile number registration (India format +91)
- **Login Page**: Login with email or mobile number
- **Logout Handler**: Secure logout functionality
- **Trial Period**: Automatic 30-day trial on registration
- **Wallet Creation**: Automatic wallet creation for new users
- **Password Validation**: Secure password requirements (8+ chars, uppercase, lowercase, digit)

#### UI/UX Components
- **Modern Layout**: Bootstrap 5 with responsive navigation
- **Low Balance Warning**: ViewComponent that displays prominent warning when ≤10 days remaining
- **Icon Integration**: Bootstrap Icons for visual appeal
- **Landing Page**: Feature showcase with trial vs. paid comparison
- **Navigation**: Context-aware menu (different for logged-in users)

#### Configuration
- **Program.cs**: Complete service registration and middleware configuration
- **appsettings.json**: Connection string and application settings
- **Identity Setup**: Password policies, lockout settings, and cookie configuration

### 🔄 To Be Completed

#### Missing Razor Pages (High Priority)

1. **Wallet Pages** (`/Pages/Wallet/`)
   - `TopUp.cshtml/.cs` - Wallet recharge page with payment gateway placeholder
   - `History.cshtml/.cs` - Transaction history with pagination

2. **Horoscope Pages** (`/Pages/Horoscope/`)
   - `Generate.cshtml/.cs` - **MOST IMPORTANT** - Main horoscope generation form
     - Birth date/time pickers
     - Location input (latitude/longitude)
     - Integration with IHoroscopeService
     - Display horoscope results with Tamil language support
     - Filter Dasa display for trial users (hide Bhukti sub-periods)
   - `History.cshtml/.cs` - Previously generated horoscopes list

3. **Profile Page** (`/Pages/Account/`)
   - `Profile.cshtml/.cs` - User profile management and trial status display

4. **Admin Pages** (`/Pages/Admin/`) - Optional but recommended
   - `Config.cshtml/.cs` - SystemConfig management interface
   - Authorization policies for admin access

#### Testing Requirements

1. **User Flow Testing**
   - Register with email only
   - Register with mobile only
   - Register with both email and mobile
   - Login with email
   - Login with mobile number

2. **Trial Period Testing**
   - Generate horoscope during trial (verify limited features)
   - Check that no wallet deduction occurs during trial
   - Verify Dasa periods show only main periods (no Bhukti) for trial users

3. **Wallet & Billing Testing**
   - Top-up wallet
   - Generate first horoscope of the day (verify deduction)
   - Generate second horoscope same day (verify no additional charge)
   - Test with insufficient balance
   - Verify low balance warning displays correctly

4. **Full Feature Access Testing**
   - After wallet top-up, verify Navamsa chart is displayed
   - Verify full Dasa periods with Bhukti sub-periods are shown
   - Verify planetary strength table/chart is displayed

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2016+ or SQL Server Express
- Visual Studio 2022, VS Code, or Rider

### Database Setup

1. **Create Database**
   ```sql
   CREATE DATABASE [TamilHoroscopeDB];
   GO
   ```

2. **Run SQL Scripts** (in order)
   ```bash
   # Navigate to Database/Scripts folder
   cd Database/Scripts
   
   # Execute scripts in order
   sqlcmd -S localhost -d TamilHoroscopeDB -i 01_CreateTables.sql
   sqlcmd -S localhost -d TamilHoroscopeDB -i 02_CreateIndexes.sql
   sqlcmd -S localhost -d TamilHoroscopeDB -i 03_SeedData.sql
   ```

3. **Update Connection String**
   - Edit `TamilHoroscope.Web/appsettings.json`
   - Update `ConnectionStrings:DefaultConnection` with your SQL Server details

### Build & Run

```bash
# Navigate to web project
cd TamilHoroscope.Web

# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run
```

Application will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## Project Structure

```
TamilHoroscope.Web/
├── Data/
│   ├── Entities/              # Entity models
│   ├── Configurations/        # EF Core configurations
│   └── ApplicationDbContext.cs
├── Services/
│   ├── Interfaces/            # Service interfaces
│   └── Implementations/       # Service implementations
├── Pages/
│   ├── Account/               # Authentication pages
│   ├── Wallet/                # Wallet management (TO DO)
│   ├── Horoscope/             # Horoscope features (TO DO)
│   ├── Admin/                 # Admin pages (TO DO)
│   └── Shared/
│       └── _Layout.cshtml     # Main layout with navigation
├── ViewComponents/            # Reusable UI components
├── Views/
│   └── Shared/
│       └── Components/        # ViewComponent views
├── wwwroot/                   # Static files (CSS, JS, images)
├── Program.cs                 # Application startup
└── appsettings.json          # Configuration

Database/
└── Scripts/
    ├── 01_CreateTables.sql
    ├── 02_CreateIndexes.sql
    ├── 03_SeedData.sql
    └── README.md
```

## Key Business Logic

### Daily Deduction Logic (Implemented in HoroscopeService)

```
When user generates horoscope:
1. Check if horoscope already generated TODAY (by GenerationDate)
   → If YES: Return cached horoscope (no charge)
   → If NO: Continue to step 2

2. Check if user is in trial period
   → If IN TRIAL: 
     * Generate limited horoscope (no Navamsa, no strength, Dasa without Bhukti in UI)
     * Mark WasTrialPeriod = true
     * AmountDeducted = 0
   → If TRIAL EXPIRED:
     * Check wallet balance >= PerDayCost
     * If sufficient: Deduct amount, generate full horoscope
     * If insufficient: Show error, redirect to top-up

3. Record generation in HoroscopeGenerations table
4. Return horoscope data
```

### Trial vs. Paid Features

| Feature | Trial Period | Paid Subscription |
|---------|-------------|-------------------|
| Duration | 30 days | Until wallet depleted |
| Birth Details | ✅ | ✅ |
| Rasi Chart | ✅ | ✅ |
| Dasa Periods | Main periods only | Full with Bhukti sub-periods |
| Navamsa Chart | ❌ | ✅ |
| Planetary Strength | ❌ | ✅ |
| Daily Cost | Free | ₹5/day (configurable) |

## Configuration (SystemConfig Table)

| Key | Default | Description |
|-----|---------|-------------|
| MinimumWalletPurchase | 100.00 | Minimum wallet top-up amount (₹) |
| PerDayCost | 5.00 | Daily horoscope generation cost (₹) |
| TrialPeriodDays | 30 | Trial period duration (days) |
| LowBalanceWarningDays | 10 | Show warning when ≤ this many days |
| MaxHoroscopesPerDay | 10 | Rate limit per user |
| DasaYears | 120 | Years to calculate for Vimshottari Dasa |

All values can be modified through the database or admin interface.

## Integration with TamilHoroscope.Core

The web application uses the existing `TamilHoroscope.Core` library for horoscope calculations:

```csharp
var calculator = new PanchangCalculator();
var horoscope = calculator.CalculateHoroscope(
    birthDetails,
    includeDasa: true,           // Include Dasa periods
    includeNavamsa: !isTrialUser, // Only for paid users
    dasaYears: 120,              // From config
    includeStrength: !isTrialUser // Only for paid users
);
```

## Security Features

- ✅ ASP.NET Core Identity with secure password hashing
- ✅ HTTPS enforcement
- ✅ CSRF protection (built-in with Razor Pages)
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ Account lockout after failed login attempts
- ✅ Secure cookie-based authentication

## API for Tamil Language Support

All astrological terms have Tamil names available:
```csharp
// From TamilHoroscope.Core
horoscope.TamilLagnaRasiName     // Lagna in Tamil
planet.TamilName                  // Planet name in Tamil
planet.TamilRasiName             // Rasi in Tamil
planet.TamilNakshatraName        // Nakshatra in Tamil
```

## Next Steps for Developer

### High Priority (Must Complete)

1. **Create Horoscope/Generate.cshtml page** - This is the core functionality
   - Use DateTime pickers for birth date/time
   - Location input fields (latitude/longitude)
   - Call `IHoroscopeService.GenerateHoroscopeAsync()`
   - Display results with Tamil language support
   - **CRITICAL**: Filter Dasa display for trial users:
     ```csharp
     @if (!isTrialUser)
     {
         // Show Bhukti sub-periods
     }
     ```

2. **Create Wallet/TopUp.cshtml page**
   - Amount input (minimum ₹100)
   - Payment gateway placeholder (mock for now)
   - Call `IWalletService.AddFundsAsync()`

3. **Create Wallet/History.cshtml page**
   - Display transaction list with pagination
   - Call `IWalletService.GetTransactionHistoryAsync()`

4. **Create Horoscope/History.cshtml page**
   - List previously generated horoscopes
   - Call `IHoroscopeService.GetGenerationHistoryAsync()`
   - Allow viewing past horoscopes

### Medium Priority

5. **Create Account/Profile.cshtml page**
   - Display user information
   - Show trial status and days remaining
   - Show wallet balance and days remaining
   - Allow password change

6. **Testing**
   - Test all user flows
   - Verify daily deduction logic
   - Test low balance warning
   - Verify feature restrictions

### Low Priority (Nice to Have)

7. **Admin Pages**
   - Config management interface
   - User management
   - Transaction reports

8. **Enhancements**
   - Email verification
   - Mobile verification (SMS)
   - Payment gateway integration
   - PDF export of horoscopes
   - Multi-language UI (Tamil/English toggle)

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database exists and scripts have been run
- Check firewall settings (port 1433)

### Build Errors
- Run `dotnet restore` to restore NuGet packages
- Check .NET 8.0 SDK is installed: `dotnet --version`
- Clean and rebuild: `dotnet clean && dotnet build`

### Runtime Errors
- Check application logs in console output
- Verify all service dependencies are registered in Program.cs
- Ensure database connection is successful

## Support

For issues or questions:
- Check Database/Scripts/README.md for database setup
- Review service implementations in Services/Implementations/
- Examine HoroscopeService.cs for billing logic details

---

**Version:** 1.0  
**Last Updated:** 2026-02-08  
**Framework:** ASP.NET Core 8.0  
**Database:** MS SQL Server 2016+
