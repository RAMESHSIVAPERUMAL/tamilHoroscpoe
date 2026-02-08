using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.ViewComponents;

/// <summary>
/// View component for displaying low balance warning banner
/// </summary>
public class LowBalanceWarningViewComponent : ViewComponent
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IWalletService _walletService;
    private readonly IConfigService _configService;

    public LowBalanceWarningViewComponent(
        ISubscriptionService subscriptionService,
        IWalletService walletService,
        IConfigService configService)
    {
        _subscriptionService = subscriptionService;
        _walletService = walletService;
        _configService = configService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Check if user is authenticated
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Content(string.Empty);
        }

        // Get userId from session
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return Content(string.Empty);
        }

        // Check if user should see the warning
        var shouldShowWarning = await _subscriptionService.ShouldShowLowBalanceWarningAsync(userId);
        if (!shouldShowWarning)
        {
            return Content(string.Empty);
        }

        // Get warning details
        var daysRemaining = await _subscriptionService.GetWalletDaysRemainingAsync(userId);
        var balance = await _walletService.GetBalanceAsync(userId);
        var perDayCost = await _configService.GetPerDayCostAsync();

        var model = new LowBalanceWarningViewModel
        {
            DaysRemaining = daysRemaining,
            CurrentBalance = balance,
            PerDayCost = perDayCost
        };

        return View(model);
    }
}

public class LowBalanceWarningViewModel
{
    public int DaysRemaining { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal PerDayCost { get; set; }
}
