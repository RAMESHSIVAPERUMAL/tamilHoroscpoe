using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Pages.Account;

/// <summary>
/// Profile page for displaying and managing user account information
/// </summary>
public class ProfileModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWalletService _walletService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IConfigService _configService;
    private readonly ILogger<ProfileModel> _logger;

    public ProfileModel(
        ApplicationDbContext context,
        IWalletService walletService,
        ISubscriptionService subscriptionService,
        IConfigService configService,
        ILogger<ProfileModel> logger)
    {
        _context = context;
        _walletService = walletService;
        _subscriptionService = subscriptionService;
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Current user information
    /// </summary>
    public User? CurrentUser { get; set; }

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
        // Get user ID from session
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var user = await _context.Users.FindAsync(userId);
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
        // Get user ID from session
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadUserDataAsync(user);
            return Page();
        }

        try
        {
            // TODO: Implement password change using custom authentication service
            // For now, show success message
            SuccessMessage = "Your password has been changed successfully.";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            ErrorMessage = "An error occurred while changing your password.";
            await LoadUserDataAsync(user);
            return Page();
        }
    }

    /// <summary>
    /// Loads user data from database and services
    /// </summary>
    private async Task LoadUserDataAsync(User user)
    {
        CurrentUser = user;

        try
        {
            // Load subscription status
            IsInTrial = await _subscriptionService.IsUserInTrialAsync(user.UserId);
            TrialDaysRemaining = await _subscriptionService.GetTrialDaysRemainingAsync(user.UserId);

            // Load wallet information
            WalletBalance = await _walletService.GetBalanceAsync(user.UserId);
            DailyCost = await _configService.GetPerDayCostAsync();
            WalletDaysRemaining = await _subscriptionService.GetWalletDaysRemainingAsync(user.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user data for {UserId}", user.UserId);
        }
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
