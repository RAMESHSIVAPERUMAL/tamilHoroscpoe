namespace TamilHoroscope.Web.Models;

/// <summary>
/// Represents a birth place with geographical coordinates
/// </summary>
public class BirthPlace
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double TimeZone { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    
    /// <summary>
    /// Display name with location hierarchy
    /// </summary>
    public string DisplayName => 
        !string.IsNullOrEmpty(State) ? $"{Name}, {State}" : 
        !string.IsNullOrEmpty(Country) ? $"{Name}, {Country}" : Name;
}
