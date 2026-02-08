using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Services.Interfaces;

/// <summary>
/// Service for system configuration management
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Gets a configuration value as string
    /// </summary>
    Task<string?> GetConfigValueAsync(string key);

    /// <summary>
    /// Gets a configuration value as decimal
    /// </summary>
    Task<decimal> GetConfigValueAsDecimalAsync(string key, decimal defaultValue = 0);

    /// <summary>
    /// Gets a configuration value as integer
    /// </summary>
    Task<int> GetConfigValueAsIntAsync(string key, int defaultValue = 0);

    /// <summary>
    /// Gets a configuration value as boolean
    /// </summary>
    Task<bool> GetConfigValueAsBoolAsync(string key, bool defaultValue = false);

    /// <summary>
    /// Gets all active configurations
    /// </summary>
    Task<List<SystemConfig>> GetAllConfigsAsync();

    /// <summary>
    /// Updates a configuration value
    /// </summary>
    Task<bool> UpdateConfigAsync(string key, string value);

    /// <summary>
    /// Gets the minimum wallet purchase amount
    /// </summary>
    Task<decimal> GetMinimumWalletPurchaseAsync();

    /// <summary>
    /// Gets the per-day cost for horoscope generation
    /// </summary>
    Task<decimal> GetPerDayCostAsync();

    /// <summary>
    /// Gets the trial period duration in days
    /// </summary>
    Task<int> GetTrialPeriodDaysAsync();

    /// <summary>
    /// Gets the low balance warning threshold in days
    /// </summary>
    Task<int> GetLowBalanceWarningDaysAsync();

    /// <summary>
    /// Gets the maximum horoscopes allowed per day
    /// </summary>
    Task<int> GetMaxHoroscopesPerDayAsync();

    /// <summary>
    /// Gets the number of years for Dasa calculation
    /// </summary>
    Task<int> GetDasaYearsAsync();
}
