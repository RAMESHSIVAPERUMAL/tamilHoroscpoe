using Microsoft.EntityFrameworkCore;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Services.Implementations;

/// <summary>
/// Implementation of wallet service for managing user wallet and transactions
/// </summary>
public class WalletService : IWalletService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WalletService> _logger;

    public WalletService(ApplicationDbContext context, ILogger<WalletService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Wallet> GetOrCreateWalletAsync(int userId)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
        {
            wallet = new Wallet
            {
                UserId = userId,
                Balance = 0,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new wallet for user {UserId}", userId);
        }

        return wallet;
    }

    public async Task<decimal> GetBalanceAsync(int userId)
    {
        var wallet = await GetOrCreateWalletAsync(userId);
        return wallet.Balance;
    }

    public async Task<Transaction> AddFundsAsync(int userId, decimal amount, string description, string? referenceId = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        var wallet = await GetOrCreateWalletAsync(userId);
        var balanceBefore = wallet.Balance;

        wallet.Balance += amount;
        wallet.LastUpdatedDate = DateTime.UtcNow;

        var transaction = new Transaction
        {
            WalletId = wallet.WalletId,
            UserId = userId,
            TransactionType = "Credit",
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            Description = description,
            TransactionDate = DateTime.UtcNow,
            ReferenceId = referenceId
        };

        _context.Transactions.Add(transaction);

        // ✅ CRITICAL: Deactivate trial immediately when user adds funds
        // Trial should end as soon as user commits to paid service by topping up
        var user = await _context.Users.FindAsync(userId);
        if (user != null && user.IsTrialActive)
        {
            user.IsTrialActive = false;
            _logger.LogInformation("Trial period deactivated for user {UserId} after wallet top-up of ₹{Amount}",
                userId, amount);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Added ₹{Amount} to wallet for user {UserId}. New balance: ₹{Balance}",
            amount, userId, wallet.Balance);

        return transaction;
    }

    public async Task<Transaction> DeductFundsAsync(int userId, decimal amount, string description)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        var wallet = await GetOrCreateWalletAsync(userId);
        var balanceBefore = wallet.Balance;

        if (wallet.Balance < amount)
            throw new InvalidOperationException($"Insufficient balance. Current balance: ₹{wallet.Balance}, Required: ₹{amount}");

        wallet.Balance -= amount;
        wallet.LastUpdatedDate = DateTime.UtcNow;

        var transaction = new Transaction
        {
            WalletId = wallet.WalletId,
            UserId = userId,
            TransactionType = "Debit",
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            Description = description,
            TransactionDate = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deducted ₹{Amount} from wallet for user {UserId}. New balance: ₹{Balance}",
            amount, userId, wallet.Balance);

        return transaction;
    }

    public async Task<bool> HasSufficientBalanceAsync(int userId, decimal amount)
    {
        var balance = await GetBalanceAsync(userId);
        return balance >= amount;
    }

    public async Task<List<Transaction>> GetTransactionHistoryAsync(int userId, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTransactionCountAsync(int userId)
    {
        return await _context.Transactions
            .CountAsync(t => t.UserId == userId);
    }

    public async Task<Transaction> RefundAsync(int userId, decimal amount, string description, string? referenceId = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        var wallet = await GetOrCreateWalletAsync(userId);
        var balanceBefore = wallet.Balance;

        wallet.Balance += amount;
        wallet.LastUpdatedDate = DateTime.UtcNow;

        var transaction = new Transaction
        {
            WalletId = wallet.WalletId,
            UserId = userId,
            TransactionType = "Refund",
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            Description = description,
            TransactionDate = DateTime.UtcNow,
            ReferenceId = referenceId
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Refunded ₹{Amount} to wallet for user {UserId}. New balance: ₹{Balance}",
            amount, userId, wallet.Balance);

        return transaction;
    }
}
