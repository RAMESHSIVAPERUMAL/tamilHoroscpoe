using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Pages.Account;

/// <summary>
/// Profile page for displaying and managing user account information
/// </summary>
[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletService _walletService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IConfigService _configService;

    public ProfileModel(
        UserManager<User> userManager,
        IWalletService walletService,
        ISubscriptionService subscriptionService,
        IConfigService configService)
    {
        _userManager = userManager;
        _walletService = walletService;
        _subscriptionService = subscriptionService;
        _configService = configService;
    }

    /// <summary>
    /// Current user information
    /// </summary>
    public new User? User { get; set; }

    /// <summary>
    /// Whether user is in trial period
    /// </summary>
    public bool IsInTrial { get; set; }

    /// <summary>
    /// Trial days remaining
    /// </summary>
    public int TrialDaysRemaining { get; set; }

    /// <summary>
    /// Wallet balance
    /// </summary>
    public decimal WalletBalance { get; set; }

    /// <summary>
    /// Daily cost for horoscope generation
    /// </summary>
    public decimal DailyCost { get; set; }

    /// <summary>
    /// Wallet days remaining based on balance
    /// </summary>
    public int WalletDaysRemaining { get; set; }

    /// <summary>
    /// Success message to display
    /// </summary>
    [TempData]
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Error message to display
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Model for password change form
    /// </summary>
    [BindProperty]
    public ChangePasswordViewModel ChangePasswordModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(base.User);
        if (user == null)
        {
            return NotFound();
        }

        await LoadUserDataAsync(user);
        return Page();
    }

    /// <summary>
    /// Handles password change request
    /// </summary>
    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        var user = await _userManager.GetUserAsync(base.User);
        if (user == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadUserDataAsync(user);
            return Page();
        }

        var result = await _userManager.ChangePasswordAsync(user, 
            ChangePasswordModel.CurrentPassword, 
            ChangePasswordModel.NewPassword);

        if (result.Succeeded)
        {
            SuccessMessage = "Your password has been changed successfully.";
            return RedirectToPage();
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        await LoadUserDataAsync(user);
        return Page();
    }

    /// <summary>
    /// Loads user data from database and services
    /// </summary>
    private async Task LoadUserDataAsync(User user)
    {
        User = user;

        // Load subscription status
        IsInTrial = await _subscriptionService.IsUserInTrialAsync(user.Id);
        TrialDaysRemaining = await _subscriptionService.GetTrialDaysRemainingAsync(user.Id);

        // Load wallet information
        WalletBalance = await _walletService.GetBalanceAsync(user.Id);
        DailyCost = await _configService.GetPerDayCostAsync();
        WalletDaysRemaining = await _subscriptionService.GetWalletDaysRemainingAsync(user.Id);
    }
}

/// <summary>
/// View model for password change form
/// </summary>
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
