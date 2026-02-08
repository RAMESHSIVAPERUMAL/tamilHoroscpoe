using Microsoft.EntityFrameworkCore;
using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Services.Implementations;

/// <summary>
/// Implementation of horoscope service with billing and subscription logic
/// </summary>
public class HoroscopeService : IHoroscopeService
{
    private readonly ApplicationDbContext _context;
    private readonly IWalletService _walletService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IConfigService _configService;
    private readonly PanchangCalculator _calculator;
    private readonly ILogger<HoroscopeService> _logger;

    public HoroscopeService(
        ApplicationDbContext context,
        IWalletService walletService,
        ISubscriptionService subscriptionService,
        IConfigService configService,
        ILogger<HoroscopeService> logger)
    {
        _context = context;
        _walletService = walletService;
        _subscriptionService = subscriptionService;
        _configService = configService;
        _calculator = new PanchangCalculator();
        _logger = logger;
    }

    public async Task<(HoroscopeData? horoscope, HoroscopeGeneration? generation, string? errorMessage)> GenerateHoroscopeAsync(
        int userId,
        DateTime birthDateTime,
        double latitude,
        double longitude,
        double timeZoneOffset,
        string? placeName = null)
    {
        try
        {
            // Step 1: Check if user has already generated a horoscope today
            var todaysGeneration = await GetTodaysGenerationAsync(userId);
            if (todaysGeneration != null)
            {
                _logger.LogInformation("User {UserId} already generated horoscope today. Returning cached result.", userId);
                
                // Regenerate horoscope from saved parameters
                var isTrialUser = await _subscriptionService.IsUserInTrialAsync(userId);
                var cachedHoroscope = await RegenerateHoroscopeAsync(todaysGeneration, isTrialUser);
                
                return (cachedHoroscope, todaysGeneration, null);
            }

            // Step 2: Check rate limiting
            var todayCount = await GetTodayGenerationCountAsync(userId);
            var maxPerDay = await _configService.GetMaxHoroscopesPerDayAsync();
            if (todayCount >= maxPerDay)
            {
                return (null, null, $"Daily limit reached. You can generate a maximum of {maxPerDay} horoscopes per day.");
            }

            // Step 3: Check trial status
            var isInTrial = await _subscriptionService.IsUserInTrialAsync(userId);
            var perDayCost = await _configService.GetPerDayCostAsync();
            var amountDeducted = 0m;

            if (isInTrial)
            {
                // Trial period: No charge, limited features
                _logger.LogInformation("User {UserId} is in trial period. Generating limited horoscope with no charge.", userId);
            }
            else
            {
                // Paid subscription: Check wallet balance and deduct
                var hasSufficientBalance = await _walletService.HasSufficientBalanceAsync(userId, perDayCost);
                if (!hasSufficientBalance)
                {
                    var balance = await _walletService.GetBalanceAsync(userId);
                    return (null, null, $"Insufficient wallet balance. Current balance: ₹{balance:F2}, Required: ₹{perDayCost:F2}. Please top up your wallet.");
                }

                // Deduct amount
                await _walletService.DeductFundsAsync(userId, perDayCost, "Daily horoscope generation fee");
                amountDeducted = perDayCost;

                _logger.LogInformation("Deducted ₹{Amount} from user {UserId} for horoscope generation.", perDayCost, userId);
            }

            // Step 4: Calculate horoscope
            var horoscope = await CalculateHoroscopeInternalAsync(
                birthDateTime,
                latitude,
                longitude,
                timeZoneOffset,
                placeName,
                isInTrial);

            // Step 5: Record generation
            var generation = new HoroscopeGeneration
            {
                UserId = userId,
                GenerationDate = DateTime.UtcNow.Date, // Store date only
                BirthDateTime = birthDateTime,
                PlaceName = placeName,
                Latitude = (decimal)latitude,
                Longitude = (decimal)longitude,
                AmountDeducted = amountDeducted,
                WasTrialPeriod = isInTrial,
                CreatedDateTime = DateTime.UtcNow
            };

            _context.HoroscopeGenerations.Add(generation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Horoscope generated successfully for user {UserId}. Trial: {IsTrial}, Amount: ₹{Amount}",
                userId, isInTrial, amountDeducted);

            return (horoscope, generation, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating horoscope for user {UserId}", userId);
            return (null, null, "An error occurred while generating the horoscope. Please try again.");
        }
    }

    public async Task<HoroscopeGeneration?> GetTodaysGenerationAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;

        return await _context.HoroscopeGenerations
            .Where(h => h.UserId == userId && h.GenerationDate == today)
            .OrderByDescending(h => h.CreatedDateTime)
            .FirstOrDefaultAsync();
    }

    public async Task<List<HoroscopeGeneration>> GetGenerationHistoryAsync(int userId, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.HoroscopeGenerations
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.CreatedDateTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<HoroscopeGeneration?> GetGenerationByIdAsync(int userId, int generationId)
    {
        return await _context.HoroscopeGenerations
            .Where(h => h.UserId == userId && h.GenerationId == generationId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetGenerationCountAsync(int userId)
    {
        return await _context.HoroscopeGenerations
            .CountAsync(h => h.UserId == userId);
    }

    public async Task<int> GetTodayGenerationCountAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;

        return await _context.HoroscopeGenerations
            .CountAsync(h => h.UserId == userId && h.GenerationDate == today);
    }

    public async Task<HoroscopeData?> RegenerateHoroscopeAsync(HoroscopeGeneration generation, bool isTrialUser)
    {
        try
        {
            return await CalculateHoroscopeInternalAsync(
                generation.BirthDateTime,
                (double)generation.Latitude,
                (double)generation.Longitude,
                5.5, // Default IST timezone
                generation.PlaceName,
                isTrialUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating horoscope for generation {GenerationId}", generation.GenerationId);
            return null;
        }
    }

    /// <summary>
    /// Internal method to calculate horoscope with feature restrictions based on subscription
    /// </summary>
    private async Task<HoroscopeData> CalculateHoroscopeInternalAsync(
        DateTime birthDateTime,
        double latitude,
        double longitude,
        double timeZoneOffset,
        string? placeName,
        bool isTrialUser)
    {
        var birthDetails = new BirthDetails
        {
            DateTime = birthDateTime,
            Latitude = latitude,
            Longitude = longitude,
            TimeZoneOffset = timeZoneOffset,
            PlaceName = placeName
        };

        var dasaYears = await _configService.GetDasaYearsAsync();

        if (isTrialUser)
        {
            // Trial users: Basic features only
            // - Show Rasi chart (always included)
            // - Show Dasa (but UI will filter to show only main periods, not Bhukti)
            // - No Navamsa chart
            // - No planetary strength
            _logger.LogDebug("Calculating horoscope for trial user with limited features");

            return _calculator.CalculateHoroscope(
                birthDetails,
                includeDasa: true,
                includeNavamsa: false,
                dasaYears: dasaYears,
                includeStrength: false);
        }
        else
        {
            // Paid users: Full features
            _logger.LogDebug("Calculating horoscope for paid user with full features");

            return _calculator.CalculateHoroscope(
                birthDetails,
                includeDasa: true,
                includeNavamsa: true,
                dasaYears: dasaYears,
                includeStrength: true);
        }
    }
}
