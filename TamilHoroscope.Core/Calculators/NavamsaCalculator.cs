using TamilHoroscope.Core.Data;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Core.Calculators;

/// <summary>
/// Calculator for Navamsa (D-9) divisional chart
/// Navamsa is the most important divisional chart in Vedic astrology, 
/// especially for analyzing marriage, dharma, and inner strength.
/// </summary>
public class NavamsaCalculator
{
    /// <summary>
    /// Calculate Navamsa position for a planet
    /// </summary>
    /// <param name="longitude">Tropical/Sidereal longitude of the planet (0-360°)</param>
    /// <returns>Navamsa longitude (0-360°)</returns>
    public double CalculateNavamsaPosition(double longitude)
    {
        // Normalize longitude to 0-360 range
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        // Get the sign (rasi) number (0-11)
        int rasi = (int)(longitude / 30.0);
        
        // Position within the sign (0-30)
        double positionInSign = longitude - (rasi * 30.0);
        
        // Each sign is divided into 9 parts (Navamsa)
        // Each Navamsa part is 3°20' = 3.333...° = 10/3°
        double navamsaPartSize = 30.0 / 9.0; // 3.333...°
        
        // Which Navamsa part within the sign (0-8)
        int navamsaPart = (int)(positionInSign / navamsaPartSize);
        
        // Navamsa rasi calculation based on the natal rasi and navamsa part
        // Formula: Starting Navamsa sign depends on the natal sign's element
        // - Aries (Fire), Leo (Fire), Sagittarius (Fire): Start from Aries
        // - Taurus (Earth), Virgo (Earth), Capricorn (Earth): Start from Capricorn
        // - Gemini (Air), Libra (Air), Aquarius (Air): Start from Libra
        // - Cancer (Water), Scorpio (Water), Pisces (Water): Start from Cancer
        
        int startingNavamsaSign = GetNavamsaStartSign(rasi);
        
        // Calculate the Navamsa rasi
        int navamsaRasi = (startingNavamsaSign + navamsaPart) % 12;
        
        // Position within the Navamsa part
        double positionInNavamsaPart = positionInSign - (navamsaPart * navamsaPartSize);
        
        // Scale the position proportionally to get position in Navamsa sign
        // The Navamsa part (3.333°) maps to a full sign (30°) in Navamsa chart
        double navamsaPositionInSign = (positionInNavamsaPart / navamsaPartSize) * 30.0;
        
        // Final Navamsa longitude
        double navamsaLongitude = (navamsaRasi * 30.0) + navamsaPositionInSign;
        
        return navamsaLongitude;
    }
    
    /// <summary>
    /// Calculate Navamsa chart for all planets
    /// </summary>
    /// <param name="planets">List of planet data from natal chart</param>
    /// <param name="language">Language for localized names (Tamil, Telugu, Kannada, Malayalam)</param>
    /// <returns>List of planet data with Navamsa positions</returns>
    public List<PlanetData> CalculateNavamsaChart(List<PlanetData> planets, string language = "Tamil")
    {
        var navamsaPlanets = new List<PlanetData>();
        
        foreach (var planet in planets)
        {
            var navamsaLongitude = CalculateNavamsaPosition(planet.Longitude);
            
            // Create new planet data for Navamsa
            var navamsaPlanet = new PlanetData
            {
                Name = planet.Name,
                Language = language, // Set language for dynamic localization
#pragma warning disable CS0618
                TamilName = planet.TamilName,
#pragma warning restore CS0618
                Longitude = navamsaLongitude,
                Latitude = planet.Latitude, // Latitude remains same
                Rasi = GetRasiNumber(navamsaLongitude),
                Nakshatra = GetNakshatraNumber(navamsaLongitude),
                IsRetrograde = planet.IsRetrograde // Retrograde status remains same
            };
            
            // Set Rasi names
            var rasiInfo = TamilNames.Rasis[navamsaPlanet.Rasi];
            navamsaPlanet.RasiName = rasiInfo.English;
#pragma warning disable CS0618
            navamsaPlanet.TamilRasiName = rasiInfo.Tamil;
#pragma warning restore CS0618
            
            // Set Nakshatra names
            var nakshatraInfo = TamilNames.Nakshatras[navamsaPlanet.Nakshatra];
            navamsaPlanet.NakshatraName = nakshatraInfo.English;
#pragma warning disable CS0618
            navamsaPlanet.TamilNakshatraName = nakshatraInfo.Tamil;
#pragma warning restore CS0618
            
            // Note: House is not calculated for Navamsa as it's based on Navamsa Lagna
            // which would need to be calculated separately
            navamsaPlanet.House = 0; // Not applicable for basic Navamsa
            
            navamsaPlanets.Add(navamsaPlanet);
        }
        
        return navamsaPlanets;
    }
    
    /// <summary>
    /// Get the starting Navamsa sign based on the natal sign
    /// </summary>
    /// <param name="rasiIndex">Rasi index (0-11, where 0=Aries)</param>
    /// <returns>Starting Navamsa sign index (0-11)</returns>
    private int GetNavamsaStartSign(int rasiIndex)
    {
        // Signs are grouped by element (Triplicity):
        // Fire signs (0, 4, 8): Aries, Leo, Sagittarius → Start from Aries (0)
        // Earth signs (1, 5, 9): Taurus, Virgo, Capricorn → Start from Capricorn (9)
        // Air signs (2, 6, 10): Gemini, Libra, Aquarius → Start from Libra (6)
        // Water signs (3, 7, 11): Cancer, Scorpio, Pisces → Start from Cancer (3)
        
        int element = rasiIndex % 4;
        
        return element switch
        {
            0 => 0,  // Fire → Aries
            1 => 9,  // Earth → Capricorn
            2 => 6,  // Air → Libra
            3 => 3,  // Water → Cancer
            _ => 0
        };
    }
    
    /// <summary>
    /// Get Rasi (zodiac sign) number from longitude
    /// </summary>
    private int GetRasiNumber(double longitude)
    {
        // Normalize longitude to 0-360
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        // Each rasi is 30 degrees, returns 1-12
        return (int)(longitude / 30.0) + 1;
    }
    
    /// <summary>
    /// Get Nakshatra number from longitude
    /// </summary>
    private int GetNakshatraNumber(double longitude)
    {
        // Normalize longitude to 0-360
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        // Each nakshatra is 13°20' (360/27), returns 1-27
        double nakshatraDegree = 360.0 / 27.0;
        return (int)(longitude / nakshatraDegree) + 1;
    }
}
