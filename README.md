# Tamil Horoscope Desktop Application

A .NET application for generating Tamil horoscopes using Thirukanitha Panachangam calculations with Swiss Ephemeris for astronomical accuracy.

## Features

- ✅ **Panchangam Calculations** - Tithi, Nakshatra, Yoga, Karana, Vara
- ✅ **Horoscope Generation** - Complete birth chart with Lagna, Navagraha positions, and houses
- ✅ **Navamsa (D-9) Divisional Chart** - Calculate and visualize the most important divisional chart in Vedic astrology
- ✅ **Tamil Language Support** - All astrological elements with Tamil names
- ✅ **Swiss Ephemeris Integration** - High-precision astronomical calculations
- ✅ **Lahiri Ayanamsa** - Standard for Tamil/Vedic astrology
- ✅ **Vimshottari Dasa/Bhukti** - Calculate major and minor planetary periods based on Moon's nakshatra
- ✅ **Multiple Locations** - Support for any geographic location

## Project Structure

```
TamilHoroscope.sln
├── TamilHoroscope.Core/       # Core calculation engine (class library)
├── TamilHoroscope.Tests/      # Unit tests (xUnit)
└── TamilHoroscope.Sample/     # Sample console application
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 / VS Code / JetBrains Rider (optional)

### Building the Project

```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the sample application
dotnet run --project TamilHoroscope.Sample
```

## Dependencies

- **SwissEphNet (2.8.0.2)** - Swiss Ephemeris for astronomical calculations
- **xUnit** - Unit testing framework
- **Newtonsoft.Json** - JSON serialization (for sample app)

## Documentation

- [Phase 2 - Calculation Engine](docs/Phase2-CalculationEngine.md) - Detailed technical documentation

## Testing

All calculations are tested against trusted sources:
- http://drikpanchang.com
- https://www.prokerala.com/astrology/panchangam/

The project includes comprehensive test coverage:
- **79+ unit tests** covering all calculation features
- Navamsa divisional chart calculations
- Edge cases and boundary conditions
- Integration tests for combined features

Run tests with:
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Advanced Features

### Navamsa (D-9) Divisional Chart

The Navamsa chart is the most important divisional chart in Vedic astrology, used for analyzing:
- Marriage and relationships
- Inner strength and dharma
- Deeper insights into planetary influences

**Usage:**
```csharp
var calculator = new PanchangCalculator();
var horoscope = calculator.CalculateHoroscope(birthDetails, 
    includeDasa: true, 
    includeNavamsa: true);

// Access Navamsa positions
foreach (var planet in horoscope.NavamsaPlanets)
{
    Console.WriteLine($"{planet.Name}: {planet.RasiName} in Navamsa");
}
```

**Calculation Method:**
- Each sign (30°) is divided into 9 parts (3°20' each)
- Starting sign depends on element (Fire/Earth/Air/Water)
- Proportional mapping maintains relative positions within each Navamsa

## Future Enhancements

- [ ] PDF export
- [ ] WPF desktop UI
- [ ] Additional divisional charts (D-10, D-12, etc.)

## License

This project is developed by RAMESHSIVAPERUMAL.

---

**Status**: Phase 2 Complete - Calculation Engine with Navamsa Support  
**Last Updated**: February 4, 2026
