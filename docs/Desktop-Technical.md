# Tamil Horoscope Desktop Application - Technical Documentation

## Architecture

### Project Structure

```
TamilHoroscope.Desktop/
├── App.xaml                          # Application definition
├── App.xaml.cs                       # Application code-behind
├── AssemblyInfo.cs                   # Assembly metadata
├── MainWindow.xaml                   # Main window UI definition
├── MainWindow.xaml.cs                # Main window logic
├── Converters/
│   └── StringToVisibilityConverter.cs # Value converter for UI binding
└── TamilHoroscope.Desktop.csproj     # Project file
```

### Technology Stack

- **Framework**: .NET 8.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Target Platform**: Windows (net8.0-windows)
- **PDF Library**: iTextSharp.LGPLv2.Core 3.4.23
- **Core Library**: TamilHoroscope.Core (project reference)

### Design Pattern

The application follows the **Code-Behind Pattern** with:
- XAML for declarative UI
- C# code-behind for logic
- Direct event handling
- No MVVM framework (keeping it simple for Phase 3)

## UI Components

### Window Configuration

```xml
Title="Tamil Horoscope Calculator - தமிழ் ஜாதகம்"
Height="900" Width="1400"
MinHeight="700" MinWidth="1200"
WindowStartupLocation="CenterScreen"
Background="#F5F5F5"
```

### Resource Dictionary

Custom styles defined in `Window.Resources`:

1. **ModernButtonStyle** - Primary action buttons
2. **ExportButtonStyle** - Success/export actions
3. **ModernTextBoxStyle** - Text input fields
4. **ModernComboBoxStyle** - Dropdown selections
5. **SectionHeaderStyle** - Section titles
6. **LabelStyle** - Field labels
7. **ModernExpanderStyle** - Collapsible sections

### Layout Structure

```
Grid (Main Container)
├── Row 0: Header (Auto height)
├── Row 1: Content (Fill)
│   ├── Column 0: Input Panel (400px)
│   └── Column 1: Results Panel (Fill)
└── Row 2: Footer (Auto height)
```

## Key Features Implementation

### 1. Input Validation

```csharp
private bool ValidateInputs(out string errorMessage)
{
    // Date validation
    if (!dpBirthDate.SelectedDate.HasValue) { ... }
    
    // Time format validation
    if (!TimeSpan.TryParse(txtBirthTime.Text, out _)) { ... }
    
    // Numeric range validations
    if (lat < -90 || lat > 90) { ... }
    if (lon < -180 || lon > 180) { ... }
    if (tz < -12 || tz > 14) { ... }
    
    return true;
}
```

**Validation Rules**:
- Date: Required, must be selected
- Time: Format HH:mm:ss, parsed as TimeSpan
- Latitude: Numeric, -90 to 90
- Longitude: Numeric, -180 to 180
- Timezone: Numeric, -12 to 14

### 2. Birth Details Parsing

```csharp
private BirthDetails ParseBirthDetails()
{
    var date = dpBirthDate.SelectedDate!.Value;
    var time = TimeSpan.Parse(txtBirthTime.Text);
    var dateTime = date.Date.Add(time);
    
    return new BirthDetails
    {
        DateTime = dateTime,
        Latitude = double.Parse(txtLatitude.Text, CultureInfo.InvariantCulture),
        Longitude = double.Parse(txtLongitude.Text, CultureInfo.InvariantCulture),
        TimeZoneOffset = double.Parse(txtTimezone.Text, CultureInfo.InvariantCulture),
        PlaceName = txtPlaceName.Text
    };
}
```

**Key Points**:
- Uses `CultureInfo.InvariantCulture` for consistent number parsing
- Combines date and time into single DateTime
- All required fields from BirthDetails model

### 3. Horoscope Calculation

```csharp
private void BtnCalculate_Click(object sender, RoutedEventArgs e)
{
    try
    {
        UpdateStatus("Calculating horoscope...", true);
        
        // Validate
        if (!ValidateInputs(out string errorMessage)) { ... }
        
        // Parse
        var birthDetails = ParseBirthDetails();
        _currentBirthDetails = birthDetails;
        
        // Calculate
        _currentHoroscope = _calculator.CalculateHoroscope(birthDetails);
        
        // Display
        DisplayResults(_currentHoroscope, birthDetails);
        
        // Enable export
        btnExportPdf.IsEnabled = true;
        
        UpdateStatus("Horoscope calculated successfully!", true);
    }
    catch (Exception ex) { ... }
}
```

**Flow**:
1. Update status to "Calculating..."
2. Validate all inputs
3. Parse birth details
4. Call calculation engine
5. Display results
6. Enable export button
7. Update status to success

### 4. Results Display

#### Panchangam Display

```csharp
txtPanchangam.Text = $"Location: {birthDetails.PlaceName}\n" +
                     $"Date/Time: {birthDetails.DateTime:yyyy-MM-dd HH:mm:ss}\n" +
                     $"Coordinates: {birthDetails.Latitude:F4}°N, {birthDetails.Longitude:F4}°E\n\n" +
                     $"Tamil Month: {horoscope.Panchang.TamilMonth}\n" +
                     // ... more fields
```

#### Data Grid Binding

```csharp
// Planets - direct binding
dgPlanets.ItemsSource = horoscope.Planets;

// Houses - with transformation
var housesDisplay = horoscope.Houses.Select(h => new
{
    h.HouseNumber,
    h.RasiName,
    h.TamilRasiName,
    h.Lord,
    PlanetsDisplay = h.Planets.Count > 0 ? string.Join(", ", h.Planets) : "-"
}).ToList();
dgHouses.ItemsSource = housesDisplay;
```

#### Conditional Sections

```csharp
// Show/hide based on checkbox
if (chkCalculateNavamsa.IsChecked == true)
{
    navamsaSection.Visibility = Visibility.Visible;
    // Update content
}
else
{
    navamsaSection.Visibility = Visibility.Collapsed;
}
```

### 5. PDF Export

#### Document Setup

```csharp
var document = new Document(PageSize.A4, 50, 50, 50, 50);
var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
document.Open();
```

**Page Setup**:
- Paper Size: A4
- Margins: 50 points (all sides)
- Orientation: Portrait

#### Content Generation

**Title Section**:
```csharp
var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 0, 255));
var title = new Paragraph("Tamil Horoscope - தமிழ் ஜாதகம்\n\n", titleFont);
title.Alignment = Element.ALIGN_CENTER;
document.Add(title);
```

**Tables**:
```csharp
// Planets table - 5 columns
var planetsTable = new PdfPTable(5);
planetsTable.WidthPercentage = 100;
planetsTable.SetWidths(new float[] { 2f, 2f, 2f, 2f, 1.5f });

// Add headers with background color
planetsTable.AddCell(new PdfPCell(new Phrase("Planet", cellFont)) 
{ 
    BackgroundColor = new BaseColor(211, 211, 211) 
});

// Add data rows
foreach (var planet in _currentHoroscope.Planets)
{
    planetsTable.AddCell(new PdfPCell(new Phrase($"{planet.Name}\n{planet.TamilName}", dataCellFont)));
    // ... more cells
}
```

**Footer**:
```csharp
var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, new BaseColor(128, 128, 128));
var footer = new Paragraph($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss} using Tamil Horoscope Calculator", footerFont);
footer.Alignment = Element.ALIGN_CENTER;
```

### 6. Status Management

```csharp
private void UpdateStatus(string message, bool isSuccess)
{
    txtStatus.Text = message;
    txtStatus.Foreground = isSuccess ? 
        System.Windows.Media.Brushes.Green : 
        System.Windows.Media.Brushes.Red;
}
```

**Status Colors**:
- Green: Success messages
- Red: Error messages
- Orange: Warning messages (in XAML)

### 7. Keyboard Shortcuts

```csharp
private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
{
    // F5 to calculate
    if (e.Key == System.Windows.Input.Key.F5)
    {
        BtnCalculate_Click(sender, e);
    }
    // Ctrl+E to export
    else if (e.Key == System.Windows.Input.Key.E && 
             (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
    {
        if (btnExportPdf.IsEnabled)
        {
            BtnExportPdf_Click(sender, e);
        }
    }
}
```

## Data Binding

### Value Converters

**StringToVisibilityConverter**:
```csharp
public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
{
    if (value is string str && !string.IsNullOrWhiteSpace(str))
    {
        return Visibility.Visible;
    }
    return Visibility.Collapsed;
}
```

**Usage in XAML**:
```xml
<Border Visibility="{Binding ElementName=txtStatus, Path=Text, Converter={StaticResource StringToVisibilityConverter}}">
    <TextBlock x:Name="txtStatus" />
</Border>
```

## Error Handling

### Try-Catch Blocks

All major operations wrapped in try-catch:
1. Calculate operation
2. Export operation
3. Validation operations

### User Feedback

**MessageBox** for critical errors:
```csharp
MessageBox.Show($"Error calculating horoscope:\n\n{ex.Message}", 
                "Calculation Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
```

**Status Bar** for informational messages:
```csharp
UpdateStatus("Horoscope calculated successfully!", true);
```

## Performance Considerations

### Calculation Speed
- Core calculation takes < 1 second
- UI updates are immediate
- No background threading needed for current workload

### Memory Management
- Calculation results stored in memory
- PDF generation creates temporary file
- Proper disposal of document objects

### UI Responsiveness
- ScrollViewers prevent layout issues
- Data grids handle large datasets efficiently
- Independent scrolling in left/right panels

## Accessibility Implementation

### Keyboard Navigation
```xml
<!-- Tab order flows naturally -->
<TextBox TabIndex="1" />
<TextBox TabIndex="2" />
<Button TabIndex="10" />
```

### Tooltips
```xml
<TextBox ToolTip="Enter the person's name" />
<CheckBox ToolTip="Enable Vimshottari Dasa calculation" />
```

### Semantic Elements
- Proper Label-Control associations
- Descriptive button text
- Clear section headers

## Testing Considerations

### Manual Testing Checklist

**Input Validation**:
- [ ] Empty date
- [ ] Invalid time format
- [ ] Out of range latitude
- [ ] Out of range longitude
- [ ] Out of range timezone

**Calculation**:
- [ ] Chennai coordinates
- [ ] Madurai coordinates
- [ ] Past dates
- [ ] Future dates
- [ ] Different timezones

**PDF Export**:
- [ ] Save to desktop
- [ ] Save to custom location
- [ ] Overwrite existing file
- [ ] Cancel save dialog

**UI Interactions**:
- [ ] Expand/collapse advanced options
- [ ] Toggle checkboxes
- [ ] Change dropdown selections
- [ ] Sort data grid columns
- [ ] Scroll both panels
- [ ] Resize window

**Keyboard Shortcuts**:
- [ ] F5 for calculate
- [ ] Ctrl+E for export
- [ ] Tab navigation
- [ ] Enter on buttons

## Future Enhancements

### Code Structure
- Implement MVVM pattern for better testability
- Add dependency injection
- Create view models for data binding
- Implement INotifyPropertyChanged

### New Features
- Database integration for birth chart storage
- Chart visualization (graphics)
- Comparison charts
- Transit calculations
- Print preview
- Multi-language support

### Performance
- Background threading for calculations
- Progress bar for long operations
- Async/await for file operations
- Caching of calculation results

### UI Improvements
- Theme switching
- Dark mode
- Responsive layout
- Touch support
- Custom controls for chart display

## Dependencies

### NuGet Packages

```xml
<PackageReference Include="iTextSharp.LGPLv2.Core" Version="3.4.23" />
```

### Project References

```xml
<ProjectReference Include="..\TamilHoroscope.Core\TamilHoroscope.Core.csproj" />
```

### Transitive Dependencies
- SwissEphNet (via TamilHoroscope.Core)
- System.Text.Encoding.CodePages (via iTextSharp)

## Build Configuration

### Project File

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net8.0-windows</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <UseWPF>true</UseWPF>
  <EnableWindowsTargeting>true</EnableWindowsTargeting>
</PropertyGroup>
```

**Key Properties**:
- **WinExe**: Windows application (not console)
- **UseWPF**: Enable WPF framework
- **EnableWindowsTargeting**: Allow cross-platform build
- **Nullable**: Enable nullable reference types

## Deployment

### Build Output

```
bin/Debug/net8.0-windows/
├── TamilHoroscope.Desktop.exe        # Main executable
├── TamilHoroscope.Desktop.dll        # Application assembly
├── TamilHoroscope.Core.dll           # Core library
├── iTextSharp.LGPLv2.Core.dll        # PDF library
├── SwissEphNet.dll                   # Ephemeris library
└── [Runtime dependencies]
```

### Distribution

**Required Files**:
1. All DLLs from bin folder
2. Swiss Ephemeris data files (if needed)
3. .NET 8.0 Runtime (or bundle with self-contained deployment)

**Recommended Approach**:
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

This creates a standalone executable with all dependencies.

## Known Limitations

1. **Windows Only**: WPF is Windows-specific
2. **Navamsa**: Calculation not yet implemented
3. **Vimshottari Dasa**: Calculation not yet implemented
4. **Chart Graphics**: No visual chart display yet
5. **Tamil Font**: Requires system to have Tamil Unicode fonts

## Troubleshooting

### Build Issues

**Error**: "NETSDK1100: To build a project targeting Windows..."
- **Solution**: Add `<EnableWindowsTargeting>true</EnableWindowsTargeting>`

**Error**: "Could not load file or assembly 'iTextSharp...'"
- **Solution**: Run `dotnet restore`

### Runtime Issues

**Issue**: Tamil characters show as boxes
- **Solution**: Install Tamil Unicode fonts (Latha, Nirmala UI)

**Issue**: PDF export fails
- **Solution**: Check write permissions, ensure iTextSharp DLL is present

---

**Version**: 1.0  
**Last Updated**: February 4, 2026  
**Author**: RAMESHSIVAPERUMAL
