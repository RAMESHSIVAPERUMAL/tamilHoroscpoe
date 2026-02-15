using Microsoft.AspNetCore.Mvc;
using TamilHoroscope.Web.Services;
using TamilHoroscope.Web.Services.Interfaces;
using TamilHoroscope.Web.Models;

namespace TamilHoroscope.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BirthPlaceController : ControllerBase
{
    private readonly BirthPlaceService _birthPlaceService;
    private readonly ILocationCacheService _locationCacheService;
    private readonly ILogger<BirthPlaceController> _logger;

    public BirthPlaceController(
        BirthPlaceService birthPlaceService, 
        ILocationCacheService locationCacheService,
        ILogger<BirthPlaceController> logger)
    {
        _birthPlaceService = birthPlaceService;
        _locationCacheService = locationCacheService;
        _logger = logger;
    }

    /// <summary>
    /// Search birth places by name with intelligent caching + Geoapify API fallback
    /// Search order: 1) Cache ? 2) Static data ? 3) Geoapify API
    /// Only confirmed locations (when user generates horoscope) are saved to cache
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return Ok(new { places = Array.Empty<object>() });
            }

            // STEP 1: Try cache first (instant lookup)
            var cachedResults = _locationCacheService.SearchLocations(q, maxResults: 10);
            
            if (cachedResults.Count > 0)
            {
                _logger.LogInformation("Cache HIT: Found {Count} results for '{Query}'", cachedResults.Count, q);
                
                var cachedPlaces = cachedResults.Select(c => new
                {
                    name = c.Name,
                    displayName = c.Name,
                    latitude = c.Latitude,
                    longitude = c.Longitude,
                    timeZone = c.TimeZoneOffset,
                    state = c.State ?? "",
                    country = c.CountryCode ?? "",
                    source = "cache" // Mark as cached
                });
                
                return Ok(new { places = cachedPlaces });
            }

            // STEP 2: Cache miss - search BirthPlaceService (static data)
            _logger.LogInformation("Cache MISS: Searching BirthPlaceService for '{Query}'", q);
            var places = _birthPlaceService.SearchPlaces(q);
            
            if (places.Any())
            {
                // Found in static data - return (will be cached when user generates horoscope)
                _logger.LogInformation("Found {Count} results in static data for '{Query}'", places.Count, q);
                
                var result = places.Select(p => new
                {
                    name = p.Name,
                    displayName = p.DisplayName,
                    latitude = p.Latitude,
                    longitude = p.Longitude,
                    timeZone = p.TimeZone,
                    state = p.State,
                    country = p.Country,
                    source = "static" // Mark as from static data
                });

                return Ok(new { places = result });
            }

            // STEP 3: Not found in cache or static data - try Geoapify API
            _logger.LogInformation("Static data MISS: Searching Geoapify API for '{Query}'", q);
            var onlineResults = await _locationCacheService.SearchOnlineAsync(q);
            
            if (onlineResults.Any())
            {
                _logger.LogInformation("Geoapify API returned {Count} results for '{Query}' (NOT saved yet - waiting for user confirmation)", 
                    onlineResults.Count, q);
                
                var apiPlaces = onlineResults.Select(c => new
                {
                    name = c.Name,
                    displayName = c.Name,
                    latitude = c.Latitude,
                    longitude = c.Longitude,
                    timeZone = c.TimeZoneOffset,
                    state = c.State ?? "",
                    country = c.CountryCode ?? "",
                    source = "api" // Mark as from API
                });
                
                return Ok(new { places = apiPlaces });
            }

            // No results found anywhere
            _logger.LogInformation("No results found for '{Query}' in cache, static data, or API", q);
            return Ok(new { places = Array.Empty<object>() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching birth places for query: {Query}", q);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get place by exact name - checks cache first
    /// </summary>
    [HttpGet("get/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        try
        {
            // Try cache first
            if (_locationCacheService.TryGetLocation(name, out var cachedLocation) && cachedLocation != null)
            {
                _logger.LogInformation("Cache HIT: Found '{Name}' in cache", name);
                
                return Ok(new
                {
                    name = cachedLocation.Name,
                    displayName = cachedLocation.Name,
                    latitude = cachedLocation.Latitude,
                    longitude = cachedLocation.Longitude,
                    timeZone = cachedLocation.TimeZoneOffset,
                    state = cachedLocation.State ?? "",
                    country = cachedLocation.CountryCode ?? "",
                    source = "cache"
                });
            }
            
            // Fall back to BirthPlaceService
            var place = _birthPlaceService.GetPlaceByName(name);
            
            if (place == null)
            {
                return NotFound(new { error = "Place not found" });
            }

            // Save to cache for next time
            await _locationCacheService.AddOrUpdateLocationAsync(new CachedLocation
            {
                Name = place.DisplayName,
                Latitude = place.Latitude,
                Longitude = place.Longitude,
                TimeZoneOffset = place.TimeZone,
                CountryCode = place.Country,
                State = place.State
            });

            return Ok(new
            {
                name = place.Name,
                displayName = place.DisplayName,
                latitude = place.Latitude,
                longitude = place.Longitude,
                timeZone = place.TimeZone,
                state = place.State,
                country = place.Country,
                source = "birthplace"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting birth place: {Name}", name);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get cache statistics (for debugging/admin)
    /// </summary>
    [HttpGet("cache/stats")]
    public IActionResult GetCacheStats()
    {
        try
        {
            var cacheSize = _locationCacheService.GetCacheSize();
            var popularLocations = _locationCacheService.GetPopularLocations(10);
            
            return Ok(new
            {
                totalCached = cacheSize,
                popularLocations = popularLocations.Select(l => new
                {
                    name = l.Name,
                    usageCount = l.UsageCount,
                    cachedAt = l.CachedAt
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache stats");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Save a confirmed location to cache
    /// Called when user generates horoscope (confirms the location)
    /// This ensures only user-confirmed locations are saved, not all search results
    /// </summary>
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmLocation([FromBody] LocationConfirmRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { error = "Location name is required" });
            }

            var location = new CachedLocation
            {
                Name = request.Name,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                TimeZoneOffset = request.TimeZoneOffset,
                State = request.State,
                CountryCode = request.CountryCode
            };

            var saved = await _locationCacheService.SaveConfirmedLocationAsync(location);

            if (saved)
            {
                _logger.LogInformation("Saved confirmed location: {Name}", request.Name);
                return Ok(new { success = true, message = "Location saved to cache" });
            }
            else
            {
                return StatusCode(500, new { error = "Failed to save location" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming location: {Name}", request.Name);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}

/// <summary>
/// Request model for confirming a location
/// </summary>
public class LocationConfirmRequest
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double TimeZoneOffset { get; set; }
    public string? State { get; set; }
    public string? CountryCode { get; set; }
}
