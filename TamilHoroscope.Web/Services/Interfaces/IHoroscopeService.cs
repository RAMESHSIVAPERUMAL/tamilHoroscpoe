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
        string? placeName = null);

    /// <summary>
    /// Checks if user has already generated a horoscope today
    /// </summary>
    Task<HoroscopeGeneration?> GetTodaysGenerationAsync(int userId);

    /// <summary>
    /// Gets horoscope generation history for a user
    /// </summary>
    Task<List<HoroscopeGeneration>> GetGenerationHistoryAsync(int userId, int pageNumber = 1, int pageSize = 20);

    /// <summary>
    /// Gets the total number of horoscope generations for a user
    /// </summary>
    Task<int> GetGenerationCountAsync(int userId);

    /// <summary>
    /// Gets the number of horoscopes generated today by a user
    /// </summary>
    Task<int> GetTodayGenerationCountAsync(int userId);

    /// <summary>
    /// Regenerates a previous horoscope (no charge)
    /// </summary>
    Task<HoroscopeData?> RegenerateHoroscopeAsync(HoroscopeGeneration generation, bool isTrialUser);
}
