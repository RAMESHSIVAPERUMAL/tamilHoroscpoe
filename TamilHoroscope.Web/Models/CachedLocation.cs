namespace TamilHoroscope.Web.Models;

/// <summary>
/// Represents a cached location with geocoding data
/// Used to avoid repeated API calls for the same location
/// </summary>
public class CachedLocation
{
    /// <summary>
    /// Display name of the location (e.g., "Chennai, Tamil Nadu")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Latitude in decimal degrees
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Timezone offset from UTC (e.g., 5.5 for IST)
    /// </summary>
    public double TimeZoneOffset { get; set; }

    /// <summary>
    /// Country code (e.g., "IN" for India)
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// State/Province name
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// When this location was first cached
    /// </summary>
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// How many times this location has been looked up
    /// </summary>
    public int UsageCount { get; set; } = 1;
}

/// <summary>
/// Container for the location cache
/// </summary>
public class LocationCache
{
    /// <summary>
    /// Dictionary of locations keyed by normalized name (lowercase, trimmed)
    /// </summary>
    public Dictionary<string, CachedLocation> Locations { get; set; } = new();

    /// <summary>
    /// Version of the cache format (for future migrations)
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Last time the cache was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Total number of locations in cache
    /// </summary>
    public int TotalLocations => Locations.Count;
}
