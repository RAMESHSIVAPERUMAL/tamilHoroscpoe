# Quick Reference: Online/Offline Birth Place Picker

## Features at a Glance

✅ **Online Search** - Search worldwide locations via OpenStreetMap API  
✅ **Offline Fallback** - Works without internet using local XML database  
✅ **Auto-Complete** - Real-time filtering as you type  
✅ **Auto-Fill** - Automatically fills latitude, longitude, timezone  
✅ **Self-Learning** - Saves new places to local database  
✅ **Smart Merge** - Combines online and offline results  
✅ **Error Handling** - Graceful degradation on network errors  
✅ **Status Indicator** - Shows Online/Offline mode in UI  

## User Guide

### Search for a Birth Place

1. **Click** on the Birth Place dropdown
2. **Type** any part of a city name (e.g., "Chen", "Paris", "Tokyo")
3. **Watch** as results appear in real-time
4. **Select** your desired location
5. **Done!** - Coordinates and timezone auto-filled

### Online Mode Indicator
Look for status message at bottom:
- `"Loaded 40 birth places successfully. Mode: Online"` ✓
  → Internet available, using API + local
- `"Loaded 40 birth places successfully. Mode: Offline"` ⊘
  → No internet, using local database only

### What Gets Auto-Filled
When you select a place:
- ✓ Latitude (e.g., 13.0827)
- ✓ Longitude (e.g., 80.2707)
- ✓ Timezone offset (e.g., 5.5 for IST)

## Developer Quick Start

### How to Use in Code

```csharp
// Initialize
var birthPlaceService = new BirthPlaceService();
birthPlaceService.LoadBirthPlaces();

// Check if online
bool isOnline = birthPlaceService.IsOnline;

// Search online (with offline fallback)
var results = await birthPlaceService.SearchPlacesOnlineAsync("Chennai");

// Search offline only
var localResults = birthPlaceService.SearchPlaces("Chennai");

// Get all cached places
var allPlaces = birthPlaceService.GetAllPlaces();
```

### Key Methods

| Method | Description | Return Type |
|--------|-------------|-------------|
| `LoadBirthPlaces()` | Loads XML + checks internet | void |
| `SearchPlacesOnlineAsync(string)` | Online search with fallback | `Task<List<BirthPlace>>` |
| `SearchPlaces(string)` | Local search only | `List<BirthPlace>` |
| `GetAllPlaces()` | Get all cached places | `List<BirthPlace>` |
| `IsOnline` | Check internet status | bool |

## API Configuration

### Current: OpenStreetMap Nominatim (FREE)

```
Endpoint: https://nominatim.openstreetmap.org/search
Rate Limit: 1 request/second
Cost: Free
Attribution Required: Yes
```

### Switch to Google Maps API

1. Get API key from: https://console.cloud.google.com
2. In `BirthPlaceService.cs`, change:

```csharp
private const string GeocodingApiUrl = 
    "https://maps.googleapis.com/maps/api/geocode/json";

// In SearchPlacesOnlineAsync method:
var url = $"{GeocodingApiUrl}?address={searchText}&key={YOUR_API_KEY}";
```

### Switch to Mapbox API

```csharp
private const string GeocodingApiUrl = 
    "https://api.mapbox.com/geocoding/v5/mapbox.places";
    
var url = $"{GeocodingApiUrl}/{searchText}.json?access_token={YOUR_TOKEN}";
```

## XML Structure

### Adding Places Manually

Edit `TamilHoroscope.Desktop/Data/BirthPlaces.xml`:

```xml
<Place>
  <Name>Your City</Name>
  <TamilName>உங்கள் நகரம்</TamilName>
  <Latitude>12.3456</Latitude>
  <Longitude>78.9012</Longitude>
  <TimeZone>5.5</TimeZone>
  <State>Your State</State>
  <Country>Your Country</Country>
</Place>
```

## Troubleshooting Quick Fixes

| Problem | Quick Fix |
|---------|-----------|
| No search results | Check internet, verify API endpoint |
| Slow performance | Reduce timeout in `BirthPlaceService.cs` |
| XML not updating | Check file permissions, run as admin |
| Duplicates | Manually edit XML to remove |
| API errors | Check User-Agent header is set |

## Performance Tips

### Optimize Search Speed
```csharp
// Add debounce delay (wait for user to finish typing)
private Timer _searchTimer = new Timer(300); // 300ms delay

_searchTimer.Tick += async (s, e) => 
{
    _searchTimer.Stop();
    await PerformSearch();
};

// On text change:
_searchTimer.Stop();
_searchTimer.Start();
```

### Reduce API Calls
```csharp
// Cache recent searches
private Dictionary<string, List<BirthPlace>> _searchCache = new();

if (_searchCache.ContainsKey(searchText))
{
    return _searchCache[searchText];
}
```

## Status Messages

| Message | Meaning |
|---------|---------|
| "Loaded X birth places successfully. Mode: Online" | Connected to internet |
| "Loaded X birth places successfully. Mode: Offline" | No internet, using cache |
| "Warning: Could not load birth places" | XML file missing or corrupt |

## Testing Checklist

- [ ] Test online search (connect to internet)
- [ ] Test offline search (disconnect internet)
- [ ] Test invalid city names
- [ ] Test special characters (தமிழ், etc.)
- [ ] Test auto-fill after selection
- [ ] Test XML update after online search
- [ ] Test with slow network (simulate)
- [ ] Test with API timeout

## Code Locations

```
Project Structure:
├── TamilHoroscope.Desktop/
│   ├── Data/
│   │   └── BirthPlaces.xml           # Local database
│   ├── Models/
│   │   ├── BirthPlace.cs             # Place model
│   │   └── GeocodeResponse.cs        # API response model
│   ├── Services/
│   │   └── BirthPlaceService.cs      # Core logic
│   ├── MainWindow.xaml               # UI definition
│   └── MainWindow.xaml.cs            # UI code-behind
```

## Common Customizations

### Change Number of Results
```csharp
// In BirthPlaceService.cs, line ~127
var url = $"{GeocodingApiUrl}?q={searchText}&format=json&limit=20";
//                                                              ^^^ Change here
```

### Change Timeout Duration
```csharp
// In BirthPlaceService.cs, line ~18
private static readonly HttpClient _httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(15) // Change from 10 to 15
};
```

### Disable Online Search
```csharp
// In MainWindow.xaml.cs, CmbBirthPlace_TextChanged method
// Comment out online search, use only local:
var filteredPlaces = _birthPlaceService.SearchPlaces(searchText);
cmbBirthPlace.ItemsSource = filteredPlaces;
```

## Best Practices

1. ✅ **Always handle exceptions** - Network can fail anytime
2. ✅ **Use async/await** - Don't block UI thread
3. ✅ **Respect API limits** - Add delays between requests
4. ✅ **Cache results** - Reduce redundant API calls
5. ✅ **Validate input** - URL-encode search text
6. ✅ **Show feedback** - Display loading indicators
7. ✅ **Test offline** - Ensure fallback works
8. ✅ **Attribute sources** - Credit OpenStreetMap

## Support Links

- OpenStreetMap Nominatim: https://nominatim.org/
- Usage Policy: https://operations.osmfoundation.org/policies/nominatim/
- Google Maps API: https://developers.google.com/maps/documentation/geocoding
- Mapbox API: https://docs.mapbox.com/api/search/geocoding/

---

**Need Help?** Check the full documentation in `OnlineOfflineBirthPlacePicker.md`
