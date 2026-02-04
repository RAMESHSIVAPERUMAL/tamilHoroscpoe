namespace TamilHoroscope.Core.Models;

/// <summary>
/// Planet position and related data
/// </summary>
public class PlanetData
{
    /// <summary>
    /// Name of the planet (in English)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tamil name of the planet
    /// </summary>
    public string TamilName { get; set; } = string.Empty;

    /// <summary>
    /// Ecliptic longitude in degrees (0-360)
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Ecliptic latitude in degrees
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Rasi (zodiac sign) number (1-12)
    /// </summary>
    public int Rasi { get; set; }

    /// <summary>
    /// Rasi name in English
    /// </summary>
    public string RasiName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil rasi name
    /// </summary>
    public string TamilRasiName { get; set; } = string.Empty;

    /// <summary>
    /// Nakshatra number (1-27)
    /// </summary>
    public int Nakshatra { get; set; }

    /// <summary>
    /// Nakshatra name in English
    /// </summary>
    public string NakshatraName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil nakshatra name
    /// </summary>
    public string TamilNakshatraName { get; set; } = string.Empty;

    /// <summary>
    /// House number (1-12) where the planet is located
    /// </summary>
    public int House { get; set; }

    /// <summary>
    /// Retrograde status
    /// </summary>
    public bool IsRetrograde { get; set; }
}
