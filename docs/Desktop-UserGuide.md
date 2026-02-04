# Tamil Horoscope Desktop Application - User Guide

## Overview

The Tamil Horoscope Desktop Application is a modern Windows WPF application that provides a user-friendly interface for generating Tamil horoscopes with advanced calculation features.

## Features

### Core Features
- **Birth Details Input**: Enter comprehensive birth information including date, time, location, and coordinates
- **Advanced Options**: Configure Vimshottari Dasa/Bhukti and Navamsa chart calculations
- **Real-time Calculation**: Instant horoscope generation using Swiss Ephemeris
- **Data Visualization**: Clear display of Panchangam, Lagna, planets, and houses
- **PDF Export**: Professional PDF reports with all horoscope details
- **Tamil Language Support**: Bilingual labels (English/Tamil) throughout the UI

### User Interface Features
- **Modern Design**: Clean, responsive layout with Material Design-inspired styling
- **Input Validation**: Real-time validation with helpful error messages
- **Tooltips**: Contextual help for all input fields
- **Keyboard Shortcuts**: 
  - F5: Calculate horoscope
  - Ctrl+E: Export to PDF
- **Accessibility**: Full keyboard navigation and screen reader support
- **Status Feedback**: Real-time updates during calculations and exports

## Getting Started

### Installation

1. Ensure you have .NET 8.0 Runtime installed
2. Clone or download the application
3. Build using Visual Studio or `dotnet build`
4. Run `TamilHoroscope.Desktop.exe`

### Basic Usage

1. **Enter Birth Details**:
   - Name (optional)
   - Date of Birth (use date picker)
   - Time of Birth (24-hour format: HH:mm:ss)
   - Place Name
   - Latitude and Longitude (decimal degrees)
   - Timezone Offset (hours from GMT)

2. **Configure Advanced Options** (optional):
   - Expand "Advanced Options" section
   - Check/uncheck Vimshottari Dasa calculation
   - Select years to display (10, 25, 50, or 120 years)
   - Check/uncheck Navamsa (D-9) chart calculation
   - Choose detailed Navamsa positions display

3. **Calculate Horoscope**:
   - Click "Calculate Horoscope" button or press F5
   - Review the status message
   - View results in the right panel

4. **Review Results**:
   - **Panchangam**: Tamil month, Vara, Tithi, Nakshatra, Yoga, Karana
   - **Lagna**: Ascendant sign and longitude
   - **Navagraha Positions**: Sortable table with planets, signs, nakshatras
   - **Houses**: 12 houses with signs, lords, and occupying planets
   - **Navamsa Chart**: Displayed if enabled (framework ready)
   - **Vimshottari Dasa**: Displayed if enabled (framework ready)

5. **Export to PDF**:
   - Click "Export to PDF" button or press Ctrl+E
   - Choose save location
   - PDF includes all calculated data in a professional format

## Sample Data

The application comes with default values for Chennai:
- Latitude: 13.0827
- Longitude: 80.2707
- Timezone: 5.5 (IST)

### Other Major Cities

**Madurai**:
- Latitude: 9.9252
- Longitude: 78.1198
- Timezone: 5.5

**Coimbatore**:
- Latitude: 11.0168
- Longitude: 76.9558
- Timezone: 5.5

**Bangalore**:
- Latitude: 12.9716
- Longitude: 77.5946
- Timezone: 5.5

## Input Validation

The application validates all inputs:

- **Date**: Must be selected
- **Time**: Must be in HH:mm:ss format (e.g., 10:30:00)
- **Latitude**: Must be between -90 and 90
- **Longitude**: Must be between -180 and 180
- **Timezone**: Must be between -12 and 14

Error messages are displayed if validation fails.

## Advanced Features

### Vimshottari Dasa Options

The Vimshottari Dasa system calculates major and minor planetary periods. The application allows you to:
- Enable/disable calculation
- Select display duration (10, 25, 50, or 120 years)

*Note: Full Vimshottari Dasa calculation is ready for implementation in Phase 3.*

### Navamsa Chart (D-9)

The Navamsa chart is the most important divisional chart in Vedic astrology. The application:
- Has the framework ready for Navamsa calculations
- Allows toggling detailed position display
- Will show planet positions in the 9th divisional chart

*Note: Full Navamsa calculation is ready for implementation in Phase 3.*

## PDF Export

The PDF export feature creates professional horoscope reports including:

1. **Title and Name**: Bilingual header with person's name
2. **Birth Details**: Date, time, place, and coordinates
3. **Panchangam**: All five elements with Tamil names
4. **Lagna**: Ascendant details
5. **Navagraha Positions**: Complete table with all planets
6. **Houses**: All 12 houses with signs, lords, and planets
7. **Footer**: Generation timestamp and application info

The PDF is optimized for printing on A4 paper.

## Technical Details

### Architecture
- **WPF Framework**: .NET 8.0 WPF application
- **Calculation Engine**: TamilHoroscope.Core library
- **Swiss Ephemeris**: High-precision astronomical calculations
- **PDF Generation**: iTextSharp.LGPLv2.Core

### Data Grid Features
- Sortable columns (click headers)
- Alternating row colors for readability
- Automatic column sizing
- Read-only (prevents accidental edits)

### Styling
- Material Design-inspired color scheme
- Primary color: Blue (#2196F3)
- Success color: Green (#4CAF50)
- Consistent spacing and typography
- Tamil Unicode font support

## Troubleshooting

### Common Issues

**Issue**: Application won't start
- **Solution**: Ensure .NET 8.0 Runtime is installed

**Issue**: Invalid time format error
- **Solution**: Use 24-hour format with leading zeros (e.g., 09:30:00, not 9:30:00)

**Issue**: Calculation fails
- **Solution**: Verify all required fields are filled and validation passes

**Issue**: PDF export fails
- **Solution**: Ensure you have write permissions to the save location

**Issue**: Tamil characters don't display
- **Solution**: Ensure Tamil Unicode fonts are installed on your system

## Keyboard Shortcuts

- **F5**: Calculate horoscope
- **Ctrl+E**: Export to PDF
- **Tab**: Navigate between fields
- **Enter**: Activate focused button
- **Escape**: Close application (when focused on window)

## Accessibility

The application supports:
- Full keyboard navigation
- Tab order optimization
- Screen reader compatibility
- Tooltips on all interactive elements
- High contrast support
- Scalable UI elements

## Future Enhancements

Planned features for future releases:
- Complete Vimshottari Dasa calculation with Bhukti and Antara levels
- Full Navamsa (D-9) chart calculation and visualization
- Additional divisional charts (D-2, D-3, etc.)
- Graphical chart display (South Indian, North Indian styles)
- Chart comparison feature
- Database for saving birth details
- Print preview
- Multi-language support (Hindi, Sanskrit)
- Compatibility analysis
- Transit predictions

## Support

For issues, suggestions, or contributions, please visit the GitHub repository:
https://github.com/RAMESHSIVAPERUMAL/tamilHoroscpoe

---

**Version**: 1.0  
**Last Updated**: February 4, 2026  
**Author**: RAMESHSIVAPERUMAL
