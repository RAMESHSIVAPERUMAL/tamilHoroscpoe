# Chart Visual Guide - South Indian Style

## Layout Structure

The South Indian style chart uses a fixed 4x4 grid where each Rasi (zodiac sign) has a specific position:

```
┌───────────┬───────────┬───────────┬───────────┐
│ 12        │ 1         │ 2         │ 3         │
│ Pisces    │ Aries     │ Taurus    │ Gemini    │
│ மீனம்     │ மேஷம்   │ ரிஷபம்   │ மிதுனம்   │
├───────────┼───────────┴───────────┼───────────┤
│ 11        │                       │ 4         │
│ Aquarius  │        ராசி            │ Cancer    │
│ கும்பம்    │     Rasi Chart        │ கடகம்     │
├───────────┤                       ├───────────┤
│ 10        │                       │ 5         │
│ Capricorn │                       │ Leo       │
│ மகரம்     │                       │ சிம்மம்   │
├───────────┼───────────┬───────────┼───────────┤
│ 9         │ 8         │ 7         │ 6         │
│ Sagitt    │ Scorpio   │ Libra     │ Virgo     │
│ தனுசு     │ விருச்சிகம்│ துலாம்   │ கன்னி    │
└───────────┴───────────┴───────────┴───────────┘
```

## Rasi Positioning (Clockwise from Top-Left)

| Position | Rasi # | English Name | Tamil Name | Sign Symbol |
|----------|--------|--------------|------------|-------------|
| Top-L    | 12     | Pisces       | மீனம்      | ♓           |
| Top-LC   | 1      | Aries        | மேஷம்     | ♈           |
| Top-RC   | 2      | Taurus       | ரிஷபம்    | ♉           |
| Top-R    | 3      | Gemini       | மிதுனம்    | ♊           |
| Right-T  | 4      | Cancer       | கடகம்     | ♋           |
| Right-B  | 5      | Leo          | சிம்மம்   | ♌           |
| Btm-R    | 6      | Virgo        | கன்னி     | ♍           |
| Btm-RC   | 7      | Libra        | துலாம்    | ♎           |
| Btm-LC   | 8      | Scorpio      | விருச்சிகம்| ♏           |
| Btm-L    | 9      | Sagittarius  | தனுசு      | ♐           |
| Left-B   | 10     | Capricorn    | மகரம்     | ♑           |
| Left-T   | 11     | Aquarius     | கும்பம்    | ♒           |
| Center   | -      | Chart Title  | ராசி      | -           |

## Planet Abbreviations

Used in the charts to save space:

| Planet  | Tamil      | Abbreviation | Symbol |
|---------|------------|--------------|--------|
| Sun     | சூரியன்    | Su           | ☉      |
| Moon    | சந்திரன்  | Mo           | ☽      |
| Mars    | செவ்வாய்  | Ma           | ♂      |
| Mercury | புதன்     | Me           | ☿      |
| Jupiter | குரு      | Ju           | ♃      |
| Venus   | சுக்கிரன் | Ve           | ♀      |
| Saturn  | சனி      | Sa           | ♄      |
| Rahu    | ராகு      | Ra           | ☊      |
| Ketu    | கேது      | Ke           | ☋      |

## Example Chart with Planets

```
┌───────────┬───────────┬───────────┬───────────┐
│ 12        │ 1         │ 2         │ 3         │
│           │           │ Ra        │ Ma        │
│           │           │           │           │
├───────────┼───────────┴───────────┼───────────┤
│ 11        │                       │ 4         │
│           │        ராசி            │ லக் Su Me │
│           │     Rasi Chart        │           │
├───────────┤                       ├───────────┤
│ 10        │                       │ 5         │
│           │                       │ Ve        │
│           │                       │           │
├───────────┼───────────┬───────────┼───────────┤
│ 9         │ 8         │ 7         │ 6         │
│           │ Ju Ke Ma  │ Mo Sa     │           │
│           │           │           │           │
└───────────┴───────────┴───────────┴───────────┘
```

**Interpretation:**
- **Lagna (Ascendant)** is in Cancer (4th box) - marked with "லக்"
- **Sun (Su)** and **Mercury (Me)** are in Cancer with Lagna
- **Rahu (Ra)** is in Taurus (2nd box)
- **Mars (Ma)** is in Gemini (3rd box)
- **Venus (Ve)** is in Leo (5th box)
- **Moon (Mo)** and **Saturn (Sa)** are in Libra (7th box)
- **Jupiter (Ju)**, **Ketu (Ke)**, and **Mars (Ma)** are in Scorpio (8th box)

## Lagna Marker

The Lagna (Ascendant) is marked with a red badge:

```
┌─────────┐
│  லக்    │  ← Red badge with Tamil "Lak"
│ Su Me   │  ← Planets in the same Rasi
└─────────┘
```

## Color Scheme

### Reference CSS Colors:
```css
/* Border color */
border: 1px solid #F88857;  /* Coral orange */

/* Background gradient */
background: radial-gradient(#FFFDE9, #FFFCD5);  /* Cream colors */

/* Planet text */
color: #800080;  /* Purple */

/* Lagna badge */
background: #DC3545;  /* Red */
color: #FFFFFF;      /* White text */
```

### In WPF:
```csharp
// Border
Color.FromRgb(0xF8, 0x88, 0x57)  // #F88857

// Background gradient
RadialGradientBrush(
    Color.FromRgb(0xFF, 0xFD, 0xE9),  // Center
    Color.FromRgb(0xFF, 0xFC, 0xD5)   // Outer
)

// Planet text
Color.FromRgb(0x80, 0x00, 0x80)  // Purple

// Lagna badge
Color.FromRgb(0xDC, 0x35, 0x45)  // Red
```

## Navamsa Chart (D-9)

The Navamsa chart follows the exact same layout as the Rasi chart, but:
- Uses `NavamsaPlanets` data instead of `Planets`
- Center title shows "நவாம்சம் Navamsa (D-9)"
- No Lagna marker (Navamsa Lagna is calculated differently)

```
┌───────────┬───────────┬───────────┬───────────┐
│ 12        │ 1         │ 2         │ 3         │
│           │           │           │           │
├───────────┼───────────┴───────────┼───────────┤
│ 11        │                       │ 4         │
│           │     நவாம்சம்          │           │
│           │     Navamsa           │           │
│           │      (D-9)            │           │
├───────────┤                       ├───────────┤
│ 10        │                       │ 5         │
│           │                       │           │
├───────────┼───────────┬───────────┼───────────┤
│ 9         │ 8         │ 7         │ 6         │
│           │           │           │           │
└───────────┴───────────┴───────────┴───────────┘
```

## Dimensions

### Canvas Size:
- **Total**: 400x400 pixels
- **Each box**: 100x100 pixels
- **Grid**: 4x4 (12 boxes + center 2x2 area)

### Box Layout:
```
Box Size: 100px × 100px

Column 0: x = 0
Column 1: x = 100
Column 2: x = 200
Column 3: x = 300

Row 0: y = 0
Row 1: y = 100
Row 2: y = 200
Row 3: y = 300

Center Area: 
  x = 100 to 300 (200px wide)
  y = 100 to 300 (200px tall)
```

## Typography

### Planet Text:
- **Font Size**: 10px
- **Weight**: Normal
- **Color**: Purple (#800080)
- **Width**: 30px per planet
- **Layout**: Horizontal wrap

### Lagna Badge:
- **Font Size**: 9px
- **Weight**: Bold
- **Color**: White
- **Background**: Red (#DC3545)
- **Padding**: 6px horizontal, 2px vertical
- **Border Radius**: 3px

### Center Title:
- **Font Size**: 12-14px
- **Weight**: Bold
- **Color**: Purple
- **Lines**: Multi-line (Tamil + English)
- **Alignment**: Center

## Usage in Application

### Display Rasi Chart:
```csharp
var rasiChart = new RasiChartControl();
rasiChart.DrawChart(horoscope);
rasiChartContainer.Child = rasiChart;
```

### Display Navamsa Chart:
```csharp
var navamsaChart = new NavamsaChartControl();
navamsaChart.DrawChart(horoscope);
navamsaChartContainer.Child = navamsaChart;
```

## Traditional Significance

### Why This Layout?
The South Indian style chart:
1. **Fixed House Positions**: Rasis never move, making it easy to remember positions
2. **Clockwise Flow**: Natural reading pattern for Tamil/Sanskrit readers
3. **Center Title**: Clear identification of chart type
4. **Visual Balance**: Symmetric layout appeals to traditional aesthetics
5. **Easy Comparison**: Multiple charts can be compared side-by-side

### Advantages:
- ✅ Rasi positions are FIXED - easy to memorize
- ✅ Houses rotate based on Lagna
- ✅ Clear visual distinction between signs
- ✅ Traditional Tamil astrology standard
- ✅ Used across South India (Tamil Nadu, Kerala, Karnataka, Andhra)

### Disadvantages:
- ❌ House numbers not explicitly shown
- ❌ Requires memorization of Rasi positions
- ❌ Less intuitive for North Indian astrologers
- ❌ Center area "wastes" space

## Comparison: North vs South Indian Style

### North Indian Style:
```
    \  12  / 1  \
     ┌────┬────┐
 11  │    │    │ 2
    ├────┼────┤
 10  │    │    │ 3
     └────┴────┘
    /  9  \ 8  /
```
- Houses are FIXED (1-12)
- Rasis rotate based on Lagna
- Diamond/rhombus shape

### South Indian Style (Our Implementation):
```
┌───┬───┬───┬───┐
│12 │ 1 │ 2 │ 3 │
├───┼───┴───┼───┤
│11 │ Title │ 4 │
├───┤       ├───┤
│10 │       │ 5 │
├───┼───┬───┼───┤
│ 9 │ 8 │ 7 │ 6 │
└───┴───┴───┴───┘
```
- Rasis are FIXED (1-12)
- Houses rotate based on Lagna
- Square grid shape

---

**Note**: This implementation follows the Tamil traditional style as used in major Tamil astrology websites and software. The reference HTML/CSS confirms this is the authentic South Indian format.
