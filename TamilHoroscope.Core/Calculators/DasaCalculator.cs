using TamilHoroscope.Core.Data;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Core.Calculators;

/// <summary>
/// Calculator for Vimshottari Dasa and Bhukti periods
/// </summary>
public class DasaCalculator
{
    /// <summary>
    /// Calculate Vimshottari Dasa periods based on Moon's nakshatra at birth
    /// </summary>
    /// <param name="birthDate">Date and time of birth</param>
    /// <param name="moonNakshatra">Moon's nakshatra number (1-27)</param>
    /// <param name="moonLongitude">Moon's longitude in degrees</param>
    /// <param name="yearsToCalculate">Number of years of Dasa periods to calculate (default: 120)</param>
    /// <returns>List of Dasa periods with Bhukti sub-periods</returns>
    public List<DasaData> CalculateVimshottariDasa(
        DateTime birthDate,
        int moonNakshatra,
        double moonLongitude,
        int yearsToCalculate = 120)
    {
        var dasas = new List<DasaData>();

        // Get the starting Dasa lord based on Moon's nakshatra
        var startingLord = TamilNames.NakshatraDasaLord[moonNakshatra];

        // Calculate the balance of the starting Dasa
        // Each nakshatra spans 13°20' (13.333... degrees)
        // Find how much of the nakshatra has been traversed
        var nakshatraStartLongitude = (moonNakshatra - 1) * 13.333333333333334;
        var traversedInNakshatra = moonLongitude - nakshatraStartLongitude;
        var fractionTraversed = traversedInNakshatra / 13.333333333333334;

        // Calculate balance of starting Dasa in years
        var startingDasaDuration = TamilNames.DasaDurations[startingLord];
        var balanceYears = startingDasaDuration * (1 - fractionTraversed);

        // Current date for tracking Dasa progression
        DateTime currentDate = birthDate;

        // Find the index of starting lord in the sequence
        int startIndex = Array.IndexOf(TamilNames.DasaSequence, startingLord);

        // Calculate total years to generate
        double totalYearsGenerated = 0;

        // First, add the balance of the starting Dasa
        if (balanceYears > 0.001) // Only add if significant balance remains
        {
            var firstDasa = CreateDasa(startingLord, currentDate, balanceYears);
            dasas.Add(firstDasa);
            currentDate = firstDasa.EndDate;
            totalYearsGenerated += balanceYears;
        }

        // Continue with subsequent Dasas in the cycle
        int currentIndex = (startIndex + 1) % TamilNames.DasaSequence.Length;
        
        while (totalYearsGenerated < yearsToCalculate)
        {
            var lord = TamilNames.DasaSequence[currentIndex];
            var duration = TamilNames.DasaDurations[lord];

            var dasa = CreateDasa(lord, currentDate, duration);
            dasas.Add(dasa);

            currentDate = dasa.EndDate;
            totalYearsGenerated += duration;

            // Move to next lord in sequence
            currentIndex = (currentIndex + 1) % TamilNames.DasaSequence.Length;
        }

        return dasas;
    }

    /// <summary>
    /// Create a Dasa period with its Bhukti sub-periods
    /// </summary>
    private DasaData CreateDasa(string lord, DateTime startDate, double durationYears)
    {
        var dasa = new DasaData
        {
            Lord = lord,
            TamilLord = TamilNames.Planets[lord],
            StartDate = startDate,
            EndDate = startDate.AddYears((int)durationYears).AddDays((durationYears - (int)durationYears) * 365.25),
            DurationYears = (int)Math.Ceiling(durationYears)
        };

        // Calculate Bhuktis for this Dasa
        dasa.Bhuktis = CalculateBhuktis(lord, startDate, durationYears);

        return dasa;
    }

    /// <summary>
    /// Calculate Bhukti (sub-periods) for a given Dasa
    /// </summary>
    private List<BhuktiData> CalculateBhuktis(string dasaLord, DateTime dasaStartDate, double dasaDurationYears)
    {
        var bhuktis = new List<BhuktiData>();

        // Find the starting index of Dasa lord in the sequence
        int dasaLordIndex = Array.IndexOf(TamilNames.DasaSequence, dasaLord);

        // Bhukti sequence starts with the Dasa lord itself
        DateTime currentDate = dasaStartDate;
        
        // Total days in the Dasa period
        double totalDasaDays = dasaDurationYears * 365.25;

        for (int i = 0; i < TamilNames.DasaSequence.Length; i++)
        {
            int bhuktiIndex = (dasaLordIndex + i) % TamilNames.DasaSequence.Length;
            var bhuktiLord = TamilNames.DasaSequence[bhuktiIndex];

            // Bhukti duration is proportional to both Dasa lord and Bhukti lord durations
            // Formula: (Dasa Lord Years × Bhukti Lord Years) / 120
            var bhuktiDurationYears = (dasaDurationYears * TamilNames.DasaDurations[bhuktiLord]) / 120.0;
            var bhuktiDurationDays = bhuktiDurationYears * 365.25;

            var bhukti = new BhuktiData
            {
                Lord = bhuktiLord,
                TamilLord = TamilNames.Planets[bhuktiLord],
                StartDate = currentDate,
                EndDate = currentDate.AddDays(bhuktiDurationDays),
                DurationDays = bhuktiDurationDays
            };

            bhuktis.Add(bhukti);
            currentDate = bhukti.EndDate;
        }

        return bhuktis;
    }
}
