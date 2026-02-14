using TamilHoroscope.Core.Models;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Services.Interfaces;

/// <summary>
/// Service for horoscope generation with subscription and billing logic
/// </summary>
public interface IHoroscopeService
{
    /// <summary>
    /// Generates a horoscope with automatic billing and feature restrictions
    /// </summary>
    Task<(HoroscopeData? horoscope, HoroscopeGeneration? generation, string? errorMessage)> GenerateHoroscopeAsync(
        int userId,
        DateTime birthDateTime,
        double latitude,
        double longitude,
        double timeZoneOffset,
        string? placeName = null,
        string? personName = null,
        string language = "Tamil");

    /// <summary>
    /// Checks if user has already generated a horoscope today
    /// </summary>
    Task<HoroscopeGeneration?> GetTodaysGenerationAsync(int userId);

    /// <summary>
    /// Gets horoscope generation history for a user with optional search filters
    /// </summary>
    Task<List<HoroscopeGeneration>> GetGenerationHistoryAsync(
        int userId, 
        int pageNumber = 1, 
        int pageSize = 20,
        string? searchPersonName = null,
        DateTime? searchBirthDate = null,
        string? searchPlaceName = null);

    /// <summary>
    /// Gets the total number of horoscope generations for a user with optional search filters
    /// </summary>
    Task<int> GetGenerationCountAsync(
        int userId,
        string? searchPersonName = null,
        DateTime? searchBirthDate = null,
        string? searchPlaceName = null);

    /// <summary>
    /// Gets a specific horoscope generation by ID for a user
    /// </summary>
    Task<HoroscopeGeneration?> GetGenerationByIdAsync(int userId, int generationId);

    /// <summary>
    /// Gets the number of horoscopes generated today by a user
    /// </summary>
    Task<int> GetTodayGenerationCountAsync(int userId);

    /// <summary>
    /// Checks if user has paid today's daily horoscope fee
    /// </summary>
    Task<bool> HasPaidTodayAsync(int userId);

    /// <summary>
    /// Regenerates a previous horoscope (no charge)
    /// </summary>
    Task<HoroscopeData?> RegenerateHoroscopeAsync(HoroscopeGeneration generation, bool isTrialUser, string language = "Tamil");
}
