# Online/Offline Birth Place Picker - Implementation Guide

## Overview
The Tamil Horoscope Calculator now features an intelligent birth place picker that automatically searches online when internet is available and seamlessly falls back to local XML data when offline.

## Architecture

### Components

1. **BirthPlaceService.cs** - Core service handling online/offline operations
   - Online geocoding via OpenStreetMap Nominatim API
   - Local XML file fallback
   - Automatic synchronization of new places to XML
   - Internet connectivity detection

2. **MainWindow.xaml.cs** - UI integration
   - Async auto-complete search
   - Real-time filtering
   - Graceful degradation on network errors

3. **BirthPlaces.xml** - Local database
   - Pre-populated with 40+ cities
   - Automatically updated with online searches
   - Offline-first architecture

## How It Works

### Online Mode (Internet Available)

```
User types "Paris" ? 
  ?
Check internet connection ? Online ?
  ?
Query Nominatim API: 
  https://nominatim.openstreetmap.org/search?q=Paris&format=json
  ?
Receive results:
  - Paris, France (48.8566, 2.3522)
  - Paris, Texas, USA (33.6609, -95.5555)
  - Paris, Ontario, Canada (43.1984, -80.3839)
  ?
Convert to BirthPlace objects
  ?
Merge with local XML data
  ?
Save new places to XML (for future offline use)
  ?
Display combined results in dropdown
  ?
User selects ? Auto-fill coordinates & timezone
```

### Offline Mode (No Internet)

```
User types "Paris" ?
  ?
Check internet connection ? Offline ?
  ?
Search local XML database
  ?
Return matching places from cache
  ?
Display results in dropdown
  ?
User selects ? Auto-fill coordinates & timezone
```

## API Integration

### Using OpenStreetMap Nominatim API

**Endpoint:** `https://nominatim.openstreetmap.org/search`

**Parameters:**
- `q` - Search query (e.g., "Chennai", "New York")
- `format=json` - Response format
- `addressdetails=1` - Include detailed address information
- `limit=10` - Maximum number of results

**Example Request:**
```
GET https://nominatim.openstreetmap.org/search?q=Chennai&format=json&addressdetails=1&limit=10
```

**Example Response:**
```json
[
  {
    "place_id": "282912316",
    "lat": "13.0836939",
    "lon": "80.270186",
    "display_name": "Chennai, Tamil Nadu, India",
    "name": "Chennai",
    "address": {
      "city": "Chennai",
      "state": "Tamil Nadu",
      "country": "India",
      "country_code": "in"
    }
  }
]
```

### Usage Policy

OpenStreetMap Nominatim has a **fair use policy**:
- **Maximum 1 request per second**
- Must provide a valid User-Agent header
- Free for light usage
- Consider hosting your own Nominatim instance for heavy usage

### Alternative APIs

You can easily switch to other geocoding APIs by modifying `BirthPlaceService.cs`:

#### 1. Google Maps Geocoding API (Paid)
```csharp
private const string GeocodingApiUrl = "https://maps.googleapis.com/maps/api/geocode/json";
// Usage: {url}?address={query}&key={YOUR_API_KEY}
```

#### 2. Mapbox Geocoding API (Free tier available)
```csharp
private const string GeocodingApiUrl = "https://api.mapbox.com/geocoding/v5/mapbox.places";
// Usage: {url}/{query}.json?access_token={YOUR_TOKEN}
```

#### 3. Azure Maps Search API
```csharp
private const string GeocodingApiUrl = "https://atlas.microsoft.com/search/address/json";
// Usage: {url}?api-version=1.0&query={query}&subscription-key={YOUR_KEY}
```

## Features

### 1. **Automatic Internet Detection**
```csharp
private async void CheckInternetConnection()
{
    try
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var response = await _httpClient.GetAsync("https://www.google.com", cts.Token);
        _isOnline = response.IsSuccessStatusCode;
    }
    catch
    {
        _isOnline = false;
    }
}
```

### 2. **Smart Search Merging**
- Searches online API first
- Merges with local XML results
- Removes duplicates based on name and coordinates
- Prioritizes online results (usually more comprehensive)

### 3. **Automatic XML Update**
New places found online are automatically saved to the local XML file:
```csharp
private async Task UpdateXmlWithNewPlaces(List<BirthPlace> newPlaces)
{
    // Checks for duplicates
    // Adds new places to XML
    // Maintains sorted order
}
```

### 4. **Timezone Calculation**
Automatically calculates timezone from longitude:
```csharp
private double CalculateTimezone(double longitude)
{
    // Approximation: longitude / 15 = UTC offset
    var offset = Math.Round(longitude / 15);
    return offset;
}
```

### 5. **Graceful Fallback**
All operations have error handling that falls back to local search:
```csharp
try
{
    // Try online search
    if (_birthPlaceService.IsOnline)
    {
        var filteredPlaces = await _birthPlaceService.SearchPlacesOnlineAsync(searchText);
        cmbBirthPlace.ItemsSource = filteredPlaces;
    }
}
catch
{
    // Fallback to local
    var filteredPlaces = _birthPlaceService.SearchPlaces(searchText);
    cmbBirthPlace.ItemsSource = filteredPlaces;
}
```

## Configuration

### Timeout Settings

```csharp
// HTTP client timeout for API requests
private static readonly HttpClient _httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(10)
};

// Request-specific timeout using CancellationToken
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var response = await _httpClient.GetAsync(url, cts.Token);
```

### Search Limits

```csharp
// Number of results from online API
private const string GeocodingApiUrl = "https://nominatim.openstreetmap.org/search";
// Add &limit=10 to URL for 10 results
```

## Benefits

### For Users
1. **Rich Search Results** - Access to worldwide location database
2. **Always Works** - Offline fallback ensures continuous operation
3. **Self-Improving** - Database grows with usage
4. **Fast** - Cached results for previously searched locations
5. **Accurate** - Up-to-date coordinates from online sources

### For Developers
1. **Simple Integration** - Works with any HTTP-based geocoding API
2. **Resilient** - Multiple layers of fallback
3. **Observable** - Status displayed in UI (Online/Offline mode)
4. **Extensible** - Easy to add more data providers
5. **Testable** - Can be tested offline and online

## Performance Considerations

### Throttling
```csharp
// Nominatim: Max 1 request per second
// Consider adding delay for high-frequency searches
await Task.Delay(1000); // Wait 1 second between requests
```

### Caching Strategy
- First search: Online API + XML merge ? Save to XML
- Subsequent searches: XML only (cached)
- Benefit: Reduces API calls and improves speed

### Async Operations
All network operations are async to prevent UI freezing:
```csharp
private async void CmbBirthPlace_TextChanged(object sender, TextChangedEventArgs e)
{
    var filteredPlaces = await _birthPlaceService.SearchPlacesOnlineAsync(searchText);
    // UI remains responsive during network call
}
```

## Testing

### Test Scenarios

1. **Online Mode**
   - Search for "Tokyo"
   - Verify API is called
   - Verify results appear
   - Verify XML is updated

2. **Offline Mode**
   - Disconnect internet
   - Search for "Chennai"
   - Verify local results appear
   - Verify no errors

3. **API Timeout**
   - Slow network simulation
   - Verify timeout occurs gracefully
   - Verify fallback to local

4. **Invalid Search**
   - Search for "asdfghjkl"
   - Verify no results found
   - Verify no crash

## Troubleshooting

### Issue: "Could not load birth places"
**Solution:** Ensure `BirthPlaces.xml` is in the `Data` folder and copied to output directory

### Issue: Online search not working
**Check:**
1. Internet connection
2. Firewall settings
3. API endpoint availability (test in browser)
4. User-Agent header (required by Nominatim)

### Issue: Slow performance
**Solutions:**
1. Reduce API timeout (currently 5 seconds)
2. Limit number of results
3. Add local caching layer
4. Implement debouncing for search input

### Issue: Duplicate entries in XML
**Solution:** The service checks for duplicates before adding. If duplicates exist, manually clean the XML file.

## Security Considerations

1. **API Keys** - If using paid APIs, store keys securely (use Azure Key Vault or environment variables)
2. **User Privacy** - Location searches are not logged or transmitted except to geocoding API
3. **HTTPS Only** - All API calls use HTTPS for encrypted communication
4. **Input Validation** - Search text is URL-encoded to prevent injection

## Future Enhancements

1. **Rate Limiting** - Implement request throttling to comply with API policies
2. **Multiple Providers** - Try fallback APIs if primary fails
3. **Location History** - Remember recently searched locations
4. **Favorites** - Allow users to mark favorite locations
5. **Batch Import** - Import locations from CSV/Excel
6. **Map Integration** - Visual map picker for coordinates
7. **Reverse Geocoding** - Convert coordinates to place names
8. **Time Zone Database** - Use accurate timezone data with DST support

## Code Example: Adding Google Maps API

```csharp
// In BirthPlaceService.cs
private const string GeocodingApiUrl = "https://maps.googleapis.com/maps/api/geocode/json";
private const string ApiKey = "YOUR_API_KEY_HERE"; // Store securely!

public async Task<List<BirthPlace>> SearchPlacesOnlineAsync(string searchText)
{
    try
    {
        var url = $"{GeocodingApiUrl}?address={Uri.EscapeDataString(searchText)}&key={ApiKey}";
        var response = await _httpClient.GetAsync(url);
        var jsonString = await response.Content.ReadAsStringAsync();
        
        // Parse Google Maps JSON response
        var googleResult = JsonSerializer.Deserialize<GoogleMapsResponse>(jsonString);
        
        // Convert to BirthPlace objects
        var places = googleResult.results.Select(r => new BirthPlace
        {
            Name = r.formatted_address,
            Latitude = r.geometry.location.lat,
            Longitude = r.geometry.location.lng,
            // ... extract other fields
        }).ToList();
        
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
- Data © OpenStreetMap contributors
- Attribution required: https://www.openstreetmap.org/copyright
- Consider displaying: "Powered by OpenStreetMap" in your UI

## Support

For issues or questions:
1. Check the troubleshooting section
2. Verify internet connectivity
3. Test API endpoint in browser
4. Check application logs
5. Contact development team

---

**Version:** 1.0  
**Last Updated:** 2024  
**Requires:** .NET 8.0, Internet connection (optional)
