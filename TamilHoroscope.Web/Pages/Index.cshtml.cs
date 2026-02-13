using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TamilHoroscope.Web.Services.Interfaces;
using AuthService = TamilHoroscope.Web.Services.Interfaces.IAuthenticationService;

namespace TamilHoroscope.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AuthService _authService;
    private readonly IWalletService _walletService;
    private readonly ISubscriptionService _subscriptionService;

    public IndexModel(
        ILogger<IndexModel> logger,
        AuthService authService,
        IWalletService walletService,
        ISubscriptionService subscriptionService)
    {
        _logger = logger;
        _authService = authService;
        _walletService = walletService;
        _subscriptionService = subscriptionService;
    }

    // Authenticated user properties
    public string? UserFullName { get; set; }
    public string? UserEmail { get; set; }
    public decimal WalletBalance { get; set; }
    public bool IsTrialActive { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public int DaysRemainingInTrial { get; set; }
    public bool HasActiveSubscription { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public int WalletDaysRemaining { get; set; }
    public bool ShouldShowLowBalanceWarning { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // Check if user is authenticated
            var userIdStr = HttpContext.Session.GetString("UserId");
            
            if (!int.TryParse(userIdStr, out var userId))
            {
                // User is not authenticated
                return;
            }

            // ? CHECK AND UPDATE TRIAL STATUS BASED ON WALLET BALANCE
            await _subscriptionService.CheckAndUpdateTrialStatusAsync(userId);

            // Load user data (refresh after potential trial update)
            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                // Invalid user or inactive account
                HttpContext.Session.Clear();
                return;
            }

            UserFullName = user.FullName;
            UserEmail = user.Email;
            IsTrialActive = user.IsTrialActive;
            TrialEndDate = user.TrialEndDate;

            // Calculate days remaining in trial
            if (IsTrialActive && user.TrialEndDate.HasValue && user.TrialEndDate.Value > DateTime.UtcNow)
            {
                DaysRemainingInTrial = (int)(user.TrialEndDate.Value - DateTime.UtcNow).TotalDays;
            }
            else
            {
                IsTrialActive = false;
                DaysRemainingInTrial = 0;
            }

            // Load wallet balance
            try
            {
                WalletBalance = await _walletService.GetBalanceAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading wallet balance for user {UserId}", userId);
                WalletBalance = 0;
            }

            // Load subscription information
            try
            {
                HasActiveSubscription = await _subscriptionService.HasActiveSubscriptionAsync(userId);
                
                // Calculate wallet-based subscription end date
                if (!IsTrialActive)
                {
                    WalletDaysRemaining = await _subscriptionService.GetWalletDaysRemainingAsync(userId);
                    if (WalletDaysRemaining > 0)
                    {
                        SubscriptionEndDate = DateTime.UtcNow.AddDays(WalletDaysRemaining);
                    }
                }
                
                // Check if should show low balance warning
                ShouldShowLowBalanceWarning = await _subscriptionService.ShouldShowLowBalanceWarningAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading subscription for user {UserId}", userId);
                HasActiveSubscription = false;
                WalletDaysRemaining = 0;
                ShouldShowLowBalanceWarning = false;
            }

            _logger.LogInformation("Dashboard loaded for user {UserId}. Trial: {IsTrialActive}, Balance: ?{Balance}", 
                userId, IsTrialActive, WalletBalance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
        }
    }
}
