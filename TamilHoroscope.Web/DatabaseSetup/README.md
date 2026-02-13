# ??? TamilHoroscope Database Setup

## ?? Complete Fresh Database Installation

This folder contains all scripts needed to create a fresh TamilHoroscope database from scratch.

---

## ?? **Quick Start (Recommended)**

### **Option 1: Run Individual Scripts in Order**

Execute these scripts in **SQL Server Management Studio (SSMS)** in this order:

1. ? `01_CreateDatabase.sql` - Creates the database
2. ? `02_CreateTables.sql` - Creates all tables
3. ? `03_CreateIndexes.sql` - Creates performance indexes
4. ? `04_InsertInitialData.sql` - Inserts test data
5. ? `05_VerifySetup.sql` - Verifies everything is correct

### **Option 2: Run All at Once (If SQLCMD Available)**

```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web\DatabaseSetup
sqlcmd -S DESKTOP-99QP7PM\SQLEXPRESS -U sa -P sasa -i 00_RunAll.sql
```

---

## ?? **What Gets Created**

### **1. Database**
- Name: `TamilHoroscope`
- Data File: 100MB (auto-grow by 10MB)
- Log File: 50MB (auto-grow by 10MB)

### **2. Tables**

| Table | Purpose | Key Features |
|-------|---------|--------------|
| **Users** | User accounts | ? Nullable trial dates<br>? Email & mobile support<br>? SHA256 password hashing |
| **Wallets** | User wallet balances | ? One per user<br>? Balance constraints |
| **Transactions** | Financial history | ? Credit/Debit/Refund<br>? Before/After balance |
| **HoroscopeGenerations** | Horoscope history | ? Daily tracking<br>? Location data |
| **SystemConfig** | App settings | ? Dynamic configuration<br>? Type-safe values |

### **3. Indexes (10 total)**

Performance indexes on:
- User lookups (Email, Mobile)
- Wallet queries
- Transaction history
- Horoscope generation tracking

### **4. Initial Data**

**System Configuration:**
- PerDayCost: ?5.00
- TrialPeriodDays: 30
- MinimumTopUpAmount: ?100.00
- LowBalanceWarningDays: 3

**Test User:**
- Email: `rameshsivaperumal@gmail.com`
- Password: `Test@4321`
- Trial: 30 days (active)
- Wallet: ?0.00

---

## ?? **Manual Step-by-Step Instructions**

### **Step 1: Open SQL Server Management Studio (SSMS)**

1. Launch SSMS
2. Connect to: `DESKTOP-99QP7PM\SQLEXPRESS`
3. Login with: `sa` / `sasa`

### **Step 2: Create Database**

1. Open: `01_CreateDatabase.sql`
2. Execute (F5)
3. Verify: "Database created successfully" message

### **Step 3: Create Tables**

1. Open: `02_CreateTables.sql`
2. Execute (F5)
3. Verify: 5 tables created

### **Step 4: Create Indexes**

1. Open: `03_CreateIndexes.sql`
2. Execute (F5)
3. Verify: 10 indexes created

### **Step 5: Insert Initial Data**

1. Open: `04_InsertInitialData.sql`
2. Execute (F5)
3. Verify: Config entries and test user created

### **Step 6: Verify Setup**

1. Open: `05_VerifySetup.sql`
2. Execute (F5)
3. Review verification report
4. Ensure all checks pass ?

---

## ? **Verification Checklist**

After running all scripts, verify:

- [ ] Database `TamilHoroscope` exists
- [ ] 5 tables created (Users, Wallets, Transactions, HoroscopeGenerations, SystemConfig)
- [ ] 10 indexes created
- [ ] 5 system config entries
- [ ] Test user exists with email `rameshsivaperumal@gmail.com`
- [ ] Test user has wallet with ?0 balance
- [ ] Test user has 30-day active trial
- [ ] Password hash matches `Test@4321`
- [ ] All trial date columns are NULLABLE

---

## ?? **Quick Verification Queries**

### **Check Tables:**
```sql
USE TamilHoroscope;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
```

### **Check Test User:**
```sql
SELECT 
    Email,
    FullName,
    IsActive,
    IsTrialActive,
    TrialEndDate,
    CASE 
        WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' 
        THEN 'Test@4321 ?'
        ELSE 'Wrong password ?'
    END AS PasswordCheck
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';
```

### **Check Nullable Columns:**
```sql
SELECT 
    COLUMN_NAME,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
    AND COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate');
```

Expected:
```
TrialStartDate             YES
TrialEndDate               YES
LastDailyFeeDeductionDate  YES
```

---

## ?? **After Database Setup**

### **1. Update Connection String**

File: `TamilHoroscope.Web/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DESKTOP-99QP7PM\\SQLEXPRESS;Database=TamilHoroscope;User Id=sa;Password=sasa;TrustServerCertificate=true;Connection Timeout=30;"
  }
}
```

### **2. Run Application**

```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet build
dotnet run
```

### **3. Test Login**

1. Navigate to: `https://localhost:7262/Account/Login`
2. Email: `rameshsivaperumal@gmail.com`
3. Password: `Test@4321`
4. Click "Log in"

**Expected:** ? Login succeeds, dashboard loads

---

## ?? **Troubleshooting**

### **Issue: Database already exists**

**Solution:** Script 01 will drop and recreate it

### **Issue: Permission denied**

**Solution:** Run SSMS as Administrator or use appropriate SQL credentials

### **Issue: File path error**

**Solution:** Adjust file paths in `01_CreateDatabase.sql` to match your SQL Server installation

### **Issue: Login still fails**

**Solution:** Run `05_VerifySetup.sql` and check all verification steps

---

## ?? **File Structure**

```
DatabaseSetup/
??? README.md                    # This file
??? 00_RunAll.sql               # Master script (SQLCMD)
??? 01_CreateDatabase.sql       # Database creation
??? 02_CreateTables.sql         # Table creation
??? 03_CreateIndexes.sql        # Index creation
??? 04_InsertInitialData.sql    # Initial data
??? 05_VerifySetup.sql          # Verification
```

---

## ?? **Test Account Details**

| Field | Value |
|-------|-------|
| **Email** | rameshsivaperumal@gmail.com |
| **Password** | Test@4321 |
| **Password Hash** | tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU= |
| **Full Name** | Ramesh Sivaperumal |
| **Trial Period** | 30 days (active) |
| **Wallet Balance** | ?0.00 |
| **Account Status** | Active |

---

## ?? **Database Schema Overview**

### **Users Table (Primary)**
```
UserId (PK)
??? Email (unique, nullable)
??? MobileNumber (unique, nullable)
??? PasswordHash (SHA256)
??? FullName
??? Dates (Created, LastLogin)
??? Trial (StartDate, EndDate, IsActive) ? ALL NULLABLE
??? LastDailyFeeDeductionDate (nullable)
```

### **Wallets Table**
```
WalletId (PK)
??? UserId (FK ? Users, unique)
??? Balance (? 0)
??? Dates (Created, LastUpdated)
```

### **Transactions Table**
```
TransactionId (PK)
??? WalletId (FK ? Wallets)
??? UserId (FK ? Users)
??? Type (Credit/Debit/Refund)
??? Amount
??? BalanceBefore
??? BalanceAfter
??? TransactionDate
```

---

## ? **Success Criteria**

Your setup is successful when:

1. ? All 5 scripts execute without errors
2. ? Verification script shows all green checkmarks
3. ? Test user can login to application
4. ? Dashboard loads correctly
5. ? No errors in application logs

---

## ?? **Need Help?**

If you encounter issues:

1. Check the **Messages** tab in SSMS for detailed errors
2. Run `05_VerifySetup.sql` to see what failed
3. Ensure SQL Server service is running
4. Verify connection string matches your SQL Server instance

---

**Last Updated:** 2024-02-14  
**Version:** 1.0  
**Status:** ? Ready for Production
