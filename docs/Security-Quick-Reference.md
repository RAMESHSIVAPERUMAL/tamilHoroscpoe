# Security Quick Reference
## Developer Cheat Sheet

---

## Rate Limiting

### Current Limits
```csharp
Max Per Minute: 10 requests
Max Per Hour:   100 requests
Tracking:       UserId or IP Address
```

### To Adjust
Edit `RateLimitingMiddleware.cs`:
```csharp
private const int MaxRequestsPerMinute = 10;   // Change here
private const int MaxRequestsPerHour = 100;     // Change here
```

---

## Token Configuration

### Token Expiry
```csharp
// Default: 5 minutes
RequestVerificationHelper.ValidateToken(userId, timestamp, token, validityMinutes: 5)
```

### Secret Key Location
**Current**: Hardcoded in `RequestVerificationHelper.cs`  
**Production**: Move to `appsettings.json`

```json
{
  "Security": {
    "SecretKey": "YOUR-SECRET-KEY-HERE"
  }
}
```

---

## Testing Security Features

### 1. Test Token Expiry
```bash
1. Open Generate Horoscope page
2. Fill form but DON'T submit
3. Wait 6 minutes
4. Click "Generate Horoscope"
Expected: "Invalid request. Please refresh the page and try again."
```

### 2. Test Rate Limiting
```bash
# PowerShell script
for ($i=1; $i -le 12; $i++) {
    Invoke-WebRequest -Uri "https://localhost:7262/Horoscope/Generate" -Method POST
    Write-Host "Request $i sent"
}
# Expected: First 10 succeed, 11-12 get HTTP 429
```

### 3. Test Checksum Validation
```javascript
// In browser console
document.getElementById('birthDetailsChecksum').value = 'tampered';
document.getElementById('horoscopeForm').submit();
// Expected: "Data validation failed"
```

---

## Logging Security Events

### Log Levels
```csharp
// Use appropriate log levels
_logger.LogWarning("Rate limit exceeded for {Identifier}", identifier);
_logger.LogError("Security violation: {Error}", errorMessage);
_logger.LogInformation("Successful generation for user {UserId}", userId);
```

### What to Log
- ? Failed validations (Warning)
- ? Rate limit hits (Warning)
- ? Tamper attempts (Error)
- ? Successful operations (Information)
- ? DON'T log sensitive data (passwords, tokens)

---

## Common Issues & Fixes

### Issue: "Session has not been configured"
**Causes**:
- Rate limiting middleware registered before session middleware
- Incorrect middleware order in Program.cs

**Fix**:
```csharp
// CORRECT ORDER in Program.cs:
app.UseSession();                              // ? Session FIRST
app.UseMiddleware<RateLimitingMiddleware>();   // ? Then rate limiting
app.UseAuthentication();
app.UseAuthorization();

// WRONG ORDER - Will cause error:
app.UseMiddleware<RateLimitingMiddleware>();   // ? Rate limiting first
app.UseSession();                              // ? Session second
```

### Issue: "Invalid request token"
**Causes**:
- Token expired (> 5 minutes)
- Server time out of sync
- Secret key mismatch

**Fix**:
```csharp
// Increase validity window
RequestVerificationHelper.ValidateToken(userId, timestamp, token, validityMinutes: 10)
```

### Issue: Rate limit hit unexpectedly
**Causes**:
- Multiple users behind same IP (NAT)
- Development testing
- Aggressive refresh

**Fix**:
```csharp
// Temporarily increase limits for testing
private const int MaxRequestsPerMinute = 50;
```

### Issue: Token generation fails in browser
**Causes**:
- Old browser (no Crypto API)
- HTTP instead of HTTPS
- CSP blocking inline scripts

**Fix**:
- Ensure HTTPS is used
- Update browser
- Check console for errors

---

## Security Checklist

### Development
- [ ] Test all validation paths
- [ ] Verify rate limits work
- [ ] Check token expiry
- [ ] Test tamper detection
- [ ] Review logs for errors

### Before Production
- [ ] Move secret key to appsettings
- [ ] Adjust rate limits for traffic
- [ ] Enable Application Insights
- [ ] Configure monitoring alerts
- [ ] Test under load
- [ ] Review all TODO comments
- [ ] Enable HTTPS redirect
- [ ] Set up log retention

### After Deployment
- [ ] Monitor rate limit hits
- [ ] Check for validation failures
- [ ] Review security logs daily
- [ ] Set up alerts for anomalies
- [ ] Test from production

---

## Quick Commands

### View Logs (Development)
```bash
# Watch logs in real-time
dotnet run | grep "Security"
```

### Clear Rate Limit Cache
```csharp
// In RateLimitingMiddleware
_requestTrackers.Clear();  // Add this method if needed
```

### Generate Test Token
```csharp
var token = RequestVerificationHelper.GenerateToken(userId: 1, DateTime.UtcNow);
Console.WriteLine($"Token: {token}");
```

---

## Code Snippets

### Add Custom Validation
```csharp
// In Generate.cshtml.cs OnPostAsync()
if (customCondition)
{
    ErrorMessage = "Custom validation failed";
    _logger.LogWarning("Custom validation failed for user {UserId}", userId);
    return Page();
}
```

### Log Security Event
```csharp
_logger.LogWarning(
    "Security event: {EventType} for user {UserId} from IP {IpAddress}",
    eventType,
    userId,
    HttpContext.Connection.RemoteIpAddress
);
```

### Adjust Rate Limit for Specific User
```csharp
// In RateLimitingMiddleware
if (identifier == "User_123")  // Admin user
{
    await _next(context);
    return;
}
```

---

## Testing URLs

### Local Development
```
Login:    https://localhost:7262/Account/Login
Generate: https://localhost:7262/Horoscope/Generate
History:  https://localhost:7262/Horoscope/History
```

### Test Credentials
```
Email:    rameshsivaperumal@gmail.com
Password: Test@4321
```

---

## Emergency Procedures

### If Under Attack

1. **Immediate Actions**:
   ```csharp
   // Temporarily reduce rate limits
   MaxRequestsPerMinute = 5;
   MaxRequestsPerHour = 20;
   ```

2. **Block IP Range**:
   ```csharp
   // In RateLimitingMiddleware
   var blockedIPs = new[] { "123.45.67.0/24" };
   if (blockedIPs.Contains(ipAddress))
   {
       context.Response.StatusCode = 403;
       return;
   }
   ```

3. **Enable CAPTCHA**:
   - Add reCAPTCHA to form
   - Validate on server-side

4. **Review Logs**:
   ```bash
   # Find attack patterns
   grep "Rate limit exceeded" logs.txt | sort | uniq -c | sort -nr
   ```

---

## Performance Tuning

### If Rate Limiting is Slow
```csharp
// Increase cleanup interval
private const int CleanupIntervalMinutes = 30;  // From 10
```

### If Token Validation is Slow
```csharp
// Use caching for frequently validated users
private static MemoryCache _tokenCache = new MemoryCache(new MemoryCacheOptions());
```

---

## Useful Links

- **Documentation**: `docs/Security-Implementation.md`
- **Summary**: `docs/Security-Summary.md`
- **Source Code**:
  - `Security/RequestVerificationHelper.cs`
  - `Middleware/RateLimitingMiddleware.cs`
  - `Pages/Horoscope/Generate.cshtml.cs`

---

**Last Updated**: February 4, 2026  
**Print This Page**: Keep handy during development!

