namespace TamilHoroscope.Desktop.Models;

/// <summary>
/// Represents a birth place with location coordinates
/// </summary>
public class BirthPlace
{
    public string Name { get; set; } = string.Empty;
    public string TamilName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double TimeZone { get; set; }
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Display name for UI (includes country/state if available)
    /// </summary>
    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(State) && State != Country)
            {
                return $"{Name} ({TamilName}) - {State}, {Country}";
            }
            else if (!string.IsNullOrWhiteSpace(Country))
            {
                return $"{Name} ({TamilName}) - {Country}";
            }
            return $"{Name} ({TamilName})";
        }
    }

    /// <summary>
    /// Search text for filtering (includes all searchable fields)
    /// </summary>
    public string SearchText => $"{Name} {TamilName} {State} {Country}".ToLower();
}
