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

    [BindProperty]
    [Required(ErrorMessage = "Language is required")]
    [Display(Name = "Display Language")]
    public string Language { get; set; } = "Tamil";

    public HoroscopeData? Horoscope { get; set; }
    public bool IsTrialUser { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? generationId = null)
    {
        // Check authentication using Session (consistent with History page)
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("User not authenticated via session, redirecting to login");
            return RedirectToPage("/Account/Login", new { returnUrl = Request.Path + Request.QueryString });
        }

        // Check if we're displaying a regenerated horoscope from history
        if (generationId.HasValue)
        {
            try
            {
                _logger.LogInformation("Loading horoscope from generation ID {GenerationId} for user {UserId}", generationId.Value, userId);

                // Get the generation record by ID
                var generation = await _horoscopeService.GetGenerationByIdAsync(userId, generationId.Value);

                if (generation == null)
                {
                    ErrorMessage = "Horoscope not found.";
                    _logger.LogWarning("Generation {GenerationId} not found for user {UserId}", generationId.Value, userId);
                    return Page();
                }

                // Check if user is currently in trial
                IsTrialUser = await _subscriptionService.IsUserInTrialAsync(userId);

                // Regenerate the horoscope (no charge)
                Horoscope = await _horoscopeService.RegenerateHoroscopeAsync(generation, IsTrialUser, Language);

                if (Horoscope == null)
                {
                    ErrorMessage = "Failed to regenerate horoscope. Please try again.";
                    _logger.LogWarning("Failed to regenerate horoscope for generation {GenerationId}", generationId.Value);
                    return Page();
                }

                // Restore form fields from generation
                PersonName = generation.PersonName ?? "Historical Record";
                BirthDate = generation.BirthDateTime.Date;
                BirthTime = generation.BirthDateTime.TimeOfDay;
                PlaceName = generation.PlaceName;
                Latitude = (double)generation.Latitude;
                Longitude = (double)generation.Longitude;
                TimeZoneOffset = 5.5; // Default IST

                _logger.LogInformation("Successfully loaded and regenerated horoscope for generation {GenerationId}, user {UserId}", generationId.Value, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading horoscope from generation ID {GenerationId} for user {UserId}", generationId.Value, userId);
                ErrorMessage = "Error loading horoscope. Please try again.";
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Check authentication using Session (consistent with History page)
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("User not authenticated via session during POST, redirecting to login");
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
                PlaceName,
                PersonName,
                Language);

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
