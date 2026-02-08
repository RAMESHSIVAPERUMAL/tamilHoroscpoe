using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Pages.Wallet;

[Authorize]
public class TopUpModel : PageModel
{
    private readonly IWalletService _walletService;
    private readonly IConfigService _configService;
    private readonly ILogger<TopUpModel> _logger;

    public TopUpModel(
        IWalletService walletService,
        IConfigService configService,
        ILogger<TopUpModel> logger)
    {
        _walletService = walletService;
        _configService = configService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 100000, ErrorMessage = "Amount must be between ₹0.01 and ₹100,000")]
    [Display(Name = "Amount (₹)")]
    public decimal Amount { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Please select a payment method")]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        if (ModelState.IsValid)
        {
            // Validate minimum purchase amount
            var minPurchase = await _configService.GetMinimumWalletPurchaseAsync();
            if (Amount < minPurchase)
            {
                ModelState.AddModelError(nameof(Amount), $"Minimum top-up amount is ₹{minPurchase:F2}");
                return Page();
            }

            // In a real application, this would integrate with a payment gateway
            // For now, we'll simulate a successful payment
            try
            {
                var referenceId = $"DEMO-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                
                var transaction = await _walletService.AddFundsAsync(
                    userId,
                    Amount,
                    $"Wallet top-up via {PaymentMethod}",
                    referenceId);

                _logger.LogInformation("Wallet topped up successfully. User: {UserId}, Amount: ₹{Amount}, Reference: {RefId}",
                    userId, Amount, referenceId);

                TempData["SuccessMessage"] = $"Your wallet has been topped up successfully! Amount: ₹{Amount:F2}, Reference ID: {referenceId}";
                
                return RedirectToPage("/Wallet/History");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error topping up wallet for user {UserId}", userId);
                ModelState.AddModelError(string.Empty, "An error occurred while processing your payment. Please try again.");
            }
        }

        return Page();
    }
}
