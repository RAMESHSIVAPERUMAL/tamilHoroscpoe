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
    /// <param name="language">Language for localized names (Tamil, Telugu, Kannada, Malayalam)</param>
    /// <returns>List of Dasa periods with Bhukti sub-periods</returns>
    public List<DasaData> CalculateVimshottariDasa(
        DateTime birthDate,
        int moonNakshatra,
        double moonLongitude,
        int yearsToCalculate = 120,
        string language = "English")
    {
        var dasas = new List<DasaData>();

        // Get the starting Dasa lord based on Moon's nakshatra
        var startingLord = LocalizedWordings.NakshatraDasaLord[moonNakshatra];

        // Calculate the balance of the starting Dasa
        // Each nakshatra spans 13°20' (13.333... degrees)
        // Find how much of the nakshatra has been traversed
        var nakshatraStartLongitude = (moonNakshatra - 1) * 13.333333333333334;
        var traversedInNakshatra = moonLongitude - nakshatraStartLongitude;
        var fractionTraversed = traversedInNakshatra / 13.333333333333334;

        // Calculate balance of starting Dasa in years
        var startingDasaDuration = LocalizedWordings.DasaDurations[startingLord];
        var balanceYears = startingDasaDuration * (1 - fractionTraversed);

        // Current date for tracking Dasa progression
        DateTime currentDate = birthDate;

        // Find the index of starting lord in the sequence
        int startIndex = Array.IndexOf(LocalizedWordings.DasaSequence, startingLord);

        // Calculate total years to generate
        double totalYearsGenerated = 0;

        // First, add the balance of the starting Dasa (with balance Bhuktis)
        if (balanceYears > 0.001) // Only add if significant balance remains
        {
            var firstDasa = CreateBalanceDasa(startingLord, currentDate, balanceYears, startingDasaDuration, fractionTraversed, language);
            dasas.Add(firstDasa);
            currentDate = firstDasa.EndDate;
            totalYearsGenerated += balanceYears;
        }

        // Continue with subsequent Dasas in the cycle
        int currentIndex = (startIndex + 1) % LocalizedWordings.DasaSequence.Length;
        
        while (totalYearsGenerated < yearsToCalculate)
        {
            var lord = LocalizedWordings.DasaSequence[currentIndex];
            var duration = LocalizedWordings.DasaDurations[lord];

            var dasa = CreateDasa(lord, currentDate, duration, language);
            dasas.Add(dasa);

            currentDate = dasa.EndDate;
            totalYearsGenerated += duration;

            // Move to next lord in sequence
            currentIndex = (currentIndex + 1) % LocalizedWordings.DasaSequence.Length;
        }

        return dasas;
    }

    /// <summary>
    /// Create a balance Dasa period with only the remaining Bhukti sub-periods
    /// </summary>
    private DasaData CreateBalanceDasa(string lord, DateTime startDate, double balanceYears, double fullDasaDuration, double fractionTraversed, string language = "English")
    {
        var dasa = new DasaData
        {
            Lord = lord,
            Language = language,
#pragma warning disable CS0618
            TamilLord = LocalizedWordings.Planets[lord],
#pragma warning restore CS0618
            StartDate = startDate,
            EndDate = startDate.AddYears((int)balanceYears).AddDays((balanceYears - (int)balanceYears) * 365.25),
            DurationYears = (int)Math.Ceiling(balanceYears)
        };

        // Calculate only the balance Bhuktis (not all 9)
        dasa.Bhuktis = CalculateBalanceBhuktis(lord, startDate, balanceYears, fullDasaDuration, fractionTraversed, language);

        return dasa;
    }

    /// <summary>
    /// Create a Dasa period with its Bhukti sub-periods
    /// </summary>
    private DasaData CreateDasa(string lord, DateTime startDate, double durationYears, string language = "English")
    {
        var dasa = new DasaData
        {
            Lord = lord,
            Language = language,
#pragma warning disable CS0618
            TamilLord = LocalizedWordings.Planets[lord],
#pragma warning restore CS0618
            StartDate = startDate,
            EndDate = startDate.AddYears((int)durationYears).AddDays((durationYears - (int)durationYears) * 365.25),
            DurationYears = (int)Math.Ceiling(durationYears)
        };

        // Calculate Bhuktis for this Dasa
        dasa.Bhuktis = CalculateBhuktis(lord, startDate, durationYears, language);

        return dasa;
    }

    /// <summary>
    /// Calculate Bhukti (sub-periods) for a given Dasa
    /// </summary>
    private List<BhuktiData> CalculateBhuktis(string dasaLord, DateTime dasaStartDate, double dasaDurationYears, string language = "English")
    {
        var bhuktis = new List<BhuktiData>();

        // Find the starting index of Dasa lord in the sequence
        int dasaLordIndex = Array.IndexOf(LocalizedWordings.DasaSequence, dasaLord);

        // Bhukti sequence starts with the Dasa lord itself
        DateTime currentDate = dasaStartDate;
        
        // Total days in the Dasa period
        double totalDasaDays = dasaDurationYears * 365.25;

        for (int i = 0; i < LocalizedWordings.DasaSequence.Length; i++)
        {
            int bhuktiIndex = (dasaLordIndex + i) % LocalizedWordings.DasaSequence.Length;
            var bhuktiLord = LocalizedWordings.DasaSequence[bhuktiIndex];

            // Bhukti duration is proportional to both Dasa lord and Bhukti lord durations
            // Formula: (Dasa Lord Years × Bhukti Lord Years) / 120
            var bhuktiDurationYears = (dasaDurationYears * LocalizedWordings.DasaDurations[bhuktiLord]) / 120.0;
            var bhuktiDurationDays = bhuktiDurationYears * 365.25;

            var bhukti = new BhuktiData
            {
                Lord = bhuktiLord,
                Language = language,
#pragma warning disable CS0618
                TamilLord = LocalizedWordings.Planets[bhuktiLord],
#pragma warning restore CS0618
                StartDate = currentDate,
                EndDate = currentDate.AddDays(bhuktiDurationDays),
                DurationDays = bhuktiDurationDays
            };

            bhuktis.Add(bhukti);
            currentDate = bhukti.EndDate;
        }

        return bhuktis;
    }

    /// <summary>
    /// Calculate only the balance Bhuktis for a balance Dasa period
    /// This is used for the first Dasa when it's not a full period
    /// </summary>
    private List<BhuktiData> CalculateBalanceBhuktis(string dasaLord, DateTime dasaStartDate, double balanceDasaYears, double fullDasaDuration, double fractionTraversed, string language = "English")
    {
        var bhuktis = new List<BhuktiData>();

        // Find the starting index of Dasa lord in the sequence
        int dasaLordIndex = Array.IndexOf(LocalizedWordings.DasaSequence, dasaLord);

        // Calculate cumulative durations to find which Bhukti to start from
        double cumulativeYears = 0;
        double elapsedYears = fullDasaDuration * fractionTraversed;
        int startingBhuktiIndex = 0;
        double bhuktiBalance = 0;

        // Find which Bhukti we're in based on elapsed time
        for (int i = 0; i < LocalizedWordings.DasaSequence.Length; i++)
        {
            int bhuktiIndex = (dasaLordIndex + i) % LocalizedWordings.DasaSequence.Length;
            var bhuktiLord = LocalizedWordings.DasaSequence[bhuktiIndex];
            
            // Full Bhukti duration in years
            var fullBhuktiDuration = (fullDasaDuration * LocalizedWordings.DasaDurations[bhuktiLord]) / 120.0;
            
            if (cumulativeYears + fullBhuktiDuration > elapsedYears)
            {
                // This is the starting Bhukti
                startingBhuktiIndex = i;
                bhuktiBalance = fullBhuktiDuration - (elapsedYears - cumulativeYears);
                break;
            }
            
            cumulativeYears += fullBhuktiDuration;
        }

        // Now calculate Bhuktis starting from the balance Bhukti
        DateTime currentDate = dasaStartDate;
        bool isFirstBhukti = true;

        for (int i = startingBhuktiIndex; i < LocalizedWordings.DasaSequence.Length; i++)
        {
            int bhuktiIndex = (dasaLordIndex + i) % LocalizedWordings.DasaSequence.Length;
            var bhuktiLord = LocalizedWordings.DasaSequence[bhuktiIndex];

            // For the first Bhukti, use the balance duration
            // For subsequent Bhuktis, calculate based on the balance Dasa proportion
            double bhuktiDurationYears;
            
            if (isFirstBhukti)
            {
                bhuktiDurationYears = bhuktiBalance;
                isFirstBhukti = false;
            }
            else
            {
                // Calculate proportional Bhukti duration based on balance Dasa
                bhuktiDurationYears = (balanceDasaYears * LocalizedWordings.DasaDurations[bhuktiLord]) / fullDasaDuration;
            }
            
            var bhuktiDurationDays = bhuktiDurationYears * 365.25;

            var bhukti = new BhuktiData
            {
                Lord = bhuktiLord,
                Language = language,
#pragma warning disable CS0618
                TamilLord = LocalizedWordings.Planets[bhuktiLord],
#pragma warning restore CS0618
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
