# ?? Login Testing Guide

## Your Credentials

- **Email:** `rameshsivaperumal@gmail.com`
- **Password:** `Test@4321`

---

## ? Quick Test Steps

### **1. Verify User in Database (Optional)**

Run this SQL script to verify your account:
```sql
USE [TamilHoroscope];

SELECT 
    UserId,
    Email,
    FullName,
    IsActive,
    IsTrialActive,
    TrialStartDate,
    TrialEndDate,
    CASE 
        WHEN LEN(PasswordHash) > 0 THEN '? Has password'
        ELSE '? No password'
    END AS PasswordStatus
FROM Users
WHERE Email = 'rameshsivaperumal@gmail.com';
```

**Expected:** 
- ? User exists
- ? IsActive = 1
- ? PasswordHash exists

---

### **2. Run the Application**

Open terminal in project directory:
```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet run
```

**Expected output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7262
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

### **3. Open Browser**

Navigate to:
```
https://localhost:7262/Account/Login
```

---

### **4. Enter Credentials**

- **Email or Mobile Number:** `rameshsivaperumal@gmail.com`
- **Password:** `Test@4321`
- **Remember me:** (optional - check if you want to stay logged in)

Click **"Log in"** button

---

### **5. Expected Results**

#### ? **SUCCESS:**
- Redirects to: `https://localhost:7262/` (home page)
- See welcome message: "Welcome, [Your Full Name]!"
- Dashboard shows:
  - Trial status (if active)
  - Wallet balance
  - Quick action buttons

#### ? **FAILURE:**
If login fails, you'll see:
- Error message: "Invalid email/mobile or password."
- Stay on login page

---

## ?? **Troubleshooting**

### **Issue 1: "Invalid email/mobile or password"**

**Possible Causes:**
1. Wrong password
2. Email case mismatch
3. User is inactive
4. Password hash doesn't match

**Solutions:**

#### **A. Verify Password Hash**

Run this C# code to get correct hash for `Test@4321`:
```csharp
using System;
using System.Security.Cryptography;
using System.Text;

var password = "Test@4321";
var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
Console.WriteLine($"Hash for '{password}': {hash}");
```

**Expected hash:** `tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=`

#### **B. Update Password in Database**

If hash doesn't match, run:
```sql
USE [TamilHoroscope];

UPDATE Users
SET PasswordHash = 'tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU='
WHERE Email = 'rameshsivaperumal@gmail.com';

PRINT '? Password updated for Test@4321';
```

#### **C. Ensure User is Active**

```sql
UPDATE Users
SET IsActive = 1
WHERE Email = 'rameshsivaperumal@gmail.com';
```

#### **D. Try Lowercase Email**

The system normalizes emails to lowercase. Try:
- `rameshsivaperumal@gmail.com` (all lowercase) ?

---

### **Issue 2: Application Won't Start**

**Check port availability:**
```cmd
netstat -ano | findstr :7262
```

If port is in use, kill the process or change port in `launchSettings.json`.

---

### **Issue 3: Database Connection Failed**

**Verify connection string in `appsettings.json`:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=DESKTOP-99QP7PM\\SQLEXPRESS;Database=TamilHoroscope;User Id=sa;Password=sasa;TrustServerCertificate=true;"
}
```

**Test connection:**
```sql
-- Run in SSMS
SELECT @@SERVERNAME AS ServerName, DB_NAME() AS DatabaseName;
```

---

## ?? **What Happens After Login**

### **1. Authentication Process**

```
User enters credentials
    ?
AuthenticationService.AuthenticateAsync()
    ?
- Normalize email to lowercase
- Query database for user
- Verify password hash
- Check IsActive status
    ?
Create authentication cookie
    ?
Store UserId in session
    ?
CheckAndUpdateTrialStatusAsync()
    ?
Redirect to dashboard
```

### **2. Session Data Stored**

After successful login:
- `Session["UserId"]` = Your user ID
- `Session["UserEmail"]` = Your email
- `Session["UserFullName"]` = Your full name
- Authentication cookie (30 minutes)

### **3. Trial Status Check**

The system automatically:
1. Checks your wallet balance
2. Checks last fee deduction date
3. Activates trial if balance < ?5 and haven't paid today
4. Updates trial dates accordingly

---

## ?? **After Successful Login**

You should see the **Dashboard** with:

### **Status Cards:**
- **Trial Active** (if in trial)
  - Days remaining
  - End date
- **Wallet Balance**
  - Current balance
  - Add funds button
- **Premium Status**
  - Active/Inactive
  - Features unlocked
- **Days Remaining**
  - Based on wallet or trial

### **Quick Actions:**
- ?? **Generate Horoscope** (main feature)
- ?? **View History** (past horoscopes)
- ?? **Top Up Wallet** (add funds)
- ?? **My Profile** (account settings)

---

## ?? **Password Hash Reference**

For your reference, here are hashes for common test passwords:

| Password | SHA256 Hash (Base64) |
|----------|---------------------|
| `Test@123` | `oKWRFTtOX8HwB6qXqwUQlWEZQbA1OfFmWrALdpqNl0k=` |
| `Test@4321` | `tMgcE7lD5Jq5Gw3XkQsU5KjGxYLLNZMYLKV8p8nHZHU=` |
| `Admin@123` | `3EqE+3UL3pKPsQYDr7VvQYlV6Y1LmDEP8F4pD0nXPx4=` |
| `Password@123` | `gvCCrXXnmT7TGwRLhXEKmBvE5OhLc8cX5lGd8Lf5Wao=` |

---

## ?? **Quick Commands**

### **Run Application:**
```cmd
cd C:\GitWorkplace\tamilHoroscpoe\TamilHoroscope.Web
dotnet run
```

### **Build Application:**
```cmd
dotnet build
```

### **Watch for Changes:**
```cmd
dotnet watch run
```

### **Check Logs:**
Check console output for:
- `[INF] User {UserId} logged in successfully` ?
- `[WRN] Failed login attempt for: {EmailOrMobile}` ?

---

## ? **Checklist**

Before testing login:

- [ ] Database migration applied (nullable trial dates)
- [ ] Application builds successfully (`dotnet build`)
- [ ] User exists in database
- [ ] User IsActive = 1
- [ ] Password hash matches
- [ ] Wallet exists for user
- [ ] Application is running

---

## ?? **Ready to Test!**

1. ? Run: `dotnet run`
2. ? Open: `https://localhost:7262/Account/Login`
3. ? Enter: `rameshsivaperumal@gmail.com` / `Test@4321`
4. ? Click: **Log in**
5. ? Expect: Redirect to dashboard with welcome message!

---

**Good luck! Your login should work now.** ??

If you encounter any issues, check the application console logs for detailed error messages.

---

**Last Updated:** 2024-02-14  
**Status:** Ready to Test  
**Expected:** ? SUCCESS
