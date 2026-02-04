using SwissEphNet;
using TamilHoroscope.Core.Data;
using TamilHoroscope.Core.Interfaces;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Core.Utilities;

namespace TamilHoroscope.Core.Calculators;

/// <summary>
/// Main calculator for Panchangam and Horoscope computations
/// </summary>
public class PanchangCalculator : IPanchangCalculator
{
    private readonly string? _ephemerisPath;

    public PanchangCalculator(string? ephemerisPath = null)
    {
        _ephemerisPath = ephemerisPath;
    }

    /// <summary>
    /// Calculate Panchangam for given birth details
    /// </summary>
    public PanchangData CalculatePanchang(BirthDetails birthDetails)
    {
        using var swissEph = new SwissEphemerisHelper(_ephemerisPath);
        
        double julianDay = JulianDay.ToJulianDay(birthDetails.DateTime, birthDetails.TimeZoneOffset);
        
        // Get Sun and Moon positions
        var sunPos = swissEph.GetPlanetPosition(julianDay, SwissEph.SE_SUN);
        var moonPos = swissEph.GetPlanetPosition(julianDay, SwissEph.SE_MOON);
        
        double sunLongitude = sunPos[0];
        double moonLongitude = moonPos[0];
        
        var panchang = new PanchangData
        {
            DateTime = birthDetails.DateTime,
            SunLongitude = sunLongitude,
            MoonLongitude = moonLongitude
        };
        
        // Calculate Tithi
        CalculateTithi(panchang, sunLongitude, moonLongitude);
        
        // Calculate Nakshatra
        CalculateNakshatra(panchang, moonLongitude);
        
        // Calculate Yoga
        CalculateYoga(panchang, sunLongitude, moonLongitude);
        
        // Calculate Karana
        CalculateKarana(panchang, sunLongitude, moonLongitude);
        
        // Calculate Vara (weekday)
        CalculateVara(panchang, birthDetails.DateTime);
        
        // Calculate Tamil month
        CalculateTamilMonth(panchang, sunLongitude);
        
        return panchang;
    }

    /// <summary>
    /// Calculate complete horoscope including chart and Panchangam
    /// </summary>
    public HoroscopeData CalculateHoroscope(BirthDetails birthDetails)
    {
        using var swissEph = new SwissEphemerisHelper(_ephemerisPath);
        
        double julianDay = JulianDay.ToJulianDay(birthDetails.DateTime, birthDetails.TimeZoneOffset);
        
        var horoscope = new HoroscopeData
        {
            BirthDetails = birthDetails,
            Panchang = CalculatePanchang(birthDetails)
        };
        
        // Calculate Houses and Lagna (Ascendant)
        var (cusps, ascmc) = swissEph.CalculateHouses(julianDay, birthDetails.Latitude, birthDetails.Longitude);
        
        horoscope.LagnaLongitude = ascmc[0]; // Ascendant
        horoscope.LagnaRasi = GetRasiNumber(ascmc[0]);
        var lagnaRasiInfo = TamilNames.Rasis[horoscope.LagnaRasi];
        horoscope.LagnaRasiName = lagnaRasiInfo.English;
        horoscope.TamilLagnaRasiName = lagnaRasiInfo.Tamil;
        
        // Calculate planetary positions for Navagraha
        CalculatePlanetaryPositions(horoscope, swissEph, julianDay, cusps);
        
        // Calculate houses with planets
        CalculateHouses(horoscope, cusps);
        
        return horoscope;
    }

    #region Private Helper Methods

    /// <summary>
    /// Calculate Tithi (lunar day) based on Sun-Moon longitude difference
    /// </summary>
    private void CalculateTithi(PanchangData panchang, double sunLongitude, double moonLongitude)
    {
        // Tithi is based on the longitudinal difference between Moon and Sun
        double diff = moonLongitude - sunLongitude;
        if (diff < 0) diff += 360.0;
        
        // Each tithi is 12 degrees
        int tithiNumber = (int)(diff / 12.0) + 1;
        if (tithiNumber > 30) tithiNumber = 30;
        
        panchang.TithiNumber = tithiNumber;
        
        // Determine Paksha (fortnight)
        if (tithiNumber <= 15)
        {
            panchang.Paksha = "Shukla Paksha";
            panchang.TamilPaksha = "வளர்பிறை";
        }
        else
        {
            panchang.Paksha = "Krishna Paksha";
            panchang.TamilPaksha = "தேய்பிறை";
        }
        
        // Get tithi name
        if (TamilNames.Tithis.TryGetValue(tithiNumber, out var tithi))
        {
            panchang.TithiName = tithi.English;
            panchang.TamilTithiName = tithi.Tamil;
        }
    }

    /// <summary>
    /// Calculate Nakshatra (lunar mansion) based on Moon's longitude
    /// </summary>
    private void CalculateNakshatra(PanchangData panchang, double moonLongitude)
    {
        // Each nakshatra is 13°20' (13.333... degrees)
        double nakshatraDegree = 360.0 / 27.0;
        int nakshatraNumber = (int)(moonLongitude / nakshatraDegree) + 1;
        
        if (nakshatraNumber > 27) nakshatraNumber = 27;
        
        panchang.NakshatraNumber = nakshatraNumber;
        
        // Get nakshatra name
        if (TamilNames.Nakshatras.TryGetValue(nakshatraNumber, out var nakshatra))
        {
            panchang.NakshatraName = nakshatra.English;
            panchang.TamilNakshatraName = nakshatra.Tamil;
        }
    }

    /// <summary>
    /// Calculate Yoga based on sum of Sun and Moon longitudes
    /// </summary>
    private void CalculateYoga(PanchangData panchang, double sunLongitude, double moonLongitude)
    {
        // Yoga is based on the sum of Sun and Moon longitudes
        double sum = (sunLongitude + moonLongitude) % 360.0;
        
        // Each yoga is 13°20' (same as nakshatra)
        double yogaDegree = 360.0 / 27.0;
        int yogaNumber = (int)(sum / yogaDegree) + 1;
        
        if (yogaNumber > 27) yogaNumber = 27;
        
        panchang.YogaNumber = yogaNumber;
        
        // Get yoga name
        if (TamilNames.Yogas.TryGetValue(yogaNumber, out var yoga))
        {
            panchang.YogaName = yoga.English;
            panchang.TamilYogaName = yoga.Tamil;
        }
    }

    /// <summary>
    /// Calculate Karana (half-tithi)
    /// </summary>
    private void CalculateKarana(PanchangData panchang, double sunLongitude, double moonLongitude)
    {
        // Each karana is half of tithi (6 degrees)
        double diff = moonLongitude - sunLongitude;
        if (diff < 0) diff += 360.0;
        
        int karanaNumber = (int)(diff / 6.0) + 1;
        
        // Karana repeats in a cycle, simplified mapping
        if (karanaNumber > 60) karanaNumber = karanaNumber % 60;
        
        // Map to the 11 karanas (simplified - first 7 are movable, last 4 are fixed)
        int mappedKarana = ((karanaNumber - 1) % 7) + 1;
        
        panchang.KaranaNumber = mappedKarana;
        
        // Get karana name
        if (TamilNames.Karanas.TryGetValue(mappedKarana, out var karana))
        {
            panchang.KaranaName = karana.English;
            panchang.TamilKaranaName = karana.Tamil;
        }
    }

    /// <summary>
    /// Calculate Vara (weekday)
    /// </summary>
    private void CalculateVara(PanchangData panchang, DateTime dateTime)
    {
        int varaNumber = (int)dateTime.DayOfWeek;
        panchang.VaraNumber = varaNumber;
        
        // Get vara name
        if (TamilNames.Varas.TryGetValue(varaNumber, out var vara))
        {
            panchang.VaraName = vara.English;
            panchang.TamilVaraName = vara.Tamil;
        }
    }

    /// <summary>
    /// Calculate Tamil month based on Sun's longitude
    /// </summary>
    private void CalculateTamilMonth(PanchangData panchang, double sunLongitude)
    {
        // Tamil months are based on Sun's position in zodiac
        // Chithirai starts when Sun enters Aries (0°)
        int monthNumber = (int)(sunLongitude / 30.0) + 1;
        
        if (monthNumber > 12) monthNumber = 12;
        
        if (TamilNames.TamilMonths.TryGetValue(monthNumber, out var tamilMonth))
        {
            panchang.TamilMonth = tamilMonth;
        }
    }

    /// <summary>
    /// Calculate planetary positions for all Navagraha
    /// </summary>
    private void CalculatePlanetaryPositions(HoroscopeData horoscope, SwissEphemerisHelper swissEph, double julianDay, double[] cusps)
    {
        // Define planets to calculate
        var planetIds = new Dictionary<string, int>
        {
            { "Sun", SwissEph.SE_SUN },
            { "Moon", SwissEph.SE_MOON },
            { "Mars", SwissEph.SE_MARS },
            { "Mercury", SwissEph.SE_MERCURY },
            { "Jupiter", SwissEph.SE_JUPITER },
            { "Venus", SwissEph.SE_VENUS },
            { "Saturn", SwissEph.SE_SATURN }
        };

        foreach (var planet in planetIds)
        {
            var pos = swissEph.GetPlanetPosition(julianDay, planet.Value);
            var planetData = CreatePlanetData(planet.Key, pos, cusps, horoscope.LagnaLongitude);
            horoscope.Planets.Add(planetData);
        }

        // Add Rahu and Ketu
        var rahuPos = swissEph.GetRahuPosition(julianDay);
        var rahuData = CreatePlanetData("Rahu", rahuPos, cusps, horoscope.LagnaLongitude);
        horoscope.Planets.Add(rahuData);

        var ketuPos = swissEph.GetKetuPosition(julianDay);
        var ketuData = CreatePlanetData("Ketu", ketuPos, cusps, horoscope.LagnaLongitude);
        horoscope.Planets.Add(ketuData);
    }

    /// <summary>
    /// Create planet data from position
    /// </summary>
    private PlanetData CreatePlanetData(string name, double[] position, double[] cusps, double lagnaLongitude)
    {
        double longitude = position[0];
        double latitude = position[1];
        
        int rasi = GetRasiNumber(longitude);
        var rasiInfo = TamilNames.Rasis[rasi];
        
        int nakshatra = GetNakshatraNumber(longitude);
        var nakshatraInfo = TamilNames.Nakshatras[nakshatra];
        
        int house = GetHouseNumber(longitude, cusps, lagnaLongitude);
        
        bool isRetrograde = position.Length > 3 && position[3] < 0; // Speed < 0 means retrograde
        
        return new PlanetData
        {
            Name = name,
            TamilName = TamilNames.Planets.TryGetValue(name, out var tamilName) ? tamilName : name,
            Longitude = longitude,
            Latitude = latitude,
            Rasi = rasi,
            RasiName = rasiInfo.English,
            TamilRasiName = rasiInfo.Tamil,
            Nakshatra = nakshatra,
            NakshatraName = nakshatraInfo.English,
            TamilNakshatraName = nakshatraInfo.Tamil,
            House = house,
            IsRetrograde = isRetrograde
        };
    }

    /// <summary>
    /// Calculate houses data
    /// </summary>
    private void CalculateHouses(HoroscopeData horoscope, double[] cusps)
    {
        for (int i = 1; i <= 12; i++)
        {
            double cuspLongitude = cusps[i];
            int rasi = GetRasiNumber(cuspLongitude);
            var rasiInfo = TamilNames.Rasis[rasi];
            
            string lord = TamilNames.RasiLords[rasi];
            string tamilLord = TamilNames.Planets.TryGetValue(lord, out var tl) ? tl : lord;
            
            // Find planets in this house
            var planetsInHouse = horoscope.Planets
                .Where(p => p.House == i)
                .Select(p => p.Name)
                .ToList();
            
            var houseData = new HouseData
            {
                HouseNumber = i,
                Cusp = cuspLongitude,
                Rasi = rasi,
                RasiName = rasiInfo.English,
                TamilRasiName = rasiInfo.Tamil,
                Lord = lord,
                TamilLord = tamilLord,
                Planets = planetsInHouse
            };
            
            horoscope.Houses.Add(houseData);
        }
    }

    /// <summary>
    /// Get Rasi (zodiac sign) number from longitude
    /// </summary>
    private int GetRasiNumber(double longitude)
    {
        // Normalize longitude to 0-360
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        // Each rasi is 30 degrees
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
        
        // Each nakshatra is 13°20' (360/27)
        double nakshatraDegree = 360.0 / 27.0;
        return (int)(longitude / nakshatraDegree) + 1;
    }

    /// <summary>
    /// Get house number where a planet is located
    /// </summary>
    private int GetHouseNumber(double planetLongitude, double[] cusps, double lagnaLongitude)
    {
        // Normalize planet longitude
        while (planetLongitude < 0) planetLongitude += 360.0;
        while (planetLongitude >= 360.0) planetLongitude -= 360.0;
        
        // Find which house the planet is in
        for (int i = 1; i <= 12; i++)
        {
            double cuspStart = cusps[i];
            double cuspEnd = i < 12 ? cusps[i + 1] : cusps[1];
            
            // Handle wrap-around at 360°/0°
            if (cuspEnd < cuspStart)
            {
                if (planetLongitude >= cuspStart || planetLongitude < cuspEnd)
                {
                    return i;
                }
            }
            else
            {
                if (planetLongitude >= cuspStart && planetLongitude < cuspEnd)
                {
                    return i;
                }
            }
        }
        
        return 1; // Default to first house if not found
    }

    #endregion
}
