using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Services.Interfaces;

/// <summary>
/// Service for wallet and transaction management
/// </summary>
public interface IWalletService
{
    /// <summary>
    /// Gets the wallet for a user (creates one if it doesn't exist)
    /// </summary>
    Task<Wallet> GetOrCreateWalletAsync(int userId);

    /// <summary>
    /// Gets the current wallet balance
    /// </summary>
    Task<decimal> GetBalanceAsync(int userId);

    /// <summary>
    /// Adds funds to the wallet
    /// </summary>
    Task<Transaction> AddFundsAsync(int userId, decimal amount, string description, string? referenceId = null);

    /// <summary>
    /// Deducts funds from the wallet
    /// </summary>
    Task<Transaction> DeductFundsAsync(int userId, decimal amount, string description);

    /// <summary>
    /// Checks if the user has sufficient balance
    /// </summary>
    Task<bool> HasSufficientBalanceAsync(int userId, decimal amount);

    /// <summary>
    /// Gets transaction history for a user
    /// </summary>
    Task<List<Transaction>> GetTransactionHistoryAsync(int userId, int pageNumber = 1, int pageSize = 20);

    /// <summary>
    /// Gets the total number of transactions for a user
    /// </summary>
    Task<int> GetTransactionCountAsync(int userId);

    /// <summary>
    /// Processes a refund
    /// </summary>
    Task<Transaction> RefundAsync(int userId, decimal amount, string description, string? referenceId = null);
}
