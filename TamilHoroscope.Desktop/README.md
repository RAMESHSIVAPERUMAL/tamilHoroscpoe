# TamilHoroscope.Desktop

Windows WPF Desktop Application for Tamil Horoscope Calculation

## Features

- Modern UI with Material Design principles
- Birth details input with validation
- Real-time horoscope generation
- South Indian style Rasi and Navamsa charts
- PDF export with charts
- English/Tamil bilingual interface
- Vimshottari Dasa and Navamsa calculations
- Keyboard shortcuts (F5 to calculate, Ctrl+E to export)
- Responsive layout with resizable panels

## Requirements

- Windows 10/11
- .NET 8.0 Runtime

## Building

```bash
dotnet restore
dotnet build
dotnet run
```

## Usage

1. Enter Birth Details:
   - Name (optional)
   - Date of Birth
   - Time of Birth (HH:mm:ss format)
   - Place Name
   - Latitude/Longitude
   - Timezone Offset

2. Configure Options (optional):
   - Enable Vimshottari Dasa calculation
   - Enable Navamsa chart

3. Calculate:
   - Press F5 or click "Calculate Horoscope"
   - Review results in the right panel

4. Export:
   - Press Ctrl+E or click "Export to PDF"
   - Choose save location

## Documentation

See the `/Docs` folder for detailed guides on:
- Birth place picker with online/offline search
- Dasa and Bhukti calculations
- Navamsa chart generation

## Architecture

### Technology Stack
- .NET 8.0
- WPF (UI framework)
- iTextSharp (PDF generation)
- TamilHoroscope.Core (calculation engine)

### Project Structure
```
TamilHoroscope.Desktop/
├── Controls/
│   ├── RasiChartControl.xaml
│   ├── NavamsaChartControl.xaml
│   └── PlanetStrengthChartControl.xaml
├── Services/
│   └── BirthPlaceService.cs
├── Models/
│   └── BirthPlace.cs
├── Data/
│   └── BirthPlaces.xml
├── MainWindow.xaml
└── App.xaml
```

## Dependencies

- TamilHoroscope.Core (project reference)
- iTextSharp.LGPLv2.Core (v3.4.23)

## Keyboard Shortcuts

- F5: Calculate horoscope
- Ctrl+E: Export to PDF

## Known Limitations

1. Windows only (WPF requires Windows)
2. Vimshottari Dasa UI ready, calculation pending
3. Print preview not implemented

## Future Enhancements

- Complete Vimshottari Dasa implementation
- North Indian style chart option
- Chart export as images (PNG, SVG)
- Database integration for saving horoscopes
- Print preview functionality
- Interactive chart tooltips

## License

Developed by RAMESHSIVAPERUMAL.

## Version

Version: 1.0
Last Updated: February 2026
Status: Production Ready

---

For issues or contributions, visit: https://github.com/RAMESHSIVAPERUMAL/tamilHoroscope
