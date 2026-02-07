# Tamil Horoscope - Documentation Index

## Essential Documentation (Keep These)

### 1. Getting Started
- **README.md** - Project overview, installation, quick start

### 2. Core Features
- **Phase2-CalculationEngine.md** - Core calculation methodology
- **Desktop-UserGuide.md** - How to use the application

### 3. Technical Reference
- **Desktop-Technical.md** - WPF implementation details
- **Complete-Parasara-Shadbala-Implementation.md** - Shadbala calculations
- **Vimshottari-Dasa.md** - Dasa/Bhukti period calculations

### 4. Chart Guides
- **Rasi-Chart-Format.md** - South Indian chart layout
- **Navamsa-D9-Chart.md** - Navamsa divisional chart

---

## Files to Archive (Move to docs/archive/)

These are historical/duplicate documentation files that should be archived:

### Bug Fixes & Iterations
- Chart-Improvements-SouthIndian-Style.md
- Chart-PDF-Export-Fix.md
- Chart-Visual-Guide.md
- House-Calculation-Fix.md
- Lagna-In-Planet-Table-Update.md
- Navagraha-Positions-Enhancement.md
- PDF-Export-Enhancement.md
- PDF-Export-Fix-Summary.md
- PDF-Export-Update.md
- Planetary-Positions-Complete-Enhancement.md
- QUICK-START-FIX.md
- Rasi-Chart-Correction-Note.md
- Rasi-Chart-Fix-Applied.md
- Rasi-Chart-Fix-Summary.md
- Rasi-Chart-Implementation-Summary.md
- Rasi-Chart-Visual-Verification.md
- Shadbala-Changes-Summary.md
- Shadbala-Improvements-Final.md
- Simplified-Display-Summary.md
- URGENT-Rasi-Chart-Fix-Steps.md
- Visual-Comparison-Before-After.md

### Duplicates & Summaries
- Complete-Shadbala-Implementation.md (duplicate of Complete-Parasara-Shadbala-Implementation.md)
- Complete-Swiss-Ephemeris-Display.md (covered in Phase2-CalculationEngine.md)
- Desktop-UI-Layout.md (covered in Desktop-Technical.md)
- Desktop-UI-Mockup.md (no longer needed)
- Documentation-Cleanup-Summary.md (this replaces it)
- Kala-Bala-Accurate-Implementation.md (covered in Complete-Parasara-Shadbala-Implementation.md)
- Phase2-Summary.md (covered in Phase2-CalculationEngine.md)
- Phase2-Verification.md (testing complete)
- Phase3-Summary.md (features complete)
- Planetary-Strength-Shadbala.md (covered in Complete-Parasara-Shadbala-Implementation.md)

---

## Recommended Documentation Structure

```
docs/
??? README.md                                      # Main documentation index
??? QUICK-START.md                                 # 5-minute getting started guide
??? USER-GUIDE.md                                  # End-user guide (from Desktop-UserGuide.md)
??? TECHNICAL-REFERENCE.md                         # Developer reference
?
??? features/
?   ??? calculation-engine.md                      # Core calculations (from Phase2-CalculationEngine.md)
?   ??? shadbala-planetary-strength.md             # Simplified Shadbala guide
?   ??? vimshottari-dasa.md                        # Dasa/Bhukti calculations
?   ??? rasi-chart.md                              # Rasi chart format
?   ??? navamsa-chart.md                           # Navamsa chart format
?
??? api/
?   ??? calculation-api.md                         # PanchangCalculator API
?   ??? models.md                                  # Data models reference
?   ??? utilities.md                               # Helper utilities
?
??? archive/                                       # Historical documentation
    ??? [all bug fix and iteration docs]
```

---

## Action Items

### 1. Create Archive Folder
```bash
mkdir docs\archive
```

### 2. Move Historical Files
Move all files listed under "Files to Archive" to `docs\archive\`

### 3. Create Consolidated Files

#### QUICK-START.md
- Installation steps
- First calculation example
- Basic usage

#### USER-GUIDE.md  
- Consolidate from Desktop-UserGuide.md
- Add screenshots
- Common workflows

#### TECHNICAL-REFERENCE.md
- Consolidate from Desktop-Technical.md
- API reference
- Architecture overview

#### features/shadbala-planetary-strength.md
- Simplified from Complete-Parasara-Shadbala-Implementation.md
- Focus on understanding, not implementation details
- Keep formula explanations simple

### 4. Update README.md
- Link to new documentation structure
- Quick navigation
- Feature highlights

---

## Benefits of Cleanup

? **Easier Navigation** - Clear hierarchy  
? **Less Duplication** - One source of truth  
? **Better Maintenance** - Update one file instead of many  
? **Faster Onboarding** - New users find info quickly  
? **Historical Preservation** - Archive keeps bug fix history  

---

**Date**: February 7, 2026  
**Action**: Documentation Cleanup Plan  
**Status**: Ready to Execute
