# Tamil Horoscope Desktop Application

A .NET application for generating Tamil horoscopes using Thirukanitha Panachangam calculations with Swiss Ephemeris for astronomical accuracy.

## Features

- ✅ **Panchangam Calculations** - Tithi, Nakshatra, Yoga, Karana, Vara
- ✅ **Horoscope Generation** - Complete birth chart with Lagna, Navagraha positions, and houses
- ✅ **Navamsa (D-9) Divisional Chart** - Calculate and visualize the most important divisional chart
- ✅ **South Indian Style Charts** - Traditional 4x4 grid layout for Rasi and Navamsa charts
- ✅ **Chart Visualization** - Professional chart display with Tamil Unicode support
- ✅ **Yoga Detection** - Detects 12+ astrological yogas (Gajakesari, Raja, Mahapurusha yogas, etc.)
- ✅ **Dosa Detection** - Detects major doshas (Mangal, Kaal Sarp, Pitra, Shakat, Kemadruma)
- ✅ **Multi-language Support** - Tamil, Telugu, Kannada, Malayalam support for planet, rasi, and nakshatra names
- ✅ **Tamil Language Support** - All astrological elements with Tamil names
- ✅ **Swiss Ephemeris Integration** - High-precision astronomical calculations
- ✅ **Lahiri Ayanamsa** - Standard for Tamil/Vedic astrology
- ✅ **WPF Desktop UI** - Modern, responsive Windows desktop application
- ✅ **PDF Export** - Export horoscope results to PDF format
- ✅ **Multiple Locations** - Support for any geographic location
- ✅ **Vimshottari Dasa/Bhukti** - Framework ready for implementation

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

The project includes comprehensive test coverage:
- **110+ unit tests** covering all calculation features including yoga and dosa detection
- Navamsa divisional chart calculations
- Edge cases and boundary conditions
- Integration tests for combined features

Run tests with:
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Advanced Features

### Yoga Detection

The application automatically detects beneficial planetary combinations (yogas) in the birth chart:

**Detected Yogas:**
- **Gajakesari Yoga** - Jupiter in kendra from Moon (wisdom, wealth, fame)
- **Raja Yoga** - Lords of kendras and trikonas in conjunction (power, authority)
- **Dhana Yoga** - Wealth combinations (prosperity, financial gains)
- **Mahapurusha Yogas** - Five great person yogas:
  - Hamsa Yoga (Jupiter)
  - Malavya Yoga (Venus)
  - Sasa Yoga (Saturn)
  - Ruchaka Yoga (Mars)
  - Bhadra Yoga (Mercury)
- **Moon-based Yogas** - Sunapha, Anapha, Durdhura yogas
- **Budha Aditya Yoga** - Sun-Mercury conjunction (intelligence)

**Usage:**
```csharp
var calculator = new PanchangCalculator();
var horoscope = calculator.CalculateHoroscope(
    birthDetails, 
    includeDasa: true, 
    includeNavamsa: true,
    includeYoga: true,     // Enable yoga detection
    language: "Tamil"      // Choose language
);

// Access detected yogas
foreach (var yoga in horoscope.Yogas)
{
    Console.WriteLine($"{yoga.Name} ({yoga.LocalName})");
    Console.WriteLine($"Strength: {yoga.Strength}/10");
    Console.WriteLine($"Description: {yoga.Description}");
}
```

### Dosa Detection

The application detects major astrological afflictions (doshas):

**Detected Doshas:**
- **Mangal Dosha (Kuja Dosha)** - Mars affliction affecting marriage
- **Kaal Sarp Dosha** - All planets between Rahu and Ketu
- **Pitra Dosha** - Ancestral affliction
- **Shakat Dosha** - Jupiter-Moon affliction
- **Kemadruma Dosha** - Moon without support

Each dosha includes:
- Severity rating (1-10)
- Detailed description of effects
- Planets and houses involved
- Traditional remedies and recommendations

**Usage:**
```csharp
var horoscope = calculator.CalculateHoroscope(
    birthDetails, 
    includeDosa: true,     // Enable dosha detection
    language: "Kannada"    // Multi-language support
);

// Access detected doshas
foreach (var dosa in horoscope.Dosas)
{
    Console.WriteLine($"{dosa.Name} ({dosa.LocalName})");
    Console.WriteLine($"Severity: {dosa.Severity}/10");
    Console.WriteLine($"Description: {dosa.Description}");
    Console.WriteLine("Remedies:");
    foreach (var remedy in dosa.Remedies)
    {
        Console.WriteLine($"  - {remedy}");
    }
}
```

### Multi-language Support

The application now supports four South Indian languages:

**Supported Languages:**
- Tamil (தமிழ்)
- Telugu (తెలుగు)
- Kannada (ಕನ್ನಡ)
- Malayalam (മലയാളം)

**Usage:**
```csharp
// Get localized names
var tamilPlanetName = TamilNames.GetPlanetName("Mars", "Tamil");      // செவ்வாய்
var teluguRasiName = TamilNames.GetRasiName(1, "Telugu");             // మేషం
var kannadaNakshatra = TamilNames.GetNakshatraName(14, "Kannada");    // ಚಿತ್ರಾ

// Calculate horoscope with specific language
var horoscope = calculator.CalculateHoroscope(
    birthDetails,
    includeYoga: true,
    includeDosa: true,
    language: "Malayalam"  // Yoga and dosha names in Malayalam
);
```

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

- [ ] Additional yogas (Neecha Bhanga, Viparita Raja, etc.)
- [ ] More doshas (Nadi, Gana, Rashi doshas for marriage compatibility)
- [ ] Compatibility analysis (Kundali matching)
- [ ] Transit predictions
- [ ] Database integration for chart storage

## License

This project is developed by RAMESHSIVAPERUMAL.

---

**Status**: Phase 3.5 Complete - Charts Implemented  
**Last Updated**: February 4, 2026  
**Chart Style**: South Indian Traditional (4x4 Grid)
