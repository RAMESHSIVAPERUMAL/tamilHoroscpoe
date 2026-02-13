# Tamil Horoscope Web - Quick Reference

## ?? Start Here

### Read First
?? **MASTER_DOCUMENTATION.md** - Complete guide

### Fix PersonName Error
?? **FIX_PERSONNAME_COLUMN.md** - Quick fix guide

---

## ? Quick Commands

```bash
# Apply database fix
dotnet ef database update

# Build & Run
dotnet build && dotnet run

# Access application
https://localhost:5001
```

---

## ?? Essential Files

| File | Purpose |
|------|---------|
| MASTER_DOCUMENTATION.md | Main documentation |
| FIX_PERSONNAME_COLUMN.md | Current fix guide |
| CLEANUP_SESSION_SUMMARY.md | What was done today |
| appsettings.json | Configuration |
| Program.cs | Application startup |

---

## ?? Common Tasks

### Register New User
1. Go to Account ? Register
2. Fill email/phone + password
3. Auto-starts 30-day trial

### Generate Horoscope
1. Login
2. Go to Horoscope ? Generate
3. Fill form (PersonName, birth details, location)
4. Click Generate
5. View charts and predictions

### Top-up Wallet
1. Go to Wallet ? Top Up
2. Enter amount (min ?100)
3. Submit (mock payment)

### View History
1. Go to Horoscope ? History
2. Click "View Again" to regenerate

---

## ?? Troubleshooting

### Build Issues
```bash
dotnet clean
dotnet restore
dotnet build
```

### Database Issues
```bash
# Check migrations
dotnet ef migrations list

# Apply migrations
dotnet ef database update
```

### PersonName Column Error
```sql
-- Check if column exists
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'HoroscopeGenerations' 
AND COLUMN_NAME = 'PersonName';

-- Add if missing
ALTER TABLE HoroscopeGenerations 
ADD PersonName NVARCHAR(100) NULL;
```

---

## ?? Key Concepts

### Trial System
- **Duration**: 30 days from registration
- **Features**: Limited (no Navamsa, no Bhukti, no Strength)
- **Cost**: Free

### Paid System
- **Cost**: ?5 per day
- **Features**: Full (all charts, Dasa Bhukti, strength)
- **Billing**: Once per day, not per generation

### Wallet System
- **Minimum**: ?100 top-up
- **Transactions**: Credit/Debit/Refund
- **History**: View all transactions

---

## ?? Database

### Main Tables
- **Users** - User accounts
- **Wallets** - Balance management
- **Transactions** - Payment history
- **HoroscopeGenerations** - Generation tracking
- **SystemConfig** - App settings

### Key Configs
```
PerDayCost: 5.00
TrialPeriodDays: 30
MaxHoroscopesPerDay: 10
MinimumWalletPurchase: 100.00
```

---

## ?? For Developers

### Project Structure
```
TamilHoroscope.Web/
??? Data/              # Database context & entities
??? Services/          # Business logic
??? Pages/             # Razor Pages
??? Migrations/        # EF migrations
??? wwwroot/           # Static files
```

### Key Services
- **HoroscopeService** - Generation logic
- **WalletService** - Payments
- **SubscriptionService** - Trial/paid status
- **ConfigService** - Settings

---

## ?? Need Help?

1. Check **MASTER_DOCUMENTATION.md** for detailed info
2. Review **FIX_PERSONNAME_COLUMN.md** for current fix
3. Check troubleshooting section in main docs
4. Review error logs in Output window

---

**Version**: 1.0  
**Framework**: ASP.NET Core 8.0  
**Database**: SQL Server  
**Status**: ? Build Successful
