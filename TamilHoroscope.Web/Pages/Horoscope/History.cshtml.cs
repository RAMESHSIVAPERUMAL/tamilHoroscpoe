using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Pages.Horoscope;

/// <summary>
/// Page for displaying horoscope generation history
/// </summary>
public class HistoryModel : PageModel
{
    private readonly IHoroscopeService _horoscopeService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HistoryModel> _logger;

    private const int PageSize = 10;

    public HistoryModel(
        IHoroscopeService horoscopeService,
        ISubscriptionService subscriptionService,
        ApplicationDbContext context,
        ILogger<HistoryModel> logger)
    {
        _horoscopeService = horoscopeService;
        _subscriptionService = subscriptionService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// List of horoscope generations
    /// </summary>
    public List<HoroscopeGeneration> Generations { get; set; } = new();

    /// <summary>
    /// Total count of generations
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Error message to display
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Search filters
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? SearchPersonName { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? SearchBirthDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchPlaceName { get; set; }

    /// <summary>
    /// Handles GET request to display history
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int page = 1)
    {
        // Get user ID from session
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            CurrentPage = page > 0 ? page : 1;

            // Get generation history with search filters
            Generations = await _horoscopeService.GetGenerationHistoryAsync(
                userId, 
                CurrentPage, 
                PageSize,
                SearchPersonName,
                SearchBirthDate,
                SearchPlaceName);
            
            TotalCount = await _horoscopeService.GetGenerationCountAsync(
                userId,
                SearchPersonName,
                SearchBirthDate,
                SearchPlaceName);
            
            TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);

            // Ensure current page is within bounds
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                return RedirectToPage(new { page = TotalPages });
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading horoscope history for user {UserId}", userIdStr);
            ErrorMessage = "An error occurred while loading your horoscope history. Please try again.";
            return Page();
        }
    }

    /// <summary>
    /// Handles regenerating a previous horoscope
    /// </summary>
    public async Task<IActionResult> OnPostRegenerateAsync(int generationId)
    {
        // Get user ID from session
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            // Get the generation record by ID
            var generation = await _horoscopeService.GetGenerationByIdAsync(userId, generationId);

            if (generation == null)
            {
                TempData["ErrorMessage"] = "Horoscope not found.";
                return RedirectToPage();
            }

            // Check if user is currently in trial
            var isTrialUser = await _subscriptionService.IsUserInTrialAsync(userId);

            // Regenerate the horoscope (no charge)
            var horoscope = await _horoscopeService.RegenerateHoroscopeAsync(generation, isTrialUser);

            if (horoscope == null)
            {
                TempData["ErrorMessage"] = "Failed to regenerate horoscope. Please try again.";
                return RedirectToPage();
            }

            // Instead of TempData, just pass the generation ID in the URL
            // The Generate page will fetch the data from database
            _logger.LogInformation("Redirecting to Generate page with generation ID {GenerationId} for user {UserId}", generation.GenerationId, userId);

            // Redirect to Generate page with generation ID
            return RedirectToPage("/Horoscope/Generate", new { generationId = generation.GenerationId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating horoscope {GenerationId} for user {UserId}", generationId, userIdStr);
            TempData["ErrorMessage"] = "An error occurred while regenerating the horoscope. Please try again.";
            return RedirectToPage();
        }
    }
}
