# Tamil Horoscope Desktop Application

A .NET application for generating Tamil horoscopes using Thirukanitha Panachangam calculations with Swiss Ephemeris for astronomical accuracy.

## Features

- ✅ **Panchangam Calculations** - Tithi, Nakshatra, Yoga, Karana, Vara
- ✅ **Horoscope Generation** - Complete birth chart with Lagna, Navagraha positions, and houses
- ✅ **Tamil Language Support** - All astrological elements with Tamil names
- ✅ **Swiss Ephemeris Integration** - High-precision astronomical calculations
- ✅ **Lahiri Ayanamsa** - Standard for Tamil/Vedic astrology
- 🔄 **Multiple Locations** - Support for any geographic location

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

Run tests with:
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Future Enhancements

- [ ] Vimshottari Dasa/Bhukti calculations
- [ ] Navamsa (D-9) divisional chart
- [ ] PDF export
- [ ] WPF desktop UI

## License

This project is developed by RAMESHSIVAPERUMAL.

---

**Status**: Phase 2 Complete - Calculation Engine Implemented  
**Last Updated**: February 2, 2026
