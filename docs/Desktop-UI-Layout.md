# Tamil Horoscope Desktop UI - Visual Layout

## Main Window Layout

```
┌────────────────────────────────────────────────────────────────────────────┐
│                     Tamil Horoscope Calculator                              │
│                        தமிழ் ஜாதக கணிப்பு                                 │
└────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────┬─────────────────────────────────────────────────────┐
│  INPUT PANEL         │  RESULTS PANEL                                      │
│  (Left - 400px)      │  (Right - Dynamic)                                  │
├──────────────────────┼─────────────────────────────────────────────────────┤
│                      │                                                     │
│ ┌──────────────────┐ │  ┌────────────────────────────────────────────┐   │
│ │ Birth Details    │ │  │ Panchangam - பஞ்சாங்கம்                   │   │
│ │ பிறப்பு விவரங்கள்│ │  │                                            │   │
│ ├──────────────────┤ │  │ Location: Chennai                          │   │
│ │ Name / பெயர்     │ │  │ Date/Time: 2024-01-01 10:00:00            │   │
│ │ [Text Input]     │ │  │ Coordinates: 13.0827°N, 80.2707°E         │   │
│ │                  │ │  │                                            │   │
│ │ Date / தேதி      │ │  │ Tamil Month: தை                            │   │
│ │ [Date Picker]    │ │  │ Vara: Monday (திங்கள்)                    │   │
│ │                  │ │  │ Tithi: Ekadashi (ஏகாதசி)                 │   │
│ │ Time / நேரம்     │ │  │ ...                                        │   │
│ │ [HH:mm:ss]       │ │  └────────────────────────────────────────────┘   │
│ │                  │ │                                                     │
│ │ Place / இடம்     │ │  ┌────────────────────────────────────────────┐   │
│ │ [Text Input]     │ │  │ Lagna (Ascendant) - லக்னம்                 │   │
│ │                  │ │  │                                            │   │
│ │ Lat / அட்சரேகை  │ │  │ Rasi: Aries (மேஷம்)                        │   │
│ │ [13.0827]        │ │  │ Longitude: 12.45°                         │   │
│ │                  │ │  └────────────────────────────────────────────┘   │
│ │ Lon / தீர்க்கரேகை│ │                                                     │
│ │ [80.2707]        │ │  ┌────────────────────────────────────────────┐   │
│ │                  │ │  │ Navagraha Positions - நவகிரக நிலைகள்      │   │
│ │ Timezone / மண்டலம│ │  │                                            │   │
│ │ [5.5]            │ │  │ ┌────────┬────────┬────────┬──────┬───┐   │   │
│ └──────────────────┘ │  │ │Planet  │Rasi    │Nakshatra│House│R  │   │   │
│                      │  │ ├────────┼────────┼────────┼──────┼───┤   │   │
│ ┌──────────────────┐ │  │ │Sun     │Capricorn│Uttara.│10   │No │   │   │
│ │▼ Advanced Options│ │  │ │Moon    │Pisces  │Revati  │12   │No │   │   │
│ │  விருப்பங்கள்    │ │  │ │Mars    │Aquarius│Dhanish │11   │No │   │   │
│ ├──────────────────┤ │  │ │...     │...     │...     │...  │...│   │   │
│ │ Vimshottari Dasa │ │  │ └────────┴────────┴────────┴──────┴───┘   │   │
│ │                  │ │  └────────────────────────────────────────────┘   │
│ │ ☑ Calculate Dasa │ │                                                     │
│ │                  │ │  ┌────────────────────────────────────────────┐   │
│ │ Years: [50▼]     │ │  │ Houses (Bhavas) - பாவங்கள்                │   │
│ │                  │ │  │                                            │   │
│ │ Navamsa Chart    │ │  │ ┌─────┬──────┬────────┬──────┬────────┐  │   │
│ │                  │ │  │ │House│Rasi  │Lord    │Planets       │  │   │
│ │ ☑ Calculate      │ │  │ ├─────┼──────┼────────┼──────┼────────┤  │   │
│ │   Navamsa        │ │  │ │1    │Aries │Mars    │Sun, Mercury  │  │   │
│ │                  │ │  │ │2    │Taurus│Venus   │-             │  │   │
│ │ ☑ Show Details   │ │  │ │...  │...   │...     │...           │  │   │
│ └──────────────────┘ │  │ └─────┴──────┴────────┴──────┴────────┘  │   │
│                      │  └────────────────────────────────────────────┘   │
│ ┌──────────────────┐ │                                                     │
│ │  Calculate       │ │  ┌────────────────────────────────────────────┐   │
│ │  Horoscope       │ │  │ Navamsa Chart (D-9) - நவாம்சம்             │   │
│ │  ஜாதகம் கணக்கிடு│ │  │                                            │   │
│ └──────────────────┘ │  │ [Placeholder for future implementation]    │   │
│                      │  └────────────────────────────────────────────┘   │
│ ┌──────────────────┐ │                                                     │
│ │  Export to PDF   │ │  ┌────────────────────────────────────────────┐   │
│ │  PDF ஆக சேமி    │ │  │ Vimshottari Dasa - விம்சோத்தரி தசா        │   │
│ └──────────────────┘ │  │                                            │   │
│                      │  │ [Placeholder for future implementation]    │   │
│ [Status: Ready]      │  └────────────────────────────────────────────┘   │
│                      │                                                     │
└──────────────────────┴─────────────────────────────────────────────────────┘
┌────────────────────────────────────────────────────────────────────────────┐
│  Tamil Horoscope Calculator v1.0 - Using Swiss Ephemeris                  │
└────────────────────────────────────────────────────────────────────────────┘
```

## Color Scheme

### Primary Colors
- **Header Background**: Blue (#2196F3)
- **Button Primary**: Blue (#2196F3)
- **Button Export**: Green (#4CAF50)
- **Background**: Light Gray (#F5F5F5)

### Text Colors
- **Headers**: Dark Gray (#212121)
- **Labels**: Medium Gray (#424242)
- **Status Success**: Green
- **Status Error**: Red

### Accent Colors
- **Hover**: Darker Blue (#1976D2)
- **Border**: Light Gray (#E0E0E0)
- **Alternate Rows**: Very Light Gray (#F5F5F5)

## Component Details

### Input Panel (Left)

**Width**: 400px fixed
**Background**: Scrollable container

**Sections**:
1. **Birth Details Card**
   - White background with rounded corners
   - Shadow effect for depth
   - Input fields with validation
   - Tooltips on hover

2. **Advanced Options Expander**
   - Collapsible section
   - Starts collapsed by default
   - Contains checkboxes and dropdowns
   - Organized into sub-sections

3. **Action Buttons**
   - Calculate button (Blue)
   - Export button (Green, initially disabled)
   - Full width buttons
   - Hover effects

4. **Status Bar**
   - Appears only when there's a status message
   - Yellow background for visibility
   - Orange text for warnings
   - Green/Red for success/error

### Results Panel (Right)

**Width**: Dynamic (fills remaining space)
**Background**: Scrollable container

**Sections**:
1. **Panchangam Card**
   - White background
   - Shows location and time details
   - All 5 Panchangam elements
   - Bilingual labels

2. **Lagna Card**
   - Compact display
   - Rasi and longitude

3. **Navagraha DataGrid**
   - Sortable columns
   - Alternating row colors
   - Shows all 9 planets
   - Includes retrograde status

4. **Houses DataGrid**
   - Shows all 12 houses
   - Lists occupying planets
   - Tamil and English signs

5. **Navamsa Card** (conditional)
   - Shows when enabled
   - Placeholder text for future implementation
   - Italic style to indicate future feature

6. **Vimshottari Dasa Card** (conditional)
   - Shows when enabled
   - Placeholder text for future implementation
   - Italic style to indicate future feature

## Interaction Flow

```
User Opens App
    ↓
Sees Default Values (Chennai)
    ↓
[Optional] Modifies Birth Details
    ↓
[Optional] Opens Advanced Options
    ↓
[Optional] Configures Dasa/Navamsa Settings
    ↓
Clicks Calculate (or presses F5)
    ↓
Validation Check
    ├─ Fail → Show Error Message
    └─ Pass → Calculate Horoscope
              ↓
         Display Results in Right Panel
              ↓
         Enable Export Button
              ↓
    [Optional] Click Export (or press Ctrl+E)
              ↓
         Choose Save Location
              ↓
         Generate PDF
              ↓
         Show Success Message
```

## Responsive Behavior

- **Minimum Size**: 1200x700 pixels
- **Default Size**: 1400x900 pixels
- **Resizable**: Yes
- **Layout**: Two-column with fixed left panel
- **Scrolling**: Both panels scroll independently
- **Content Adaptation**: Right panel expands to fill space

## Accessibility Features

1. **Keyboard Navigation**
   - Tab order follows logical flow
   - All controls accessible via keyboard
   - Shortcut keys for main actions

2. **Screen Reader Support**
   - All labels properly associated
   - Tooltips provide additional context
   - Status updates announced

3. **Visual Accessibility**
   - High contrast text
   - Clear focus indicators
   - Sufficient font sizes
   - Color not sole indicator

## State Management

### Application States

1. **Initial State**
   - Default values shown
   - Calculate button enabled
   - Export button disabled
   - Results panel hidden

2. **Calculating State**
   - Status shows "Calculating..."
   - Brief loading indication
   - UI remains responsive

3. **Results State**
   - Results panel visible
   - All data populated
   - Export button enabled
   - Status shows success

4. **Error State**
   - Error message in status bar
   - Red text color
   - Validation errors highlighted
   - Export button remains disabled

## Future UI Enhancements

Planned visual improvements:

1. **Chart Visualization**
   - South Indian chart style (diamond)
   - North Indian chart style (square)
   - Interactive planet positions

2. **Theme Support**
   - Light theme (current)
   - Dark theme
   - High contrast theme

3. **Animations**
   - Smooth transitions
   - Loading indicators
   - Result appearance effects

4. **Responsive Design**
   - Tablet mode layout
   - Mobile-friendly views
   - Adaptive panel sizing

---

**Note**: This is a text-based visual representation. The actual application features smooth gradients, shadows, and professional styling that creates a modern, polished appearance.
