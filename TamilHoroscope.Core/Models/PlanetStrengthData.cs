using TamilHoroscope.Core.Data;

namespace TamilHoroscope.Core.Models;

/// <summary>
/// Represents the strength (Bala) of a planet in the horoscope
/// Simplified version based on key strength factors
/// </summary>
public class PlanetStrengthData
{
    /// <summary>
    /// Name of the planet
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Language for localized names (Tamil, Telugu, Kannada, Malayalam)
    /// </summary>
    public string Language { get; set; } = "Tamil";

    /// <summary>
    /// Tamil name of the planet (deprecated - use LocalizedName)
    /// </summary>
    [Obsolete("Use LocalizedName property instead")]
    public string TamilName { get; set; } = string.Empty;

    /// <summary>
    /// Localized name of the planet based on Language property
    /// </summary>
    public string LocalizedName => TamilNames.GetPlanetName(Name, Language);

    /// <summary>
    /// Total strength in Rupas (1 Rupa = 60 Virupas)
    /// </summary>
    public double TotalStrength { get; set; }

    /// <summary>
    /// Total strength in Virupas (for detailed display)
    /// </summary>
    public double TotalStrengthVirupas => TotalStrength * 60;

    /// <summary>
    /// Positional strength (Sthana Bala) - based on sign placement
    /// </summary>
    public double PositionalStrength { get; set; }

    /// <summary>
    /// Directional strength (Dig Bala) - based on house placement
    /// </summary>
    public double DirectionalStrength { get; set; }

    /// <summary>
    /// Motional strength (Chesta Bala) - based on speed and retrograde status
    /// </summary>
    public double MotionalStrength { get; set; }

    /// <summary>
    /// Natural strength (Naisargika Bala) - inherent strength of the planet
    /// </summary>
    public double NaturalStrength { get; set; }

    /// <summary>
    /// Temporal strength (Kala Bala) - based on time factors
    /// </summary>
    public double TemporalStrength { get; set; }

    /// <summary>
    /// Aspectual strength (Drik Bala) - based on aspects received
    /// </summary>
    public double AspectualStrength { get; set; }

    /// <summary>
    /// Strength percentage relative to maximum possible strength
    /// </summary>
    public double StrengthPercentage { get; set; }

    /// <summary>
    /// Strength grade (Excellent, Good, Average, Weak, Very Weak)
    /// </summary>
    public string StrengthGrade
    {
        get
        {
            return StrengthPercentage switch
            {
                >= 80 => "Excellent",
                >= 60 => "Good",
                >= 40 => "Average",
                >= 20 => "Weak",
                _ => "Very Weak"
            };
        }
    }

    /// <summary>
    /// Tamil strength grade
    /// </summary>
    public string TamilStrengthGrade
    {
        get
        {
            return StrengthPercentage switch
            {
                >= 80 => "??? ??????",
                >= 60 => "??????",
                >= 40 => "??????",
                >= 20 => "???????",
                _ => "??????? ???????"
            };
        }
    }

    /// <summary>
    /// Required minimum strength for benefic results (in Rupas)
    /// </summary>
    public double RequiredStrength { get; set; }

    /// <summary>
    /// Whether the planet has sufficient strength
    /// </summary>
    public bool HasSufficientStrength => TotalStrength >= RequiredStrength;
}
