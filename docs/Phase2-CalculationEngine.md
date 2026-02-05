# Phase 2: Calculation Engine Implementation

## Overview
This document describes the implementation of the Tamil Horoscope Calculation Engine for the Tamil Thirukanitha Panachangam desktop application.

## Architecture

### Project Structure
```
TamilHoroscope.sln
├── TamilHoroscope.Core/          # Core calculation library
│   ├── Models/                    # Data models
│   ├── Interfaces/                # Interfaces
│   ├── Calculators/              # Calculation implementations
│   ├── Utilities/                # Helper utilities
│   └── Data/                     # Tamil names and lookup tables
├── TamilHoroscope.Tests/         # Unit tests
└── TamilHoroscope.Sample/        # Sample console application
```

## Components

### 1. Models

#### BirthDetails
Contains all information needed for calculations:
- DateTime (local time)
- Latitude/Longitude (decimal degrees)
- TimeZoneOffset (hours from UTC)
- PlaceName (optional)

#### PanchangData
Complete Panchangam information:
- Tithi (lunar day)
- Nakshatra (lunar mansion)
- Yoga
- Karana
- Vara (weekday)
- Paksha (fortnight)
- Tamil month
- Sun/Moon longitudes

#### HoroscopeData
Complete horoscope including:
- Birth details
- Panchangam data
- Lagna (Ascendant)
- Planetary positions (Navagraha)
- Houses (12 Bhavas)

#### PlanetData
Detailed planet information:
- Name (English and Tamil)
- Ecliptic longitude/latitude
- Rasi (zodiac sign)
- Nakshatra
- House placement
- Retrograde status

#### HouseData
House (Bhava) information:
- House number (1-12)
- Cusp longitude
- Rasi in the house
- Lord of the house
- Planets in the house

### 2. Interfaces

#### IPanchangCalculator
Main interface for calculations:
- `CalculatePanchang(BirthDetails)`: Returns PanchangData
- `CalculateHoroscope(BirthDetails)`: Returns HoroscopeData

### 3. Calculators

#### PanchangCalculator
Main implementation of IPanchangCalculator:

**Panchangam Calculations:**
- **Tithi**: Based on longitudinal difference between Moon and Sun (each tithi = 12°)
- **Nakshatra**: Based on Moon's ecliptic longitude (each nakshatra = 13°20')
- **Yoga**: Sum of Sun and Moon longitudes divided by 13°20'
- **Karana**: Half-tithi (each karana = 6°)
- **Vara**: Weekday from local date/time
- **Tamil Month**: Based on Sun's zodiac position

**Horoscope Calculations:**
- **Lagna**: Calculated using Swiss Ephemeris houses() function
- **Planetary Positions**: All 9 Navagraha (Sun, Moon, Mars, Mercury, Jupiter, Venus, Saturn, Rahu, Ketu)
- **Houses**: 12 houses with cusps, rasis, lords, and planet placements
- **Rasi Assignment**: Based on planetary longitude (each rasi = 30°)

### 4. Utilities

#### JulianDay
Utilities for Julian Day conversion:
- `ToJulianDay(DateTime, timeZoneOffset)`: Convert DateTime to Julian Day
- `FromJulianDay(julianDay)`: Convert Julian Day to DateTime

#### SwissEphemerisHelper
Wrapper for Swiss Ephemeris library:
- Planetary position calculations
- Houses and Ascendant calculation
- Lahiri (Chitrapaksha) ayanamsa configuration
- Rahu/Ketu position calculations

### 5. Data

#### TamilNames
Tamil language lookup tables:
- Nakshatra names (27 lunar mansions)
- Rasi names (12 zodiac signs)
- Planet names (Navagraha)
- Vara names (7 weekdays)
- Tamil month names (12 months)
- Tithi, Yoga, and Karana names
- Rasi lords (planetary rulership)

## Swiss Ephemeris Integration

### Package
- **NuGet Package**: SwissEphNet 2.8.0.2
- **Purpose**: High-precision astronomical calculations

### Configuration
- **Ayanamsa**: Lahiri (Chitrapaksha) - SE_SIDM_LAHIRI
  - Standard for Tamil/Vedic astrology
  - Accounts for precession of equinoxes
- **Calculation Mode**: Sidereal (tropical positions adjusted by ayanamsa)

### Calculations Performed
1. **Planetary Longitudes**: For all 9 Navagraha
2. **Houses**: Using Placidus house system
3. **Ascendant**: From houses calculation
4. **Rahu/Ketu**: Using true node calculation

## Calculation Methodology

### Panchangam

#### Tithi Calculation
```
diff = Moon_Longitude - Sun_Longitude
if (diff < 0) diff += 360°
tithi_number = (diff / 12°) + 1
```

#### Nakshatra Calculation
```
nakshatra_degree = 360° / 27 = 13.333...°
nakshatra_number = (Moon_Longitude / nakshatra_degree) + 1
```

#### Yoga Calculation
```
sum = (Sun_Longitude + Moon_Longitude) % 360°
yoga_number = (sum / 13.333...°) + 1
```

#### Karana Calculation
```
diff = Moon_Longitude - Sun_Longitude
if (diff < 0) diff += 360°
karana_temp = (diff / 6°) + 1
karana_number = ((karana_temp - 1) % 7) + 1
```

### Horoscope

#### Rasi Assignment
```
rasi_number = (longitude / 30°) + 1
```

#### House Placement
Planets are assigned to houses based on their longitude relative to house cusps.

## Testing

### Test Coverage
- Panchangam calculations for multiple locations
- Horoscope generation with all components
- Tamil name mappings
- Edge cases and boundary conditions

### Reference Sources
Calculations verified against:
- http://drikpanchang.com
- https://www.prokerala.com/astrology/panchangam/

### Sample Locations
- Chennai (13.0827°N, 80.2707°E)
- Madurai (9.9252°N, 78.1198°E)
- Coimbatore (11.0168°N, 76.9558°E)

## Usage Examples

### Calculate Panchangam
```csharp
var calculator = new PanchangCalculator();
var birthDetails = new BirthDetails
{
    DateTime = new DateTime(2024, 1, 1, 10, 0, 0),
    Latitude = 13.0827,
    Longitude = 80.2707,
    TimeZoneOffset = 5.5
};

var panchang = calculator.CalculatePanchang(birthDetails);
Console.WriteLine($"Nakshatra: {panchang.NakshatraName} ({panchang.TamilNakshatraName})");
```

### Calculate Horoscope
```csharp
var calculator = new PanchangCalculator();
var birthDetails = new BirthDetails
{
    DateTime = new DateTime(2024, 1, 1, 10, 0, 0),
    Latitude = 13.0827,
    Longitude = 80.2707,
    TimeZoneOffset = 5.5,
    PlaceName = "Chennai"
};

var horoscope = calculator.CalculateHoroscope(birthDetails);
Console.WriteLine($"Lagna: {horoscope.LagnaRasiName} ({horoscope.TamilLagnaRasiName})");

foreach (var planet in horoscope.Planets)
{
    Console.WriteLine($"{planet.Name}: {planet.RasiName}, House {planet.House}");
}
```

## Tamil Language Support

All astrological elements have both English and Tamil names:
- நட்சத்திரம் (Nakshatra)
- ராசி (Rasi)
- லக்னம் (Lagna)
- கிரகங்கள் (Planets)
- திதி (Tithi)
- யோகம் (Yoga)
- கரணம் (Karana)

## Future Enhancements (Phase 3)

1. **Vimshottari Dasa/Bhukti**: Calculate major and minor periods
2. **Navamsa Chart**: Divisional chart (D-9)
3. **Other Divisional Charts**: D-10, D-12, D-16, etc.
4. **Strength Calculations**: Shadbala, Ashtakavarga
5. **Yogas Detection**: Identify auspicious/inauspicious combinations
6. **PDF Export**: Generate formatted horoscope reports

## Known Limitations

1. **House System**: Currently uses Placidus. Whole Sign houses may be added later.
2. **Ephemeris Data**: Requires Swiss Ephemeris data files for extended date ranges.
3. **Time Precision**: Calculations assume standard time zones; doesn't account for historical changes.

## Dependencies

- **.NET 8.0**: Target framework
- **SwissEphNet 2.8.0.2**: Astronomical calculations
- **xUnit**: Testing framework

## References

1. Swiss Ephemeris Documentation: https://www.astro.com/swisseph/
2. Lahiri Ayanamsa: Standard for Indian astrology
3. Tamil Astrology Conventions: Traditional Panchangam methodology
4. Verification Sources:
   - Drik Panchang: http://drikpanchang.com
   - Prokerala: https://www.prokerala.com/astrology/panchangam/

## Contributing

When contributing to this calculation engine:
1. Maintain accuracy of astronomical calculations
2. Preserve Tamil astrology conventions
3. Add unit tests for all changes
4. Verify against reference sources
5. Update documentation

## License

This project is part of the Tamil Horoscope Desktop Application.

---

**Last Updated**: February 2, 2026
**Phase**: 2 - Calculation Engine Implementation
**Status**: Complete
