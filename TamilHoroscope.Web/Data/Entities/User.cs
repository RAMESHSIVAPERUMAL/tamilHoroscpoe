using Microsoft.AspNetCore.Identity;

namespace TamilHoroscope.Web.Data.Entities;

/// <summary>
/// Represents a user in the Tamil Horoscope system with trial period tracking
/// </summary>
public class User : IdentityUser<int>
{
    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Mobile number (Indian format +91)
    /// </summary>
    public string? MobileNumber { get; set; }

    /// <summary>
    /// Date when the account was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the email has been verified
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    /// Whether the mobile number has been verified
    /// </summary>
    public bool IsMobileVerified { get; set; }

    /// <summary>
    /// Whether the account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last login date and time
    /// </summary>
    public DateTime? LastLoginDate { get; set; }

    /// <summary>
    /// Date when the trial period started
    /// </summary>
    public DateTime TrialStartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the trial period ends
    /// </summary>
    public DateTime TrialEndDate { get; set; }

    /// <summary>
    /// Whether the trial period is currently active
    /// </summary>
    public bool IsTrialActive { get; set; } = true;

    // Navigation properties
    public virtual Wallet? Wallet { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<HoroscopeGeneration> HoroscopeGenerations { get; set; } = new List<HoroscopeGeneration>();
}
