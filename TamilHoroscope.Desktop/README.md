# TamilHoroscope.Desktop

Windows WPF Desktop Application for Tamil Horoscope Calculation

## Overview

Modern desktop application for generating Tamil horoscopes with professional PDF export capability.

## Features

- ✅ **Modern UI**: Material Design-inspired WPF interface
- ✅ **Birth Details Input**: Comprehensive form with validation
- ✅ **Real-time Calculation**: Instant horoscope generation
- ✅ **PDF Export**: Professional horoscope reports
- ✅ **Tamil Language**: Bilingual interface (English/Tamil)
- ✅ **Advanced Options**: Vimshottari Dasa and Navamsa settings
- ✅ **Accessibility**: Keyboard shortcuts and tooltips
- ✅ **Responsive**: Resizable layout with scrolling panels

## Requirements

- **OS**: Windows 10/11
- **Runtime**: .NET 8.0 Runtime
- **Optional**: Tamil Unicode fonts for proper Tamil text rendering

## Building

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

## Usage

1. **Enter Birth Details**:
   - Name (optional)
   - Date of Birth
   - Time of Birth (HH:mm:ss format)
   - Place Name
   - Latitude/Longitude
   - Timezone Offset

2. **Configure Options** (optional):
   - Expand "Advanced Options"
   - Enable Vimshottari Dasa calculation
   - Enable Navamsa chart

3. **Calculate**:
   - Click "Calculate Horoscope" or press F5
   - Review results in right panel

4. **Export**:
   - Click "Export to PDF" or press Ctrl+E
   - Choose save location

## Documentation

See `/docs` folder for complete documentation:
- **Desktop-UserGuide.md**: End-user documentation
- **Desktop-Technical.md**: Developer documentation
- **Desktop-UI-Layout.md**: UI specifications
- **Desktop-UI-Mockup.md**: Visual mockups
- **Phase3-Summary.md**: Implementation summary

## Architecture

### Technology Stack
- **.NET 8.0**: Application framework
- **WPF**: UI framework
- **iTextSharp**: PDF generation
- **TamilHoroscope.Core**: Calculation engine

### Project Structure
```
TamilHoroscope.Desktop/
├── App.xaml               # Application resources
├── MainWindow.xaml        # Main UI definition
├── MainWindow.xaml.cs     # Business logic
└── Converters/            # Value converters
```

## Dependencies

- `TamilHoroscope.Core` (project reference)
- `iTextSharp.LGPLv2.Core` (v3.4.23)

## Keyboard Shortcuts

- **F5**: Calculate horoscope
- **Ctrl+E**: Export to PDF

## Known Limitations

1. **Windows Only**: WPF requires Windows OS
2. **Navamsa**: Calculation not yet implemented (placeholder)
3. **Vimshottari Dasa**: Calculation not yet implemented (placeholder)
4. **Charts**: No graphical chart display yet

## Future Enhancements

- Implement Vimshottari Dasa calculation
- Implement Navamsa chart calculation
- Add visual chart graphics (South Indian, North Indian styles)
- Database integration for saving charts
- Print preview functionality

## License

This project is developed by RAMESHSIVAPERUMAL.

## Version

**Current**: 1.0  
**Last Updated**: February 4, 2026  
**Status**: Production Ready

---

For issues or contributions, visit: https://github.com/RAMESHSIVAPERUMAL/tamilHoroscope
