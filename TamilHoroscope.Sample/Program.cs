using System.Text.Json;
using System.Text.Json.Serialization;
using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Sample;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("Tamil Horoscope Calculation Engine - Sample Output");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        var calculator = new PanchangCalculator();

        Console.WriteLine("Sample 1: Chennai, January 1, 2024, 10:00 AM");
        var chennai = new BirthDetails
        {
            DateTime = new DateTime(2024, 1, 1, 10, 0, 0),
            Latitude = 13.0827,
            Longitude = 80.2707,
            TimeZoneOffset = 5.5,
            PlaceName = "Chennai"
        };
        DisplayHoroscope(calculator, chennai);

        Console.WriteLine();
        Console.WriteLine(new string('-', 80));
        Console.WriteLine();

        Console.WriteLine("Sample 2: Madurai, March 15, 2024, 2:30 PM");
        var madurai = new BirthDetails
        {
            DateTime = new DateTime(2024, 3, 15, 14, 30, 0),
            Latitude = 9.9252,
            Longitude = 78.1198,
            TimeZoneOffset = 5.5,
            PlaceName = "Madurai"
        };
        DisplayHoroscope(calculator, madurai);

        Console.WriteLine();
        Console.WriteLine(new string('-', 80));
        Console.WriteLine();

        Console.WriteLine("Sample 3: Coimbatore, June 21, 2024, 8:15 AM");
        var coimbatore = new BirthDetails
        {
            DateTime = new DateTime(2024, 6, 21, 8, 15, 0),
            Latitude = 11.0168,
            Longitude = 76.9558,
            TimeZoneOffset = 5.5,
            PlaceName = "Coimbatore"
        };
        DisplayHoroscope(calculator, coimbatore);

        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("Sample calculations completed successfully!");
        Console.WriteLine("=".PadRight(80, '='));
    }

    static void DisplayHoroscope(PanchangCalculator calculator, BirthDetails birthDetails)
    {
        var horoscope = calculator.CalculateHoroscope(birthDetails, includeDasa: true, dasaYears: 120);

        Console.WriteLine($"Location: {birthDetails.PlaceName}");
        Console.WriteLine($"Date/Time: {birthDetails.DateTime:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Coordinates: {birthDetails.Latitude:F4}°N, {birthDetails.Longitude:F4}°E");
        Console.WriteLine();

        Console.WriteLine("PANCHANGAM:");
        Console.WriteLine($"  Tamil Month: {horoscope.Panchang.TamilMonth}");
        Console.WriteLine($"  Vara (Weekday): {horoscope.Panchang.VaraName} ({horoscope.Panchang.TamilVaraName})");
        Console.WriteLine($"  Tithi: {horoscope.Panchang.TithiName} ({horoscope.Panchang.TamilTithiName})");
        Console.WriteLine($"  Paksha: {horoscope.Panchang.Paksha} ({horoscope.Panchang.TamilPaksha})");
        Console.WriteLine($"  Nakshatra: {horoscope.Panchang.NakshatraName} ({horoscope.Panchang.TamilNakshatraName})");
        Console.WriteLine($"  Yoga: {horoscope.Panchang.YogaName} ({horoscope.Panchang.TamilYogaName})");
        Console.WriteLine($"  Karana: {horoscope.Panchang.KaranaName} ({horoscope.Panchang.TamilKaranaName})");
        Console.WriteLine();

        Console.WriteLine("LAGNA (ASCENDANT):");
        Console.WriteLine($"  Rasi: {horoscope.LagnaRasiName} ({horoscope.TamilLagnaRasiName})");
        Console.WriteLine($"  Longitude: {horoscope.LagnaLongitude:F2}°");
        Console.WriteLine();

        Console.WriteLine("NAVAGRAHA POSITIONS:");
        Console.WriteLine($"  {"Planet",-12} {"Tamil Name",-15} {"Rasi",-15} {"Tamil Rasi",-15} {"Nakshatra",-18} {"House",-6} {"Retrograde"}");
        Console.WriteLine($"  {new string('-', 115)}");
        
        foreach (var planet in horoscope.Planets)
        {
            var retrograde = planet.IsRetrograde ? "Yes" : "No";
            Console.WriteLine($"  {planet.Name,-12} {planet.TamilName,-15} {planet.RasiName,-15} {planet.TamilRasiName,-15} {planet.NakshatraName,-18} {planet.House,-6} {retrograde}");
        }
        Console.WriteLine();

        Console.WriteLine("HOUSES (BHAVAS):");
        Console.WriteLine($"  {"House",-8} {"Rasi",-15} {"Tamil Rasi",-15} {"Lord",-10} {"Planets"}");
        Console.WriteLine($"  {new string('-', 80)}");
        
        foreach (var house in horoscope.Houses)
        {
            var planets = house.Planets.Count > 0 ? string.Join(", ", house.Planets) : "-";
            Console.WriteLine($"  {house.HouseNumber,-8} {house.RasiName,-15} {house.TamilRasiName,-15} {house.Lord,-10} {planets}");
        }
        Console.WriteLine();

        // Display Vimshottari Dasa if available
        if (horoscope.VimshottariDasas != null && horoscope.VimshottariDasas.Count > 0)
        {
            Console.WriteLine("VIMSHOTTARI DASA (Major Periods):");
            Console.WriteLine($"  {"Dasa Lord",-15} {"Tamil Lord",-15} {"Start Date",-15} {"End Date",-15} {"Years",-6}");
            Console.WriteLine($"  {new string('-', 80)}");
            
            foreach (var dasa in horoscope.VimshottariDasas.Take(10)) // Show first 10 dasas
            {
                Console.WriteLine($"  {dasa.Lord,-15} {dasa.TamilLord,-15} {dasa.StartDate:yyyy-MM-dd}  {dasa.EndDate:yyyy-MM-dd}  {dasa.DurationYears,-6}");
            }
            Console.WriteLine();

            // Display Bhuktis for the current Dasa
            var currentDasa = horoscope.VimshottariDasas.FirstOrDefault(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);
            if (currentDasa != null)
            {
                Console.WriteLine($"CURRENT DASA - {currentDasa.Lord} Bhukti (Sub-Periods):");
                Console.WriteLine($"  {"Bhukti Lord",-15} {"Tamil Lord",-15} {"Start Date",-15} {"End Date",-15} {"Days",-8}");
                Console.WriteLine($"  {new string('-', 80)}");
                
                foreach (var bhukti in currentDasa.Bhuktis)
                {
                    Console.WriteLine($"  {bhukti.Lord,-15} {bhukti.TamilLord,-15} {bhukti.StartDate:yyyy-MM-dd}  {bhukti.EndDate:yyyy-MM-dd}  {bhukti.DurationDays,-8:F1}");
                }
                Console.WriteLine();
            }
            else
            {
                // Show bhuktis for the first dasa as an example
                var firstDasa = horoscope.VimshottariDasas[0];
                Console.WriteLine($"FIRST DASA - {firstDasa.Lord} Bhukti (Sub-Periods):");
                Console.WriteLine($"  {"Bhukti Lord",-15} {"Tamil Lord",-15} {"Start Date",-15} {"End Date",-15} {"Days",-8}");
                Console.WriteLine($"  {new string('-', 80)}");
                
                foreach (var bhukti in firstDasa.Bhuktis)
                {
                    Console.WriteLine($"  {bhukti.Lord,-15} {bhukti.TamilLord,-15} {bhukti.StartDate:yyyy-MM-dd}  {bhukti.EndDate:yyyy-MM-dd}  {bhukti.DurationDays,-8:F1}");
                }
                Console.WriteLine();
            }
        }
    }
}
