using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Xml.Linq;
using TamilHoroscope.Desktop.Models;

namespace TamilHoroscope.Desktop.Services;

/// <summary>
/// Service to load and search birth places from XML data with online API fallback
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

    // Configuration for geocoding API - You can use OpenStreetMap Nominatim (free) or Google Maps API
    // OpenStreetMap Nominatim: https://nominatim.openstreetmap.org/search?q={query}&format=json&addressdetails=1&limit=5
    // Note: Nominatim has usage policy - max 1 request per second
    private const string GeocodingApiUrl = "https://nominatim.openstreetmap.org/search";
    
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
                throw new FileNotFoundException($"Could not find {DataFileName} in any expected location.");
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
    /// Searches places online using geocoding API if internet is available
    /// </summary>
    public async Task<List<BirthPlace>> SearchPlacesOnlineAsync(string searchText)
    {
        if (!_isOnline || string.IsNullOrWhiteSpace(searchText))
        {
            return SearchPlaces(searchText);
        }

        try
        {
            // Using OpenStreetMap Nominatim API (free but has usage limits)
            // Format: https://nominatim.openstreetmap.org/search?q={query}&format=json&addressdetails=1&limit=10
            var url = $"{GeocodingApiUrl}?q={Uri.EscapeDataString(searchText)}&format=json&addressdetails=1&limit=10";
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "TamilHoroscopeCalculator/1.0");

            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await _httpClient.GetAsync(url, cts.Token);
            
            if (!response.IsSuccessStatusCode)
            {
                // Fallback to local search
                return SearchPlaces(searchText);
            }

            var jsonString = await response.Content.ReadAsStringAsync(cts.Token);
            var results = JsonSerializer.Deserialize<List<NominatimResult>>(jsonString);

            if (results == null || results.Count == 0)
            {
                return SearchPlaces(searchText);
            }

            // Convert API results to BirthPlace and merge with local data
            var onlinePlaces = results.Select(r => new BirthPlace
            {
                Name = r.display_name?.Split(',')[0].Trim() ?? r.name ?? "Unknown",
                TamilName = "", // API doesn't provide Tamil names
                Latitude = double.Parse(r.lat ?? "0"),
                Longitude = double.Parse(r.lon ?? "0"),
                TimeZone = CalculateTimezone(double.Parse(r.lon ?? "0")),
                State = r.address?.state ?? "",
                Country = r.address?.country ?? ""
            }).ToList();

            // Update XML with new places (optional - save for offline use)
            await UpdateXmlWithNewPlaces(onlinePlaces);

            // Combine online results with local results
            var localResults = SearchPlaces(searchText);
            var combinedResults = onlinePlaces
                .Concat(localResults)
                .GroupBy(p => $"{p.Name}_{p.Latitude:F2}_{p.Longitude:F2}")
                .Select(g => g.First())
                .OrderBy(p => p.Name)
                .ToList();

            return combinedResults;
        }
        catch (Exception)
        {
            // Fallback to local search on any error
            return SearchPlaces(searchText);
        }
    }

    /// <summary>
    /// Updates XML file with new places found online
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

                foreach (var place in newPlaces)
                {
                    // Check if place already exists
                    var exists = _birthPlaces.Any(p => 
                        p.Name.Equals(place.Name, StringComparison.OrdinalIgnoreCase) &&
                        Math.Abs(p.Latitude - place.Latitude) < 0.01 &&
                        Math.Abs(p.Longitude - place.Longitude) < 0.01);

                    if (!exists)
                    {
                        var placeElement = new XElement("Place",
                            new XElement("Name", place.Name),
                            new XElement("TamilName", place.TamilName),
                            new XElement("Latitude", place.Latitude.ToString("F4")),
                            new XElement("Longitude", place.Longitude.ToString("F4")),
                            new XElement("TimeZone", place.TimeZone.ToString("F1")),
                            new XElement("State", place.State),
                            new XElement("Country", place.Country)
                        );
                        
                        root.Add(placeElement);
                        _birthPlaces.Add(place);
                    }
                }

                doc.Save(_xmlFilePath);
                
                // Resort the list
                _birthPlaces = _birthPlaces.OrderBy(p => p.Name).ToList();
            });
        }
        catch
        {
            // Silently fail if we can't update the XML
        }
    }

    /// <summary>
    /// Calculates approximate timezone from longitude
    /// </summary>
    private double CalculateTimezone(double longitude)
    {
        // Rough approximation: longitude / 15 = UTC offset
        var offset = Math.Round(longitude / 15);
        return offset;
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
/// Nominatim API response model
/// </summary>
internal class NominatimResult
{
    public string? place_id { get; set; }
    public string? licence { get; set; }
    public string? osm_type { get; set; }
    public string? osm_id { get; set; }
    public string? lat { get; set; }
    public string? lon { get; set; }
    public string? display_name { get; set; }
    public string? name { get; set; }
    public NominatimAddress? address { get; set; }
}

internal class NominatimAddress
{
    public string? city { get; set; }
    public string? town { get; set; }
    public string? village { get; set; }
    public string? state { get; set; }
    public string? country { get; set; }
    public string? country_code { get; set; }
}

