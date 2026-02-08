namespace TamilHoroscope.Web.Data.Entities;

/// <summary>
/// Represents a wallet transaction (credit, debit, or refund)
/// </summary>
public class Transaction
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// Foreign key to the Wallet
    /// </summary>
    public int WalletId { get; set; }

    /// <summary>
    /// Foreign key to the User
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Type of transaction: Credit, Debit, or Refund
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// Transaction amount in INR
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Wallet balance before the transaction
    /// </summary>
    public decimal BalanceBefore { get; set; }

    /// <summary>
    /// Wallet balance after the transaction
    /// </summary>
    public decimal BalanceAfter { get; set; }

    /// <summary>
    /// Description of the transaction
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date and time when the transaction occurred
    /// </summary>
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Reference ID for external payment systems
    /// </summary>
    public string? ReferenceId { get; set; }

    // Navigation properties
    public virtual Wallet Wallet { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
