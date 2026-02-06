using System.Text;
using SwissEphNet;

namespace TamilHoroscope.Core.Utilities;

/// <summary>
/// Wrapper for Swiss Ephemeris calculations
/// </summary>
public class SwissEphemerisHelper : IDisposable
{
    private readonly SwissEph _swissEph;
    private bool _disposed;

    public SwissEphemerisHelper(string? ephemerisPath = null)
    {
        _swissEph = new SwissEph();
        
        // Set ephemeris path if provided
        if (!string.IsNullOrEmpty(ephemerisPath))
        {
            _swissEph.swe_set_ephe_path(ephemerisPath);
        }
        
        // Set Lahiri (Chitrapaksha) ayanamsa as standard for Tamil astrology
        _swissEph.swe_set_sid_mode(SwissEph.SE_SIDM_LAHIRI, 0, 0);
    }

    /// <summary>
    /// Get planetary position (longitude, latitude) for a given planet
    /// </summary>
    /// <param name="julianDay">Julian Day Number</param>
    /// <param name="planet">Planet ID (SE_SUN, SE_MOON, etc.)</param>
    /// <param name="flags">Calculation flags</param>
    /// <returns>Array containing longitude, latitude, distance, speed in long, speed in lat, speed in dist</returns>
    public double[] GetPlanetPosition(double julianDay, int planet, int flags = SwissEph.SEFLG_SIDEREAL | SwissEph.SEFLG_SPEED)
    {
        double[] xx = new double[6];
        string serr = string.Empty;
        
        int result = _swissEph.swe_calc_ut(julianDay, planet, flags, xx, ref serr);
        
        if (result < 0)
        {
            throw new Exception($"Swiss Ephemeris calculation error: {serr}");
        }
        
        return xx;
    }

    /// <summary>
    /// Calculate houses and ascendant
    /// </summary>
    /// <param name="julianDay">Julian Day Number</param>
    /// <param name="latitude">Geographic latitude</param>
    /// <param name="longitude">Geographic longitude</param>
    /// <param name="houseSystem">House system ('P' for Placidus, 'W' for Whole Sign, etc.)</param>
    /// <returns>Tuple of (cusps array, ascmc array)</returns>
    public (double[] cusps, double[] ascmc) CalculateHouses(double julianDay, double latitude, double longitude, char houseSystem = 'W')
    {
        double[] cusps = new double[13]; // cusps[1] to cusps[12] are house cusps
        double[] ascmc = new double[10];  // ascmc[0] is Ascendant, ascmc[1] is MC, etc.
        
        int result = _swissEph.swe_houses(julianDay, latitude, longitude, houseSystem, cusps, ascmc);
        
        if (result < 0)
        {
            throw new Exception("Swiss Ephemeris houses calculation error");
        }
        
        return (cusps, ascmc);
    }

    /// <summary>
    /// Get ayanamsa value for a given Julian Day
    /// </summary>
    /// <param name="julianDay">Julian Day Number</param>
    /// <returns>Ayanamsa in degrees</returns>
    public double GetAyanamsa(double julianDay)
    {
        return _swissEph.swe_get_ayanamsa_ut(julianDay);
    }

    /// <summary>
    /// Calculate Rahu (North Node) position
    /// Uses Mean Node as per traditional Vedic/Tamil astrology (Thirukanitha Panchangam)
    /// </summary>
    public double[] GetRahuPosition(double julianDay)
    {
        return GetPlanetPosition(julianDay, SwissEph.SE_MEAN_NODE);
    }

    /// <summary>
    /// Calculate Ketu (South Node) position (opposite of Rahu)
    /// </summary>
    public double[] GetKetuPosition(double julianDay)
    {
        var rahuPos = GetRahuPosition(julianDay);
        rahuPos[0] = (rahuPos[0] + 180.0) % 360.0; // Ketu is 180° opposite to Rahu
        return rahuPos;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _swissEph?.Dispose();
            _disposed = true;
        }
    }
}
