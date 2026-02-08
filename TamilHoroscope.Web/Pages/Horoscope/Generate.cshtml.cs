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
    [Required(ErrorMessage = "Person name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Person Name")]
    public string PersonName { get; set; } = string.Empty;

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

    public async Task<IActionResult> OnGetAsync(bool regenerated = false)
    {
        // Check if we're displaying a regenerated horoscope from history
        if (regenerated && TempData.ContainsKey("RegeneratedHoroscope"))
        {
            try
            {
                // Deserialize the horoscope data from TempData
                var horoscopeJson = TempData["RegeneratedHoroscope"]?.ToString();
                if (!string.IsNullOrEmpty(horoscopeJson))
                {
                    Horoscope = System.Text.Json.JsonSerializer.Deserialize<HoroscopeData>(horoscopeJson);
                }

                // Restore PersonName
                PersonName = TempData["RegeneratedPersonName"]?.ToString() ?? "Historical Record";

                // Restore form fields
                if (TempData.ContainsKey("RegeneratedBirthDate"))
                {
                    if (DateTime.TryParse(TempData["RegeneratedBirthDate"]?.ToString(), out var birthDate))
                    {
                        BirthDate = birthDate;
                    }
                }

                if (TempData.ContainsKey("RegeneratedBirthTime"))
                {
                    if (TimeSpan.TryParse(TempData["RegeneratedBirthTime"]?.ToString(), out var birthTime))
                    {
                        BirthTime = birthTime;
                    }
                }

                PlaceName = TempData["RegeneratedPlaceName"]?.ToString();
                
                if (TempData.ContainsKey("RegeneratedLatitude") && 
                    double.TryParse(TempData["RegeneratedLatitude"]?.ToString(), out var lat))
                {
                    Latitude = lat;
                }

                if (TempData.ContainsKey("RegeneratedLongitude") && 
                    double.TryParse(TempData["RegeneratedLongitude"]?.ToString(), out var lon))
                {
                    Longitude = lon;
                }

                if (TempData.ContainsKey("RegeneratedTimeZoneOffset") && 
                    double.TryParse(TempData["RegeneratedTimeZoneOffset"]?.ToString(), out var tz))
                {
                    TimeZoneOffset = tz;
                }

                if (TempData.ContainsKey("RegeneratedIsTrialUser") && 
                    bool.TryParse(TempData["RegeneratedIsTrialUser"]?.ToString(), out var isTrial))
                {
                    IsTrialUser = isTrial;
                }

                // Get user ID to check trial status (if not already set)
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                {
                    IsTrialUser = await _subscriptionService.IsUserInTrialAsync(userId);
                }

                _logger.LogInformation("Displaying regenerated horoscope from history");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading regenerated horoscope from TempData");
                ErrorMessage = "Error loading horoscope. Please try again.";
            }
        }

        return Page();
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
