namespace TamilHoroscope.Desktop.Models;

/// <summary>
/// Response model from geocoding API
/// </summary>
public class GeocodeResponse
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}
