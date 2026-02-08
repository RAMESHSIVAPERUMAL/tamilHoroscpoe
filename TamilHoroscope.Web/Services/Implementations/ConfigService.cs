using Microsoft.EntityFrameworkCore;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Services.Implementations;

/// <summary>
/// Implementation of configuration service
/// </summary>
public class ConfigService : IConfigService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConfigService> _logger;

    public ConfigService(ApplicationDbContext context, ILogger<ConfigService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string?> GetConfigValueAsync(string key)
    {
        var config = await _context.SystemConfigs
            .FirstOrDefaultAsync(c => c.ConfigKey == key && c.IsActive);

        return config?.ConfigValue;
    }

    public async Task<decimal> GetConfigValueAsDecimalAsync(string key, decimal defaultValue = 0)
    {
        var value = await GetConfigValueAsync(key);
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        if (decimal.TryParse(value, out var result))
            return result;

        _logger.LogWarning("Failed to parse config value for key {Key} as decimal. Using default value {Default}", key, defaultValue);
        return defaultValue;
    }

    public async Task<int> GetConfigValueAsIntAsync(string key, int defaultValue = 0)
    {
        var value = await GetConfigValueAsync(key);
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        if (int.TryParse(value, out var result))
            return result;

        _logger.LogWarning("Failed to parse config value for key {Key} as int. Using default value {Default}", key, defaultValue);
        return defaultValue;
    }

    public async Task<bool> GetConfigValueAsBoolAsync(string key, bool defaultValue = false)
    {
        var value = await GetConfigValueAsync(key);
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        if (bool.TryParse(value, out var result))
            return result;

        _logger.LogWarning("Failed to parse config value for key {Key} as bool. Using default value {Default}", key, defaultValue);
        return defaultValue;
    }

    public async Task<List<SystemConfig>> GetAllConfigsAsync()
    {
        return await _context.SystemConfigs
            .Where(c => c.IsActive)
            .OrderBy(c => c.ConfigKey)
            .ToListAsync();
    }

    public async Task<bool> UpdateConfigAsync(string key, string value)
    {
        var config = await _context.SystemConfigs
            .FirstOrDefaultAsync(c => c.ConfigKey == key);

        if (config == null)
        {
            _logger.LogWarning("Config key {Key} not found", key);
            return false;
        }

        config.ConfigValue = value;
        config.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Config key {Key} updated to value {Value}", key, value);

        return true;
    }

    public Task<decimal> GetMinimumWalletPurchaseAsync()
        => GetConfigValueAsDecimalAsync("MinimumWalletPurchase", 100.00m);

    public Task<decimal> GetPerDayCostAsync()
        => GetConfigValueAsDecimalAsync("PerDayCost", 5.00m);

    public Task<int> GetTrialPeriodDaysAsync()
        => GetConfigValueAsIntAsync("TrialPeriodDays", 30);

    public Task<int> GetLowBalanceWarningDaysAsync()
        => GetConfigValueAsIntAsync("LowBalanceWarningDays", 10);

    public Task<int> GetMaxHoroscopesPerDayAsync()
        => GetConfigValueAsIntAsync("MaxHoroscopesPerDay", 10);

    public Task<int> GetDasaYearsAsync()
        => GetConfigValueAsIntAsync("DasaYears", 120);
}
