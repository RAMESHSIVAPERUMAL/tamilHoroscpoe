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

        // ? FIXED: Handle nullable trial dates properly
        // If IsTrialActive is true BUT dates are null, user is not in valid trial
        // Trial is only active if: flag is true AND end date exists AND end date is future
        if (!user.IsTrialActive)
            return false;

        // If trial active flag is set but no end date, it's invalid state
        if (!user.TrialEndDate.HasValue)
        {
            _logger.LogWarning("User {UserId} has IsTrialActive=true but TrialEndDate is null. Invalid state.",
                userId);
            return false;
        }

        // Check if trial hasn't expired
        return user.TrialEndDate.Value > DateTime.UtcNow;
    }

    public async Task<int> GetTrialDaysRemainingAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return 0;

        // ? FIXED: Safely handle nullable trial dates
        if (!user.IsTrialActive)
            return 0;

        if (!user.TrialEndDate.HasValue)
        {
            _logger.LogWarning("User {UserId} has IsTrialActive=true but TrialEndDate is null.",
                userId);
            return 0;
        }

        var daysRemaining = (user.TrialEndDate.Value - DateTime.UtcNow).Days;
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

    public async Task CheckAndUpdateTrialStatusAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found for trial status check", userId);
            return;
        }

        var balance = await _walletService.GetBalanceAsync(userId);
        var today = DateTime.UtcNow.Date;
        var perDayCost = await _configService.GetPerDayCostAsync();

        // Check if wallet balance is insufficient for daily fee
        var hasInsufficientBalance = balance < perDayCost;

        // Check if last daily fee deduction is not today (or never deducted)
        var lastFeeDeductionNotToday = user.LastDailyFeeDeductionDate?.Date != today;

        // Check if user is currently in an active trial (flag set AND end date in future)
        var currentlyInActiveTrial = user.IsTrialActive 
            && user.TrialEndDate.HasValue 
            && user.TrialEndDate.Value > DateTime.UtcNow;

        // SCENARIO 1: User has insufficient balance AND hasn't paid today
        if (hasInsufficientBalance && lastFeeDeductionNotToday)
        {
            if (!currentlyInActiveTrial)
            {
                // Activate new trial period
                user.IsTrialActive = true;
                user.TrialStartDate = DateTime.UtcNow;
                user.TrialEndDate = DateTime.UtcNow.AddDays(30);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Trial period activated for user {UserId}. Balance: ?{Balance}, Start: {Start}, End: {End}",
                    userId, balance, user.TrialStartDate, user.TrialEndDate);
            }
            else
            {
                _logger.LogDebug("User {UserId} is already in active trial period until {TrialEndDate}", 
                    userId, user.TrialEndDate);
            }
        }
        // SCENARIO 2: User has sufficient balance
        else if (!hasInsufficientBalance)
        {
            // If user is in trial AND has paid today, deactivate trial
            if (currentlyInActiveTrial && user.LastDailyFeeDeductionDate?.Date == today)
            {
                user.IsTrialActive = false;
                // Optionally set TrialEndDate to null to indicate trial is no longer active
                // user.TrialEndDate = null;
                
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Trial deactivated for user {UserId} - user paid today's fee with sufficient balance. Balance: ?{Balance}",
                    userId, balance);
            }
            else if (currentlyInActiveTrial && lastFeeDeductionNotToday)
            {
                // User has balance but hasn't paid today - keep trial active until they actually pay
                _logger.LogDebug(
                    "User {UserId} has sufficient balance (?{Balance}) but hasn't paid today - trial remains active",
                    userId, balance);
            }
            else if (!currentlyInActiveTrial)
            {
                // User has sufficient balance and not in trial - this is the normal paid user state
                _logger.LogDebug(
                    "User {UserId} has sufficient balance (?{Balance}) and is on paid plan (not in trial)",
                    userId, balance);
            }
        }
        // SCENARIO 3: Insufficient balance but already paid today
        else
        {
            // User paid today but balance is now insufficient
            // Don't activate trial because they already got service today
            _logger.LogDebug(
                "User {UserId} has insufficient balance (?{Balance}) but already paid/used service today. Trial status: {TrialActive}",
                userId, balance, user.IsTrialActive);
        }
    }
}
