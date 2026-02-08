# Quick Reference: Birth Place Picker

## Features
- Online search via OpenStreetMap API
- Offline fallback with local XML database
- Real-time auto-complete
- Auto-fill latitude, longitude, and timezone
- Saves new places for future offline use

## User Guide

### Searching for a Birth Place
1. Click the Birth Place field
2. Type a city name (e.g., "Chennai", "Paris")
3. Select your location from results
4. Coordinates and timezone auto-fill

### Online/Offline Status
- "Mode: Online" - Using internet and API
- "Mode: Offline" - Using local database only

The app automatically switches based on internet availability.

## Developer Guide

### Using BirthPlaceService

```csharp
var service = new BirthPlaceService();
service.LoadBirthPlaces();

// Search with online fallback
var results = await service.SearchPlacesOnlineAsync("Chennai");

// Search local only
var localResults = service.SearchPlaces("Chennai");

// Check online status
if (service.IsOnline)
{
    // Connected to internet
}
```

### Key Methods
- `LoadBirthPlaces()` - Load XML and check internet
- `SearchPlacesOnlineAsync(string)` - Search online with offline fallback
- `SearchPlaces(string)` - Search local database only
- `GetAllPlaces()` - Get all cached places
- `IsOnline` - Check internet connection status

## API Configuration

### Current API: OpenStreetMap Nominatim
- Free tier
- Rate limit: 1 request/second
- Endpoint: https://nominatim.openstreetmap.org/search

### Switch to Google Maps API

```csharp
// In BirthPlaceService.cs
private const string GeocodingApiUrl = 
    "https://maps.googleapis.com/maps/api/geocode/json";
var url = $"{GeocodingApiUrl}?address={Uri.EscapeDataString(searchText)}&key={YOUR_API_KEY}";
```

### Switch to Mapbox API

```csharp
private const string GeocodingApiUrl = 
    "https://api.mapbox.com/geocoding/v5/mapbox.places";
var url = $"{GeocodingApiUrl}/{Uri.EscapeDataString(searchText)}.json?access_token={YOUR_TOKEN}";
```

## Adding Places Manually

Edit `TamilHoroscope.Desktop/Data/BirthPlaces.xml`:

```xml
<Place>
  <Name>City Name</Name>
  <TamilName>????? ?????</TamilName>
  <Latitude>12.3456</Latitude>
  <Longitude>78.9012</Longitude>
  <TimeZone>5.5</TimeZone>
  <State>State Name</State>
  <Country>Country Name</Country>
</Place>
```

## Troubleshooting

| Problem | Solution |
|---------|----------|
| No search results | Check internet connection or API endpoint |
| Slow search | Verify network speed and API response time |
| XML not updating | Check file permissions, run as admin |
| Duplicate entries | Manually clean BirthPlaces.xml |
| API errors | Verify User-Agent header is set |

## Performance Tips

### Add Search Debounce
```csharp
private Timer _searchTimer = new Timer(300);
_searchTimer.Tick += async (s, e) => 
{
    _searchTimer.Stop();
    await PerformSearch();
};
```

### Cache Recent Searches
```csharp
private Dictionary<string, List<BirthPlace>> _cache = new();
if (_cache.ContainsKey(searchText))
    return _cache[searchText];
```

## Code Locations

- BirthPlaceService: `Services/BirthPlaceService.cs`
- Birth Place Model: `Models/BirthPlace.cs`
- API Response Model: `Models/GeocodeResponse.cs`
- Local Database: `Data/BirthPlaces.xml`
- UI Integration: `MainWindow.xaml.cs`

## Testing Checklist

- [ ] Online search with internet
- [ ] Offline search without internet
- [ ] Invalid city names return empty
- [ ] Auto-fill coordinates after selection
- [ ] XML updates after online search
- [ ] No duplicates in results
- [ ] Graceful handling of API timeout
- [ ] Tamil character search works

## Best Practices

1. Always handle exceptions - network can fail anytime
2. Use async/await to keep UI responsive
3. Respect API rate limits
4. Cache results to reduce API calls
5. Validate and URL-encode search input
6. Show loading indicators during search
7. Test both online and offline scenarios
8. Credit API sources (e.g., OpenStreetMap)

## External Resources

- OpenStreetMap Nominatim: https://nominatim.org/
- Usage Policy: https://operations.osmfoundation.org/policies/nominatim/
- Google Maps API: https://developers.google.com/maps/documentation/geocoding
- Mapbox API: https://docs.mapbox.com/api/search/geocoding/
