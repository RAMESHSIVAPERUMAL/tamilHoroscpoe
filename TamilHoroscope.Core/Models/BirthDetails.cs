namespace TamilHoroscope.Core.Models;

/// <summary>
/// Birth details required for horoscope and Panchangam calculations
/// </summary>
public class BirthDetails
{
    /// <summary>
    /// Date and time of birth (local time)
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// Latitude of birthplace in decimal degrees (positive for North, negative for South)
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude of birthplace in decimal degrees (positive for East, negative for West)
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Time zone offset from UTC in hours (e.g., +5.5 for IST)
    /// </summary>
    public double TimeZoneOffset { get; set; }

    /// <summary>
    /// Name of the birthplace (optional)
    /// </summary>
    public string? PlaceName { get; set; }
}
