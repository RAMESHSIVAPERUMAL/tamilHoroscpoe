# Tamil Horoscope Calculator - Documentation

## ?? Quick Navigation

| For... | Start Here |
|--------|------------|
| **First-time Users** | [QUICK-START.md](QUICK-START.md) - Get running in 5 minutes |
| **End Users** | [Desktop-UserGuide.md](Desktop-UserGuide.md) - How to use the app |
| **Developers** | [Phase2-CalculationEngine.md](Phase2-CalculationEngine.md) - Technical reference |
| **Understanding Shadbala** | [SHADBALA-SIMPLE-GUIDE.md](SHADBALA-SIMPLE-GUIDE.md) - Simplified guide |

---

## ?? Essential Documentation

### Getting Started
1. **[QUICK-START.md](QUICK-START.md)** - 5-minute setup guide
   - Prerequisites
   - Installation
   - First calculation
   - Common features

2. **[Desktop-UserGuide.md](Desktop-UserGuide.md)** - Complete user manual
   - Detailed workflows
   - All features explained
   - Troubleshooting
   - Tips and tricks

### Technical Reference

3. **[Phase2-CalculationEngine.md](Phase2-CalculationEngine.md)** - Core calculations
   - Swiss Ephemeris integration
   - Panchangam methodology
   - Horoscope calculations
   - Ayanamsa (Lahiri)
   - Technical specifications

4. **[Desktop-Technical.md](Desktop-Technical.md)** - WPF implementation
   - Architecture
   - UI components
   - Data binding
   - PDF export
   - Code structure

### Feature Guides

5. **[SHADBALA-SIMPLE-GUIDE.md](SHADBALA-SIMPLE-GUIDE.md)** ? NEW
   - **Human-friendly explanation** of planetary strength
   - Six types of strength explained simply
   - Real examples with interpretation
   - Remedies for weak planets
   - **AI-friendly structure** for future fine-tuning

6. **[Vimshottari-Dasa.md](Vimshottari-Dasa.md)** - Dasa/Bhukti periods
   - Calculation methodology
   - 120-year cycle
   - Current period identification
   - Interpretation

7. **[Rasi-Chart-Format.md](Rasi-Chart-Format.md)** - South Indian chart
   - Traditional layout
   - Fixed positions
   - Visual format

8. **[Navamsa-D9-Chart.md](Navamsa-D9-Chart.md)** - D-9 divisional chart
   - Calculation formulas
   - Element-based starting signs
   - Interpretation

---

## ??? Documentation Structure

```
docs/
??? README.md                          # This file - Documentation index
??? QUICK-START.md                     # 5-minute getting started
??? SHADBALA-SIMPLE-GUIDE.md          # ? Simplified planetary strength guide
?
??? Desktop-UserGuide.md               # End-user manual
??? Desktop-Technical.md               # Developer technical reference
?
??? Phase2-CalculationEngine.md        # Core calculation engine
??? Vimshottari-Dasa.md               # Dasa/Bhukti periods
??? Rasi-Chart-Format.md              # Rasi chart layout
??? Navamsa-D9-Chart.md               # Navamsa chart
?
??? Complete-Parasara-Shadbala-*.md   # Detailed Shadbala implementation
?
??? archive/                           # Historical documentation
    ??? Chart-Improvements-*.md        # Chart iteration history
    ??? PDF-Export-*.md                # PDF export improvements
    ??? Rasi-Chart-Fix-*.md           # Chart bug fixes
    ??? ... (30+ historical docs)
```

---

## ? Current Features

### Core Calculations ?
- ? **Panchangam** - Tithi, Nakshatra, Yoga, Karana, Vara, Tamil Month
- ? **Horoscope** - Lagna, 9 Navagraha positions, 12 Houses
- ? **Navamsa (D-9)** - Complete divisional chart
- ? **Shadbala** - Six-fold planetary strength (Parasara method)
- ? **Vimshottari Dasa** - 120-year Dasa/Bhukti periods

### Desktop Application ?
- ? **Modern WPF UI** - Clean, professional interface
- ? **Online/Offline Place Search** - 600+ cities + worldwide search
- ? **South Indian Charts** - Traditional Rasi & Navamsa visualization
- ? **PDF Export** - Complete horoscope reports with charts
- ? **Bilingual** - Tamil & English throughout
- ? **Keyboard Shortcuts** - F5 (Calculate), Ctrl+E (Export)

### Accuracy ?
- ? **Swiss Ephemeris** - NASA-grade astronomical accuracy
- ? **Lahiri Ayanamsa** - Standard for Vedic/Tamil astrology
- ? **Parasara Method** - Classical Shadbala calculations
- ? **Verified** - Cross-checked with drikpanchang.com & prokerala.com

---

## ?? Quick Start Example

**Input**:
```
Name: Ramesh
Date: July 18, 1983
Time: 06:35:00
Place: Kumbakonam
```

**Output** (Sample):
- ?? Lagna: Cancer (?????) at 98.96°
- ?? Sun: Cancer at 91.25°
- ?? Moon: Libra at 192.67°
- ? Nakshatra: Swati (??????)
- ?? Tamil Month: Aadi (???)
- ?? Strongest Planet: Mercury (424.8 R)

---

## ?? Technical Stack

| Component | Technology |
|-----------|------------|
| **Framework** | .NET 8.0 |
| **Language** | C# 12.0 |
| **UI** | WPF (Windows Presentation Foundation) |
| **Astronomy** | SwissEphNet 2.8.0.2 |
| **PDF** | iTextSharp.LGPLv2.Core 3.4.23 |
| **Testing** | xUnit |

---

## ?? Test Coverage

| Category | Tests | Status |
|----------|-------|--------|
| **Panchangam** | 7 tests | ? All passing |
| **Horoscope** | 5 tests | ? All passing |
| **Navamsa** | 41 tests | ? All passing |
| **Shadbala** | 15 tests | ? All passing |
| **Dasa** | 8 tests | ? All passing |
| **Total** | 76+ tests | ? 100% pass rate |

---

## ?? What Makes This Special

1. **Accurate** - Swiss Ephemeris + Lahiri Ayanamsa = NASA-grade accuracy
2. **Traditional** - Follows Brihat Parashara Hora Shastra (classical Vedic texts)
3. **Complete** - Full Shadbala, Navamsa, Dasa/Bhukti - not just basic calculations
4. **Bilingual** - Tamil + English throughout (??????, ????, ???????????)
5. **Professional** - PDF reports with charts, suitable for practicing astrologers
6. **Open Source** - Full code available for learning and customization

---

## ?? Learning Path

### Beginner
1. Start with [QUICK-START.md](QUICK-START.md)
2. Try a sample calculation
3. Read [Desktop-UserGuide.md](Desktop-UserGuide.md)

### Intermediate
1. Understand [SHADBALA-SIMPLE-GUIDE.md](SHADBALA-SIMPLE-GUIDE.md)
2. Learn [Vimshottari-Dasa.md](Vimshottari-Dasa.md)
3. Study [Navamsa-D9-Chart.md](Navamsa-D9-Chart.md)

### Advanced
1. Deep dive into [Phase2-CalculationEngine.md](Phase2-CalculationEngine.md)
2. Review [Desktop-Technical.md](Desktop-Technical.md)
3. Explore [Complete-Parasara-Shadbala-Implementation.md](Complete-Parasara-Shadbala-Implementation.md)

---

## ?? Support

**Issues**: [GitHub Issues](https://github.com/RAMESHSIVAPERUMAL/tamilHoroscpoe/issues)  
**Repository**: https://github.com/RAMESHSIVAPERUMAL/tamilHoroscpoe  
**Documentation**: This folder

---

## ?? Version History

| Version | Date | Highlights |
|---------|------|------------|
| **1.0** | Feb 2, 2026 | Core calculation engine |
| **1.5** | Feb 4, 2026 | Desktop UI + PDF export |
| **1.6** | Feb 4, 2026 | Chart visualization |
| **1.7** | Feb 6, 2026 | Complete Shadbala |
| **1.8** | Feb 7, 2026 | Vimshottari Dasa |
| **2.0** | Feb 7, 2026 | **Documentation cleanup** ? |

---

## ?? Recent Updates

### February 7, 2026 - Documentation Cleanup ?

**What's New**:
- ? Created **[SHADBALA-SIMPLE-GUIDE.md](SHADBALA-SIMPLE-GUIDE.md)** - Human-friendly strength guide
- ? Created **[QUICK-START.md](QUICK-START.md)** - 5-minute setup guide
- ? Moved 30+ historical docs to `archive/` folder
- ? Simplified documentation structure
- ? Added quick navigation table
- ? Updated this README

**Benefits**:
- ?? Easier to find what you need
- ?? Clear learning path
- ??? Historical docs preserved but out of the way
- ?? AI-friendly structure for future improvements

---

## ?? Future Plans

### Short Term
- [ ] Add more divisional charts (D-2, D-3, D-10, D-12)
- [ ] North Indian chart style option
- [ ] Chart export as images (PNG/JPG)

### Medium Term
- [ ] Transit calculations
- [ ] Yoga detection (Raja Yoga, Dhana Yoga, etc.)
- [ ] Muhurta (auspicious timing) calculations

### Long Term
- [ ] Web application version
- [ ] Mobile apps (iOS/Android via MAUI)
- [ ] Database for storing multiple horoscopes
- [ ] Comparison/matching (Kundali Milan)

---

**Last Updated**: February 7, 2026  
**Documentation Version**: 2.0  
**Status**: ? Production Ready + Documentation Cleaned Up  
**Author**: RAMESHSIVAPERUMAL
