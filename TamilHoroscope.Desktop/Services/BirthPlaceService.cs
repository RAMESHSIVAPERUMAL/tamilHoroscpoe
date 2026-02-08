using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Xml.Linq;
using TamilHoroscope.Desktop.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace TamilHoroscope.Desktop.Services;

/// <summary>
/// Service to load and search birth places from XML data with Geoapify API fallback
/// Implements intelligent caching to minimize API calls
/// </summary>
public class BirthPlaceService
{
    private List<BirthPlace> _birthPlaces = new();
    private const string DataFileName = "BirthPlaces.xml";
    private static readonly HttpClient _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10)
    };
    
    private string? _xmlFilePath;
    private bool _isOnline = false;
    private readonly IConfiguration? _configuration;
    
    // Geoapify API configuration
    private string _apiKey;
    private string _secondaryApiKey;
    private string _apiBaseUrl;
    private bool _useCache;
    private bool _autoSaveNewPlaces;

    /// <summary>
    /// Initializes a new instance of BirthPlaceService with configuration
    /// </summary>
    public BirthPlaceService()
    {
        // Try to load configuration
        try
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            
            _configuration = builder.Build();
            
            // Load API keys from config
            _apiKey = _configuration["GeoapifyApi:PrimaryKey"] ?? "6d25e45d65a143e8a170be688d3eb487";
            _secondaryApiKey = _configuration["GeoapifyApi:SecondaryKey"] ?? "5b875fd2d7c3486aa9f6eb50b95e4221";
            _apiBaseUrl = _configuration["GeoapifyApi:BaseUrl"] ?? "https://api.geoapify.com/v1/geocode/autocomplete";
            
            // Load cache settings
            _useCache = bool.TryParse(_configuration["CacheSettings:UseCache"], out var useCache) ? useCache : true;
            _autoSaveNewPlaces = bool.TryParse(_configuration["CacheSettings:AutoSaveNewPlaces"], out var autoSave) ? autoSave : true;
        }
        catch
        {
            // Fallback to hardcoded values if config fails
            _apiKey = "6d25e45d65a143e8a170be688d3eb487";
            _secondaryApiKey = "5b875fd2d7c3486aa9f6eb50b95e4221";
            _apiBaseUrl = "https://api.geoapify.com/v1/geocode/autocomplete";
            _useCache = true;
            _autoSaveNewPlaces = true;
        }
    }
    
    /// <summary>
    /// Loads birth places from XML file
    /// </summary>
    public void LoadBirthPlaces()
    {
        try
        {
            // Try multiple possible locations for the XML file
            var possiblePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", DataFileName),
                Path.Combine(Directory.GetCurrentDirectory(), "Data", DataFileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataFileName),
                DataFileName
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _xmlFilePath = path;
                    break;
                }
            }

            if (_xmlFilePath == null)
            {
                // Create new XML file if not exists
                var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                Directory.CreateDirectory(dataDir);
                _xmlFilePath = Path.Combine(dataDir, DataFileName);
                CreateDefaultXmlFile();
            }

            LoadFromXml();
            
            // Check if online
            CheckInternetConnection();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading birth places: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates a default XML file with basic structure
    /// </summary>
    private void CreateDefaultXmlFile()
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("BirthPlaces",
                new XComment("Birth places database - automatically populated from Geoapify API"),
                new XComment("Format: Name, TamilName, Latitude, Longitude, TimeZone, State, Country")
            )
        );
        doc.Save(_xmlFilePath!);
    }

    /// <summary>
    /// Loads birth places from local XML file
    /// </summary>
    private void LoadFromXml()
    {
        if (_xmlFilePath == null) return;

        var doc = XDocument.Load(_xmlFilePath);
        _birthPlaces = doc.Root?
            .Elements("Place")
            .Select(e => new BirthPlace
            {
                Name = e.Element("Name")?.Value ?? string.Empty,
                TamilName = e.Element("TamilName")?.Value ?? string.Empty,
                Latitude = double.Parse(e.Element("Latitude")?.Value ?? "0"),
                Longitude = double.Parse(e.Element("Longitude")?.Value ?? "0"),
                TimeZone = double.Parse(e.Element("TimeZone")?.Value ?? "0"),
                State = e.Element("State")?.Value ?? string.Empty,
                Country = e.Element("Country")?.Value ?? string.Empty
            })
            .OrderBy(p => p.Name)
            .ToList() ?? new List<BirthPlace>();
    }

    /// <summary>
    /// Checks if internet connection is available
    /// </summary>
    private async void CheckInternetConnection()
    {
        try
        {
            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(3));
            var response = await _httpClient.GetAsync("https://www.google.com", cts.Token);
            _isOnline = response.IsSuccessStatusCode;
        }
        catch
        {
            _isOnline = false;
        }
    }

    /// <summary>
    /// Searches places online using Geoapify API if internet is available
    /// This method is called ONLY when local XML cache has NO results
    /// Returns results WITHOUT auto-saving (saved when user clicks Calculate)
    /// </summary>
    public async Task<List<BirthPlace>> SearchPlacesOnlineAsync(string searchText)
    {
        // Validate search text
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return _birthPlaces;
        }

        // IMPORTANT: This method should only be called when local cache has no results
        // The UI (MainWindow) checks local cache first
        
        System.Diagnostics.Debug.WriteLine($"SearchPlacesOnlineAsync called for '{searchText}' (local cache had no results)");

        // Check if we're online
        if (!_isOnline)
        {
            System.Diagnostics.Debug.WriteLine("Offline - cannot call API");
            return new List<BirthPlace>();
        }

        // Call Geoapify API with the EXACT user-typed text
        try
        {
            var onlinePlaces = await SearchGeoapifyAsync(searchText);
            
            if (onlinePlaces.Count > 0)
            {
                // NOTE: Not saving here! Will be saved when user confirms by clicking "Calculate"
                System.Diagnostics.Debug.WriteLine($"Geoapify returned {onlinePlaces.Count} results for '{searchText}' (NOT saved yet - waiting for user confirmation)");
                return onlinePlaces;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Geoapify returned no results for '{searchText}'");
                return new List<BirthPlace>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Geoapify API error: {ex.Message}");
            return new List<BirthPlace>();
        }
    }

    /// <summary>
    /// Saves a confirmed birth place to XML
    /// Called when user confirms location by clicking "Calculate Horoscope"
    /// This ensures only user-confirmed locations are saved, not all API search results
    /// </summary>
    public async Task SaveConfirmedPlaceAsync(BirthPlace place)
    {
        if (_xmlFilePath == null || place == null)
        {
            return;
        }

        try
        {
            await Task.Run(() =>
            {
                var doc = XDocument.Load(_xmlFilePath);
                var root = doc.Root;
                
                if (root == null) return;

                // Check if place already exists (within 0.01 degree precision)
                var exists = _birthPlaces.Any(p => 
                    p.Name.Equals(place.Name, StringComparison.OrdinalIgnoreCase) &&
                    Math.Abs(p.Latitude - place.Latitude) < 0.01 &&
                    Math.Abs(p.Longitude - place.Longitude) < 0.01);

                if (!exists)
                {
                    var placeElement = new XElement("Place",
                        new XElement("Name", place.Name),
                        new XElement("TamilName", place.TamilName ?? ""),
                        new XElement("Latitude", place.Latitude.ToString("F6")),
                        new XElement("Longitude", place.Longitude.ToString("F6")),
                        new XElement("TimeZone", place.TimeZone.ToString("F1")),
                        new XElement("State", place.State ?? ""),
                        new XElement("Country", place.Country ?? "")
                    );
                    
                    root.Add(placeElement);
                    doc.Save(_xmlFilePath);
                    
                    // Add to in-memory cache
                    _birthPlaces.Add(place);
                    _birthPlaces = _birthPlaces.OrderBy(p => p.Name).ToList();
                    
                    System.Diagnostics.Debug.WriteLine($"✅ Saved confirmed location to XML: {place.Name}, {place.State}, {place.Country}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⏭️  Location already exists in XML: {place.Name}");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error saving confirmed place to XML: {ex.Message}");
        }
    }

    /// <summary>
    /// Searches Geoapify API for locations using the EXACT user-typed text
    /// </summary>
    private async Task<List<BirthPlace>> SearchGeoapifyAsync(string searchText)
    {
        try
        {
            // Geoapify autocomplete endpoint
            // https://api.geoapify.com/v1/geocode/autocomplete?text=Kannur&apiKey=YOUR_KEY
            
            // IMPORTANT: Use Uri.EscapeDataString to properly encode the user's search text
            var url = $"{_apiBaseUrl}?text={Uri.EscapeDataString(searchText)}&limit=10&apiKey={_apiKey}";
            
            System.Diagnostics.Debug.WriteLine($"Geoapify API URL: {url}");
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "TamilHoroscopeCalculator/1.0");

            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await _httpClient.GetAsync(url, cts.Token);
            
            if (!response.IsSuccessStatusCode)
            {
                // Try secondary key if primary fails
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("Primary key failed, trying secondary key");
                    url = $"{_apiBaseUrl}?text={Uri.EscapeDataString(searchText)}&limit=10&apiKey={_secondaryApiKey}";
                    response = await _httpClient.GetAsync(url, cts.Token);
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Geoapify API failed: {response.StatusCode}");
                    return new List<BirthPlace>();
                }
            }

            var jsonString = await response.Content.ReadAsStringAsync(cts.Token);
            System.Diagnostics.Debug.WriteLine($"Geoapify Response: {jsonString.Substring(0, Math.Min(200, jsonString.Length))}...");
            
            var apiResponse = JsonSerializer.Deserialize<GeoapifyResponse>(jsonString);

            if (apiResponse == null || apiResponse.features == null || apiResponse.features.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No results from Geoapify API");
                return new List<BirthPlace>();
            }

            // Convert API results to BirthPlace objects
            var results = new List<BirthPlace>();
            foreach (var feature in apiResponse.features)
            {
                if (feature.geometry?.coordinates != null && feature.geometry.coordinates.Length >= 2)
                {
                    var lon = feature.geometry.coordinates[0];
                    var lat = feature.geometry.coordinates[1];
                    
                    var place = new BirthPlace
                    {
                        Name = feature.properties?.name ?? feature.properties?.city ?? "Unknown",
                        TamilName = "", // Geoapify doesn't provide Tamil names
                        Latitude = lat,
                        Longitude = lon,
                        TimeZone = CalculateTimezone(lon),
                        State = feature.properties?.state ?? "",
                        Country = feature.properties?.country ?? ""
                    };
                    
                    results.Add(place);
                }
            }

            return results;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Geoapify search error: {ex.Message}");
            return new List<BirthPlace>();
        }
    }

    /// <summary>
    /// Merges online and local results, removing duplicates
    /// </summary>
    private List<BirthPlace> MergeResults(List<BirthPlace> onlineResults, List<BirthPlace> localResults)
    {
        var merged = new List<BirthPlace>(onlineResults);
        
        foreach (var local in localResults)
        {
            // Check if this local result is already in online results (within 0.01 degree)
            bool isDuplicate = merged.Any(o => 
                o.Name.Equals(local.Name, StringComparison.OrdinalIgnoreCase) &&
                Math.Abs(o.Latitude - local.Latitude) < 0.01 &&
                Math.Abs(o.Longitude - local.Longitude) < 0.01);
            
            if (!isDuplicate)
            {
                merged.Add(local);
            }
        }
        
        return merged.OrderBy(p => p.Name).ToList();
    }

    /// <summary>
    /// Updates XML file with new places found online
    /// Checks for duplicates before adding
    /// </summary>
    private async Task UpdateXmlWithNewPlaces(List<BirthPlace> newPlaces)
    {
        if (_xmlFilePath == null || newPlaces.Count == 0) return;

        try
        {
            await Task.Run(() =>
            {
                var doc = XDocument.Load(_xmlFilePath);
                var root = doc.Root;
                
                if (root == null) return;

                int addedCount = 0;
                foreach (var place in newPlaces)
                {
                    // Check if place already exists (within 0.01 degree precision)
                    var exists = _birthPlaces.Any(p => 
                        p.Name.Equals(place.Name, StringComparison.OrdinalIgnoreCase) &&
                        Math.Abs(p.Latitude - place.Latitude) < 0.01 &&
                        Math.Abs(p.Longitude - place.Longitude) < 0.01);

                    if (!exists)  // ← Only add NEW places
                    {
                        var placeElement = new XElement("Place",
                            new XElement("Name", place.Name),
                            new XElement("TamilName", place.TamilName),
                            new XElement("Latitude", place.Latitude.ToString("F6")),
                            new XElement("Longitude", place.Longitude.ToString("F6")),
                            new XElement("TimeZone", place.TimeZone.ToString("F1")),
                            new XElement("State", place.State),
                            new XElement("Country", place.Country)
                        );
                        
                        root.Add(placeElement);  // ← ADD TO XML
                        _birthPlaces.Add(place); // ← ADD TO MEMORY CACHE
                        addedCount++;
                    }
                }

                if (addedCount > 0)
                {
                    doc.Save(_xmlFilePath);  // ← SAVE XML FILE
                    Debug.WriteLine($"Added {addedCount} new places to XML cache");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating XML: {ex.Message}");
        }
    }

    /// <summary>
    /// Calculates approximate timezone from longitude
    /// </summary>
    private double CalculateTimezone(double longitude)
    {
        // Rough approximation: longitude / 15 = UTC offset
        var offset = Math.Round(longitude / 15);
        
        // Clamp to valid timezone range
        return Math.Max(-12, Math.Min(14, offset));
    }

    /// <summary>
    /// Gets connection status
    /// </summary>
    public bool IsOnline => _isOnline;

    /// <summary>
    /// Gets all birth places
    /// </summary>
    public List<BirthPlace> GetAllPlaces()
    {
        return _birthPlaces;
    }

    /// <summary>
    /// Searches birth places by name (supports partial match) - Local search
    /// </summary>
    public List<BirthPlace> SearchPlaces(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return _birthPlaces;
        }

        var search = searchText.ToLower();
        return _birthPlaces
            .Where(p => p.SearchText.Contains(search))
            .ToList();
    }

    /// <summary>
    /// Finds a birth place by exact name match
    /// </summary>
    public BirthPlace? FindPlaceByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        // Try exact match first
        var place = _birthPlaces.FirstOrDefault(p => 
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        // If not found, try to match the display name
        if (place == null)
        {
            place = _birthPlaces.FirstOrDefault(p => 
                p.DisplayName.StartsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        return place;
    }
}

/// <summary>
/// Geoapify API response model
/// </summary>
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



