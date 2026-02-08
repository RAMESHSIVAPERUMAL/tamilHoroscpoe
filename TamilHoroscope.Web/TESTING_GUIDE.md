# Testing Guide - Database-First Authentication

## Quick Test Sequence

### Prerequisites
- Visual Studio running
- Application started (F5)
- Connection string configured in `appsettings.json`
- Database tables created (run SQL scripts if not done)

---

## Test 1: User Registration

### Steps
1. Navigate to `https://localhost:7XXX/Account/Register`
2. Fill in the form:
   - **Full Name:** John Doe
   - **Email:** john@example.com
   - **Mobile:** 9876543210
   - **Password:** Test@1234
   - **Confirm Password:** Test@1234
3. Click **Register**

### Expected Result
? Redirects to Login page  
? No errors displayed

### Database Verification
```sql
-- In SQL Server Management Studio
SELECT * FROM Users WHERE Email = 'john@example.com';
-- Should return 1 row with all data

SELECT * FROM Wallets WHERE UserId = [USER_ID_FROM_ABOVE];
-- Should return 1 row with Balance = 0.00
```

---

## Test 2: User Login

### Steps
1. You should be on `/Account/Login` page (from Test 1)
2. Fill in the form:
   - **Email or Mobile Number:** john@example.com (or 9876543210)
   - **Password:** Test@1234
   - **Remember me:** (optional)
3. Click **Login**

### Expected Result
? Redirects to home page  
? No error messages  
? Session created in browser

### Database Verification
```sql
-- Check LastLoginDate was updated
SELECT UserId, Email, LastLoginDate 
FROM Users 
WHERE Email = 'john@example.com';

-- LastLoginDate should be recent (within seconds)
```

### Session Verification (in Browser Dev Tools)
1. Open **Developer Tools** (F12)
2. Go to **Storage** ? **Cookies** ? `https://localhost:7XXX`
3. Look for `.AspNetCore.Session` cookie
4. It should exist and have a value

---

## Test 3: Access Profile Page

### Steps
1. After logging in, navigate to `https://localhost:7XXX/Account/Profile`
2. Page should load

### Expected Result
? Profile page loads  
? Shows user information:
  - Full Name: John Doe
  - Email: john@example.com (with Verified status)
  - Mobile: 9876543210
  - Account Created date
  - Last Login date
? Shows wallet status (Balance: ?0.00)
? Shows trial period information (30 days)

### Error Cases
If you get redirected to Login:
- Session might have expired
- `UserId` not set in session
- Check `appsettings.json` Session timeout

---

## Test 4: Test Session Timeout

### Steps
1. Logged in on Profile page
2. Open browser Developer Tools (F12)
3. Go to **Storage** ? **Cookies**
4. Delete `.AspNetCore.Session` cookie
5. Refresh the Profile page

### Expected Result
? Redirects to Login page  
? Error message or login form displays

---

## Test 5: Test with Mobile Number Login

### Steps
1. Go to Login page
2. Fill in:
   - **Email or Mobile:** 9876543210
   - **Password:** Test@1234
3. Click Login

### Expected Result
? Successfully logs in with mobile number  
? Redirects to home page

---

## Test 6: Test Password Validation

### Steps
1. Go to Register page
2. Fill in:
   - **Full Name:** Test User
   - **Email:** test@example.com
   - **Mobile:** 9876543210
   - **Password:** short (less than 8 characters)
   - **Confirm Password:** short
3. Click Register

### Expected Result
? Error message: "The password must be at least 8 characters long."  
? Form not submitted

---

## Test 7: Test Duplicate Email

### Steps
1. Go to Register page
2. Fill in:
   - **Full Name:** Another User
   - **Email:** john@example.com (same as Test 1)
   - **Mobile:** 9999999999
   - **Password:** Test@1234
   - **Confirm Password:** Test@1234
3. Click Register

### Expected Result
? Error message: "Either email or mobile number must be provided." or "Email already registered"  
? Form not submitted

---

## Test 8: Test Email-Only Registration

### Steps
1. Go to Register page
2. Fill in:
   - **Full Name:** Email Only User
   - **Email:** emailonly@example.com
   - **Mobile:** (leave blank)
   - **Password:** Test@1234
   - **Confirm Password:** Test@1234
3. Click Register
4. Login with the email

### Expected Result
? Successfully registers with just email  
? Can login with email

---

## Test 9: Test Mobile-Only Registration

### Steps
1. Go to Register page
2. Fill in:
   - **Full Name:** Mobile Only User
   - **Email:** (leave blank)
   - **Mobile:** 9111111111
   - **Password:** Test@1234
   - **Confirm Password:** Test@1234
3. Click Register
4. Login with the mobile number

### Expected Result
? Successfully registers with just mobile  
? Can login with mobile number

---

## Test 10: Test Invalid Credentials

### Steps
1. Go to Login page
2. Fill in:
   - **Email or Mobile:** john@example.com
   - **Password:** WrongPassword123
3. Click Login

### Expected Result
? Error message: "Invalid email/mobile or password."  
? Stays on Login page
? Form not submitted

---

## Browser Console Debugging

### Check Session Values
Open Browser Console (F12) and run:
```javascript
// Check if AJAX can see session data
fetch('/session-test')
    .then(r => r.json())
    .then(d => console.log(d))
```

### Check Cookies
```javascript
// List all cookies
document.cookie.split(';').forEach(c => console.log(c));
```

---

## Database Debugging Queries

### Check All Users
```sql
SELECT UserId, Email, MobileNumber, FullName, CreatedDate, LastLoginDate, IsActive
FROM Users
ORDER BY CreatedDate DESC;
```

### Check User Wallets
```sql
SELECT w.WalletId, w.UserId, u.Email, u.FullName, w.Balance, w.CreatedDate
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.UserId
ORDER BY w.CreatedDate DESC;
```

### Check Transaction History
```sql
SELECT TOP 10 t.TransactionId, t.UserId, u.Email, t.TransactionType, t.Amount, t.TransactionDate
FROM Transactions t
INNER JOIN Users u ON t.UserId = u.UserId
ORDER BY t.TransactionDate DESC;
```

---

## Common Issues & Solutions

### Issue 1: "User not found" on Login
**Cause:** Email/Mobile doesn't exist in database
**Solution:** Register the user first in Test 1

### Issue 2: Redirects to Login from Profile
**Cause:** Session expired or not set
**Solution:** 
- Clear cookies and login again
- Check `appsettings.json` Session timeout setting
- Check that Login sets session properly

### Issue 3: Profile shows empty data
**Cause:** User ID not in session or user not in database
**Solution:**
- Verify session has UserId
- Verify user exists in database with that ID

### Issue 4: Registration fails silently
**Cause:** Validation errors not displayed
**Solution:**
- Check browser console for errors
- Check SQL Server for duplicate entries
- Verify all form fields are filled

### Issue 5: Can't login with mobile number
**Cause:** Mobile number not properly stored
**Solution:**
- Check database: SELECT * FROM Users WHERE MobileNumber = 'xxx'
- Verify mobile number is 10 digits starting with 6-9

---

## Logging

Enable detailed logging in `appsettings.json`:

```json
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft": "Information",
    "Microsoft.EntityFrameworkCore": "Debug"
  }
}
```

Watch the **Output** window in Visual Studio for detailed logs.

---

## Performance Testing

### Load Test
Register and login 10 different users:
- Should each take < 1 second
- No database timeouts
- No memory leaks

### Session Concurrency
Open 2 browser tabs, login with 2 different accounts:
- Each should have separate session
- Data should not mix between sessions

---

## Checklist

- [ ] Test 1: Registration works, user created
- [ ] Test 2: Login works, session created
- [ ] Test 3: Profile page loads with correct data
- [ ] Test 4: Session timeout redirects to login
- [ ] Test 5: Mobile number login works
- [ ] Test 6: Password validation works
- [ ] Test 7: Duplicate email detection works
- [ ] Test 8: Email-only registration works
- [ ] Test 9: Mobile-only registration works
- [ ] Test 10: Invalid credentials rejected
- [ ] Database: All users created correctly
- [ ] Database: All wallets created correctly
- [ ] Logging: No errors in output window

---

## Next Steps After Testing

Once all tests pass:

1. **Create Logout page** - `/Account/Logout.cshtml.cs`
2. **Create SessionHelper** - Centralize session logic
3. **Implement Password Change** - In Profile
4. **Add Email Verification** - Send verification emails
5. **Add Horoscope Generation** - Integrate payment
6. **Deploy to Staging** - Test in staging environment
7. **Deploy to Production** - Release to users

---

**Status:** Ready to Test  
**Expected Duration:** 30-60 minutes  
**Difficulty:** Easy  
**Pass Rate Target:** 10/10 tests  

Good luck! ??
