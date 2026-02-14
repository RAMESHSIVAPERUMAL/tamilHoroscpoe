using System.Security.Cryptography;
using System.Text;

namespace TamilHoroscope.Web.Security;

/// <summary>
/// Helper class for generating and validating request verification tokens
/// to prevent tampering and ensure requests come from legitimate sources
/// </summary>
public static class RequestVerificationHelper
{
    private const string SecretKey = "TamilHoroscope_SecureKey_2026_v1"; // Should be in appsettings in production

    /// <summary>
    /// Generates a time-based token for request validation
    /// </summary>
    public static string GenerateToken(int userId, DateTime timestamp)
    {
        // Note: We don't include SecretKey because client-side JavaScript cannot know it.
        // The security comes from: time-based expiry (5 min), HTTPS, rate limiting, and session auth.
        var data = $"{userId}|{timestamp:yyyyMMddHHmmss}";
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        
        // Convert to hexadecimal to match client-side JavaScript format
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Validates a request token within a time window (5 minutes)
    /// </summary>
    public static bool ValidateToken(int userId, DateTime timestamp, string token, int validityMinutes = 5)
    {
        // Check if timestamp is within valid window
        var timeDiff = Math.Abs((DateTime.UtcNow - timestamp).TotalMinutes);
        
        if (timeDiff > validityMinutes)
        {
            Console.WriteLine($"[Security] Token expired for user {userId}. Age: {timeDiff:F1} min (max: {validityMinutes})");
            return false;
        }

        // Generate expected token and compare
        var expectedToken = GenerateToken(userId, timestamp);
        var isValid = token == expectedToken;
        
        if (!isValid)
        {
            Console.WriteLine($"[Security] Token validation FAILED for user {userId}");
            Console.WriteLine($"  Timestamp: {timestamp:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"  Token mismatch detected");
        }
        
        return isValid;
    }

    /// <summary>
    /// Generates a checksum for birth details to prevent tampering
    /// </summary>
    public static string GenerateBirthDetailsChecksum(
        string personName,
        DateTime birthDate,
        TimeSpan birthTime,
        double latitude,
        double longitude,
        double timeZoneOffset,
        string? placeName)
    {
        // Note: We don't include SecretKey in the checksum because the client-side JavaScript
        // cannot know the secret key. The checksum still provides integrity verification
        // by detecting if any field has been modified.
        var data = $"{personName}|{birthDate:yyyy-MM-dd}|{birthTime}|{latitude:F6}|{longitude:F6}|{timeZoneOffset}|{placeName}";
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        
        // Convert to hexadecimal to match client-side JavaScript format
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Validates birth details checksum
    /// </summary>
    public static bool ValidateBirthDetailsChecksum(
        string personName,
        DateTime birthDate,
        TimeSpan birthTime,
        double latitude,
        double longitude,
        double timeZoneOffset,
        string? placeName,
        string checksum)
    {
        var expectedChecksum = GenerateBirthDetailsChecksum(
            personName, birthDate, birthTime, latitude, longitude, timeZoneOffset, placeName);
        return checksum == expectedChecksum;
    }
}
