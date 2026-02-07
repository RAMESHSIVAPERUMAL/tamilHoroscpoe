# Planetary Strength (Shadbala) Implementation

## Overview
This implementation provides a **simplified Shadbala calculation** for all planets in a horoscope. Shadbala (meaning "Six-fold Strength") is a comprehensive system in Vedic astrology for measuring planetary power and effectiveness.

## Features Implemented

### 1. Six Strength Components

The calculator evaluates each planet based on six key factors:

#### a) **Positional Strength (Sthana Bala)**
- Based on sign placement (exaltation, own sign, friend's sign, etc.)
- Exaltation: 60 Rupas
- Own Sign: 45 Rupas
- Friend's Sign: 30 Rupas
- Neutral: 15 Rupas
- Enemy's Sign: 7.5 Rupas
- Debilitation: 0 Rupas

#### b) **Directional Strength (Dig Bala)**
- Based on house placement
- Jupiter & Mercury: Strong in 1st house (East)
- Sun & Mars: Strong in 10th house (South)
- Saturn: Strong in 7th house (West)
- Moon & Venus: Strong in 4th house (North)
- Maximum: 60 Rupas, decreases with angular distance

#### c) **Motional Strength (Chesta Bala)**
- Based on planetary speed and retrograde status
- Retrograde planets: 60 Rupas (maximum strength)
- Direct motion: Strength based on speed relative to average
- Sun and Moon: Fixed at 30 Rupas (always direct)

#### d) **Natural Strength (Naisargika Bala)**
- Inherent luminosity and power of the planet
- Sun: 60 Rupas (brightest)
- Moon: 51.43 Rupas
- Venus: 42.86 Rupas
- Jupiter: 34.29 Rupas
- Mercury: 25.71 Rupas
- Mars: 17.14 Rupas
- Saturn, Rahu, Ketu: 8.57 Rupas each

#### e) **Temporal Strength (Kala Bala)**
- Based on time factors (day/night, waxing/waning moon)
- Benefics (Jupiter, Venus, Mercury, Moon): Strong during day
- Malefics (Sun, Mars, Saturn, Rahu, Ketu): Strong at night
- Additional strength based on lunar fortnight (Paksha)

#### f) **Aspectual Strength (Drik Bala)**
- Based on aspects received from other planets
- Benefic aspects increase strength (+10 Rupas per aspect)
- Malefic aspects decrease strength (-5 Rupas per aspect)
- Range: 0-60 Rupas

### 2. Total Strength Calculation

**Total Strength = Positional + Directional + Motional + Natural + Temporal + Aspectual**

- Measured in **Rupas** (1 Rupa = 60 Virupas)
- Maximum possible: ~390 Rupas
- Percentage calculated relative to maximum

### 3. Strength Grading

| Percentage | Grade | Tamil | Color |
|-----------|-------|-------|-------|
| ? 80% | Excellent | ??? ?????? | Forest Green |
| 60-79% | Good | ?????? | Lime Green |
| 40-59% | Average | ?????? | Gold |
| 20-39% | Weak | ??????? | Orange |
| < 20% | Very Weak | ??????? ??????? | Red |

### 4. Required Minimum Strength

Each planet has a minimum required strength for positive results:

| Planet | Required (Rupas) |
|--------|------------------|
| Sun | 5.0 |
| Moon | 6.0 |
| Mars | 5.0 |
| Mercury | 7.0 |
| Jupiter | 6.5 |
| Venus | 5.5 |
| Saturn | 5.0 |
| Rahu | 5.0 |
| Ketu | 5.0 |

## UI Display

### Bar Chart Visualization
- Each planet shown with a horizontal bar
- Bar length represents strength relative to maximum
- Color coding:
  - Green: Strong (?60%)
  - Gold: Average (40-59%)
  - Orange/Red: Weak (<40%)
- Red vertical line: Indicates required minimum strength
- Display shows:
  - Planet name (English & Tamil)
  - Total strength in Rupas and Virupas
  - Strength percentage
  - Grade (Excellent/Good/Average/Weak/Very Weak)

### Summary Statistics
- Count of strong planets (?60%)
- Count of weak planets (<40%)
- Helpful notes about interpretation

## PDF Export

The planetary strength section is exported to PDF with:
1. **Strength Table**: Shows all planets with their strength values, percentages, and grades
2. **Bar Chart Visualization**: Rendered image of the strength bars
3. **Explanation**: Brief description of the calculation methodology

## Usage

### In Code

```csharp
// Calculate horoscope with strength calculation
var calculator = new PanchangCalculator();
var horoscope = calculator.CalculateHoroscope(
    birthDetails, 
    includeDasa: true, 
    includeNavamsa: true, 
    dasaYears: 120,
    includeStrength: true  // Enable strength calculation
);

// Access strength data
if (horoscope.PlanetStrengths != null)
{
    foreach (var strength in horoscope.PlanetStrengths)
    {
        Console.WriteLine($"{strength.Name}: {strength.TotalStrength:F1} Rupas ({strength.StrengthGrade})");
    }
}
```

### In UI

1. Check the "Calculate Planetary Strength (Shadbala)" option in Advanced Options
2. Click "Calculate Horoscope"
3. View the strength chart section before the Dasa section
4. Export to PDF includes the strength analysis

## Interpretation Guidelines

### Strong Planets (?60%)
- Give positive results during their dasas and transits
- Their significations are well-supported in the chart
- Can overcome obstacles and deliver promised results

### Average Planets (40-59%)
- Give mixed results based on other factors
- May require supportive transits to manifest fully
- Results depend on house placement and aspects

### Weak Planets (<40%)
- May struggle to deliver results
- Their significations may face challenges
- Require special attention and remedial measures

### Above Required Minimum
- A planet above its required minimum (red line) can give beneficial results
- Even if overall percentage is low, meeting the minimum is crucial
- Example: Saturn with 20% (6 Rupas) meets its 5 Rupa requirement

## Technical Details

### Calculator Location
- **File**: `TamilHoroscope.Core/Calculators/PlanetStrengthCalculator.cs`
- **Model**: `TamilHoroscope.Core/Models/PlanetStrengthData.cs`
- **UI Control**: `TamilHoroscope.Desktop/Controls/PlanetStrengthChartControl.xaml(.cs)`

### Dependencies
- Requires completed horoscope calculation (planets, houses, panchangam)
- Uses Swiss Ephemeris planetary data
- Integrates with existing calculation pipeline

## Limitations & Future Enhancements

### Current Limitations
1. **Simplified Exaltation/Debilitation**: Uses exact degrees for precision in full Shadbala
2. **Simplified Aspects**: Only considers 7th house aspects; full Shadbala includes special aspects (3rd, 10th for Jupiter, etc.)
3. **No Bhava Bala**: House strength not calculated separately
4. **No Ashtakavarga**: Bindus (points) system not implemented

### Planned Enhancements (Future)
1. **Full Shadbala Calculation**: All sub-components of six balas
2. **Ashtakavarga**: 8-fold division for precise predictions
3. **Vimshopaka Bala**: Weighted divisional chart strengths
4. **Ishta/Kashta Phala**: Benefic and malefic points
5. **Varshaphala**: Annual strength variations

## References

1. **Brihat Parashara Hora Shastra** - Classical Shadbala methodology
2. **Phaladeepika** - Practical applications
3. **Modern Vedic Astrology** - Computational methods
4. **Traditional Tamil Astrology** - Regional interpretations

## Notes

- This is a **simplified but functional** implementation
- Provides meaningful strength analysis for practical use
- Can be extended to full Shadbala as needed
- Results verified against traditional calculations
- Suitable for both learning and practical application

---

**Implementation Date**: February 2026  
**Version**: 1.0 (Simplified Shadbala)  
**Status**: ? Complete and Tested  
**Location in UI**: Before Vimshottari Dasa section  
**Location in PDF**: After Navamsa, before Dasa
