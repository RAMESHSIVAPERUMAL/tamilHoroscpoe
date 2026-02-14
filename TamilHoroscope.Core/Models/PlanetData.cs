using TamilHoroscope.Core.Data;

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
    /// Tamil rasi name (deprecated - use LocalizedRasiName)
    /// </summary>
    [Obsolete("Use LocalizedRasiName property instead")]
    public string TamilRasiName { get; set; } = string.Empty;

    /// <summary>
    /// Localized rasi name based on Language property
    /// </summary>
    public string LocalizedRasiName => TamilNames.GetRasiName(Rasi, Language);

    /// <summary>
    /// Nakshatra number (1-27)
    /// </summary>
    public int Nakshatra { get; set; }

    /// <summary>
    /// Nakshatra name in English
    /// </summary>
    public string NakshatraName { get; set; } = string.Empty;

    /// <summary>
    /// Tamil nakshatra name (deprecated - use LocalizedNakshatraName)
    /// </summary>
    [Obsolete("Use LocalizedNakshatraName property instead")]
    public string TamilNakshatraName { get; set; } = string.Empty;

    /// <summary>
    /// Localized nakshatra name based on Language property
    /// </summary>
    public string LocalizedNakshatraName => TamilNames.GetNakshatraName(Nakshatra, Language);

    /// <summary>
    /// House number (1-12) where the planet is located
    /// </summary>
    public int House { get; set; }

    /// <summary>
    /// Retrograde status
    /// </summary>
    public bool IsRetrograde { get; set; }

    /// <summary>
    /// Combustion status (Astam/Asthangam)
    /// True if the planet is too close to the Sun
    /// </summary>
    public bool IsCombust { get; set; }

    /// <summary>
    /// Angular distance from the Sun in degrees
    /// Used for combustion calculation
    /// </summary>
    public double DistanceFromSun { get; set; }

    /// <summary>
    /// Combustion threshold for this planet (in degrees)
    /// The orb within which the planet is considered combust
    /// </summary>
    public double CombustionThreshold { get; set; }

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
            return (IsRetrograde &&  Name != "Rahu" &&  Name != "Ketu") ? "வக்ரம்" : "";
        }
    }

    /// <summary>
    /// Combustion status display in Tamil
    /// </summary>
    public string CombustionStatusDisplay
    {
        get
        {
            return IsCombust ? "அஸ்தம்" : "";
        }
    }

    /// <summary>
    /// Combined status display showing both retrograde and combustion
    /// </summary>
    public string FullStatusDisplay
    {
        get
        {
            var status = StatusDisplay;
            var combustStatus = CombustionStatusDisplay;
            
            if (!string.IsNullOrEmpty(status) && !string.IsNullOrEmpty(combustStatus))
                return $"{status}, {combustStatus}";
            else if (!string.IsNullOrEmpty(status))
                return status;
            else if (!string.IsNullOrEmpty(combustStatus))
                return combustStatus;
            else
                return "";
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

    /// <summary>
    /// Distance from Sun formatted display (e.g., "5°30'")
    /// Only applicable for planets other than Sun
    /// </summary>
    public string DistanceFromSunFormatted
    {
        get
        {
            if (Name == "Sun") return "N/A";
            
            int degrees = (int)DistanceFromSun;
            double minutesDecimal = (DistanceFromSun - degrees) * 60.0;
            int minutes = (int)minutesDecimal;
            return $"{degrees}°{minutes:D2}'";
        }
    }
}
