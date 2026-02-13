using System.Xml.Linq;
using TamilHoroscope.Web.Models;

namespace TamilHoroscope.Web.Services;

/// <summary>
/// Service for managing birth place data with XML cache
/// Similar to Desktop implementation but adapted for web
/// </summary>
public class BirthPlaceService
{
    private readonly List<BirthPlace> _cachedPlaces;
    private readonly ILogger<BirthPlaceService> _logger;
    private readonly string _dataFilePath;

    public BirthPlaceService(IWebHostEnvironment environment, ILogger<BirthPlaceService> logger)
    {
        _logger = logger;
        _dataFilePath = Path.Combine(environment.ContentRootPath, "Data", "BirthPlaces.xml");
        _cachedPlaces = new List<BirthPlace>();
        
        LoadBirthPlaces();
    }

    /// <summary>
    /// Loads birth places from XML file
    /// </summary>
    public void LoadBirthPlaces()
    {
        try
        {
            if (!File.Exists(_dataFilePath))
            {
                _logger.LogWarning("Birth places XML file not found at: {Path}", _dataFilePath);
                LoadDefaultPlaces();
                return;
            }

            var doc = XDocument.Load(_dataFilePath);
            var places = doc.Descendants("Place")
                .Select(p => new BirthPlace
                {
                    Name = p.Element("Name")?.Value ?? string.Empty,
                    Latitude = double.Parse(p.Element("Latitude")?.Value ?? "0"),
                    Longitude = double.Parse(p.Element("Longitude")?.Value ?? "0"),
                    TimeZone = double.Parse(p.Element("TimeZone")?.Value ?? "5.5"),
                    Country = p.Element("Country")?.Value,
                    State = p.Element("State")?.Value
                })
                .ToList();

            _cachedPlaces.Clear();
            _cachedPlaces.AddRange(places);

            _logger.LogInformation("Loaded {Count} birth places from XML", _cachedPlaces.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading birth places from XML");
            LoadDefaultPlaces();
        }
    }

    /// <summary>
    /// Loads default Indian cities if XML file is not available
    /// </summary>
    private void LoadDefaultPlaces()
    {
        _cachedPlaces.Clear();
        _cachedPlaces.AddRange(new[]
        {
            // Tamil Nadu
            new BirthPlace { Name = "Chennai", Latitude = 13.0827, Longitude = 80.2707, TimeZone = 5.5, State = "Tamil Nadu", Country = "India" },
            new BirthPlace { Name = "Coimbatore", Latitude = 11.0168, Longitude = 76.9558, TimeZone = 5.5, State = "Tamil Nadu", Country = "India" },
            new BirthPlace { Name = "Madurai", Latitude = 9.9252, Longitude = 78.1198, TimeZone = 5.5, State = "Tamil Nadu", Country = "India" },
            new BirthPlace { Name = "Trichy", Latitude = 10.7905, Longitude = 78.7047, TimeZone = 5.5, State = "Tamil Nadu", Country = "India" },
            new BirthPlace { Name = "Salem", Latitude = 11.6643, Longitude = 78.1460, TimeZone = 5.5, State = "Tamil Nadu", Country = "India" },
            new BirthPlace { Name = "Tirunelveli", Latitude = 8.7139, Longitude = 77.7567, TimeZone = 5.5, State = "Tamil Nadu", Country = "India" },
            
            // Major Indian Cities
            new BirthPlace { Name = "Mumbai", Latitude = 19.0760, Longitude = 72.8777, TimeZone = 5.5, State = "Maharashtra", Country = "India" },
            new BirthPlace { Name = "Delhi", Latitude = 28.7041, Longitude = 77.1025, TimeZone = 5.5, State = "Delhi", Country = "India" },
            new BirthPlace { Name = "Bangalore", Latitude = 12.9716, Longitude = 77.5946, TimeZone = 5.5, State = "Karnataka", Country = "India" },
            new BirthPlace { Name = "Hyderabad", Latitude = 17.3850, Longitude = 78.4867, TimeZone = 5.5, State = "Telangana", Country = "India" },
            new BirthPlace { Name = "Kolkata", Latitude = 22.5726, Longitude = 88.3639, TimeZone = 5.5, State = "West Bengal", Country = "India" },
            new BirthPlace { Name = "Pune", Latitude = 18.5204, Longitude = 73.8567, TimeZone = 5.5, State = "Maharashtra", Country = "India" },
            new BirthPlace { Name = "Ahmedabad", Latitude = 23.0225, Longitude = 72.5714, TimeZone = 5.5, State = "Gujarat", Country = "India" },
            new BirthPlace { Name = "Jaipur", Latitude = 26.9124, Longitude = 75.7873, TimeZone = 5.5, State = "Rajasthan", Country = "India" },
            
            // Kerala
            new BirthPlace { Name = "Thiruvananthapuram", Latitude = 8.5241, Longitude = 76.9366, TimeZone = 5.5, State = "Kerala", Country = "India" },
            new BirthPlace { Name = "Kochi", Latitude = 9.9312, Longitude = 76.2673, TimeZone = 5.5, State = "Kerala", Country = "India" },
            new BirthPlace { Name = "Kozhikode", Latitude = 11.2588, Longitude = 75.7804, TimeZone = 5.5, State = "Kerala", Country = "India" },
            
            // International
            new BirthPlace { Name = "Singapore", Latitude = 1.3521, Longitude = 103.8198, TimeZone = 8.0, Country = "Singapore" },
            new BirthPlace { Name = "Dubai", Latitude = 25.2048, Longitude = 55.2708, TimeZone = 4.0, Country = "UAE" },
            new BirthPlace { Name = "London", Latitude = 51.5074, Longitude = -0.1278, TimeZone = 0.0, Country = "UK" },
            new BirthPlace { Name = "New York", Latitude = 40.7128, Longitude = -74.0060, TimeZone = -5.0, Country = "USA" }
        });

        _logger.LogInformation("Loaded {Count} default birth places", _cachedPlaces.Count);
    }

    /// <summary>
    /// Searches places by name (local cache only)
    /// </summary>
    public List<BirthPlace> SearchPlaces(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return new List<BirthPlace>();

        var searchLower = searchText.ToLowerInvariant();

        return _cachedPlaces
            .Where(p => p.Name.ToLowerInvariant().Contains(searchLower) ||
                       (p.State?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                       (p.Country?.ToLowerInvariant().Contains(searchLower) ?? false))
            .OrderBy(p => p.Name.ToLowerInvariant().StartsWith(searchLower) ? 0 : 1)
            .ThenBy(p => p.Name)
            .Take(20)
            .ToList();
    }

    /// <summary>
    /// Gets all cached places
    /// </summary>
    public List<BirthPlace> GetAllPlaces() => _cachedPlaces.ToList();

    /// <summary>
    /// Gets place by exact name match
    /// </summary>
    public BirthPlace? GetPlaceByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return _cachedPlaces
            .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
