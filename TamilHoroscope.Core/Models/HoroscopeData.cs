namespace TamilHoroscope.Core.Models;

/// <summary>
/// Complete horoscope data including chart and Panchangam
/// </summary>
public class HoroscopeData
{
    /// <summary>
    /// Birth details used for calculations
    /// </summary>
    public BirthDetails BirthDetails { get; set; } = new();

    /// <summary>
    /// Panchangam data at time of birth
    /// </summary>
    public PanchangData Panchang { get; set; } = new();

    /// <summary>
    /// Lagna (Ascendant) in degrees
    /// </summary>
    public double LagnaLongitude { get; set; }

    /// <summary>
    /// Lagna rasi number (1-12)
    /// </summary>
    public int LagnaRasi { get; set; }

    /// <summary>
    /// Lagna rasi name in English
    /// </summary>
    public string LagnaRasiName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil lagna rasi name
    /// </summary>
    public string TamilLagnaRasiName { get; set; } = string.Empty;

    /// <summary>
    /// Planetary positions for Navagraha
    /// </summary>
    public List<PlanetData> Planets { get; set; } = new();

    /// <summary>
    /// House (Bhava) data
    /// </summary>
    public List<HouseData> Houses { get; set; } = new();

    /// <summary>
    /// Navamsa chart data (optional, for divisional chart)
    /// </summary>
    public List<PlanetData>? NavamsaPlanets { get; set; }
}
