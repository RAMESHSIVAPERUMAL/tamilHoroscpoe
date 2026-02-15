using TamilHoroscope.Web.Models;

namespace TamilHoroscope.Web.Services.Interfaces;

/// <summary>
/// Service for caching location data to avoid repeated geocoding API calls
/// </summary>
public interface ILocationCacheService
{
    /// <summary>
    /// Try to get a location from cache
    /// </summary>
    /// <param name="locationName">Name of the location (case-insensitive)</param>
    /// <param name="cachedLocation">The cached location if found</param>
    /// <returns>True if location was found in cache, false otherwise</returns>
    bool TryGetLocation(string locationName, out CachedLocation? cachedLocation);

    /// <summary>
    /// Add or update a location in the cache
    /// </summary>
    /// <param name="location">Location data to cache</param>
    /// <returns>True if successfully saved</returns>
    Task<bool> AddOrUpdateLocationAsync(CachedLocation location);

    /// <summary>
    /// Search for locations matching a query
    /// </summary>
    /// <param name="query">Search query (case-insensitive)</param>
    /// <param name="maxResults">Maximum number of results to return</param>
    /// <returns>List of matching locations</returns>
    List<CachedLocation> SearchLocations(string query, int maxResults = 10);

    /// <summary>
    /// Search for locations online using Geoapify API when cache has no results
    /// This is the fallback after cache and static data searches fail
    /// </summary>
    /// <param name="searchText">Location name to search</param>
    /// <returns>List of locations from API (NOT auto-saved to cache)</returns>
    Task<List<CachedLocation>> SearchOnlineAsync(string searchText);

    /// <summary>
    /// Save a confirmed location to cache
    /// Called when user confirms location by generating horoscope
    /// </summary>
    /// <param name="location">Location to save</param>
    /// <returns>True if successfully saved</returns>
    Task<bool> SaveConfirmedLocationAsync(CachedLocation location);

    /// <summary>
    /// Get cache statistics
    /// </summary>
    /// <returns>Total number of cached locations</returns>
    int GetCacheSize();

    /// <summary>
    /// Get most frequently used locations
    /// </summary>
    /// <param name="count">Number of locations to return</param>
    /// <returns>List of popular locations</returns>
    List<CachedLocation> GetPopularLocations(int count = 20);

    /// <summary>
    /// Clear entire cache (for admin/maintenance)
    /// </summary>
    Task ClearCacheAsync();
}
