using System.Text.Json;
using TamilHoroscope.Web.Models;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Services.Implementations;

/// <summary>
/// Service for caching location data to avoid repeated geocoding API calls
/// Stores locations in a JSON file (locations.json) in the Data directory
/// Supports Geoapify API fallback for new locations
/// </summary>
public class LocationCacheService : ILocationCacheService
{
    private readonly string _cacheFilePath;
    private readonly ILogger<LocationCacheService> _logger;
    private readonly IConfiguration _configuration;
    private LocationCache _cache;
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

    // Geoapify API configuration
    private readonly string _apiKey;
    private readonly string _secondaryApiKey;
    private readonly string _apiBaseUrl;

    public LocationCacheService(IWebHostEnvironment env, ILogger<LocationCacheService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        // Load Geoapify API settings
        _apiKey = _configuration["GeoapifyApi:PrimaryKey"] ?? "6d25e45d65a143e8a170be688d3eb487";
        _secondaryApiKey = _configuration["GeoapifyApi:SecondaryKey"] ?? "5b875fd2d7c3486aa9f6eb50b95e4221";
        _apiBaseUrl = _configuration["GeoapifyApi:BaseUrl"] ?? "https://api.geoapify.com/v1/geocode/autocomplete";
        
        // Store cache in Data directory
        var dataDirectory = Path.Combine(env.ContentRootPath, "Data");
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }
        
        _cacheFilePath = Path.Combine(dataDirectory, "locations.json");
        
        // Load existing cache or create new one
        _cache = LoadCacheFromFile();
        
        _logger.LogInformation("LocationCacheService initialized with {Count} locations from {FilePath}", 
            _cache.TotalLocations, _cacheFilePath);
    }

    public bool TryGetLocation(string locationName, out CachedLocation? cachedLocation)
    {
        var normalizedName = NormalizeName(locationName);
        
        if (_cache.Locations.TryGetValue(normalizedName, out var location))
        {
            // Increment usage count
            location.UsageCount++;
            
            // Save asynchronously (fire and forget)
            _ = SaveCacheToDiskAsync();
            
            cachedLocation = location;
            _logger.LogInformation("Cache HIT for location: {LocationName} (used {UsageCount} times)", 
                locationName, location.UsageCount);
            return true;
        }
        
        cachedLocation = null;
        _logger.LogInformation("Cache MISS for location: {LocationName}", locationName);
        return false;
    }

    public async Task<bool> AddOrUpdateLocationAsync(CachedLocation location)
    {
        try
        {
            var normalizedName = NormalizeName(location.Name);
            
            if (_cache.Locations.ContainsKey(normalizedName))
            {
                // Update existing
                var existing = _cache.Locations[normalizedName];
                existing.Latitude = location.Latitude;
                existing.Longitude = location.Longitude;
                existing.TimeZoneOffset = location.TimeZoneOffset;
                existing.CountryCode = location.CountryCode;
                existing.State = location.State;
                existing.UsageCount++;
                
                _logger.LogInformation("Updated cached location: {LocationName}", location.Name);
            }
            else
            {
                // Add new
                _cache.Locations[normalizedName] = location;
                _logger.LogInformation("Added new location to cache: {LocationName}", location.Name);
            }
            
            _cache.LastUpdated = DateTime.UtcNow;
            
            // Save to disk
            await SaveCacheToDiskAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating location in cache: {LocationName}", location.Name);
            return false;
        }
    }

    public List<CachedLocation> SearchLocations(string query, int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<CachedLocation>();
        }
        
        var normalizedQuery = query.ToLowerInvariant().Trim();
        
        var results = _cache.Locations.Values
            .Where(loc => loc.Name.ToLowerInvariant().Contains(normalizedQuery))
            .OrderByDescending(loc => loc.UsageCount) // Most used first
            .ThenBy(loc => loc.Name) // Then alphabetically
            .Take(maxResults)
            .ToList();
        
        _logger.LogInformation("Search for '{Query}' returned {Count} results", query, results.Count);
        
        return results;
    }

    public async Task<List<CachedLocation>> SearchOnlineAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return new List<CachedLocation>();
        }

        _logger.LogInformation("Searching Geoapify API for '{SearchText}' (cache had no results)", searchText);

        try
        {
            var url = $"{_apiBaseUrl}?text={Uri.EscapeDataString(searchText)}&limit=10&apiKey={_apiKey}";
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "TamilHoroscopeWeb/1.0");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await _httpClient.GetAsync(url, cts.Token);
            
            if (!response.IsSuccessStatusCode)
            {
                // Try secondary key if primary fails
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Primary Geoapify key failed, trying secondary key");
                    url = $"{_apiBaseUrl}?text={Uri.EscapeDataString(searchText)}&limit=10&apiKey={_secondaryApiKey}";
                    response = await _httpClient.GetAsync(url, cts.Token);
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Geoapify API failed: {StatusCode}", response.StatusCode);
                    return new List<CachedLocation>();
                }
            }

            var jsonString = await response.Content.ReadAsStringAsync(cts.Token);
            var apiResponse = JsonSerializer.Deserialize<GeoapifyResponse>(jsonString);

            if (apiResponse == null || apiResponse.features == null || apiResponse.features.Count == 0)
            {
                _logger.LogInformation("No results from Geoapify API for '{SearchText}'", searchText);
                return new List<CachedLocation>();
            }

            // Convert API results to CachedLocation objects
            var results = new List<CachedLocation>();
            foreach (var feature in apiResponse.features)
            {
                if (feature.geometry?.coordinates != null && feature.geometry.coordinates.Length >= 2)
                {
                    var lon = feature.geometry.coordinates[0];
                    var lat = feature.geometry.coordinates[1];
                    
                    var location = new CachedLocation
                    {
                        Name = feature.properties?.name ?? feature.properties?.city ?? "Unknown",
                        Latitude = lat,
                        Longitude = lon,
                        TimeZoneOffset = CalculateTimezone(lon),
                        State = feature.properties?.state,
                        CountryCode = feature.properties?.country,
                        CachedAt = DateTime.UtcNow,
                        UsageCount = 0 // Not saved yet - will increment when user confirms
                    };
                    
                    results.Add(location);
                }
            }

            _logger.LogInformation("Geoapify API returned {Count} results for '{SearchText}' (NOT saved yet - waiting for user confirmation)", 
                results.Count, searchText);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Geoapify API for '{SearchText}'", searchText);
            return new List<CachedLocation>();
        }
    }

    public async Task<bool> SaveConfirmedLocationAsync(CachedLocation location)
    {
        try
        {
            var normalizedName = NormalizeName(location.Name);
            
            // Check if already exists
            var exists = _cache.Locations.ContainsKey(normalizedName);
            
            if (exists)
            {
                // Just increment usage count
                _cache.Locations[normalizedName].UsageCount++;
                _logger.LogInformation("??  Location already exists in cache: {LocationName}", location.Name);
            }
            else
            {
                // Add new confirmed location
                location.UsageCount = 1;
                location.CachedAt = DateTime.UtcNow;
                _cache.Locations[normalizedName] = location;
                _logger.LogInformation("? Saved confirmed location to cache: {LocationName}, {State}, {Country}", 
                    location.Name, location.State, location.CountryCode);
            }
            
            _cache.LastUpdated = DateTime.UtcNow;
            await SaveCacheToDiskAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error saving confirmed location: {LocationName}", location.Name);
            return false;
        }
    }

    public int GetCacheSize()
    {
        return _cache.TotalLocations;
    }

    public List<CachedLocation> GetPopularLocations(int count = 20)
    {
        return _cache.Locations.Values
            .OrderByDescending(loc => loc.UsageCount)
            .Take(count)
            .ToList();
    }

    public async Task ClearCacheAsync()
    {
        _logger.LogWarning("Clearing entire location cache ({Count} locations)", _cache.TotalLocations);
        
        _cache = new LocationCache();
        await SaveCacheToDiskAsync();
    }

    private LocationCache LoadCacheFromFile()
    {
        try
        {
            if (!File.Exists(_cacheFilePath))
            {
                _logger.LogInformation("No existing cache file found at {FilePath}, creating new cache", _cacheFilePath);
                return CreateDefaultCache();
            }
            
            var json = File.ReadAllText(_cacheFilePath);
            var cache = JsonSerializer.Deserialize<LocationCache>(json);
            
            if (cache == null)
            {
                _logger.LogWarning("Failed to deserialize cache, creating new cache");
                return CreateDefaultCache();
            }
            
            _logger.LogInformation("Loaded {Count} locations from cache file", cache.TotalLocations);
            return cache;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cache from file {FilePath}, creating new cache", _cacheFilePath);
            return CreateDefaultCache();
        }
    }

    private async Task SaveCacheToDiskAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Pretty print for readability
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var json = JsonSerializer.Serialize(_cache, options);
            await File.WriteAllTextAsync(_cacheFilePath, json);
            
            _logger.LogDebug("Saved {Count} locations to cache file", _cache.TotalLocations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving cache to file {FilePath}", _cacheFilePath);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private string NormalizeName(string name)
    {
        return name.ToLowerInvariant().Trim();
    }

    private double CalculateTimezone(double longitude)
    {
        // Rough approximation: longitude / 15 = UTC offset
        var offset = Math.Round(longitude / 15);
        
        // Clamp to valid timezone range
        return Math.Max(-12, Math.Min(14, offset));
    }

    private LocationCache CreateDefaultCache()
    {
        var cache = new LocationCache();
        
        // Pre-populate with major Indian cities
        var defaultLocations = new List<CachedLocation>
        {
            new() { Name = "Chennai, Tamil Nadu", Latitude = 13.0827, Longitude = 80.2707, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Tamil Nadu" },
            new() { Name = "Mumbai, Maharashtra", Latitude = 19.0760, Longitude = 72.8777, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Maharashtra" },
            new() { Name = "Delhi", Latitude = 28.7041, Longitude = 77.1025, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Delhi" },
            new() { Name = "Bangalore, Karnataka", Latitude = 12.9716, Longitude = 77.5946, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Karnataka" },
            new() { Name = "Kolkata, West Bengal", Latitude = 22.5726, Longitude = 88.3639, TimeZoneOffset = 5.5, CountryCode = "IN", State = "West Bengal" },
            new() { Name = "Hyderabad, Telangana", Latitude = 17.3850, Longitude = 78.4867, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Telangana" },
            new() { Name = "Pune, Maharashtra", Latitude = 18.5204, Longitude = 73.8567, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Maharashtra" },
            new() { Name = "Ahmedabad, Gujarat", Latitude = 23.0225, Longitude = 72.5714, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Gujarat" },
            new() { Name = "Jaipur, Rajasthan", Latitude = 26.9124, Longitude = 75.7873, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Rajasthan" },
            new() { Name = "Coimbatore, Tamil Nadu", Latitude = 11.0168, Longitude = 76.9558, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Tamil Nadu" },
            new() { Name = "Madurai, Tamil Nadu", Latitude = 9.9252, Longitude = 78.1198, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Tamil Nadu" },
            new() { Name = "Thiruvananthapuram, Kerala", Latitude = 8.5241, Longitude = 76.9366, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Kerala" },
            new() { Name = "Kochi, Kerala", Latitude = 9.9312, Longitude = 76.2673, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Kerala" },
            new() { Name = "Visakhapatnam, Andhra Pradesh", Latitude = 17.6868, Longitude = 83.2185, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Andhra Pradesh" },
            new() { Name = "Lucknow, Uttar Pradesh", Latitude = 26.8467, Longitude = 80.9462, TimeZoneOffset = 5.5, CountryCode = "IN", State = "Uttar Pradesh" },
        };
        
        foreach (var location in defaultLocations)
        {
            var normalizedName = NormalizeName(location.Name);
            cache.Locations[normalizedName] = location;
        }
        
        _logger.LogInformation("Created default cache with {Count} pre-populated locations", cache.TotalLocations);
        
        // Assign to field BEFORE attempting to save
        _cache = cache;
        
        // Save to disk
        _ = SaveCacheToDiskAsync();
        
        return cache;
    }
}

// Geoapify API response models
internal class GeoapifyResponse
{
    public List<GeoapifyFeature>? features { get; set; }
}

internal class GeoapifyFeature
{
    public GeoapifyProperties? properties { get; set; }
    public GeoapifyGeometry? geometry { get; set; }
}

internal class GeoapifyProperties
{
    public string? name { get; set; }
    public string? city { get; set; }
    public string? state { get; set; }
    public string? country { get; set; }
    public string? formatted { get; set; }
}

internal class GeoapifyGeometry
{
    public double[]? coordinates { get; set; } // [longitude, latitude]
}

