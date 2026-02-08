# Birth Place Picker - Implementation Guide

## Overview
The birth place picker intelligently searches online when internet is available and falls back to a local XML database when offline.

## Architecture

### Components
- **BirthPlaceService.cs** - Core service for online/offline operations
- **MainWindow.xaml.cs** - UI integration
- **BirthPlaces.xml** - Local cache database

### How It Works

**Online Mode:**
1. User types city name
2. App checks internet connection
3. Query OpenStreetMap Nominatim API
4. Merge with local XML results
5. Save new places to XML for future offline use
6. Display combined results

**Offline Mode:**
1. User types city name
2. Search local XML database
3. Return matching places
4. Display results

## API Integration

### OpenStreetMap Nominatim
- **Endpoint:** https://nominatim.openstreetmap.org/search
- **Rate Limit:** 1 request per second
- **Cost:** Free
- **Attribution:** Required

### Example Request/Response

Request:
```
GET https://nominatim.openstreetmap.org/search?q=Chennai&format=json&addressdetails=1&limit=10
```

Response:
```json
[
  {
    "lat": "13.0836939",
    "lon": "80.270186",
    "display_name": "Chennai, Tamil Nadu, India",
    "address": {
      "city": "Chennai",
      "state": "Tamil Nadu",
      "country": "India"
    }
  }
]
```

## Features

### Automatic Internet Detection
The service checks for internet connectivity and automatically switches modes.

### Smart Search Merging
- Searches online API first
- Merges with local database
- Removes duplicates
- Prioritizes online results

### Automatic XML Update
New places found online are automatically saved to the local database.

### Timezone Calculation
Automatically calculates timezone from longitude (longitude / 15 = UTC offset).

### Graceful Fallback
All operations have error handling that falls back to local search.

## Configuration

### Timeout Settings
```csharp
// HTTP client timeout
private static readonly HttpClient _httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(10)
};

// Request-specific timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var response = await _httpClient.GetAsync(url, cts.Token);
```

### Search Limits
```csharp
// Number of results from API
var url = $"{GeocodingApiUrl}?q={searchText}&format=json&limit=10";
```

## Switching to Other APIs

### Google Maps API
```csharp
private const string GeocodingApiUrl = "https://maps.googleapis.com/maps/api/geocode/json";
// Usage: {url}?address={query}&key={YOUR_API_KEY}
```

### Mapbox API
```csharp
private const string GeocodingApiUrl = "https://api.mapbox.com/geocoding/v5/mapbox.places";
// Usage: {url}/{query}.json?access_token={YOUR_TOKEN}
```

### Azure Maps API
```csharp
private const string GeocodingApiUrl = "https://atlas.microsoft.com/search/address/json";
// Usage: {url}?api-version=1.0&query={query}&subscription-key={YOUR_KEY}
```

## Performance

### Throttling
Nominatim allows 1 request per second. Add delays if needed:
```csharp
await Task.Delay(1000); // Wait between requests
```

### Caching Strategy
- First search: Online API + XML merge ? Save to XML
- Later searches: Use cached XML (faster)

### Async Operations
All network calls are async to prevent UI freezing.

## Testing

### Test Scenarios
1. **Online Mode:** Search for a city, verify API call, check results appear
2. **Offline Mode:** Disconnect internet, search local results
3. **Timeout:** Simulate slow network, verify graceful fallback
4. **Invalid Input:** Search non-existent city, verify no crash

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Could not load birth places" | Ensure BirthPlaces.xml exists in Data folder |
| Online search not working | Check internet, firewall, API endpoint availability |
| Slow performance | Reduce API timeout or limit results |
| Duplicate entries | Clean up BirthPlaces.xml manually |

## Security

1. **API Keys:** Store securely using Azure Key Vault or environment variables
2. **User Privacy:** Searches only sent to geocoding API, not logged
3. **HTTPS Only:** All API calls use encrypted connection
4. **Input Validation:** Search text is URL-encoded

## Benefits

### For Users
- Worldwide location search
- Works offline
- Auto-filling coordinates and timezone
- Self-improving database with usage

### For Developers
- Simple API integration
- Multiple fallback layers
- Observable online/offline status
- Easy to extend or switch providers
- Fully testable

## Future Enhancements

1. Multiple API providers with automatic fallback
2. Search history and favorites
3. Interactive map picker
4. Batch import from CSV/Excel
5. Reverse geocoding (coordinates ? place names)
6. Accurate timezone with DST support
7. Request rate limiting
8. Local caching layer for improved performance

## Code Example: Using with Google Maps

```csharp
private const string GeocodingApiUrl = "https://maps.googleapis.com/maps/api/geocode/json";
private const string ApiKey = "YOUR_API_KEY"; // Store securely!

public async Task<List<BirthPlace>> SearchPlacesOnlineAsync(string searchText)
{
    try
    {
        var url = $"{GeocodingApiUrl}?address={Uri.EscapeDataString(searchText)}&key={ApiKey}";
        var response = await _httpClient.GetAsync(url);
        var jsonString = await response.Content.ReadAsStringAsync();
        
        // Parse Google Maps JSON and convert to BirthPlace objects
        var places = ParseGoogleMapsResponse(jsonString);
        
        return places;
    }
    catch
    {
        return SearchPlaces(searchText);
    }
}
```

## License & Attribution

When using OpenStreetMap Nominatim:
- Data: OpenStreetMap contributors
- Attribution: https://www.openstreetmap.org/copyright
- Display: "Powered by OpenStreetMap" in UI

## References

- OpenStreetMap Nominatim: https://nominatim.org/
- Usage Policy: https://operations.osmfoundation.org/policies/nominatim/
- Google Maps Geocoding: https://developers.google.com/maps/documentation/geocoding
- Mapbox Geocoding: https://docs.mapbox.com/api/search/geocoding/
- Azure Maps Search: https://docs.microsoft.com/en-us/azure/azure-maps/

---

**Version:** 1.0  
**Last Updated:** 2024  
**Requires:** .NET 8.0, Optional internet connection
