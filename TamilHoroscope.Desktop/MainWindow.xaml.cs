using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Desktop.Models;
using TamilHoroscope.Desktop.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace TamilHoroscope.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private HoroscopeData? _currentHoroscope;
    private BirthDetails? _currentBirthDetails;
    private readonly PanchangCalculator _calculator;
    private readonly BirthPlaceService _birthPlaceService;
    private List<BirthPlace> _allPlaces = new();

    public MainWindow()
    {
        InitializeComponent();
        _calculator = new PanchangCalculator();
        _birthPlaceService = new BirthPlaceService();

        // Load birth places data
        LoadBirthPlaces();

        // Set default date to today
        dpBirthDate.SelectedDate = DateTime.Today;

        // Add keyboard shortcuts
        this.KeyDown += MainWindow_KeyDown;
    }

    /// <summary>
    /// Loads birth places from XML data file
    /// </summary>
    private void LoadBirthPlaces()
    {
        try
        {
            _birthPlaceService.LoadBirthPlaces();
            _allPlaces = _birthPlaceService.GetAllPlaces();
            
            // Set initial items for ComboBox
            cmbBirthPlace.ItemsSource = _allPlaces;
            cmbBirthPlace.DisplayMemberPath = "DisplayName";
            
            // Set default selection to Chennai
            var chennai = _allPlaces.FirstOrDefault(p => p.Name == "Chennai");
            if (chennai != null)
            {
                cmbBirthPlace.SelectedItem = chennai;
            }

            var onlineStatus = _birthPlaceService.IsOnline ? "Online" : "Offline";
            UpdateStatus($"Loaded {_allPlaces.Count} birth places successfully. Mode: {onlineStatus}", true);
        }
        catch (Exception ex)
        {
            UpdateStatus($"Warning: Could not load birth places - {ex.Message}", false);
            MessageBox.Show($"Could not load birth places database:\n\n{ex.Message}\n\nYou can still enter coordinates manually.", 
                          "Birth Places Data", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    /// <summary>
    /// Handles text changes in the ComboBox for auto-complete functionality
    /// Priority: Local XML first, then API if no results found
    /// </summary>
    private async void CmbBirthPlace_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Don't process if selection is being changed programmatically
        if (cmbBirthPlace.SelectedItem != null)
        {
            return;
        }

        var textBox = e.OriginalSource as TextBox;
        if (textBox == null)
        {
            return;
        }

        // If text is empty, reset to all places
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            cmbBirthPlace.ItemsSource = _allPlaces;
            return;
        }

        // Open dropdown for autocomplete
        if (!cmbBirthPlace.IsDropDownOpen)
        {
            cmbBirthPlace.IsDropDownOpen = true;
        }

        // Filter places based on search text
        var searchText = textBox.Text;
        var currentText = textBox.Text;
        var caretIndex = textBox.CaretIndex;
        
        try
        {
            // PRIORITY 1: Check local XML cache FIRST
            var localResults = _birthPlaceService.SearchPlaces(searchText);
            
            // If we have results from local cache, use them immediately
            if (localResults.Count > 0)
            {
                cmbBirthPlace.ItemsSource = localResults;
                
                // Restore text and caret position
                textBox.Text = currentText;
                textBox.CaretIndex = caretIndex;
                
                System.Diagnostics.Debug.WriteLine($"Using local XML cache for '{searchText}': {localResults.Count} results");
                return;
            }
            
            // PRIORITY 2: If NO results in local cache AND online, try API
            if (_birthPlaceService.IsOnline)
            {
                System.Diagnostics.Debug.WriteLine($"No local results for '{searchText}', calling API...");
                
                var onlineResults = await _birthPlaceService.SearchPlacesOnlineAsync(searchText);
                
                // Only update if the text hasn't changed during the async operation
                if (textBox.Text == currentText && cmbBirthPlace.SelectedItem == null)
                {
                    cmbBirthPlace.ItemsSource = onlineResults;
                    
                    // Restore text and caret position
                    textBox.Text = currentText;
                    textBox.CaretIndex = caretIndex;
                }
            }
            else
            {
                // Offline and no local results - show empty
                cmbBirthPlace.ItemsSource = new List<BirthPlace>();
                
                System.Diagnostics.Debug.WriteLine($"Offline and no local results for '{searchText}'");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in CmbBirthPlace_TextChanged: {ex.Message}");
            
            // On error, try local search again
            var filteredPlaces = _birthPlaceService.SearchPlaces(searchText);
            cmbBirthPlace.ItemsSource = filteredPlaces;
            
            // Restore text and caret position
            textBox.Text = currentText;
            textBox.CaretIndex = caretIndex;
        }
    }

    /// <summary>
    /// Handles selection changes in the birth place ComboBox
    /// </summary>
    private void CmbBirthPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbBirthPlace.SelectedItem is BirthPlace selectedPlace)
        {
            // Auto-fill latitude, longitude, and timezone
            txtLatitude.Text = selectedPlace.Latitude.ToString("F4", CultureInfo.InvariantCulture);
            txtLongitude.Text = selectedPlace.Longitude.ToString("F4", CultureInfo.InvariantCulture);
            txtTimezone.Text = selectedPlace.TimeZone.ToString("F1", CultureInfo.InvariantCulture);
        }
    }

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

    private async void BtnCalculate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            UpdateStatus("Calculating horoscope...", true);

            // Validate inputs
            if (!ValidateInputs(out string errorMessage))
            {
                UpdateStatus($"Validation Error: {errorMessage}", false);
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse birth details
            var birthDetails = ParseBirthDetails();
            _currentBirthDetails = birthDetails;

            // Save the confirmed birth place to XML (if it's from API/not already in cache)
            if (cmbBirthPlace.SelectedItem is BirthPlace selectedPlace)
            {
                // Save the user-confirmed location to XML
                await _birthPlaceService.SaveConfirmedPlaceAsync(selectedPlace);
            }

            // Calculate horoscope with optional features based on UI settings
            bool includeDasa = chkCalculateDasa.IsChecked == true;
            bool includeNavamsa = chkCalculateNavamsa.IsChecked == true;
            bool includeStrength = chkCalculateStrength.IsChecked == true;
            int dasaYears = cmbDasaYears.SelectedIndex switch
            {
                0 => 10,
                1 => 25,
                2 => 50,
                3 => 120,
                _ => 50
            };

            _currentHoroscope = _calculator.CalculateHoroscope(birthDetails, includeDasa, includeNavamsa, dasaYears, includeStrength);

            // Display results
            DisplayResults(_currentHoroscope, birthDetails);

            // Enable export button
            btnExportPdf.IsEnabled = true;

            UpdateStatus("Horoscope calculated successfully!", true);
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error: {ex.Message}", false);
            MessageBox.Show($"Error calculating horoscope:\n\n{ex.Message}", "Calculation Error", 
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateInputs(out string errorMessage)
    {
        errorMessage = string.Empty;

        // Validate date
        if (!dpBirthDate.SelectedDate.HasValue)
        {
            errorMessage = "Please select a birth date.";
            return false;
        }

        // Validate time
        if (string.IsNullOrWhiteSpace(txtBirthTime.Text))
        {
            errorMessage = "Please enter birth time.";
            return false;
        }

        if (!TimeSpan.TryParse(txtBirthTime.Text, out _))
        {
            errorMessage = "Invalid time format. Use HH:mm:ss (e.g., 10:30:00)";
            return false;
        }

        // Validate latitude
        if (!double.TryParse(txtLatitude.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) || 
            lat < -90 || lat > 90)
        {
            errorMessage = "Invalid latitude. Must be between -90 and 90.";
            return false;
        }

        // Validate longitude
        if (!double.TryParse(txtLongitude.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lon) || 
            lon < -180 || lon > 180)
        {
            errorMessage = "Invalid longitude. Must be between -180 and 180.";
            return false;
        }

        // Validate timezone
        if (!double.TryParse(txtTimezone.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double tz) || 
            tz < -12 || tz > 14)
        {
            errorMessage = "Invalid timezone offset. Must be between -12 and 14.";
            return false;
        }

        return true;
    }

    private BirthDetails ParseBirthDetails()
    {
        var date = dpBirthDate.SelectedDate!.Value;
        var time = TimeSpan.Parse(txtBirthTime.Text);
        var dateTime = date.Date.Add(time);

        // Get place name from ComboBox
        string placeName = "Unknown";
        if (cmbBirthPlace.SelectedItem is BirthPlace selectedPlace)
        {
            placeName = selectedPlace.Name;
        }
        else if (!string.IsNullOrWhiteSpace(cmbBirthPlace.Text))
        {
            placeName = cmbBirthPlace.Text;
        }

        return new BirthDetails
        {
            DateTime = dateTime,
            Latitude = double.Parse(txtLatitude.Text, CultureInfo.InvariantCulture),
            Longitude = double.Parse(txtLongitude.Text, CultureInfo.InvariantCulture),
            TimeZoneOffset = double.Parse(txtTimezone.Text, CultureInfo.InvariantCulture),
            PlaceName = placeName
        };
    }

    private void DisplayResults(HoroscopeData horoscope, BirthDetails birthDetails)
    {
        // Show results panel
        resultsPanel.Visibility = Visibility.Visible;

        // Display Panchangam
        txtPanchangam.Text = $"Location: {birthDetails.PlaceName}\n" +
                            $"Date/Time: {birthDetails.DateTime:yyyy-MM-dd HH:mm:ss}\n" +
                            $"Coordinates: {birthDetails.Latitude:F4}°N, {birthDetails.Longitude:F4}°E\n\n" +
                            $"Tamil Month: {horoscope.Panchang.TamilMonth}\n" +
                            $"Vara (Weekday): {horoscope.Panchang.VaraName} ({horoscope.Panchang.TamilVaraName})\n" +
                            $"Tithi: {horoscope.Panchang.TithiName} ({horoscope.Panchang.TamilTithiName})\n" +
                            $"Paksha: {horoscope.Panchang.Paksha} ({horoscope.Panchang.TamilPaksha})\n" +
                            $"Nakshatra: {horoscope.Panchang.NakshatraName} ({horoscope.Panchang.TamilNakshatraName})\n" +
                            $"Yoga: {horoscope.Panchang.YogaName} ({horoscope.Panchang.TamilYogaName})\n" +
                            $"Karana: {horoscope.Panchang.KaranaName} ({horoscope.Panchang.TamilKaranaName})";

        // Display Lagna
        txtLagna.Text = $"Rasi: {horoscope.LagnaRasiName} ({horoscope.TamilLagnaRasiName})\n" +
                       $"Longitude: {horoscope.LagnaLongitude:F2}°";

        // Display Planets with Lagna as first row
        var planetsWithLagna = new List<PlanetData>();
        
        // Add Lagna as first row
        planetsWithLagna.Add(new PlanetData
        {
            Name = "Lagna",
            TamilName = "லக்னம்",
            Longitude = horoscope.LagnaLongitude,
            Latitude = 0.0,
            Speed = 0.0,
            Rasi = horoscope.LagnaRasi,
            RasiName = horoscope.LagnaRasiName,
            TamilRasiName = horoscope.TamilLagnaRasiName,
            Nakshatra = GetNakshatraNumber(horoscope.LagnaLongitude),
            House = 1, // Lagna is always 1st house
            IsRetrograde = false
        });
        
        // Add nakshatra name for Lagna
        var lagnaData = planetsWithLagna[0];
        var nakshatraInfo = TamilHoroscope.Core.Data.TamilNames.Nakshatras[lagnaData.Nakshatra];
        lagnaData.NakshatraName = nakshatraInfo.English;
        lagnaData.TamilNakshatraName = nakshatraInfo.Tamil;
        
        // Add all planets
        planetsWithLagna.AddRange(horoscope.Planets);
        
        dgPlanets.ItemsSource = planetsWithLagna;

        // Display Rasi Chart
        chartSection.Visibility = Visibility.Visible;
        var rasiChart = new Controls.RasiChartControl();
        rasiChart.DrawChart(horoscope);
        rasiChartContainer.Child = rasiChart;

        // Display Navamsa Chart if calculated
        if (chkCalculateNavamsa.IsChecked == true && horoscope.NavamsaPlanets != null && horoscope.NavamsaPlanets.Count > 0)
        {
            navamsaChartBorder.Visibility = Visibility.Visible;
            
            var navamsaChart = new Controls.NavamsaChartControl();
            navamsaChart.DrawChart(horoscope);
            navamsaChartContainer.Child = navamsaChart;
        }
        else
        {
            navamsaChartBorder.Visibility = Visibility.Collapsed;
        }

        // Show/hide Planetary Strength section
        if (chkCalculateStrength.IsChecked == true && horoscope.PlanetStrengths != null && horoscope.PlanetStrengths.Count > 0)
        {
            strengthSection.Visibility = Visibility.Visible;
            
            var strengthChart = new Controls.PlanetStrengthChartControl();
            strengthChart.DisplayStrengths(horoscope.PlanetStrengths);
            strengthChartContainer.Child = strengthChart;
        }
        else
        {
            strengthSection.Visibility = Visibility.Collapsed;
        }

        // Show/hide Dasa section
        if (chkCalculateDasa.IsChecked == true && horoscope.VimshottariDasas != null && horoscope.VimshottariDasas.Count > 0)
        {
            dasaSection.Visibility = Visibility.Visible;
            
            // Display Dasa information
            var dasaText = "Vimshottari Dasa / Bhukti Periods:\n\n";
            
            // Find current dasa
            var currentDasa = horoscope.VimshottariDasas.FirstOrDefault(d => 
                d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);
            
            // Display each Dasa with its Bhukti periods
            foreach (var dasa in horoscope.VimshottariDasas.Take(15)) // Show first 15 Dasas
            {
                var isCurrent = dasa == currentDasa;
                var dasaMarker = isCurrent ? " ← CURRENT DASA" : "";
                var durationYears = (dasa.EndDate - dasa.StartDate).Days / 365.25;
                
                // Dasa Header with period
                dasaText += $"\n{'=', 80}\n";
                dasaText += $"{dasa.Lord} Dasa ({dasa.TamilLord} தசா){dasaMarker}\n";
                dasaText += $"Period: {dasa.StartDate:yyyy-MM-dd} to {dasa.EndDate:yyyy-MM-dd} ({durationYears:F1} years)\n";
                dasaText += $"{'=', 80}\n\n";
                
                // Display all Bhuktis for this Dasa
                if (dasa.Bhuktis != null && dasa.Bhuktis.Count > 0)
                {
                    dasaText += "  Bhukti Periods:\n";
                    dasaText += "  " + new string('-', 76) + "\n";
                    
                    foreach (var bhukti in dasa.Bhuktis)
                    {
                        var isCurrentBhukti = isCurrent && 
                            bhukti.StartDate <= DateTime.Now && 
                            bhukti.EndDate >= DateTime.Now;
                        var bhuktiMarker = isCurrentBhukti ? " ← NOW" : "";
                        var bhuktiDurationDays = (bhukti.EndDate - bhukti.StartDate).Days;
                        var bhuktiDurationMonths = bhuktiDurationDays / 30.0;
                        
                        dasaText += $"  • {bhukti.Lord,-10} ({bhukti.TamilLord,-10}): " +
                                  $"{bhukti.StartDate:yyyy-MM-dd} to {bhukti.EndDate:yyyy-MM-dd} " +
                                  $"({bhuktiDurationMonths:F1} months){bhuktiMarker}\n";
                    }
                    
                    dasaText += "\n";
                }
                else
                {
                    dasaText += "  (No Bhukti details available)\n\n";
                }
            }
            
            // Add summary at the end
            if (currentDasa != null)
            {
                var currentBhukti = currentDasa.Bhuktis?.FirstOrDefault(b => 
                    b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);
                
                dasaText += "\n" + new string('=', 80) + "\n";
                dasaText += "CURRENT STATUS SUMMARY\n";
                dasaText += new string('=', 80) + "\n";
                dasaText += $"Current Dasa: {currentDasa.Lord} ({currentDasa.TamilLord})\n";
                dasaText += $"Dasa Period: {currentDasa.StartDate:yyyy-MM-dd} to {currentDasa.EndDate:yyyy-MM-dd}\n";
                
                if (currentBhukti != null)
                {
                    dasaText += $"\nCurrent Bhukti: {currentBhukti.Lord} ({currentBhukti.TamilLord})\n";
                    dasaText += $"Bhukti Period: {currentBhukti.StartDate:yyyy-MM-dd} to {currentBhukti.EndDate:yyyy-MM-dd}\n";
                    
                    var daysRemaining = (currentBhukti.EndDate - DateTime.Now).Days;
                    dasaText += $"Days Remaining: {daysRemaining}\n";
                }
                
                dasaText += new string('=', 80) + "\n";
            }
            
            txtDasa.Text = dasaText;
            txtDasa.FontStyle = FontStyles.Normal;
            txtDasa.Foreground = System.Windows.Media.Brushes.Black;
        }
        else
        {
            dasaSection.Visibility = Visibility.Collapsed;
        }
    }

    private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
    {
        if (_currentHoroscope == null || _currentBirthDetails == null)
        {
            MessageBox.Show("Please calculate horoscope first.", "Export Error", 
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            UpdateStatus("Exporting to PDF...", true);

            // Create save file dialog
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"Horoscope_{txtName.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                DefaultExt = ".pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                ExportToPdf(saveDialog.FileName);
                UpdateStatus($"Successfully exported to: {saveDialog.FileName}", true);
                MessageBox.Show($"Horoscope exported successfully!\n\nFile: {saveDialog.FileName}", 
                              "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                UpdateStatus("Export cancelled.", false);
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"Export error: {ex.Message}", false);
            MessageBox.Show($"Error exporting to PDF:\n\n{ex.Message}", "Export Error", 
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportToPdf(string filePath)
    {
        // Create PDF document - use A4 LANDSCAPE for charts
        var document = new Document(PageSize.A4, 40, 40, 40, 40);
        var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
        document.Open();

        // Fonts
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 0, 255));
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        var subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        var cellFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
        var dataCellFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

        // Add title
        var title = new Paragraph("Tamil Horoscope - தமிழ் ஜாதகம்\n\n", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        // Add name if provided
        if (!string.IsNullOrWhiteSpace(txtName.Text))
        {
            var nameFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var name = new Paragraph($"Name: {txtName.Text}\n\n", nameFont);
            name.Alignment = Element.ALIGN_CENTER;
            document.Add(name);
        }

        // Add birth details section
        document.Add(new Paragraph("Birth Details - பிறப்பு விவரங்கள்", headerFont));
        document.Add(new Paragraph($"Date: {_currentBirthDetails!.DateTime:yyyy-MM-dd (dddd)}", normalFont));
        document.Add(new Paragraph($"Time: {_currentBirthDetails.DateTime:HH:mm:ss}", normalFont));
        document.Add(new Paragraph($"Place: {_currentBirthDetails.PlaceName}", normalFont));
        document.Add(new Paragraph($"Coordinates: {_currentBirthDetails.Latitude:F4}°N, {_currentBirthDetails.Longitude:F4}°E", normalFont));
        document.Add(new Paragraph($"Timezone: UTC+{_currentBirthDetails.TimeZoneOffset:F1}", normalFont));
        document.Add(new Paragraph("\n"));

        // Add Panchangam section (COMPLETE with all elements)
        document.Add(new Paragraph("Panchangam - பஞ்சாங்கம்", headerFont));
        var panchangTable = new PdfPTable(2);
        panchangTable.WidthPercentage = 100;
        panchangTable.SetWidths(new float[] { 1f, 2f });
        
        AddPanchangRow(panchangTable, "Tamil Month:", _currentHoroscope!.Panchang.TamilMonth, normalFont);
        AddPanchangRow(panchangTable, "Vara (Weekday):", $"{_currentHoroscope.Panchang.VaraName} ({_currentHoroscope.Panchang.TamilVaraName})", normalFont);
        AddPanchangRow(panchangTable, "Tithi (Lunar Day):", $"{_currentHoroscope.Panchang.TithiName} ({_currentHoroscope.Panchang.TamilTithiName})", normalFont);
        AddPanchangRow(panchangTable, "Paksha (Fortnight):", $"{_currentHoroscope.Panchang.Paksha} ({_currentHoroscope.Panchang.TamilPaksha})", normalFont);
        AddPanchangRow(panchangTable, "Nakshatra:", $"{_currentHoroscope.Panchang.NakshatraName} ({_currentHoroscope.Panchang.TamilNakshatraName})", normalFont);
        AddPanchangRow(panchangTable, "Yoga:", $"{_currentHoroscope.Panchang.YogaName} ({_currentHoroscope.Panchang.TamilYogaName})", normalFont);
        AddPanchangRow(panchangTable, "Karana:", $"{_currentHoroscope.Panchang.KaranaName} ({_currentHoroscope.Panchang.TamilKaranaName})", normalFont);
        
        document.Add(panchangTable);
        document.Add(new Paragraph("\n"));

        // Add Lagna section
        document.Add(new Paragraph("Lagna (Ascendant) - லக்னம்", headerFont));
        document.Add(new Paragraph($"Rasi: {_currentHoroscope.LagnaRasiName} ({_currentHoroscope.TamilLagnaRasiName})", normalFont));
        document.Add(new Paragraph($"Longitude: {_currentHoroscope.LagnaLongitude:F2}° ({GetDegreesMinutes(_currentHoroscope.LagnaLongitude % 30)})", normalFont));
        document.Add(new Paragraph("\n"));

        // Add Charts on new page
        document.NewPage();
        document.Add(new Paragraph("Birth Charts - ஜாதக கட்டம்", headerFont));
        document.Add(new Paragraph("\n"));

        // Create Rasi Chart as image
        document.Add(new Paragraph("Rasi Chart (D-1) - ராசி கட்டம்", subHeaderFont));
        document.Add(new Paragraph("\n"));

        // Render Rasi Chart to image
        var rasiChartControl = new Controls.RasiChartControl();
        rasiChartControl.DrawChart(_currentHoroscope);
        var rasiChartImage = RenderControlToImage(rasiChartControl, 400, 400);
        if (rasiChartImage != null)
        {
            // Center the image
            rasiChartImage.Alignment = Element.ALIGN_CENTER;
            rasiChartImage.ScaleToFit(350f, 350f);
            document.Add(rasiChartImage);
            document.NewPage();
        }
        document.Add(new Paragraph("\n"));

        // Create Navamsa Chart if calculated
        if (_currentHoroscope.NavamsaPlanets != null && _currentHoroscope.NavamsaPlanets.Count > 0)
        {
            document.Add(new Paragraph("Navamsa Chart (D-9) - நவாம்சம் கட்டம்", subHeaderFont));

            // Render Navamsa Chart to image
            var navamsaChartControl = new Controls.NavamsaChartControl();
            navamsaChartControl.DrawChart(_currentHoroscope);
            var navamsaChartImage = RenderControlToImage(navamsaChartControl, 400, 400);
            if (navamsaChartImage != null)
            {
                // Center the image
                navamsaChartImage.Alignment = Element.ALIGN_CENTER;
                navamsaChartImage.ScaleToFit(350f, 350f);
                document.Add(navamsaChartImage);
                document.NewPage();
            }
        }
        document.Add(new Paragraph("\n"));

        // Add Planets table (Rasi Chart - D1) - Simplified to match screen display
        document.Add(new Paragraph("Navagraha Positions (Rasi Chart - D1) - நவகிரக நிலைகள்", headerFont));
        document.Add(new Paragraph("\n"));
        var planetsTable = new PdfPTable(9); // Changed from 6 to 9 columns
        planetsTable.WidthPercentage = 100;
        planetsTable.SetWidths(new float[] { 1.5f, 1.5f, 2f, 2f, 1.5f, 2.5f, 0.8f, 1f, 1f });

        // Table headers - matching screen display
        planetsTable.AddCell(CreateHeaderCell("Planet", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Tamil", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Rasi", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Longitude", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Degree", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Nakshatra", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Pada", cellFont));
        planetsTable.AddCell(CreateHeaderCell("House", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Status", cellFont));

        // Add Lagna as first row
        planetsTable.AddCell(new PdfPCell(new Phrase("Lagna", dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) }); // Light yellow background
        planetsTable.AddCell(new PdfPCell(new Phrase("லக்னம்", smallFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) });
        planetsTable.AddCell(new PdfPCell(new Phrase($"{_currentHoroscope.LagnaRasiName}\n{_currentHoroscope.TamilLagnaRasiName}", smallFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) });
        
        // Calculate Lagna longitude formatted
        int lagnaDeg = (int)_currentHoroscope.LagnaLongitude;
        double lagnaMinDec = (_currentHoroscope.LagnaLongitude - lagnaDeg) * 60.0;
        int lagnaMin = (int)lagnaMinDec;
        double lagnaSecDec = (lagnaMinDec - lagnaMin) * 60.0;
        int lagnaSec = (int)lagnaSecDec;
        string lagnaLongitudeFormatted = $"{lagnaDeg}°{lagnaMin:D2}'{lagnaSec:D2}\"";
        
        planetsTable.AddCell(new PdfPCell(new Phrase(lagnaLongitudeFormatted, smallFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) });
        planetsTable.AddCell(new PdfPCell(new Phrase(GetDegreesMinutes(_currentHoroscope.LagnaLongitude % 30), smallFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) });
        
        // Calculate Lagna Nakshatra
        int lagnaNakshatra = GetNakshatraNumber(_currentHoroscope.LagnaLongitude);
        var lagnaNakshatraInfo = TamilHoroscope.Core.Data.TamilNames.Nakshatras[lagnaNakshatra];
        planetsTable.AddCell(new PdfPCell(new Phrase($"{lagnaNakshatraInfo.English}\n{lagnaNakshatraInfo.Tamil}", smallFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) });
        
        // Calculate Lagna Pada
        double lagnaPosinNaks = (_currentHoroscope.LagnaLongitude % (360.0 / 27.0));
        int lagnaPada = (int)(lagnaPosinNaks / ((360.0 / 27.0) / 4.0)) + 1;
        if (lagnaPada > 4) lagnaPada = 4;
        planetsTable.AddCell(new PdfPCell(new Phrase(lagnaPada.ToString(), dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) });
        planetsTable.AddCell(new PdfPCell(new Phrase("1", dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) }); // Lagna is always 1st house
        planetsTable.AddCell(new PdfPCell(new Phrase("", dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 250, 205) }); // No status for Lagna

        // Table data - matching screen display
        foreach (var planet in _currentHoroscope.Planets)
        {
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.Name, dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.TamilName, smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase($"{planet.RasiName}\n{planet.TamilRasiName}", smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.LongitudeFormatted, smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.DegreeFormatted, smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase($"{planet.NakshatraName}\n{planet.TamilNakshatraName}", smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraPada.ToString(), dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.House.ToString(), dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.StatusDisplay, dataCellFont)) 
            { 
                HorizontalAlignment = Element.ALIGN_CENTER 
            });
        }

        document.Add(planetsTable);
        document.Add(new Paragraph("\n"));

        // Add Navamsa Planets table if calculated - matching screen display format
        if (_currentHoroscope.NavamsaPlanets != null && _currentHoroscope.NavamsaPlanets.Count > 0)
        {
            document.Add(new Paragraph("Navamsa Positions (D-9 Chart) - நவாம்சம்", headerFont));
            document.Add(new Paragraph("\n"));
            var navamsaTable = new PdfPTable(7); // Simplified columns (no House for Navamsa)
            navamsaTable.WidthPercentage = 100;
            navamsaTable.SetWidths(new float[] { 1.5f, 1.5f, 2f, 2f, 1.5f, 2.5f, 0.8f });

            // Headers - matching essential display
            navamsaTable.AddCell(CreateHeaderCell("Planet", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Tamil", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Navamsa Rasi", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Longitude", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Degree", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Nakshatra", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Pada", cellFont));

            // Data
            foreach (var planet in _currentHoroscope.NavamsaPlanets)
            {
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.Name, dataCellFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.TamilName, smallFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase($"{planet.RasiName}\n{planet.TamilRasiName}", smallFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.LongitudeFormatted, smallFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.DegreeFormatted, smallFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraName, smallFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraPada.ToString(), dataCellFont)));
            }

        document.Add(navamsaTable);
            document.Add(new Paragraph("\n"));
        }

        // Add Planetary Strength (Shadbala) section if calculated
        if (_currentHoroscope.PlanetStrengths != null && _currentHoroscope.PlanetStrengths.Count > 0)
        {
            document.NewPage(); // New page for Strength
            
            document.Add(new Paragraph("Planetary Strength (Shadbala) - கிரக பலம்", headerFont));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Note: Rahu and Ketu are excluded as they don't have Shadbala in traditional Vedic astrology.", smallFont));
            document.Add(new Paragraph("\n"));

            // Create detailed components table
            var componentsTable = new PdfPTable(10);
            componentsTable.WidthPercentage = 100;
            componentsTable.SetWidths(new float[] { 1.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1.2f, 1f, 1.2f });

            // Headers
            componentsTable.AddCell(CreateHeaderCell("Planet", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Positional", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Directional", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Motional", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Natural", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Temporal", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Aspectual", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Total", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Required", cellFont));
            componentsTable.AddCell(CreateHeaderCell("Grade", cellFont));

            // Data rows
            foreach (var strength in _currentHoroscope.PlanetStrengths)
            {
                // Planet name cell with Tamil
                var planetCell = new PdfPCell(new Phrase($"{strength.Name}\n{strength.TamilName}", smallFont));
                componentsTable.AddCell(planetCell);

                // Strength components
                componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.PositionalStrength:F1}", dataCellFont))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });
                componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.DirectionalStrength:F1}", dataCellFont))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });
                componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.MotionalStrength:F1}", dataCellFont))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });
                componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.NaturalStrength:F1}", dataCellFont))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });
                componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.TemporalStrength:F1}", dataCellFont))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });
                componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.AspectualStrength:F1}", dataCellFont))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });

                // Total strength (bold)
                var totalCell = new PdfPCell(new Phrase($"{strength.TotalStrength:F1}", subHeaderFont));
                totalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                componentsTable.AddCell(totalCell);

                // Required minimum (red)
                var requiredFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new BaseColor(220, 20, 60));
                var requiredCell = new PdfPCell(new Phrase($"{strength.RequiredStrength:F1}", requiredFont));
                requiredCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                componentsTable.AddCell(requiredCell);

                // Grade with color indicator
                var gradeFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, GetPdfColorForStrength(strength.StrengthPercentage));
                var gradeCell = new PdfPCell(new Phrase(strength.StrengthGrade, gradeFont));
                gradeCell.HorizontalAlignment = Element.ALIGN_CENTER;
                componentsTable.AddCell(gradeCell);
            }

            document.Add(componentsTable);
            document.Add(new Paragraph("\n"));

            // Create summary strength table (simpler view)
            document.Add(new Paragraph("Strength Summary", subHeaderFont));
            document.Add(new Paragraph("\n"));
            
            var strengthTable = new PdfPTable(4);
            strengthTable.WidthPercentage = 100;
            strengthTable.SetWidths(new float[] { 2f, 1.5f, 1.5f, 2f });

            // Headers
            strengthTable.AddCell(CreateHeaderCell("Planet / கிரகம்", cellFont));
            strengthTable.AddCell(CreateHeaderCell("Strength", cellFont));
            strengthTable.AddCell(CreateHeaderCell("Percentage", cellFont));
            strengthTable.AddCell(CreateHeaderCell("Grade / தரம்", cellFont));

            // Data rows
            foreach (var strength in _currentHoroscope.PlanetStrengths)
            {
                // Planet name cell with Tamil
                var planetCell = new PdfPCell(new Phrase($"{strength.Name}\n{strength.TamilName}", smallFont));
                strengthTable.AddCell(planetCell);

                // Strength in Rupas and Virupas
                var strengthCell = new PdfPCell(new Phrase(
                    $"{strength.TotalStrength:F1} R\n({strength.TotalStrengthVirupas:F0} V)", 
                    dataCellFont));
                strengthCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                strengthTable.AddCell(strengthCell);

                // Percentage
                var percentCell = new PdfPCell(new Phrase($"{strength.StrengthPercentage:F0}%", dataCellFont));
                percentCell.HorizontalAlignment = Element.ALIGN_CENTER;
                strengthTable.AddCell(percentCell);

                // Grade with color indicator
                var gradeFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, GetPdfColorForStrength(strength.StrengthPercentage));
                var gradeCell = new PdfPCell(new Phrase($"{strength.StrengthGrade}\n{strength.TamilStrengthGrade}", gradeFont));
                strengthTable.AddCell(gradeCell);
            }

            document.Add(strengthTable);
            document.Add(new Paragraph("\n"));

            // Add strength bar chart as image
            var strengthChart = new Controls.PlanetStrengthChartControl();
            strengthChart.DisplayStrengths(_currentHoroscope.PlanetStrengths);
            var strengthChartImage = RenderControlToImage(strengthChart, 700, 500);
            if (strengthChartImage != null)
            {
                strengthChartImage.Alignment = Element.ALIGN_CENTER;
                strengthChartImage.ScaleToFit(500f, 350f);
                document.Add(strengthChartImage);
            }

            // Add explanation
            document.Add(new Paragraph("\n"));
            var explanationFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            var explanation = new Paragraph(
                "Strength Components Explained:\n" +
                "• Positional (Sthana Bala): Based on sign placement (exaltation, own sign, etc.)\n" +
                "• Directional (Dig Bala): Based on house placement relative to cardinal directions\n" +
                "• Motional (Chesta Bala): Based on speed and retrograde status\n" +
                "• Natural (Naisargika Bala): Inherent luminosity and power of the planet\n" +
                "• Temporal (Kala Bala): Based on time factors (day/night, paksha)\n" +
                "• Aspectual (Drik Bala): Based on aspects received from other planets\n\n" +
                "Units: R = Rupas (1 Rupa = 60 Virupas), V = Virupas. " +
                "Required minimum strength varies by planet and is shown in red. " +
                "A planet meeting or exceeding its required minimum can deliver positive results.",
                explanationFont);
            explanation.Alignment = Element.ALIGN_JUSTIFIED;
            document.Add(explanation);
        }

        // Add Vimshottari Dasa if calculated
        if (_currentHoroscope.VimshottariDasas != null && _currentHoroscope.VimshottariDasas.Count > 0)
        {
            document.NewPage(); // New page for Dasa
            
            document.Add(new Paragraph("Vimshottari Dasa / Bhukti - விம்சோத்தரி தசா", headerFont));
            document.Add(new Paragraph("\n"));

            // Find current dasa
            var currentDasa = _currentHoroscope.VimshottariDasas.FirstOrDefault(d =>
                d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);

            // Display each Dasa with its Bhukti periods
            foreach (var dasa in _currentHoroscope.VimshottariDasas.Take(12)) // Show first 12 Dasas in PDF
            {
                var isCurrent = dasa == currentDasa;
                var durationYears = (dasa.EndDate - dasa.StartDate).Days / 365.25;
                
                // Dasa Header
                var dasaHeaderFont = isCurrent ? 
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, new BaseColor(0, 0, 128)) : 
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                
                var dasaHeader = new Paragraph(
                    $"{dasa.Lord} Dasa ({dasa.TamilLord} தசா)" + 
                    (isCurrent ? " ← CURRENT DASA" : ""), 
                    dasaHeaderFont);
                dasaHeader.SpacingBefore = 10;
                document.Add(dasaHeader);
                
                var dasaPeriod = new Paragraph(
                    $"Period: {dasa.StartDate:yyyy-MM-dd} to {dasa.EndDate:yyyy-MM-dd} ({durationYears:F1} years)", 
                    normalFont);
                document.Add(dasaPeriod);
                
                // Bhukti Table for this Dasa
                if (dasa.Bhuktis != null && dasa.Bhuktis.Count > 0)
                {
                    var bhuktiTable = new PdfPTable(5);
                    bhuktiTable.WidthPercentage = 100;
                    bhuktiTable.SetWidths(new float[] { 2f, 2f, 2.5f, 2.5f, 1.5f });
                    bhuktiTable.SpacingBefore = 5;
                    bhuktiTable.SpacingAfter = 10;
                    
                    // Bhukti headers
                    bhuktiTable.AddCell(CreateHeaderCell("Bhukti Lord", smallFont));
                    bhuktiTable.AddCell(CreateHeaderCell("Tamil Name", smallFont));
                    bhuktiTable.AddCell(CreateHeaderCell("Start Date", smallFont));
                    bhuktiTable.AddCell(CreateHeaderCell("End Date", smallFont));
                    bhuktiTable.AddCell(CreateHeaderCell("Duration", smallFont));
                    
                    // Bhukti rows
                    foreach (var bhukti in dasa.Bhuktis)
                    {
                        var isCurrentBhukti = isCurrent && 
                            bhukti.StartDate <= DateTime.Now && 
                            bhukti.EndDate >= DateTime.Now;
                        
                        var bhuktiFont = isCurrentBhukti ? subHeaderFont : dataCellFont;
                        var bgColor = isCurrentBhukti ? 
                            new BaseColor(255, 255, 150) : 
                            (isCurrent ? new BaseColor(240, 240, 255) : new BaseColor(255, 255, 255));
                        
                        var bhuktiDurationDays = (bhukti.EndDate - bhukti.StartDate).Days;
                        var bhuktiDurationMonths = bhuktiDurationDays / 30.0;
                        
                        bhuktiTable.AddCell(new PdfPCell(new Phrase(bhukti.Lord, bhuktiFont)) 
                            { BackgroundColor = bgColor, Padding = 3 });
                        bhuktiTable.AddCell(new PdfPCell(new Phrase(bhukti.TamilLord, bhuktiFont)) 
                            { BackgroundColor = bgColor, Padding = 3 });
                        bhuktiTable.AddCell(new PdfPCell(new Phrase(bhukti.StartDate.ToString("yyyy-MM-dd"), bhuktiFont)) 
                            { BackgroundColor = bgColor, Padding = 3 });
                        bhuktiTable.AddCell(new PdfPCell(new Phrase(bhukti.EndDate.ToString("yyyy-MM-dd"), bhuktiFont)) 
                            { BackgroundColor = bgColor, Padding = 3 });
                        bhuktiTable.AddCell(new PdfPCell(new Phrase($"{bhuktiDurationMonths:F1} mo", bhuktiFont)) 
                            { BackgroundColor = bgColor, Padding = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                    }
                    
                    document.Add(bhuktiTable);
                }
            }
            
            // Add current status summary
            if (currentDasa != null)
            {
                document.Add(new Paragraph("\n"));
                var summaryHeader = new Paragraph("CURRENT STATUS SUMMARY", subHeaderFont);
                summaryHeader.SpacingBefore = 10;
                document.Add(summaryHeader);
                
                document.Add(new Paragraph($"Current Dasa: {currentDasa.Lord} ({currentDasa.TamilLord})", normalFont));
                document.Add(new Paragraph($"Dasa Period: {currentDasa.StartDate:yyyy-MM-dd} to {currentDasa.EndDate:yyyy-MM-dd}", normalFont));
                
                var currentBhukti = currentDasa.Bhuktis?.FirstOrDefault(b =>
                    b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);
                    
                if (currentBhukti != null)
                {
                    document.Add(new Paragraph($"Current Bhukti: {currentBhukti.Lord} ({currentBhukti.TamilLord})", normalFont));
                    document.Add(new Paragraph($"Bhukti Period: {currentBhukti.StartDate:yyyy-MM-dd} to {currentBhukti.EndDate:yyyy-MM-dd}", normalFont));
                    
                    var daysRemaining = (currentBhukti.EndDate - DateTime.Now).Days;
                    document.Add(new Paragraph($"Days Remaining in Current Bhukti: {daysRemaining}", normalFont));
                }
            }
        }

      

        // Add footer
        document.Add(new Paragraph("\n\n"));
        var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, new BaseColor(128, 128, 128));
        var footer = new Paragraph($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss} using Tamil Horoscope Calculator\nSwiss Ephemeris for Astronomical Accuracy - Lahiri Ayanamsa", footerFont);
        footer.Alignment = Element.ALIGN_CENTER;
        document.Add(footer);

        document.Close();
    }

    private void AddPanchangRow(PdfPTable table, string label, string value, iTextSharp.text.Font font)
    {
        table.AddCell(new PdfPCell(new Phrase(label, font)) { Border = Rectangle.NO_BORDER, PaddingBottom = 5 });
        table.AddCell(new PdfPCell(new Phrase(value, font)) { Border = Rectangle.NO_BORDER, PaddingBottom = 5 });
    }

    private PdfPCell CreateHeaderCell(string text, iTextSharp.text.Font font)
    {
        return new PdfPCell(new Phrase(text, font))
        {
            BackgroundColor = new BaseColor(211, 211, 211),
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 5
        };
    }

    private string GetDegreesMinutes(double degrees)
    {
        int deg = (int)degrees;
        double minutesDecimal = (degrees - deg) * 60;
        int min = (int)minutesDecimal;
        return $"{deg}°{min:D2}'";
    }

    /// <summary>
    /// Get PDF color based on strength percentage
    /// </summary>
    private BaseColor GetPdfColorForStrength(double percentage)
    {
        return percentage switch
        {
            >= 80 => new BaseColor(34, 139, 34),    // Forest Green
            >= 60 => new BaseColor(50, 205, 50),    // Lime Green
            >= 40 => new BaseColor(255, 215, 0),    // Gold
            >= 20 => new BaseColor(255, 140, 0),    // Dark Orange
            _ => new BaseColor(220, 20, 60)         // Crimson
        };
    }

    /// <summary>
    /// Renders a WPF UserControl to an iTextSharp Image for PDF inclusion
    /// </summary>
    private iTextSharp.text.Image? RenderControlToImage(UserControl control, int width, int height)
    {
        try
        {
            // Set the size of the control
            control.Width = width;
            control.Height = height;
            control.Measure(new System.Windows.Size(width, height));
            control.Arrange(new Rect(0, 0, width, height));
            control.UpdateLayout();

            // Create a RenderTargetBitmap
            var renderBitmap = new RenderTargetBitmap(
                width,
                height,
                96,  // DPI X
                96,  // DPI Y
                PixelFormats.Pbgra32);

            renderBitmap.Render(control);

            // Convert to PNG byte array
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Create iTextSharp Image from byte array
                return iTextSharp.text.Image.GetInstance(imageBytes);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error rendering control to image: {ex.Message}");
            return null;
        }
    }

    private string GenerateTextChart(HoroscopeData horoscope, bool isNavamsa)
    {
        // Generate ASCII art representation of the chart
        var planets = isNavamsa ? horoscope.NavamsaPlanets : horoscope.Planets;
        
        // Group planets by Rasi
        var rasiToPlanets = new Dictionary<int, List<string>>();
        for (int i = 1; i <= 12; i++)
        {
            rasiToPlanets[i] = new List<string>();
        }

        foreach (var planet in planets!)
        {
            var abbrev = planet.Name switch
            {
                "Sun" => "Su",
                "Moon" => "Mo",
                "Mars" => "Ma",
                "Mercury" => "Me",
                "Jupiter" => "Ju",
                "Venus" => "Ve",
                "Saturn" => "Sa",
                "Rahu" => "Ra",
                "Ketu" => "Ke",
                _ => planet.Name.Substring(0, Math.Min(2, planet.Name.Length))
            };
            rasiToPlanets[planet.Rasi].Add(abbrev);
        }

        // Add Lagna marker (only for Rasi chart)
        if (!isNavamsa)
        {
            rasiToPlanets[horoscope.LagnaRasi].Insert(0, "La");
        }

        // Traditional South Indian format positions
        var positions = new Dictionary<int, (int row, int col)>
        {
            { 12, (0, 0) }, { 1, (0, 1) }, { 2, (0, 2) }, { 3, (0, 3) },
            { 11, (1, 0) }, { 4, (1, 3) },
            { 10, (2, 0) }, { 5, (2, 3) },
            { 9, (3, 0) }, { 8, (3, 1) }, { 7, (3, 2) }, { 6, (3, 3) }
        };

        var grid = new string[4, 4];
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                grid[i, j] = "";

        foreach (var kvp in positions)
        {
            var (row, col) = kvp.Value;
            var planets_list = rasiToPlanets[kvp.Key];
            grid[row, col] = planets_list.Count > 0 ? string.Join(" ", planets_list) : "";
        }

        // Build the chart
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("╔════════════╦════════════╦════════════╦════════════╗");
        sb.AppendLine($"║ {grid[0, 0],-10} ║ {grid[0, 1],-10} ║ {grid[0, 2],-10} ║ {grid[0, 3],-10} ║");
        sb.AppendLine("╠════════════╬════════════╬════════════╬════════════╣");
        sb.AppendLine($"║ {grid[1, 0],-10} ║            ║            ║ {grid[1, 3],-10} ║");
        sb.AppendLine("╠════════════╣            ║            ╠════════════╣");
        sb.AppendLine($"║ {grid[2, 0],-10} ║            ║            ║ {grid[2, 3],-10} ║");
        sb.AppendLine("╠════════════╬════════════╬════════════╬════════════╣");
        sb.AppendLine($"║ {grid[3, 0],-10} ║ {grid[3, 1],-10} ║ {grid[3, 2],-10} ║ {grid[3, 3],-10} ║");
        sb.AppendLine("╚════════════╩════════════╩════════════╩════════════╝");

        sb.AppendLine("\nLegend: La=Lagna, Su=Sun, Mo=Moon, Ma=Mars, Me=Mercury,");
        sb.AppendLine("        Ju=Jupiter, Ve=Venus, Sa=Saturn, Ra=Rahu, Ke=Ketu");

        return sb.ToString();
    }

    private void UpdateStatus(string message, bool isSuccess)
    {
        txtStatus.Text = message;
        txtStatus.Foreground = isSuccess ? 
            System.Windows.Media.Brushes.Green : 
            System.Windows.Media.Brushes.Red;
    }
    
    /// <summary>
    /// Calculate Nakshatra number from longitude
    /// </summary>
    private int GetNakshatraNumber(double longitude)
    {
        // Normalize longitude to 0-360
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        // Each nakshatra is 13°20' (360/27)
        double nakshatraDegree = 360.0 / 27.0;
        return (int)(longitude / nakshatraDegree) + 1;
    }
}