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
        string? placeName = null,
        string? personName = null,
        string language = "English")
    {
        try
        {
            // Step 1: Check rate limiting
            var todayCount = await GetTodayGenerationCountAsync(userId);
            var maxPerDay = await _configService.GetMaxHoroscopesPerDayAsync();
            if (todayCount >= maxPerDay)
            {
                return (null, null, $"Daily limit reached. You can generate a maximum of {maxPerDay} horoscopes per day.");
            }

            // Step 2: Get user to check last fee deduction date
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (null, null, "User not found.");
            }

            // Step 3: Check if today's fee has been deducted
            var today = DateTime.UtcNow.Date;
            var hasPaidToday = user.LastDailyFeeDeductionDate?.Date == today;

            // Step 4: Determine if user should be treated as trial or paid
            var isInTrial = await _subscriptionService.IsUserInTrialAsync(userId);
            var treatAsTrial = isInTrial || !hasPaidToday; // Treat as trial if not paid today
            
            var perDayCost = await _configService.GetPerDayCostAsync();
            var amountDeducted = 0m;

            if (isInTrial)
            {
                // Trial period: No charge, limited features
                _logger.LogInformation("User {UserId} is in trial period. Generating limited horoscope with no charge.", userId);
            }
            else if (!hasPaidToday)
            {
                // Not in trial but haven't paid today - deduct daily fee
                var hasSufficientBalance = await _walletService.HasSufficientBalanceAsync(userId, perDayCost);
                if (!hasSufficientBalance)
                {
                    var balance = await _walletService.GetBalanceAsync(userId);
                    _logger.LogWarning("User {UserId} has insufficient balance. Balance: ₹{Balance}, Required: ₹{Required}", 
                        userId, balance, perDayCost);
                    
                    // Insufficient balance - treat as trial for today
                    treatAsTrial = true;
                    _logger.LogInformation("User {UserId} insufficient balance. Generating as trial for today.", userId);
                }
                else
                {
                    // Deduct amount
                    await _walletService.DeductFundsAsync(userId, perDayCost, "Daily horoscope generation fee");
                    amountDeducted = perDayCost;

                    // Update last fee deduction date
                    user.LastDailyFeeDeductionDate = today;
                    await _context.SaveChangesAsync();

                    treatAsTrial = false; // Paid successfully
                    _logger.LogInformation("Deducted ₹{Amount} from user {UserId} for daily horoscope generation fee.", perDayCost, userId);
                }
            }
            else
            {
                // Already paid today - no charge, full features
                _logger.LogInformation("User {UserId} already paid today's fee. Generating full-featured horoscope.", userId);
                treatAsTrial = false;
            }

            // Step 5: Calculate horoscope
            var horoscope = await CalculateHoroscopeInternalAsync(
                birthDateTime,
                latitude,
                longitude,
                timeZoneOffset,
                placeName,
                treatAsTrial,
                language);

            // Step 6: Record generation
            var generation = new HoroscopeGeneration
            {
                UserId = userId,
                GenerationDate = DateTime.UtcNow.Date, // Store date only
                PersonName = personName,
                BirthDateTime = birthDateTime,
                PlaceName = placeName,
                Latitude = (decimal)latitude,
                Longitude = (decimal)longitude,
                AmountDeducted = amountDeducted,
                WasTrialPeriod = treatAsTrial,
                CreatedDateTime = DateTime.UtcNow
            };

            _context.HoroscopeGenerations.Add(generation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Horoscope generated successfully for user {UserId}. TreatAsTrial: {TreatAsTrial}, Amount: ₹{Amount}",
                userId, treatAsTrial, amountDeducted);

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

    public async Task<List<HoroscopeGeneration>> GetGenerationHistoryAsync(
        int userId, 
        int pageNumber = 1, 
        int pageSize = 20,
        string? searchPersonName = null,
        DateTime? searchBirthDate = null,
        string? searchPlaceName = null)
    {
        var query = _context.HoroscopeGenerations
            .Where(h => h.UserId == userId);

        // Apply search filters
        if (!string.IsNullOrWhiteSpace(searchPersonName))
        {
            query = query.Where(h => h.PersonName != null && h.PersonName.Contains(searchPersonName));
        }

        if (searchBirthDate.HasValue)
        {
            query = query.Where(h => h.BirthDateTime.Date == searchBirthDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(searchPlaceName))
        {
            query = query.Where(h => h.PlaceName != null && h.PlaceName.Contains(searchPlaceName));
        }

        return await query
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

    public async Task<int> GetGenerationCountAsync(
        int userId,
        string? searchPersonName = null,
        DateTime? searchBirthDate = null,
        string? searchPlaceName = null)
    {
        var query = _context.HoroscopeGenerations
            .Where(h => h.UserId == userId);

        // Apply search filters
        if (!string.IsNullOrWhiteSpace(searchPersonName))
        {
            query = query.Where(h => h.PersonName != null && h.PersonName.Contains(searchPersonName));
        }

        if (searchBirthDate.HasValue)
        {
        query = query.Where(h => h.BirthDateTime.Date == searchBirthDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(searchPlaceName))
        {
            query = query.Where(h => h.PlaceName != null && h.PlaceName.Contains(searchPlaceName));
        }

        return await query.CountAsync();
    }

    public async Task<int> GetTodayGenerationCountAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;

        return await _context.HoroscopeGenerations
            .CountAsync(h => h.UserId == userId && h.GenerationDate == today);
    }

    public async Task<bool> HasPaidTodayAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        var today = DateTime.UtcNow.Date;
        return user.LastDailyFeeDeductionDate?.Date == today;
    }

    public async Task<HoroscopeData?> RegenerateHoroscopeAsync(HoroscopeGeneration generation, bool isTrialUser, string language = "English")
    {
        try
        {
            return await CalculateHoroscopeInternalAsync(
                generation.BirthDateTime,
                (double)generation.Latitude,
                (double)generation.Longitude,
                5.5, // Default IST timezone
                generation.PlaceName,
                isTrialUser,
                language);
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
        bool isTrialUser,
        string language = "English")
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
            // - Include yoga and dosa detection (free for all users)
            _logger.LogDebug("Calculating horoscope for trial user with limited features");

            return _calculator.CalculateHoroscope(
                birthDetails,
                includeDasa: true,
                includeNavamsa: false,
                dasaYears: dasaYears,
                includeStrength: false,
                includeYoga: true,
                includeDosa: true,
                language: language);
        }
        else
        {
            // Paid users: Full features including yoga and dosa detection
            _logger.LogDebug("Calculating horoscope for paid user with full features");

            return _calculator.CalculateHoroscope(
                birthDetails,
                includeDasa: true,
                includeNavamsa: true,
                dasaYears: dasaYears,
                includeStrength: true,
                includeYoga: true,
                includeDosa: true,
                language: language);
        }
    }

    public async Task<List<(string PlaceName, double Latitude, double Longitude, int UsageCount)>> GetPopularLocationsAsync(int count = 5)
    {
        try
        {
            // Get top N locations by usage count from HoroscopeGenerations table
            var popularLocations = await _context.HoroscopeGenerations
                .Where(h => h.PlaceName != null && h.PlaceName != string.Empty)
                .GroupBy(h => new { h.PlaceName, h.Latitude, h.Longitude })
                .Select(g => new
                {
                    PlaceName = g.Key.PlaceName!,
                    Latitude = g.Key.Latitude,
                    Longitude = g.Key.Longitude,
                    UsageCount = g.Count()
                })
                .OrderByDescending(x => x.UsageCount)
                .Take(count)
                .ToListAsync();

            return popularLocations
                .Select(x => (x.PlaceName, (double)x.Latitude, (double)x.Longitude, x.UsageCount))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching popular locations");
            return new List<(string, double, double, int)>();
        }
    }
}
