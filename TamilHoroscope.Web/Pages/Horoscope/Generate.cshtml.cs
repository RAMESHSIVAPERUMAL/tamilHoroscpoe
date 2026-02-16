using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Web.Services.Interfaces;
using TamilHoroscope.Web.Security;
using TamilHoroscope.Web.Models;

namespace TamilHoroscope.Web.Pages.Horoscope;

[Authorize]
public class GenerateModel : PageModel
{
    private readonly IHoroscopeService _horoscopeService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILocationCacheService _locationCacheService;
    private readonly ILogger<GenerateModel> _logger;

    public GenerateModel(
        IHoroscopeService horoscopeService,
        ISubscriptionService subscriptionService,
        ILocationCacheService locationCacheService,
        ILogger<GenerateModel> logger)
    {
        _horoscopeService = horoscopeService;
        _subscriptionService = subscriptionService;
        _locationCacheService = locationCacheService;
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
    public string Language { get; set; } = "English";

    // Hidden field for request verification
    [BindProperty]
    public string? RequestToken { get; set; }

    [BindProperty]
    public string? RequestTimestamp { get; set; }

    [BindProperty]
    public string? BirthDetailsChecksum { get; set; }

    public HoroscopeData? Horoscope { get; set; }
    public bool IsTrialUser { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Popular locations for quick selection
    public List<(string Name, double Lat, double Lon)> PopularLocations { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? generationId = null)
    {
        // Check authentication using Session (consistent with History page)
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("User not authenticated via session, redirecting to login");
            return RedirectToPage("/Account/Login", new { returnUrl = Request.Path + Request.QueryString });
        }

        // Load popular locations
        await LoadPopularLocationsAsync();

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

                // IMPORTANT: Clear security tokens when loading from history
                // This forces the client-side JavaScript to regenerate them on next submission
                RequestToken = null;
                RequestTimestamp = null;
                BirthDetailsChecksum = null;

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

        // Load popular locations (needed for display after POST)
        await LoadPopularLocationsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // SERVER-SIDE VALIDATION: Verify request token
        if (!string.IsNullOrEmpty(RequestToken) && !string.IsNullOrEmpty(RequestTimestamp))
        {
            // Parse timestamp as UTC (JavaScript sends ISO 8601 UTC timestamp)
            if (DateTime.TryParse(RequestTimestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out var timestamp))
            {
                // Ensure we're comparing in UTC
                var timestampUtc = timestamp.Kind == DateTimeKind.Utc ? timestamp : timestamp.ToUniversalTime();
                
                if (!RequestVerificationHelper.ValidateToken(userId, timestampUtc, RequestToken))
                {
                    _logger.LogWarning("Invalid request token for user {UserId}. Timestamp: {Timestamp}, Now: {Now}, Diff: {Diff} minutes", 
                        userId, timestampUtc, DateTime.UtcNow, (DateTime.UtcNow - timestampUtc).TotalMinutes);
                    ErrorMessage = "Invalid request. Please refresh the page and try again.";
                    return Page();
                }
            }
            else
            {
                _logger.LogWarning("Failed to parse request timestamp for user {UserId}: {Timestamp}", userId, RequestTimestamp);
                ErrorMessage = "Invalid request timestamp. Please refresh the page and try again.";
                return Page();
            }
        }

        // NOTE: Checksum validation disabled to allow language changes
        // Language is a display preference and doesn't affect calculation
        // Birth details are already validated by ModelState and range checks below

        // ADDITIONAL VALIDATION: Range checks
        if (Latitude < -90 || Latitude > 90)
        {
            ErrorMessage = "Invalid latitude value. Must be between -90 and 90.";
            return Page();
        }

        if (Longitude < -180 || Longitude > 180)
        {
            ErrorMessage = "Invalid longitude value. Must be between -180 and 180.";
            return Page();
        }

        if (TimeZoneOffset < -12 || TimeZoneOffset > 14)
        {
            ErrorMessage = "Invalid timezone offset. Must be between -12 and +14.";
            return Page();
        }

        if (BirthDate.Year < 1900 || BirthDate.Year > DateTime.Now.Year + 1)
        {
            ErrorMessage = "Invalid birth year. Must be between 1900 and current year.";
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
                
                // IMPORTANT: Save confirmed location to cache
                // This ensures only user-confirmed locations are saved, not all API search results
                if (!string.IsNullOrWhiteSpace(PlaceName))
                {
                    var confirmedLocation = new CachedLocation
                    {
                        Name = PlaceName,
                        Latitude = Latitude,
                        Longitude = Longitude,
                        TimeZoneOffset = TimeZoneOffset,
                        State = null, // Extract from PlaceName if needed
                        CountryCode = null // Extract from PlaceName if needed
                    };
                    
                    // Save to cache (fire and forget)
                    _ = _locationCacheService.SaveConfirmedLocationAsync(confirmedLocation);
                    _logger.LogInformation("Saved confirmed location to cache: {PlaceName}", PlaceName);
                }
                
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

    /// <summary>
    /// Loads popular locations from database and adds default important cities
    /// </summary>
    private async Task LoadPopularLocationsAsync()
    {
        try
        {
            // Get top 5 most frequently used locations from database
            var popularFromDb = await _horoscopeService.GetPopularLocationsAsync(5);
            
            // Add to list
            PopularLocations.AddRange(popularFromDb.Select(x => (x.PlaceName, x.Latitude, x.Longitude)));
            
            _logger.LogInformation("Loaded {Count} popular locations from database", popularFromDb.Count);
            
            // Add default important cities (always include these)
            var defaultCities = new List<(string Name, double Lat, double Lon)>
            {
                ("Chennai", 13.0827, 80.2707),
                ("Bangalore", 12.9716, 77.5946),
                ("Thiruvananthapuram", 8.5241, 76.9366),
                ("Hyderabad", 17.3850, 78.4867),
                ("Amaravati", 16.5742, 80.3585)
            };
            
            // Add default cities if not already in popular list
            foreach (var city in defaultCities)
            {
                if (!PopularLocations.Any(x => x.Name.Equals(city.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    PopularLocations.Add(city);
                }
            }
            
            // Limit to 10 total locations for display
            PopularLocations = PopularLocations.Take(10).ToList();
            
            _logger.LogInformation("Popular locations list finalized with {Count} locations", PopularLocations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading popular locations, using defaults");
            
            // Fallback to default cities only
            PopularLocations = new List<(string, double, double)>
            {
                ("Chennai", 13.0827, 80.2707),
                ("Bangalore", 12.9716, 77.5946),
                ("Thiruvananthapuram", 8.5241, 76.9366),
                ("Hyderabad", 17.3850, 78.4867),
                ("Amaravati", 16.5742, 80.3585)
            };
        }
    }
}
