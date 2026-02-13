# ?? QUICK START - Fresh Database Setup

## ? **5-Minute Complete Setup**

Follow these steps to create a fresh TamilHoroscope database:

---

## **Step 1: Open SQL Server Management Studio**

1. Launch **SSMS**
2. Connect to: `DESKTOP-99QP7PM\SQLEXPRESS`
3. Login: `sa` / `sasa`

---

## **Step 2: Run Scripts in Order**

Navigate to: `C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web\DatabaseSetup\`

Execute these files in **SSMS** (F5) in this **exact order**:

### ? **Script 1:** `01_CreateDatabase.sql`
- Creates TamilHoroscope database
- **Wait for:** "Database created successfully"

### ? **Script 2:** `02_CreateTables.sql`
- Creates 5 tables
- **Wait for:** "ALL TABLES CREATED SUCCESSFULLY"

### ? **Script 3:** `03_CreateIndexes.sql`
- Creates 10 indexes
- **Wait for:** "ALL INDEXES CREATED SUCCESSFULLY"

### ? **Script 4:** `04_InsertInitialData.sql`
- Inserts test user and config
- **Wait for:** "INITIAL DATA INSERTED SUCCESSFULLY"

### ? **Script 5:** `05_VerifySetup.sql`
- Verifies everything
- **Check:** All items show ?

---

## **Step 3: Test Login**

### **A. Run Application**
```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet build
dotnet run
```

### **B. Login**
- URL: `https://localhost:7262/Account/Login`
- Email: `rameshsivaperumal@gmail.com`
- Password: `Test@4321`

### **C. Expected Result**
? Login succeeds  
? Dashboard loads  
? Trial shows "30 days active"  
? Wallet shows "?0.00"

---

## **?? What You Get**

### **Database:**
- Name: `TamilHoroscope`
- Tables: 5 (Users, Wallets, Transactions, HoroscopeGenerations, SystemConfig)
- Indexes: 10 (for performance)

### **Test Account:**
- **Email:** rameshsivaperumal@gmail.com
- **Password:** Test@4321
- **Trial:** 30 days (active)
- **Balance:** ?0.00

### **System Config:**
- Daily cost: ?5
- Trial period: 30 days
- Minimum top-up: ?100

---

## **? Success Checklist**

- [ ] Database `TamilHoroscope` exists
- [ ] 5 tables created
- [ ] 10 indexes created
- [ ] Test user exists
- [ ] Wallet exists for test user
- [ ] Trial dates are NULLABLE
- [ ] Application builds successfully
- [ ] Login works with test account
- [ ] Dashboard displays correctly

---

## **?? Important Notes**

1. **Old Database:** Scripts will **DROP** existing TamilHoroscope database
2. **Data Loss:** All old data will be deleted
3. **Nullable Dates:** Trial dates are properly configured as NULLABLE
4. **Password Hash:** Pre-configured for `Test@4321`

---

## **?? If Login Still Fails**

### **Quick Debug:**

```sql
USE TamilHoroscope;

-- Check user
SELECT Email, IsActive, TrialStartDate, TrialEndDate,
       CASE WHEN PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=' 
       THEN '? Test@4321' ELSE '? Wrong' END AS PasswordCheck
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';

-- Check nullable columns
SELECT COLUMN_NAME, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
AND COLUMN_NAME IN ('TrialStartDate', 'TrialEndDate', 'LastDailyFeeDeductionDate');
```

**Expected:**
- User exists ?
- IsActive = 1 ?
- PasswordCheck = "? Test@4321"
- All three columns show IS_NULLABLE = 'YES' ?

---

## **?? Need Help?**

1. Check SSMS **Messages** tab for errors
2. Run `05_VerifySetup.sql` for detailed diagnosis
3. Ensure all 5 scripts completed successfully
4. Check application console for detailed logs

---

**Time Required:** ~5 minutes  
**Difficulty:** Easy  
**Result:** Fresh, working database ?
