# Vimshottari Dasa Implementation

## Overview

Vimshottari Dasa is a system used in Vedic astrology to predict the timing of events in a person's life. It divides a person's life into major periods (Dasas) and sub-periods (Bhuktis), each ruled by one of the nine planets (Navagraha).

## Implementation Details

### Algorithm

The Vimshottari Dasa system is based on the Moon's nakshatra (lunar mansion) at the time of birth. The system operates on a 120-year cycle, with each of the nine planets ruling for a specific duration.

### Planet Durations

| Planet  | Duration (Years) |
|---------|-----------------|
| Ketu    | 7               |
| Venus   | 20              |
| Sun     | 6               |
| Moon    | 10              |
| Mars    | 7               |
| Rahu    | 18              |
| Jupiter | 16              |
| Saturn  | 19              |
| Mercury | 17              |
| **Total** | **120**       |

### Nakshatra to Dasa Lord Mapping

Each of the 27 nakshatras is ruled by a specific planet in the Vimshottari system:

| Nakshatra Range | Dasa Lord |
|----------------|-----------|
| 1, 10, 19      | Ketu      |
| 2, 11, 20      | Venus     |
| 3, 12, 21      | Sun       |
| 4, 13, 22      | Moon      |
| 5, 14, 23      | Mars      |
| 6, 15, 24      | Rahu      |
| 7, 16, 25      | Jupiter   |
| 8, 17, 26      | Saturn    |
| 9, 18, 27      | Mercury   |

### Calculation Process

1. **Determine Starting Dasa Lord**: Based on the Moon's nakshatra at birth
2. **Calculate Balance of First Dasa**: 
   - Each nakshatra spans 13°20' (13.333... degrees)
   - Calculate how much of the nakshatra has been traversed by the Moon
   - Balance = Full Dasa Years × (1 - Fraction Traversed)
3. **Generate Subsequent Dasas**: Follow the sequence in order until reaching the desired number of years
4. **Calculate Bhuktis**: For each Dasa, calculate 9 sub-periods in the same sequence
   - Bhukti Duration = (Dasa Years × Bhukti Lord Years) / 120

### Bhukti Calculation

Each Dasa is further divided into 9 Bhuktis, one for each planet. The Bhukti sequence starts with the Dasa lord itself and follows the same planetary order.

Example: In Venus Dasa, the Bhukti sequence is:
1. Venus-Venus
2. Venus-Sun
3. Venus-Moon
4. Venus-Mars
5. Venus-Rahu
6. Venus-Jupiter
7. Venus-Saturn
8. Venus-Mercury
9. Venus-Ketu

## Usage

### Basic Usage

```csharp
using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;

var birthDetails = new BirthDetails
{
    DateTime = new DateTime(2000, 1, 1, 10, 0, 0),
    Latitude = 13.0827,  // Chennai
    Longitude = 80.2707,
    TimeZoneOffset = 5.5
};

var calculator = new PanchangCalculator();

// Calculate horoscope with Dasa
var horoscope = calculator.CalculateHoroscope(
    birthDetails, 
    includeDasa: true,     // Enable Dasa calculation
    dasaYears: 120         // Calculate for 120 years (full cycle)
);

// Access Dasa data
if (horoscope.VimshottariDasas != null)
{
    foreach (var dasa in horoscope.VimshottariDasas)
    {
        Console.WriteLine($"{dasa.Lord}: {dasa.StartDate:yyyy-MM-dd} to {dasa.EndDate:yyyy-MM-dd}");
        
        // Access Bhuktis
        foreach (var bhukti in dasa.Bhuktis)
        {
            Console.WriteLine($"  {bhukti.Lord}: {bhukti.StartDate:yyyy-MM-dd} to {bhukti.EndDate:yyyy-MM-dd}");
        }
    }
}
```

### Direct Dasa Calculation

```csharp
using TamilHoroscope.Core.Calculators;

var dasaCalculator = new DasaCalculator();

var dasas = dasaCalculator.CalculateVimshottariDasa(
    birthDate: new DateTime(2000, 1, 1),
    moonNakshatra: 10,           // Magha
    moonLongitude: 126.0,        // Degrees
    yearsToCalculate: 120
);
```

## Model Classes

### DasaData

Represents a major Dasa period.

```csharp
public class DasaData
{
    public string Lord { get; set; }              // Planet name (e.g., "Venus")
    public string TamilLord { get; set; }          // Tamil name (e.g., "சுக்கிரன்")
    public DateTime StartDate { get; set; }        // Start date of Dasa
    public DateTime EndDate { get; set; }          // End date of Dasa
    public int DurationYears { get; set; }         // Duration in years
    public List<BhuktiData> Bhuktis { get; set; } // Sub-periods
}
```

### BhuktiData

Represents a sub-period within a Dasa.

```csharp
public class BhuktiData
{
    public string Lord { get; set; }         // Planet name
    public string TamilLord { get; set; }    // Tamil name
    public DateTime StartDate { get; set; }  // Start date of Bhukti
    public DateTime EndDate { get; set; }    // End date of Bhukti
    public double DurationDays { get; set; } // Duration in days
}
```

## Testing

The implementation includes comprehensive unit tests covering:

- Correct starting Dasa lord based on nakshatra
- Proper sequence of Dasas
- Continuity of Dasa periods (no gaps or overlaps)
- Correct number of Bhuktis per Dasa (9)
- Continuity of Bhukti periods
- Bhukti sequence starting with Dasa lord
- Integration with horoscope calculation
- Total duration verification (120 years)

Run tests with:
```bash
dotnet test --filter "FullyQualifiedName~DasaCalculatorTests"
```

## References

The implementation is based on standard Vedic astrology references:

1. **ProKerala Astrological Algorithm**: https://www.prokerala.com/astrology/dasa-bhukti/
2. **Swiss Ephemeris Documentation**: https://www.astro.com/swisseph/
3. Traditional Vedic astrology texts on Vimshottari Dasa system

## Accuracy

The calculations use:
- High-precision Swiss Ephemeris for planetary positions
- Lahiri Ayanamsa (standard for Vedic astrology)
- Standard Vimshottari Dasa algorithm
- Accurate nakshatra determination based on Moon's longitude

The results can be verified against established astrology software and websites like:
- ProKerala.com
- AstroSage.com
- Drikpanchang.com

## Future Enhancements

Potential future improvements:

1. **Other Dasa Systems**: Implement alternative systems like Yogini Dasa, Ashtottari Dasa
2. **Pratyantar Dasa**: Add third-level sub-periods
3. **Dasa Interpretation**: Add textual interpretations for each period
4. **Transit Analysis**: Correlate Dasa periods with current transits
5. **Customizable Output**: Support different date formats and display options
