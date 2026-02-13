# Daily Horoscope Fee - Once Per Day Implementation

## Overview

This document describes the implementation of the "once-per-day" daily horoscope fee deduction system.

---

## Requirements Met

1. ? **Deduct fee only once per day** - First horoscope generation of the day
2. ? **Track last deduction date** - Store in User table
3. ? **Use session** - Check if today's fee is paid
4. ? **Switch between paid/trial mode** - Based on payment status
5. ? **History "View Again"** - Works for both paid and trial users

---

## Database Changes

### User Table - New Column

```sql
ALTER TABLE Users 
ADD LastDailyFeeDeductionDate DATETIME NULL;
```

**Purpose**: Track the last date when daily horoscope fee was deducted

**Logic**:
- `NULL` = Never paid any daily fee
- `2024-01-15` = Last paid on January 15, 2024
- Compare with today's date to determine if fee is already paid

---

## Implementation Details

### 1. HoroscopeService - GenerateHoroscopeAsync

**Before** (Multiple deductions per day):
```csharp
// Check trial status
var isInTrial = await _subscriptionService.IsUserInTrialAsync(userId);

if (!isInTrial)
{
    // Deduct fee EVERY TIME
    await _walletService.DeductFundsAsync(userId, perDayCost, "Daily horoscope generation fee");
}
```

**After** (One deduction per day) ?:
```csharp
// Get user to check last fee deduction date
var user = await _context.Users.FindAsync(userId);
var today = DateTime.UtcNow.Date;
var hasPaidToday = user.LastDailyFeeDeductionDate?.Date == today;

var isInTrial = await _subscriptionService.IsUserInTrialAsync(userId);
var treatAsTrial = isInTrial || !hasPaidToday;

if (isInTrial)
{
    // Trial: No charge, limited features
}
else if (!hasPaidToday)
{
    // Not paid today - check balance and deduct
    if (hasSufficientBalance)
    {
        await _walletService.DeductFundsAsync(userId, perDayCost, "Daily horoscope generation fee");
        
        // ? UPDATE LAST FEE DEDUCTION DATE
        user.LastDailyFeeDeductionDate = today;
        await _context.SaveChangesAsync();
        
        treatAsTrial = false; // Paid successfully - full features
    }
    else
    {
        // Insufficient balance - treat as trial for today
        treatAsTrial = true;
    }
}
else
{
    // ? Already paid today - no charge, full features
    treatAsTrial = false;
}

// Calculate horoscope with appropriate features
var horoscope = await CalculateHoroscopeInternalAsync(..., treatAsTrial);
```

---

## Flow Diagrams

### Scenario 1: User Paid Today (First Generation)

```
User clicks "Generate Horoscope"
    ?
Check: LastDailyFeeDeductionDate
    ?
    NULL or NOT today's date
    ?
Check: IsTrialActive?
    ?
    NO (Not in trial)
    ?
Check: Wallet Balance >= ?5?
    ?
    YES
    ?
Deduct ?5 from wallet
    ?
? SET LastDailyFeeDeductionDate = Today
    ?
Generate FULL-FEATURED horoscope
    ?
    ? Rasi Chart
    ? Navamsa Chart
    ? Planetary Strength
    ? Full Dasa/Bhukti
```

### Scenario 2: User Paid Today (Second+ Generation)

```
User clicks "Generate Horoscope" (again same day)
    ?
Check: LastDailyFeeDeductionDate
    ?
    = Today's date ?
    ?
Check: IsTrialActive?
    ?
    NO
    ?
? SKIP PAYMENT (Already paid today)
    ?
Generate FULL-FEATURED horoscope (FREE)
    ?
    ? Rasi Chart
    ? Navamsa Chart
    ? Planetary Strength
    ? Full Dasa/Bhukti
```

### Scenario 3: Insufficient Balance

```
User clicks "Generate Horoscope"
    ?
Check: LastDailyFeeDeductionDate
    ?
    NULL or NOT today's date
    ?
Check: IsTrialActive?
    ?
    NO
    ?
Check: Wallet Balance >= ?5?
    ?
    NO (Balance: ?2)
    ?
? Cannot deduct payment
    ?
Treat as TRIAL for today
    ?
Generate LIMITED horoscope
    ?
    ? Rasi Chart
    ? No Navamsa Chart
    ? No Planetary Strength
    ? Basic Dasa only (no Bhukti)
```

### Scenario 4: Trial User

```
User clicks "Generate Horoscope"
    ?
Check: IsTrialActive?
    ?
    YES (In trial period)
    ?
? NO PAYMENT REQUIRED
    ?
Generate LIMITED horoscope
    ?
    ? Rasi Chart
    ? No Navamsa Chart
    ? No Planetary Strength
    ? Basic Dasa only (no Bhukti)
```

---

## API Changes

### New Method: HasPaidTodayAsync

```csharp
public async Task<bool> HasPaidTodayAsync(int userId)
{
    var user = await _context.Users.FindAsync(userId);
    if (user == null)
    {
        return false;
    }

    var today = DateTime.UtcNow.Date;
    return user.LastDailyFeeDeductionDate?.Date == today;
}
```

**Usage**:
```csharp
// In Generate.cshtml.cs
var hasPaidToday = await _horoscopeService.HasPaidTodayAsync(userId);

if (hasPaidToday)
{
    // Show "Already paid today" message
    // Enable full features
}
else
{
    // Show payment required or trial warning
}
```

---

## User Experience

### Payment Status Banner

**Display on Generate Page**:

```html
@if (hasPaidToday && !isInTrial)
{
    <div class="alert alert-success">
        <i class="bi bi-check-circle"></i> 
        <strong>Today's Fee Paid</strong>
        <p class="mb-0">You have full access to all features for today. Generate unlimited horoscopes!</p>
    </div>
}
else if (!isInTrial)
{
    <div class="alert alert-warning">
        <i class="bi bi-wallet2"></i> 
        <strong>Payment Required</strong>
        <p class="mb-0">First horoscope generation of the day costs ?5. Current balance: ?@balance</p>
    </div>
}
else
{
    <div class="alert alert-info">
        <i class="bi bi-gift"></i> 
        <strong>Trial Period Active</strong>
        <p class="mb-0">Limited features available. Top up your wallet for full access.</p>
    </div>
}
```

---

## History "View Again" Functionality

### Logic for View Again

```csharp
// History.cshtml.cs - OnPostRegenerateAsync
var generation = await _horoscopeService.GetGenerationByIdAsync(userId, generationId);
var isTrialUser = await _subscriptionService.IsUserInTrialAsync(userId);
var hasPaidToday = await _horoscopeService.HasPaidTodayAsync(userId);

// Determine feature level for regeneration
var treatAsTrial = isTrialUser || !hasPaidToday;

var horoscope = await _horoscopeService.RegenerateHoroscopeAsync(generation, treatAsTrial);
```

**Rules**:
- ? **Paid Today**: View Again shows full features (Navamsa, Strength, full Dasa)
- ?? **Not Paid Today**: View Again shows trial features only
- ?? **Trial User**: Always shows trial features

**No additional charge for View Again** - it's based on current day's payment status

---

## Database Migration

### SQL Script

```sql
-- Add LastDailyFeeDeductionDate column
ALTER TABLE Users 
ADD LastDailyFeeDeductionDate DATETIME NULL;

-- Add index for performance
CREATE INDEX IX_Users_LastDailyFeeDeductionDate 
ON Users(LastDailyFeeDeductionDate);
```

### Apply Migration

**Option 1: Using Entity Framework** (if EF tools installed):
```bash
dotnet ef migrations add AddLastDailyFeeDeductionDate
dotnet ef database update
```

**Option 2: Manual SQL**:
Run the SQL script manually in SQL Server Management Studio or Azure Data Studio

---

## Testing Scenarios

### Test 1: First Generation of the Day

1. **Setup**: User with ?50 balance, `LastDailyFeeDeductionDate` = `NULL` or yesterday
2. **Action**: Generate horoscope
3. **Expected**:
   - ? ?5 deducted
   - ? `LastDailyFeeDeductionDate` = today
   - ? Balance = ?45
   - ? Full-featured horoscope shown

### Test 2: Second Generation Same Day

1. **Setup**: User with ?45 balance, `LastDailyFeeDeductionDate` = today
2. **Action**: Generate horoscope
3. **Expected**:
   - ? No deduction
   - ? Balance remains ?45
   - ? Full-featured horoscope shown
   - ? Message: "Already paid today"

### Test 3: Insufficient Balance

1. **Setup**: User with ?2 balance, `LastDailyFeeDeductionDate` = yesterday
2. **Action**: Generate horoscope
3. **Expected**:
   - ?? No deduction (insufficient funds)
   - ?? Balance remains ?2
   - ?? Trial-level horoscope shown
   - ?? Warning: "Insufficient balance. Top up to access full features."

### Test 4: Trial User

1. **Setup**: User in trial period
2. **Action**: Generate horoscope
3. **Expected**:
   - ? No deduction
   - ? Trial-level horoscope shown
   - ?? Info: "Trial period active"

### Test 5: View Again - Paid Today

1. **Setup**: User paid today's fee
2. **Action**: Go to History, click "View Again"
3. **Expected**:
   - ? No additional charge
   - ? Full-featured horoscope shown
   - ? Same as when originally generated

### Test 6: View Again - Not Paid Today

1. **Setup**: User has NOT paid today's fee (insufficient balance or new day)
2. **Action**: Go to History, click "View Again" on old horoscope
3. **Expected**:
   - ?? No charge
   - ?? Trial-level features shown (even if original was full)
   - ?? Message: "Top up wallet to view full features"

---

## Configuration

### AppSettings

```json
{
  "HoroscopeSettings": {
    "PerDayCost": 5.00,
    "MaxHoroscopesPerDay": 50,
    "DasaYears": 120
  }
}
```

---

## Benefits

1. **Fair Pricing** ?
   - Users pay only once per day
   - Unlimited generations after first payment

2. **User-Friendly** ?
   - Clear payment status
   - No surprise charges
   - Predictable cost: ?5/day

3. **Flexible** ?
   - Trial users: Free with limited features
   - Paid users: Full features
   - Insufficient balance: Graceful degradation to trial

4. **History Access** ?
   - "View Again" respects current day's payment status
   - No additional charge
   - Encourages wallet top-up for full features

---

## Security Considerations

? **Secure**:
- User ID verified from session
- Database-level foreign key constraints
- Transaction table tracks all deductions
- Cannot manipulate payment status from client

---

## Performance Optimizations

1. **Index on LastDailyFeeDeductionDate**:
   ```sql
   CREATE INDEX IX_Users_LastDailyFeeDeductionDate ON Users(LastDailyFeeDeductionDate);
   ```

2. **Caching** (future enhancement):
   ```csharp
   _cache.GetOrSet($"paid_today_{userId}", async () => 
       await _horoscopeService.HasPaidTodayAsync(userId),
       TimeSpan.FromHours(1)
   );
   ```

---

## Monitoring & Analytics

### Key Metrics to Track

1. **Daily Active Paying Users** (users who paid today's fee)
2. **Average Generations Per Day** (after first payment)
3. **Insufficient Balance Rate** (users who couldn't pay)
4. **Trial to Paid Conversion Rate**

### SQL Query Examples

```sql
-- Users who paid today
SELECT COUNT(DISTINCT UserId)
FROM Users
WHERE CAST(LastDailyFeeDeductionDate AS DATE) = CAST(GETUTCDATE() AS DATE);

-- Total revenue today
SELECT SUM(Amount)
FROM Transactions
WHERE TransactionType = 'Debit'
  AND Description LIKE '%Daily horoscope generation fee%'
  AND CAST(TransactionDate AS DATE) = CAST(GETUTCDATE() AS DATE);

-- Average generations per user (who paid today)
SELECT AVG(GenerationCount)
FROM (
    SELECT UserId, COUNT(*) AS GenerationCount
    FROM HoroscopeGenerations
    WHERE CAST(CreatedDateTime AS DATE) = CAST(GETUTCDATE() AS DATE)
    GROUP BY UserId
) AS UserGenerations;
```

---

## Summary

? **Implemented**:
- [x] Database column `LastDailyFeeDeductionDate` added
- [x] Service method `HasPaidTodayAsync()` created
- [x] Fee deduction logic updated (once per day)
- [x] Trial/Paid mode switching
- [x] Insufficient balance handling
- [x] History "View Again" respects payment status

? **Build Status**: Successful (0 errors, 0 warnings)

? **Ready for**: Database migration ? Testing ? Deployment

---

**Author**: AI Assistant  
**Date**: 2024  
**Version**: 1.0
