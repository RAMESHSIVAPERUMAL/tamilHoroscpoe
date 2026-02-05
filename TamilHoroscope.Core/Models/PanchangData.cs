namespace TamilHoroscope.Core.Models;

/// <summary>
/// Panchangam data calculated for a specific date and location
/// </summary>
public class PanchangData
{
    /// <summary>
    /// Date and time for which Panchangam is calculated
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// Tithi (lunar day) number (1-30)
    /// </summary>
    public int TithiNumber { get; set; }

    /// <summary>
    /// Tithi name in English
    /// </summary>
    public string TithiName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil tithi name
    /// </summary>
    public string TamilTithiName { get; set; } = string.Empty;

    /// <summary>
    /// Nakshatra (lunar mansion) number (1-27)
    /// </summary>
    public int NakshatraNumber { get; set; }

    /// <summary>
    /// Nakshatra name in English
    /// </summary>
    public string NakshatraName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil nakshatra name
    /// </summary>
    public string TamilNakshatraName { get; set; } = string.Empty;

    /// <summary>
    /// Yoga number (1-27)
    /// </summary>
    public int YogaNumber { get; set; }

    /// <summary>
    /// Yoga name in English
    /// </summary>
    public string YogaName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil yoga name
    /// </summary>
    public string TamilYogaName { get; set; } = string.Empty;

    /// <summary>
    /// Karana number (1-11)
    /// </summary>
    public int KaranaNumber { get; set; }

    /// <summary>
    /// Karana name in English
    /// </summary>
    public string KaranaName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil karana name
    /// </summary>
    public string TamilKaranaName { get; set; } = string.Empty;

    /// <summary>
    /// Vara (weekday) number (0-6, Sunday=0)
    /// </summary>
    public int VaraNumber { get; set; }

    /// <summary>
    /// Vara name in English
    /// </summary>
    public string VaraName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil vara name
    /// </summary>
    public string TamilVaraName { get; set; } = string.Empty;

    /// <summary>
    /// Paksha (fortnight) - Shukla or Krishna
    /// </summary>
    public string Paksha { get; set; } = string.Empty;

    /// <summary>
    /// Tamil paksha name
    /// </summary>
    public string TamilPaksha { get; set; } = string.Empty;

    /// <summary>
    /// Tamil month name
    /// </summary>
    public string TamilMonth { get; set; } = string.Empty;

    /// <summary>
    /// Sun's longitude in degrees
    /// </summary>
    public double SunLongitude { get; set; }

    /// <summary>
    /// Moon's longitude in degrees
    /// </summary>
    public double MoonLongitude { get; set; }
}
