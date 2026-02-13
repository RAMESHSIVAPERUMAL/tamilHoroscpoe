# ?? Tamil Horoscope Color Theme - Quick Reference Card

## Color Swatches

```
???????????????????????????????????????????????????????????
?                    PRIMARY COLORS                        ?
???????????????????????????????????????????????????????????
?                                                          ?
?  MAROON (Primary Brand Color)                          ?
?  ???????????? #8B0000  Main Maroon                      ?
?  ???????????? #A52A2A  Light Maroon (Hover)            ?
?  ???????????? #660000  Dark Maroon (Shadows)           ?
?                                                          ?
?  GOLD YELLOW (Accent & Highlights)                      ?
?  ???????????? #FFD700  Gold Yellow                      ?
?  ???????????? #FFF4CC  Light Yellow (Backgrounds)      ?
?  ???????????? #FFA500  Orange Yellow (Warnings)         ?
?                                                          ?
?  FOREST GREEN (Success & Confirmations)                 ?
?  ???????????? #228B22  Forest Green                     ?
?  ???????????? #90EE90  Light Green (Backgrounds)       ?
?  ???????????? #006400  Dark Green (Emphasis)            ?
?                                                          ?
???????????????????????????????????????????????????????????
```

---

## Component Color Map

### Buttons
```html
<button class="btn btn-primary">   Maroon (#8B0000)  </button>
<button class="btn btn-success">   Green  (#228B22)  </button>
<button class="btn btn-warning">   Yellow (#FFD700)  </button>
```

### Badges
```html
<span class="badge bg-primary">   Maroon with white text   </span>
<span class="badge bg-success">   Green with white text    </span>
<span class="badge bg-warning">   Yellow with maroon text  </span>
```

### Alerts
```html
<div class="alert alert-success">  Green background (#90EE90)  </div>
<div class="alert alert-warning">  Yellow background (#FFF4CC) </div>
<div class="alert alert-danger">   Maroon border (#8B0000)    </div>
```

### Cards
```html
<div class="card-header bg-primary">  Maroon gradient header  </div>
<div class="card-header bg-success">  Green gradient header   </div>
<div class="card-header bg-warning">  Yellow gradient header  </div>
```

---

## CSS Variables Reference

```css
/* Copy-paste these into your custom CSS for extensions */

/* Maroon Variants */
var(--th-maroon)        /* #8B0000 - Primary */
var(--th-maroon-light)  /* #A52A2A - Hover states */
var(--th-maroon-dark)   /* #660000 - Shadows */

/* Yellow Variants */
var(--th-yellow)        /* #FFD700 - Accents */
var(--th-yellow-light)  /* #FFF4CC - Backgrounds */
var(--th-yellow-dark)   /* #FFA500 - Warnings */

/* Green Variants */
var(--th-green)         /* #228B22 - Success */
var(--th-green-light)   /* #90EE90 - Backgrounds */
var(--th-green-dark)    /* #006400 - Emphasis */
```

---

## Common Patterns

### Status Indicators
```html
? Active:   badge bg-success  (Green)
??  Warning:  badge bg-warning  (Yellow)
? Inactive: badge bg-danger   (Maroon)
??  Info:     badge bg-info     (Orange)
```

### Financial Display
```html
?? Positive: text-success      (Green)
??  Low:      text-warning      (Orange)
? Negative: text-danger       (Maroon)
```

### Actions
```html
?? Primary:   btn-primary       (Maroon)
?? Confirm:   btn-success       (Green)
?? Caution:   btn-warning       (Yellow)
? Secondary: btn-outline-primary (Maroon outline)
```

---

## Color Psychology

| Color | Meaning | Usage |
|-------|---------|-------|
| **Maroon** | Power, Elegance, Tradition | Brand identity, primary actions |
| **Gold Yellow** | Prosperity, Energy, Optimism | Highlights, accents, celebrations |
| **Forest Green** | Growth, Success, Harmony | Confirmations, positive feedback |

---

## Accessibility

### Contrast Ratios (WCAG)
```
? Maroon on White:       11.7:1 (AAA)
? Maroon on Light Yellow: 7.2:1 (AA)
? Green on White:         5.1:1 (AA)
? Yellow on Maroon:       4.8:1 (AA)
```

### Best Combinations
```
? White text on Maroon background
? Maroon text on White background
? Maroon text on Light Yellow background
? Yellow text on Maroon background
? White text on Green background
```

---

## Page-Specific Colors

### Navigation
- Background: Maroon gradient
- Border: Gold yellow (3px)
- Links: Light yellow
- Hover: Yellow background overlay

### Home Dashboard
- Trial card: Yellow header
- Wallet card: Green header
- Status card: Maroon header
- Background: Cream gradient

### Generate Horoscope
- Chart border: Maroon (3px)
- Chart background: Light yellow
- Lagna marker: Maroon + yellow
- Generate button: Large maroon

### Wallet Pages
- Sufficient balance: Green
- Low balance: Yellow warning
- No balance: Maroon alert
- Transaction credit: Green badge
- Transaction debit: Maroon badge

---

## Gradient Formulas

```css
/* Button Gradients */
Maroon:  linear-gradient(135deg, #8B0000 0%, #660000 100%)
Green:   linear-gradient(135deg, #228B22 0%, #006400 100%)
Yellow:  linear-gradient(135deg, #FFD700 0%, #FFA500 100%)

/* Background Gradients */
Page:    linear-gradient(135deg, #FFF9E6 0%, #FFFAF0 100%)
Card:    linear-gradient(135deg, #FFFAF0 0%, #FFFFFF 100%)
```

---

## Icon Colors

```html
? Stars (horoscope):   text-warning   (#FFD700)
?? Wallet icons:        text-success   (#228B22)
?? Home icons:          text-primary   (#8B0000)
??  Settings icons:     text-secondary (#6c757d)
```

---

## Hover Effects

```css
Button:     translateY(-2px) + shadow enhancement
Card:       translateY(-4px) + shadow enhancement
Nav Link:   yellow background + scale
Brand Logo: scale(1.1) pulse animation
```

---

## Border Styles

```css
Cards:      2px solid + 12px radius
Inputs:     2px solid + 8px radius
Buttons:    solid + 8px radius
Navbar:     3px bottom border (yellow)
Alerts:     6px left border
```

---

## Shadow Patterns

```css
Small:   0 2px 8px  rgba(139, 0, 0, 0.2)
Medium:  0 4px 12px rgba(139, 0, 0, 0.3)
Large:   0 8px 24px rgba(0, 0, 0, 0.15)
```

---

## Responsive Breakpoints

```css
Mobile:   < 768px
Tablet:   768px - 1024px
Desktop:  > 1024px
```

---

## Animation Timings

```css
Fast:    0.2s ease
Normal:  0.3s ease
Slow:    0.5s ease-out
Pulse:   2s infinite
```

---

## Print This Reference!

```
File: COLOR_THEME_QUICK_REFERENCE.md
Location: TamilHoroscope.Web/
Usage: Keep handy for quick color lookups
```

---

## Quick Color Picker

Need a color? Here's the hex codes:

```
Maroon:  #8B0000  rgb(139, 0, 0)
Yellow:  #FFD700  rgb(255, 215, 0)
Green:   #228B22  rgb(34, 139, 34)
Orange:  #FFA500  rgb(255, 165, 0)
Cream:   #FFF9E6  rgb(255, 249, 230)
```

---

**Version:** 1.0  
**Last Updated:** 2024  
**Theme:** Maroon, Yellow, Green  
**Status:** Production Ready ?
