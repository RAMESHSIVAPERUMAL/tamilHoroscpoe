using TamilHoroscope.Core.Data;

namespace TamilHoroscope.Core.Models;

/// <summary>
/// House (Bhava) data
/// </summary>
public class HouseData
{
    /// <summary>
    /// House number (1-12)
    /// </summary>
    public int HouseNumber { get; set; }

    /// <summary>
    /// Starting longitude (cusp) of the house in degrees
    /// </summary>
    public double Cusp { get; set; }

    /// <summary>
    /// Rasi (zodiac sign) in the house
    /// </summary>
    public int Rasi { get; set; }

    /// <summary>
    /// Rasi name in English
    /// </summary>
    public string RasiName { get; set; } = string.Empty;

    /// <summary>
    /// Language for localized names (English, Tamil, Telugu, Kannada, Malayalam)
    /// </summary>
    public string Language { get; set; } = "English";

    /// <summary>
    /// Tamil rasi name (deprecated - use LocalizedRasiName)
    /// </summary>
    [Obsolete("Use LocalizedRasiName property instead")]
    public string TamilRasiName { get; set; } = string.Empty;

    /// <summary>
    /// Localized rasi name based on Language property
    /// </summary>
    public string LocalizedRasiName => LocalizedWordings.GetRasiName(Rasi, Language);

    /// <summary>
    /// Lord of the house
    /// </summary>
    public string Lord { get; set; } = string.Empty;

    /// <summary>
    /// Tamil name of the lord (deprecated - use LocalizedLord)
    /// </summary>
    [Obsolete("Use LocalizedLord property instead")]
    public string TamilLord { get; set; } = string.Empty;

    /// <summary>
    /// Localized name of the lord based on Language property
    /// </summary>
    public string LocalizedLord => LocalizedWordings.GetPlanetName(Lord, Language);

    /// <summary>
    /// Planets located in this house
    /// </summary>
    public List<string> Planets { get; set; } = new();
}
