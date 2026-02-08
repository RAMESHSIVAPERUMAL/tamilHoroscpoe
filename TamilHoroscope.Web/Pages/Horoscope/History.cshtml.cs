using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Pages.Horoscope;

/// <summary>
/// Page for displaying horoscope generation history
/// </summary>
[Authorize]
public class HistoryModel : PageModel
{
    private readonly IHoroscopeService _horoscopeService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<HistoryModel> _logger;

    private const int PageSize = 10;

    public HistoryModel(
        IHoroscopeService horoscopeService,
        ISubscriptionService subscriptionService,
        UserManager<User> userManager,
        ILogger<HistoryModel> logger)
    {
        _horoscopeService = horoscopeService;
        _subscriptionService = subscriptionService;
        _userManager = userManager;
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
    /// Handles GET request to display history
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int page = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            CurrentPage = page > 0 ? page : 1;

            // Get generation history
            Generations = await _horoscopeService.GetGenerationHistoryAsync(user.Id, CurrentPage, PageSize);
            TotalCount = await _horoscopeService.GetGenerationCountAsync(user.Id);
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
            _logger.LogError(ex, "Error loading horoscope history for user {UserId}", user.Id);
            ErrorMessage = "An error occurred while loading your horoscope history. Please try again.";
            return Page();
        }
    }

    /// <summary>
    /// Handles regenerating a previous horoscope
    /// </summary>
    public async Task<IActionResult> OnPostRegenerateAsync(int generationId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            // Get the generation record by ID
            var generation = await _horoscopeService.GetGenerationByIdAsync(user.Id, generationId);

            if (generation == null)
            {
                TempData["ErrorMessage"] = "Horoscope not found.";
                return RedirectToPage();
            }

            // Check if user is currently in trial
            var isTrialUser = await _subscriptionService.IsUserInTrialAsync(user.Id);

            // Regenerate the horoscope (no charge)
            var horoscope = await _horoscopeService.RegenerateHoroscopeAsync(generation, isTrialUser);

            if (horoscope == null)
            {
                TempData["ErrorMessage"] = "Failed to regenerate horoscope. Please try again.";
                return RedirectToPage();
            }

            // Store the horoscope data in TempData to display on Generate page
            TempData["RegeneratedHoroscope"] = System.Text.Json.JsonSerializer.Serialize(horoscope);
            TempData["RegeneratedBirthDate"] = generation.BirthDateTime.ToString("yyyy-MM-dd");
            TempData["RegeneratedBirthTime"] = generation.BirthDateTime.ToString("HH:mm");
            TempData["RegeneratedPlaceName"] = generation.PlaceName;
            TempData["RegeneratedLatitude"] = generation.Latitude.ToString();
            TempData["RegeneratedLongitude"] = generation.Longitude.ToString();
            TempData["RegeneratedIsTrialUser"] = isTrialUser;

            // Redirect to Generate page to display the horoscope
            return RedirectToPage("/Horoscope/Generate", new { regenerated = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating horoscope {GenerationId} for user {UserId}", generationId, user.Id);
            TempData["ErrorMessage"] = "An error occurred while regenerating the horoscope. Please try again.";
            return RedirectToPage();
        }
    }
}
