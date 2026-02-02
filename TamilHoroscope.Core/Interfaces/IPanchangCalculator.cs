using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Core.Interfaces;

/// <summary>
/// Interface for Panchangam and Horoscope calculations
/// </summary>
public interface IPanchangCalculator
{
    /// <summary>
    /// Calculate Panchangam for given birth details
    /// </summary>
    /// <param name="birthDetails">Birth details including date, time, and location</param>
    /// <returns>Calculated Panchangam data</returns>
    PanchangData CalculatePanchang(BirthDetails birthDetails);

    /// <summary>
    /// Calculate complete horoscope including chart and Panchangam
    /// </summary>
    /// <param name="birthDetails">Birth details including date, time, and location</param>
    /// <returns>Complete horoscope data</returns>
    HoroscopeData CalculateHoroscope(BirthDetails birthDetails);
}
