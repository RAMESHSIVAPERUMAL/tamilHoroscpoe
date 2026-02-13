# Trial Management with Wallet Top-Up - Detailed Test Scenarios

## ?? Understanding Non-Nullable Date Fields

### Database Schema (Important!)

```csharp
public class User
{
    // These are NOT NULLABLE - Always have values
    public DateTime TrialStartDate { get; set; } = DateTime.UtcNow;
    public DateTime TrialEndDate { get; set; }
    
    // This IS NULLABLE - Can be NULL
    public DateTime? LastDailyFeeDeductionDate { get; set; }
    
    // Boolean flag
    public bool IsTrialActive { get; set; } = true;
}
```

**Key Point**: Since `TrialStartDate` and `TrialEndDate` are **NOT NULL**, we rely on:
1. `IsTrialActive` (bool flag)
2. `TrialEndDate > DateTime.UtcNow` (not expired)

**Both must be true** for active trial:
```csharp
var isInActiveTrial = user.IsTrialActive && user.TrialEndDate > DateTime.UtcNow;
```

---

## ?? Complete Test Scenarios with Wallet Top-Up

### Scenario 1: User in Trial Tops Up Wallet

#### Initial State
```
UserId: 101
IsTrialActive: true
TrialStartDate: 2024-02-01 (10 days ago)
TrialEndDate: 2024-03-01 (20 days from now)
Balance: ?0
LastDailyFeeDeductionDate: NULL
```

#### Action 1: User Tops Up ?100
```
POST /Wallet/TopUp
Amount: ?100
```

**After Top-Up**:
```
Balance: ?100
IsTrialActive: true (UNCHANGED)
TrialEndDate: 2024-03-01 (UNCHANGED)
LastDailyFeeDeductionDate: NULL (UNCHANGED)
```

**Why No Change?**
- User just added money
- Hasn't generated a horoscope yet
- Trial remains active until user actually uses paid service

#### Action 2: User Generates Horoscope
```
POST /Horoscope/Generate
BirthDate: 1990-01-01
...
```

**Horoscope Generation Logic**:
```csharp
// In HoroscopeService.GenerateHoroscopeAsync()

// 1. Get user
var user = _context.Users.Find(userId);

// 2. Check if paid today
var today = DateTime.UtcNow.Date;
var hasPaidToday = user.LastDailyFeeDeductionDate?.Date == today;

// 3. Check trial status
var isInTrial = await _subscriptionService.IsUserInTrialAsync(userId);

// 4. Determine mode
if (isInTrial && !hasPaidToday)
{
    // In trial, hasn't paid today
    // Check balance
    if (balance >= perDayCost)
    {
        // HAS BALANCE - Deduct and switch to paid
        await _walletService.DeductFundsAsync(userId, 5m, "Daily fee");
        user.LastDailyFeeDeductionDate = today;
        await _context.SaveChangesAsync();
        
        treatAsTrial = false; // Generate FULL horoscope
    }
    else
    {
        // No balance - use trial
        treatAsTrial = true; // Generate TRIAL horoscope
    }
}
```

**After Generation**:
```
Balance: ?95 (?100 - ?5)
IsTrialActive: true (still true, will be deactivated on next login/dashboard load)
TrialEndDate: 2024-03-01
LastDailyFeeDeductionDate: 2024-02-11 (TODAY)
```

#### Action 3: User Refreshes Dashboard
```
GET /Index (or user logs in again)
```

**CheckAndUpdateTrialStatusAsync() runs**:
```csharp
// 1. Get data
balance = ?95
lastFeeDeductionDate = 2024-02-11 (TODAY)
today = 2024-02-11

// 2. Check conditions
hasInsufficientBalance = false (?95 >= ?5)
lastFeeDeductionNotToday = false (2024-02-11 == 2024-02-11)
currentlyInActiveTrial = true (IsTrialActive=true, TrialEndDate > Now)

// 3. Apply logic (SCENARIO 2)
if (!hasInsufficientBalance) // true
{
    if (currentlyInActiveTrial && lastDailyFeeDeductionDate == today) // true
    {
        // DEACTIVATE TRIAL
        user.IsTrialActive = false;
        await _context.SaveChangesAsync();
    }
}
```

**Final State**:
```
Balance: ?95
IsTrialActive: false ? (DEACTIVATED)
TrialEndDate: 2024-03-01 (kept for history)
LastDailyFeeDeductionDate: 2024-02-11
```

**Dashboard Shows**: "Premium Active" (Paid User)

---

### Scenario 2: Trial User Tops Up ?5 Exactly

#### Initial State
```
UserId: 102
IsTrialActive: true
TrialStartDate: 2024-02-01
TrialEndDate: 2024-03-01 (18 days from now)
Balance: ?0
LastDailyFeeDeductionDate: NULL
```

#### Action 1: User Tops Up ?5
```
Balance: ?5
IsTrialActive: true (unchanged)
```

#### Action 2: User Logs In (Dashboard Load)
```csharp
// CheckAndUpdateTrialStatusAsync()
balance = ?5
hasInsufficientBalance = false (?5 >= ?5)
lastFeeDeductionNotToday = true (NULL != today)
currentlyInActiveTrial = true

// SCENARIO 2 - User has balance
// But hasn't paid today, so keep trial active
```

**State After Login**:
```
IsTrialActive: true (UNCHANGED - hasn't used service yet)
```

#### Action 3: User Generates Horoscope
```csharp
// In GenerateHoroscopeAsync()
// User is in trial, has balance, hasn't paid today
// System deducts ?5

Balance: ?0 (?5 - ?5)
LastDailyFeeDeductionDate: TODAY
```

#### Action 4: Dashboard Refresh
```csharp
// CheckAndUpdateTrialStatusAsync()
balance = ?0
hasInsufficientBalance = true
lastFeeDeductionNotToday = false (today == today)

// SCENARIO 3 - Insufficient but paid today
// Don't activate new trial (already got service)
// But IsTrialActive = true, so deactivate it

if (currentlyInActiveTrial && lastDailyFeeDeductionDate == today)
{
    user.IsTrialActive = false; // Deactivate
}
```

**Final State**:
```
Balance: ?0
IsTrialActive: false ?
LastDailyFeeDeductionDate: TODAY
```

**Tomorrow**: If balance still ?0, new trial activates!

---

### Scenario 3: Paid User Balance Drops to Zero

#### Initial State (Paid User)
```
UserId: 103
IsTrialActive: false
TrialStartDate: 2024-01-15 (old trial)
TrialEndDate: 2024-02-15 (expired)
Balance: ?5
LastDailyFeeDeductionDate: 2024-02-10 (yesterday)
```

#### Action 1: User Generates Horoscope Today
```csharp
// In GenerateHoroscopeAsync()
// Not in trial (IsTrialActive = false)
// Has balance, hasn't paid today
// Deduct ?5

Balance: ?0
LastDailyFeeDeductionDate: 2024-02-11 (TODAY)
```

#### Action 2: User Logs In Again Today
```csharp
// CheckAndUpdateTrialStatusAsync()
balance = ?0
hasInsufficientBalance = true
lastFeeDeductionNotToday = false (today == today)
currentlyInActiveTrial = false

// SCENARIO 3 - Insufficient but paid today
// Don't activate trial (already used service today)
```

**State**: No change (still not in trial)

#### Action 3: User Logs In TOMORROW
```csharp
// CheckAndUpdateTrialStatusAsync()
balance = ?0
hasInsufficientBalance = true
lastFeeDeductionNotToday = true (yesterday != today)
currentlyInActiveTrial = false

// SCENARIO 1 - Insufficient balance AND not paid today
// ACTIVATE NEW TRIAL!

user.IsTrialActive = true;
user.TrialStartDate = DateTime.UtcNow;
user.TrialEndDate = DateTime.UtcNow.AddDays(30);
```

**Final State**:
```
Balance: ?0
IsTrialActive: true ? (NEW TRIAL ACTIVATED)
TrialStartDate: 2024-02-12 (TODAY)
TrialEndDate: 2024-03-13 (30 days from today)
```

---

### Scenario 4: User Tops Up During Trial, But Doesn't Generate

#### Initial State
```
IsTrialActive: true
TrialEndDate: 2024-03-01 (15 days from now)
Balance: ?0
LastDailyFeeDeductionDate: 2024-02-09 (2 days ago)
```

#### Action 1: User Tops Up ?100
```
Balance: ?100
```

#### Action 2: User Logs In (Multiple Times, Same Day)
```csharp
// CheckAndUpdateTrialStatusAsync()
balance = ?100
hasInsufficientBalance = false
lastFeeDeductionNotToday = true (2 days ago != today)
currentlyInActiveTrial = true

// SCENARIO 2 - Has balance but hasn't paid today
// Keep trial active (user hasn't actually used paid service)
```

**State**: Trial remains active

#### Action 3: User Generates Horoscope Tomorrow
```
Tomorrow: 2024-02-12
LastDailyFeeDeductionDate: 2024-02-09 (3 days ago)

// In GenerateHoroscopeAsync()
// In trial, has balance, hasn't paid today
// Deduct ?5

Balance: ?95
LastDailyFeeDeductionDate: 2024-02-12 (TODAY)
```

#### Action 4: Dashboard Load
```csharp
// CheckAndUpdateTrialStatusAsync()
balance = ?95
currentlyInActiveTrial = true
lastDailyFeeDeductionDate = today

// DEACTIVATE TRIAL
user.IsTrialActive = false;
```

**Final State**: Paid user ?

---

### Scenario 5: Trial Expires, User Has Balance

#### Initial State
```
IsTrialActive: true
TrialStartDate: 2024-01-15
TrialEndDate: 2024-02-11 (TODAY - about to expire)
Balance: ?50
LastDailyFeeDeductionDate: 2024-02-09 (2 days ago)
```

#### Action 1: User Logs In (After Trial Expires)
```
Current Time: 2024-02-11 18:00 (evening)
TrialEndDate: 2024-02-11 00:00 (expired)

// CheckAndUpdateTrialStatusAsync()
currentlyInActiveTrial = false (TrialEndDate < DateTime.UtcNow)
balance = ?50
hasInsufficientBalance = false

// SCENARIO 2 - Has balance, trial expired
// Trial already expired, no action needed
```

**State**: Trial inactive (expired naturally)

#### Action 2: User Generates Horoscope
```csharp
// In GenerateHoroscopeAsync()
isInTrial = false (expired)
hasBalance = true

// Regular paid generation
// Deduct ?5

Balance: ?45
LastDailyFeeDeductionDate: 2024-02-11 (TODAY)
```

**Final State**: Paid user (transitioned from expired trial)

---

## ?? Summary Table

| Scenario | Initial Trial | Balance | Last Fee | Action | Final Trial | Reason |
|----------|--------------|---------|----------|--------|-------------|---------|
| 1 | Active | ?100 (topped up) | NULL | Generate ? Dashboard | Deactivated ? | Paid today |
| 2 | Active | ?5 (exact) | NULL | Generate ? Dashboard | Deactivated ? | Paid today, balance ?0 |
| 3 | Inactive | ?0 | Yesterday | Login tomorrow | Activated ? | Insufficient + not paid |
| 4 | Active | ?100 (topped up) | 2 days ago | Login only | Active ? | Hasn't used service |
| 5 | Expired | ?50 | 2 days ago | Generate | Inactive ? | Expired, now paid |

---

## ?? Key Takeaways

1. **Trial deactivates when**:
   - User has sufficient balance (? ?5) **AND**
   - User paid today's fee (`LastDailyFeeDeductionDate` = today)

2. **Trial activates when**:
   - Balance insufficient (< ?5) **AND**
   - Last fee deduction ? today (or NULL) **AND**
   - Not currently in active trial

3. **Topping up wallet alone doesn't deactivate trial**:
   - User must actually generate a horoscope
   - System waits for user to use paid service

4. **Trial dates are NOT NULL**:
   - Always have values
   - Use `IsTrialActive` flag + `TrialEndDate` check

5. **One free/paid day rule**:
   - If `LastDailyFeeDeductionDate` = today, no new trial that day
   - Prevents abuse

---

**Status**: ? Tested and Documented  
**Date**: 2024  
**Version**: 1.1
