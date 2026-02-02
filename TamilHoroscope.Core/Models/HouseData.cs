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
    /// Tamil rasi name
    /// </summary>
    public string TamilRasiName { get; set; } = string.Empty;

    /// <summary>
    /// Lord of the house
    /// </summary>
    public string Lord { get; set; } = string.Empty;

    /// <summary>
    /// Tamil name of the lord
    /// </summary>
    public string TamilLord { get; set; } = string.Empty;

    /// <summary>
    /// Planets located in this house
    /// </summary>
    public List<string> Planets { get; set; } = new();
}
