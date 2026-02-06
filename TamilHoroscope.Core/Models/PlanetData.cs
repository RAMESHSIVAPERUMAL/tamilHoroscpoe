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
    /// Daily speed (degrees per day)
    /// Positive = Direct motion, Negative = Retrograde motion
    /// </summary>
    public double Speed { get; set; }

    /// <summary>
    /// Distance from Earth in Astronomical Units (AU)
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Speed in latitude (degrees per day)
    /// </summary>
    public double SpeedInLatitude { get; set; }

    /// <summary>
    /// Speed in distance (AU per day)
    /// </summary>
    public double SpeedInDistance { get; set; }

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

    /// <summary>
    /// Formatted degree position within the rasi (e.g., "23°45'")
    /// </summary>
    public string DegreeFormatted
    {
        get
        {
            double degreeInSign = Longitude % 30.0;
            int degrees = (int)degreeInSign;
            double minutesDecimal = (degreeInSign - degrees) * 60.0;
            int minutes = (int)minutesDecimal;
            return $"{degrees}°{minutes:D2}'";
        }
    }

    /// <summary>
    /// Status display showing retrograde or direct motion
    /// </summary>
    public string StatusDisplay
    {
        get
        {
            return IsRetrograde ? "R" : "D";
        }
    }

    /// <summary>
    /// Full longitude display in degree-minute-second format (e.g., "102°34'15\"")
    /// </summary>
    public string LongitudeFormatted
    {
        get
        {
            int degrees = (int)Longitude;
            double minutesDecimal = (Longitude - degrees) * 60.0;
            int minutes = (int)minutesDecimal;
            double secondsDecimal = (minutesDecimal - minutes) * 60.0;
            int seconds = (int)secondsDecimal;
            return $"{degrees}°{minutes:D2}'{seconds:D2}\"";
        }
    }

    /// <summary>
    /// Speed display in degrees per day (e.g., "13°15'/day" or "-0°45'/day")
    /// </summary>
    public string SpeedDisplay
    {
        get
        {
            int degrees = (int)Math.Abs(Speed);
            double minutesDecimal = (Math.Abs(Speed) - degrees) * 60.0;
            int minutes = (int)minutesDecimal;
            string sign = Speed < 0 ? "-" : "";
            return $"{sign}{degrees}°{minutes:D2}'/day";
        }
    }

    /// <summary>
    /// Nakshatra Pada (quarter) - 1, 2, 3, or 4
    /// </summary>
    public int NakshatraPada
    {
        get
        {
            double nakshatraDegree = 360.0 / 27.0; // 13°20'
            double padaDegree = nakshatraDegree / 4.0; // 3°20' per pada
            
            // Normalize longitude
            double normLongitude = Longitude % 360.0;
            if (normLongitude < 0) normLongitude += 360.0;
            
            // Position within current nakshatra
            double positionInNakshatra = normLongitude % nakshatraDegree;
            
            // Calculate pada (1-4)
            int pada = (int)(positionInNakshatra / padaDegree) + 1;
            if (pada > 4) pada = 4;
            
            return pada;
        }
    }

    /// <summary>
    /// Formatted latitude display in degree-minute format (e.g., "+2°15'" or "-1°30'")
    /// </summary>
    public string LatitudeFormatted
    {
        get
        {
            int degrees = (int)Math.Abs(Latitude);
            double minutesDecimal = (Math.Abs(Latitude) - degrees) * 60.0;
            int minutes = (int)minutesDecimal;
            string sign = Latitude < 0 ? "-" : "+";
            return $"{sign}{degrees}°{minutes:D2}'";
        }
    }

    /// <summary>
    /// Distance display in AU (e.g., "1.02 AU" or "0.95 AU")
    /// </summary>
    public string DistanceFormatted
    {
        get
        {
            return $"{Distance:F6} AU";
        }
    }

    /// <summary>
    /// Speed in latitude display (e.g., "+0°03'/day" or "-0°02'/day")
    /// </summary>
    public string SpeedInLatitudeFormatted
    {
        get
        {
            int degrees = (int)Math.Abs(SpeedInLatitude);
            double minutesDecimal = (Math.Abs(SpeedInLatitude) - degrees) * 60.0;
            int minutes = (int)minutesDecimal;
            string sign = SpeedInLatitude < 0 ? "-" : "+";
            return $"{sign}{degrees}°{minutes:D2}'/day";
        }
    }

    /// <summary>
    /// Speed in distance display (e.g., "+0.000123 AU/day" or "-0.000045 AU/day")
    /// </summary>
    public string SpeedInDistanceFormatted
    {
        get
        {
            string sign = SpeedInDistance < 0 ? "" : "+";
            return $"{sign}{SpeedInDistance:F6} AU/day";
        }
    }
}
