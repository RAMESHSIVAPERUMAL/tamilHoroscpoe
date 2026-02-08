using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Pages.Horoscope;

[Authorize]
public class GenerateModel : PageModel
{
    private readonly IHoroscopeService _horoscopeService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<GenerateModel> _logger;

    public GenerateModel(
        IHoroscopeService horoscopeService,
        ISubscriptionService subscriptionService,
        ILogger<GenerateModel> logger)
    {
        _horoscopeService = horoscopeService;
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Birth date is required")]
    [Display(Name = "Birth Date")]
    public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-25);

    [BindProperty]
    [Required(ErrorMessage = "Birth time is required")]
    [Display(Name = "Birth Time")]
    public TimeSpan BirthTime { get; set; } = new TimeSpan(12, 0, 0);

    [BindProperty]
    [Display(Name = "Place Name")]
    public string? PlaceName { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Latitude is required")]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    [Display(Name = "Latitude")]
    public double Latitude { get; set; } = 13.0827; // Chennai

    [BindProperty]
    [Required(ErrorMessage = "Longitude is required")]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    [Display(Name = "Longitude")]
    public double Longitude { get; set; } = 80.2707; // Chennai

    [BindProperty]
    [Required]
    [Display(Name = "Time Zone")]
    public double TimeZoneOffset { get; set; } = 5.5; // IST

    public HoroscopeData? Horoscope { get; set; }
    public bool IsTrialUser { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // Just display the form
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Combine date and time
            var birthDateTime = BirthDate.Date.Add(BirthTime);

            // Call horoscope service (handles all billing logic)
            var (horoscope, generation, errorMessage) = await _horoscopeService.GenerateHoroscopeAsync(
                userId,
                birthDateTime,
                Latitude,
                Longitude,
                TimeZoneOffset,
                PlaceName);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ErrorMessage = errorMessage;
                if (errorMessage.Contains("Insufficient wallet balance"))
                {
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToPage("/Wallet/TopUp");
                }
                return Page();
            }

            if (horoscope != null)
            {
                Horoscope = horoscope;
                IsTrialUser = await _subscriptionService.IsUserInTrialAsync(userId);
                
                _logger.LogInformation("Horoscope generated for user {UserId}. Trial: {IsTrial}", 
                    userId, IsTrialUser);
                
                // Stay on same page to display results
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating horoscope for user {UserId}", userId);
            ErrorMessage = "An unexpected error occurred. Please try again.";
        }

        return Page();
    }
}
