namespace TamilHoroscope.Web.Data.Entities;

/// <summary>
/// Represents a user's wallet with prepaid balance
/// </summary>
public class Wallet
{
    /// <summary>
    /// Unique identifier for the wallet
    /// </summary>
    public int WalletId { get; set; }

    /// <summary>
    /// Foreign key to the User
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Current wallet balance in INR
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Date when the wallet was last updated
    /// </summary>
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the wallet was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
