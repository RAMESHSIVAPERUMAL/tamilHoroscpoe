# Security Implementation Guide
## Tamil Horoscope Web Application

**Last Updated**: February 4, 2026  
**Version**: 1.0

---

## Overview

This document outlines the comprehensive security measures implemented to protect the horoscope generation system from tampering, unauthorized access, and abuse.

## Security Layers

### 1. **Request Verification Tokens**

#### Purpose
Prevent form tampering and ensure requests originate from legitimate sources.

#### Implementation
- **Client-Side**: JavaScript generates SHA-256 hash tokens before form submission
- **Server-Side**: `RequestVerificationHelper` validates tokens with time-based expiry
- **Token Types**:
  - **Request Token**: User ID + Timestamp hash
  - **Birth Details Checksum**: Birth data integrity verification

#### Code Location
- `TamilHoroscope.Web/Security/RequestVerificationHelper.cs`
- `Generate.cshtml` (JavaScript security section)
- `Generate.cshtml.cs` (Validation in `OnPostAsync`)

#### Token Validation
```csharp
RequestVerificationHelper.ValidateToken(userId, timestamp, token, validityMinutes: 5)
```

- **Validity Window**: 5 minutes
- **Algorithm**: SHA-256
- **Secret Key**: Stored in code (should move to appsettings in production)

---

### 2. **Rate Limiting**

#### Purpose
Prevent abuse by limiting requests per user/IP address.

#### Limits
- **Per Minute**: 10 requests
- **Per Hour**: 100 requests
- **Tracking**: User ID (authenticated) or IP Address (anonymous)

#### Implementation
- **Middleware**: `RateLimitingMiddleware`
- **Response**: HTTP 429 (Too Many Requests) with `Retry-After` header
- **Cleanup**: Automatic expiration of tracking data after 2 hours

#### Code Location
`TamilHoroscope.Web/Middleware/RateLimitingMiddleware.cs`

#### Configuration
```csharp
MaxRequestsPerMinute = 10;
MaxRequestsPerHour = 100;
CleanupIntervalMinutes = 10;
```

---

### 3. **Server-Side Validation**

#### Purpose
Validate all user inputs on the server regardless of client-side checks.

#### Validations
- **Range Checks**:
  - Latitude: -90 to 90
  - Longitude: -180 to 180
  - Timezone: -12 to +14
  - Birth Year: 1900 to current year + 1

- **Data Integrity**:
  - Birth details checksum verification
  - Token timestamp validation
  - Anti-forgery token verification

#### Code Location
`Generate.cshtml.cs` - `OnPostAsync()` method

---

### 4. **Client-Side Tamper Detection**

#### Purpose
Detect and respond to client-side manipulation attempts.

#### Features
- **Mutation Observers**: Monitor hidden token fields for changes
- **Tamper Counting**: Track suspicious modification attempts
- **Auto-Logout**: Redirect to logout after 5 tamper attempts
- **Developer Tools Detection**: Log when F12 or DevTools are opened (non-blocking)

#### Implementation
JavaScript security section in `Generate.cshtml`

```javascript
const observer = new MutationObserver(function(mutations) {
    // Detect token field tampering
    tamperAttempts++;
    if (tamperAttempts >= maxTamperAttempts) {
        window.location.href = '/Account/Logout';
    }
});
```

---

### 5. **Anti-Forgery Tokens**

#### Purpose
Protect against Cross-Site Request Forgery (CSRF) attacks.

#### Implementation
ASP.NET Core built-in anti-forgery tokens:
```razor
@Html.AntiForgeryToken()
```

- Automatically validated by ASP.NET Core
- Prevents form submissions from external sites

---

### 6. **Session-Based Authentication**

#### Purpose
Secure user authentication with session management.

#### Features
- **Session Timeout**: 30 minutes sliding expiration
- **HttpOnly Cookies**: Prevent JavaScript access
- **SameSite Policy**: Strict (prevent cross-site requests)
- **Secure Cookies**: HTTPS only

#### Configuration
```csharp
options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
options.Cookie.HttpOnly = true;
options.Cookie.SameSite = SameSiteMode.Strict;
```

---

## Security Best Practices

### ? Implemented

1. ? **Input Validation**: All inputs validated server-side
2. ? **Rate Limiting**: Prevents brute-force and abuse
3. ? **Token Verification**: Time-based tokens with 5-minute expiry
4. ? **Data Integrity**: Checksums for birth details
5. ? **Session Security**: HttpOnly, Secure, SameSite cookies
6. ? **CSRF Protection**: Anti-forgery tokens
7. ? **Logging**: Suspicious activity logged
8. ? **Authentication**: Required for all horoscope generation

### ?? Recommended for Production

1. ?? **Move Secret Key to Configuration**:
   ```csharp
   // appsettings.json
   "Security": {
     "SecretKey": "YOUR-PRODUCTION-SECRET-KEY"
   }
   ```

2. ?? **Add IP Whitelist/Blacklist** for known bad actors

3. ?? **Implement CAPTCHA** for suspicious activity

4. ?? **Add Database Logging** for security events

5. ?? **Enable Application Insights** or similar monitoring

6. ?? **Implement Content Security Policy (CSP)** headers

7. ?? **Add Request Signing** for API calls

---

## Attack Prevention Matrix

| Attack Type | Prevention Method | Status |
|-------------|------------------|--------|
| **Form Tampering** | Request tokens + checksums | ? Implemented |
| **CSRF** | Anti-forgery tokens | ? Implemented |
| **Rate Limiting Bypass** | Per-user + per-IP tracking | ? Implemented |
| **Session Hijacking** | Secure cookies + HttpOnly | ? Implemented |
| **SQL Injection** | Entity Framework parameterization | ? Implemented |
| **XSS** | Razor encoding | ? Built-in |
| **Replay Attacks** | Time-based token expiry | ? Implemented |
| **Man-in-the-Middle** | HTTPS enforcement | ? Implemented |
| **Brute Force** | Rate limiting | ? Implemented |

---

## Testing Security

### Manual Testing

1. **Token Expiry Test**:
   - Generate horoscope
   - Wait 6 minutes
   - Submit form
   - **Expected**: Token validation error

2. **Rate Limiting Test**:
   - Submit 11 requests in 1 minute
   - **Expected**: HTTP 429 after 10th request

3. **Checksum Validation Test**:
   - Open DevTools
   - Modify hidden `birthDetailsChecksum` field
   - Submit form
   - **Expected**: Data validation error

4. **Tamper Detection Test**:
   - Open DevTools
   - Try to modify `requestToken` field 5 times
   - **Expected**: Auto-logout

### Automated Testing

```csharp
[Fact]
public void RequestVerificationHelper_ValidateToken_ExpiredToken_ReturnsFalse()
{
    var timestamp = DateTime.UtcNow.AddMinutes(-10);
    var token = RequestVerificationHelper.GenerateToken(1, timestamp);
    var result = RequestVerificationHelper.ValidateToken(1, timestamp, token, 5);
    Assert.False(result);
}
```

---

## Monitoring & Logging

### Security Events Logged

- ? Failed token validations
- ? Rate limit violations
- ? Checksum mismatches
- ? Tamper detection alerts
- ? Successful authentications
- ? Horoscope generations

### Log Levels

- **Warning**: Rate limits, invalid tokens
- **Error**: Security violations, exceptions
- **Information**: Normal operations, successful requests

---

## Limitations & Considerations

### Current Limitations

1. **Secret Key in Code**: Should be moved to secure configuration
2. **No CAPTCHA**: Vulnerable to sophisticated bots
3. **Basic Rate Limiting**: Advanced users can bypass per-IP limits with VPN
4. **Client-Side Checks**: Can be bypassed (but server-side validation catches it)

### Privacy Considerations

- Birth data is **NOT encrypted** in database
- Session data stored in memory (consider Redis for production)
- No PII is logged in security events

---

## Compliance

### GDPR Considerations

- ? User can delete account (deletes all data)
- ? No tracking without consent
- ? Data minimization (only necessary fields)
- ?? Birth data retained for history feature

### Security Standards

- ? OWASP Top 10 coverage
- ? ASP.NET Core security best practices
- ? Secure cookies (HttpOnly, Secure, SameSite)
- ? HTTPS enforcement

---

## Troubleshooting

### Common Issues

**Issue**: "Session has not been configured for this application or request"  
**Cause**: Rate limiting middleware registered before session middleware  
**Solution**: Ensure correct middleware order in `Program.cs`:
```csharp
// CORRECT ORDER:
app.UseSession();                              // ? Session FIRST
app.UseMiddleware<RateLimitingMiddleware>();   // ? Rate limiting AFTER
app.UseAuthentication();
app.UseAuthorization();
```

**Issue**: "Invalid request token"  
**Cause**: Token expired or clock skew  
**Solution**: Increase `validityMinutes` to 10

**Issue**: Rate limit hit unexpectedly  
**Cause**: Multiple users behind same IP (NAT)  
**Solution**: Reduce `MaxRequestsPerMinute` or improve tracking

**Issue**: Form submission fails silently  
**Cause**: JavaScript token generation failed  
**Solution**: Check browser console for errors, ensure Crypto API support

---

## Production Deployment Checklist

Before deploying to production:

- [ ] Move secret key to appsettings/Azure Key Vault
- [ ] Review and adjust rate limit thresholds
- [ ] Enable Application Insights logging
- [ ] Configure HTTPS/TLS certificate
- [ ] Test all security features in staging
- [ ] Document incident response procedures
- [ ] Enable database backups
- [ ] Configure firewall rules
- [ ] Review CORS policies
- [ ] Enable request logging
- [ ] Set up monitoring alerts

---

## Contact & Support

For security concerns or vulnerability reports:
- **Email**: security@tamilhoroscope.com (fictitious)
- **Responsible Disclosure**: 90-day window

---

**Document Version**: 1.0  
**Author**: RAMESHSIVAPERUMAL  
**Classification**: Internal Use

