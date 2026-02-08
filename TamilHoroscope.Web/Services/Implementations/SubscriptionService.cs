using Microsoft.EntityFrameworkCore;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Services.Implementations;

/// <summary>
/// Implementation of subscription service for trial and paid subscription management
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;
    private readonly IWalletService _walletService;
    private readonly IConfigService _configService;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        ApplicationDbContext context,
        IWalletService walletService,
        IConfigService configService,
        ILogger<SubscriptionService> logger)
    {
        _context = context;
        _walletService = walletService;
        _configService = configService;
        _logger = logger;
    }

    public async Task<bool> IsUserInTrialAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        // Check if trial is active and hasn't expired
        return user.IsTrialActive && user.TrialEndDate > DateTime.UtcNow;
    }

    public async Task<int> GetTrialDaysRemainingAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsTrialActive)
            return 0;

        var daysRemaining = (user.TrialEndDate - DateTime.UtcNow).Days;
        return Math.Max(0, daysRemaining);
    }

    public async Task<bool> HasActiveSubscriptionAsync(int userId)
    {
        // User has active subscription if they're in trial OR have wallet balance
        var isInTrial = await IsUserInTrialAsync(userId);
        if (isInTrial)
            return true;

        var balance = await _walletService.GetBalanceAsync(userId);
        var perDayCost = await _configService.GetPerDayCostAsync();

        return balance >= perDayCost;
    }

    public async Task<int> GetWalletDaysRemainingAsync(int userId)
    {
        var balance = await _walletService.GetBalanceAsync(userId);
        var perDayCost = await _configService.GetPerDayCostAsync();

        if (perDayCost <= 0)
        {
            _logger.LogWarning("Invalid per-day cost configuration: {PerDayCost}", perDayCost);
            return 0;
        }

        return (int)Math.Floor(balance / perDayCost);
    }

    public async Task<bool> ShouldShowLowBalanceWarningAsync(int userId)
    {
        // Don't show warning if user is in trial
        if (await IsUserInTrialAsync(userId))
            return false;

        var daysRemaining = await GetWalletDaysRemainingAsync(userId);
        var warningThreshold = await _configService.GetLowBalanceWarningDaysAsync();

        return daysRemaining <= warningThreshold && daysRemaining > 0;
    }

    public async Task DeactivateTrialAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found for trial deactivation", userId);
            return;
        }

        user.IsTrialActive = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Trial deactivated for user {UserId}", userId);
    }
}
