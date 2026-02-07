# Tamil Horoscope - Quick Start Guide

## ?? Get Started in 5 Minutes

### Step 1: Prerequisites

**System Requirements**:
- Windows 10/11
- .NET 8.0 Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))

**For Developers**:
- Visual Studio 2022 or VS Code
- .NET 8.0 SDK

---

### Step 2: Run the Application

#### Option A: Run Executable (End Users)
```bash
# Navigate to release folder
cd TamilHoroscope.Desktop\bin\Release\net8.0-windows

# Run the application
TamilHoroscope.Desktop.exe
```

#### Option B: Run from Source (Developers)
```bash
# Clone the repository
git clone https://github.com/RAMESHSIVAPERUMAL/tamilHoroscpoe.git
cd tamilHoroscpoe

# Restore packages
dotnet restore

# Run the application
dotnet run --project TamilHoroscope.Desktop
```

---

### Step 3: Calculate Your First Horoscope

1. **Enter Name** (Optional)
   - Type your name in the "Name" field

2. **Select Birth Date**
   - Click the calendar icon
   - Choose your birth date

3. **Enter Birth Time**
   - Format: HH:mm:ss (24-hour)
   - Example: 06:35:00 for 6:35 AM

4. **Choose Birth Place**
   - Start typing city name
   - Select from dropdown (online search enabled)
   - OR manually enter coordinates

5. **Select Options** (Optional)
   - ? Calculate Vimshottari Dasa (120 years)
   - ? Calculate Navamsa Chart (D-9)
   - ? Calculate Planetary Strength (Shadbala)

6. **Click "Calculate Horoscope"** or press **F5**

---

### Step 4: View Results

Your horoscope will display:

? **Panchangam** - Tamil Month, Tithi, Nakshatra, Yoga, Karana  
? **Lagna** - Ascendant sign and degree  
? **Navagraha Positions** - All 9 planets with details  
? **Rasi Chart** - South Indian style birth chart  
? **Navamsa Chart** - D-9 divisional chart (if enabled)  
? **Planetary Strength** - Shadbala calculations (if enabled)  
? **Vimshottari Dasa** - Major and minor periods (if enabled)  

---

### Step 5: Export to PDF

1. **Click "Export to PDF"** or press **Ctrl+E**
2. **Choose location** and filename
3. **Click Save**

The PDF will contain:
- Complete horoscope details
- Birth charts (visual)
- All planetary positions
- Strength analysis (if calculated)
- Dasa/Bhukti periods (if calculated)

---

## ?? Sample Calculation

**Example Input**:
```
Name: Ramesh
Date: July 18, 1983
Time: 06:35:00
Place: Kumbakonam, Tamil Nadu
Coordinates: 10.9601°N, 79.3845°E
Timezone: UTC+5.5 (IST)
```

**Expected Output**:
- Lagna: Cancer (?????) at 98.96°
- Sun: Cancer at 91.25°
- Moon: Libra at 192.67°
- Nakshatra: Swati (??????)
- Tamil Month: ??? (Aadi)
- Tithi: Chaturthi (?????????)

---

## ?? Common Features

### Birth Place Search
- **Online Mode**: Searches worldwide locations via geocoding API
- **Offline Mode**: Searches from local database (600+ Indian cities)
- **Auto-fill**: Automatically fills latitude, longitude, timezone

### Keyboard Shortcuts
- **F5** - Calculate Horoscope
- **Ctrl+E** - Export to PDF
- **Tab** - Navigate between fields

### Advanced Options
- **Dasa Years**: Choose 10, 25, 50, or 120 years
- **Navamsa**: Include D-9 divisional chart
- **Strength**: Calculate planetary fitness (Shadbala)

---

## ?? Troubleshooting

### Issue: "Could not load birth places"
**Solution**: Application works offline with manual coordinates entry

### Issue: Tamil text shows as boxes
**Solution**: Install Tamil Unicode fonts (Latha, Nirmala UI)

### Issue: PDF export fails
**Solution**: Check folder write permissions

### Issue: Wrong time zone
**Solution**: Verify UTC offset for your location
- IST: +5.5
- EST: -5.0
- GMT: 0.0

---

## ?? Next Steps

1. **Read User Guide** - `docs/Desktop-UserGuide.md`
2. **Understand Shadbala** - `docs/SHADBALA-SIMPLE-GUIDE.md`
3. **Learn Dasa/Bhukti** - `docs/Vimshottari-Dasa.md`
4. **Technical Details** - `docs/Phase2-CalculationEngine.md`

---

## ?? Tips

? **Tip 1**: Use online place search for accurate coordinates  
? **Tip 2**: Enable all options for complete analysis  
? **Tip 3**: Export to PDF for offline reference  
? **Tip 4**: Check planetary strength to understand weak areas  
? **Tip 5**: Use Dasa/Bhukti to know current planetary periods  

---

## ?? Support

**Issues**: [GitHub Issues](https://github.com/RAMESHSIVAPERUMAL/tamilHoroscpoe/issues)  
**Documentation**: `docs/` folder  
**Email**: [Your Email]

---

## ? Success!

You're now ready to calculate Tamil horoscopes with:
- ? Swiss Ephemeris accuracy
- ? Traditional Vedic calculations
- ? Tamil language support
- ? Professional PDF reports

**Enjoy exploring your cosmic blueprint! ??**

---

**Version**: 1.0  
**Last Updated**: February 7, 2026  
**License**: [Your License]
