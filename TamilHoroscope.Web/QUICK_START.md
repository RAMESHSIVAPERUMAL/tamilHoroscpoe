# Quick Start Guide - Database-First Authentication

## Step 1: Restart Visual Studio

The changes made are significant (changing base class). You must:

1. **Stop the Debugger** - Press Shift+F5 in Visual Studio
2. **Close the Solution** - File > Close Solution
3. **Reopen the Solution** - File > Open Recent > tamilHoroscope
4. **Rebuild** - Ctrl+Shift+B

This ensures the new code changes are fully compiled.

---

## Step 2: Clean and Rebuild

```
Build > Clean Solution
Build > Rebuild Solution
```

If there are any remaining build errors, they'll appear in the Error List. Most errors about "ENC0014" will be gone after restart.

---

## Step 3: Run Database Scripts

Your database should already have the tables created. Just run the seed data if not done:

```powershell
# In Package Manager Console
# The database tables should already exist from your scripts
# If not, run: Database\Scripts\01_CreateTables.sql
# Then: Database\Scripts\02_CreateIndexes.sql
# Then: Database\Scripts\03_SeedData.sql
```

---

## Step 4: Test Registration

### In Visual Studio:
1. Press **F5** to start debugging
2. Navigate to `https://localhost:7XXX/Account/Register`
3. Fill in the form:
   - Full Name: Test User
   - Email: test@example.com (or leave blank and use mobile)
   - Mobile: 9876543210 (optional)
   - Password: TestPass123!
   - Confirm Password: TestPass123!
4. Click Register

### Expected Result:
- ? Redirects to Login page
- ? User record created in database
- ? Wallet record created for user
- ? Password stored as SHA256 hash

---

## Step 5: Verify in Database

Open SQL Server Management Studio and run:

```sql
-- Check if user was created
SELECT * FROM Users WHERE FullName = 'Test User';

-- Check if wallet was created
SELECT * FROM Wallets WHERE UserId = [USERID_FROM_ABOVE];

-- Check transaction history (should be empty or have initialization)
SELECT * FROM Transactions WHERE UserId = [USERID_FROM_ABOVE];
```

---

## Troubleshooting

### "IAuthenticationService not found"
- Solution: Rebuild the project
- Make sure `Program.cs` has: `builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();`

### "ENC0014: Updating the base class"
- Solution: Restart Visual Studio
- This is a hot reload limitation when changing base classes
- Stop debugger ? Rebuild ? Start again

### "Column does not exist"
- Solution: Check User entity matches your database
- Run: `SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users'`
- Compare with User.cs properties

### Transaction errors
- Check connection string in `appsettings.json`
- Ensure `TrustServerCertificate=true` is set

---

## Code Structure

### Authentication Service
```
TamilHoroscope.Web/Services/Implementations/
??? AuthenticationService.cs (implementation)
??? IAuthenticationService.cs (interface)
```

### Database Context
```
TamilHoroscope.Web/Data/
??? ApplicationDbContext.cs (DbContext - no Identity)
??? Entities/
?   ??? User.cs (Plain POCO, no Identity inheritance)
??? Configurations/
    ??? UserConfiguration.cs (Database-first mappings)
```

### Pages
```
TamilHoroscope.Web/Pages/Account/
??? Register.cshtml.cs (Uses IAuthenticationService)
```

---

## Key Features Implemented

### 1. User Registration
```csharp
var (success, message, user) = await _authService.RegisterUserAsync(
    "test@example.com",
    "9876543210",
    "Test User",
    "TestPass123!");
```

**Features:**
- ? Email/Mobile validation
- ? Duplicate checking
- ? Transaction handling
- ? Automatic wallet creation
- ? Error logging

### 2. User Authentication
```csharp
var (success, user) = await _authService.AuthenticateAsync(
    "test@example.com",
    "TestPass123!");
```

**Features:**
- ? Email or Mobile login
- ? Password verification
- ? Account status check
- ? Last login tracking

### 3. Password Security
```csharp
// SHA256 hashing (no external library needed)
var hash = HashPassword("TestPass123!");
var isValid = VerifyPassword("TestPass123!", hash);
```

---

## Transaction Example

When you register a user, the service does this:

```csharp
using (var transaction = await _context.Database.BeginTransactionAsync())
{
    try
    {
        // 1. Validate input
        // 2. Check for duplicates
        
        // 3. Create user
        var user = new User { ... };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // 4. Create wallet
        var wallet = new Wallet { UserId = user.UserId, ... };
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        
        // 5. Commit if everything succeeded
        await transaction.CommitAsync();
        
        return (true, "Success", user);
    }
    catch (Exception ex)
    {
        // Transaction auto-rolls back on exception
        _logger.LogError(ex, "Registration failed");
        return (false, "Error message", null);
    }
}
```

**What this guarantees:**
- User and Wallet are always created together
- No orphaned users without wallets
- All-or-nothing: no partial registrations on error

---

## Next: Create Login Service

After testing registration, create login:

```csharp
public class LoginModel : PageModel
{
    private readonly IAuthenticationService _authService;

    public async Task<IActionResult> OnPostAsync()
    {
        var (success, user) = await _authService.AuthenticateAsync(
            Input.EmailOrMobile,
            Input.Password);

        if (success && user != null)
        {
            // TODO: Create session
            // HttpContext.Session.SetString("UserId", user.UserId.ToString());
            return LocalRedirect("/Dashboard");
        }

        ModelState.AddModelError("", "Invalid credentials");
        return Page();
    }
}
```

---

## Summary

| What | Details |
|------|---------|
| **Authentication** | Custom, no ASP.NET Identity |
| **Password Hashing** | SHA256 (built-in .NET) |
| **Transactions** | Explicit, ACID-compliant |
| **External Packages** | None (only EF Core & built-in) |
| **Database Approach** | Database-First |
| **Error Handling** | Comprehensive with logging |
| **Status** | Ready to test |

---

## Commands Reference

```powershell
# Restart application
F5

# Stop debugger
Shift+F5

# Rebuild
Ctrl+Shift+B

# Run tests
Ctrl+R, A
```

---

**Ready?** Press **F5** and test the registration!
