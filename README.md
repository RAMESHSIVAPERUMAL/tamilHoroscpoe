# Tamil Horoscope Desktop Application

A .NET application for generating Tamil horoscopes using Thirukanitha Panachangam calculations with Swiss Ephemeris for astronomical accuracy.

## Features

- ✅ **Panchangam Calculations** - Tithi, Nakshatra, Yoga, Karana, Vara
- ✅ **Horoscope Generation** - Complete birth chart with Lagna, Navagraha positions, and houses
- ✅ **Tamil Language Support** - All astrological elements with Tamil names
- ✅ **Swiss Ephemeris Integration** - High-precision astronomical calculations
- ✅ **Lahiri Ayanamsa** - Standard for Tamil/Vedic astrology
- ✅ **WPF Desktop UI** - Modern, responsive Windows desktop application
- ✅ **PDF Export** - Export horoscope results to PDF format
- 🔄 **Multiple Locations** - Support for any geographic location
- 🔄 **Navamsa Chart (D-9)** - Framework ready for implementation
- 🔄 **Vimshottari Dasa/Bhukti** - Framework ready for implementation

## Project Structure

```
TamilHoroscope.sln
├── TamilHoroscope.Core/       # Core calculation engine (class library)
├── TamilHoroscope.Desktop/    # WPF desktop application
├── TamilHoroscope.Tests/      # Unit tests (xUnit)
└── TamilHoroscope.Sample/     # Sample console application
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 / VS Code / JetBrains Rider (optional)
- Windows OS (for running WPF Desktop application)

### Building the Project

```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the sample console application
dotnet run --project TamilHoroscope.Sample

# Run the WPF desktop application (Windows only)
dotnet run --project TamilHoroscope.Desktop
```

### Using the Desktop Application

1. **Enter Birth Details**: Fill in name, date, time, place, coordinates, and timezone
2. **Advanced Options**: Optionally configure Vimshottari Dasa and Navamsa chart options
3. **Calculate**: Click "Calculate Horoscope" or press F5
4. **View Results**: Review Panchangam, Lagna, planetary positions, and houses
5. **Export**: Click "Export to PDF" or press Ctrl+E to save results

### Desktop Application Features

- **Modern UI**: Clean, responsive interface with Tamil language support
- **Input Validation**: Real-time validation with helpful error messages and tooltips
- **Advanced Options**: Collapsible section for Vimshottari Dasa and Navamsa settings
- **Data Grids**: Sortable tables for planets and houses
- **PDF Export**: Professional horoscope reports with tables and Tamil text
- **Keyboard Shortcuts**: F5 to calculate, Ctrl+E to export
- **Accessibility**: Keyboard navigation and screen reader support
- **Status Feedback**: Real-time status updates during calculations and exports

## Dependencies

- **SwissEphNet (2.8.0.2)** - Swiss Ephemeris for astronomical calculations
- **iTextSharp.LGPLv2.Core (3.4.23)** - PDF generation for Desktop app
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
- [ ] Additional divisional charts (D-2, D-3, etc.)
- [ ] Chart visualization with graphics
- [ ] Multi-language support (English, Tamil, Hindi)

## License

This project is developed by RAMESHSIVAPERUMAL.

---

**Status**: Phase 3 Complete - Desktop UI Implemented  
**Last Updated**: February 4, 2026
