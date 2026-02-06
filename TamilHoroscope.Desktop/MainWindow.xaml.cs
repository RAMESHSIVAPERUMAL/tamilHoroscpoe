using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
    /// Handles text changes in the ComboBox for auto-complete functionality with online search
    /// </summary>
    private async void CmbBirthPlace_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!cmbBirthPlace.IsDropDownOpen)
        {
            cmbBirthPlace.IsDropDownOpen = true;
        }

        var textBox = e.OriginalSource as TextBox;
        if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
        {
            cmbBirthPlace.ItemsSource = _allPlaces;
            return;
        }

        // Filter places based on search text
        var searchText = textBox.Text;
        
        // Show loading indicator
        var currentText = textBox.Text;
        
        try
        {
            // Try online search first if connected
            if (_birthPlaceService.IsOnline)
            {
                var filteredPlaces = await _birthPlaceService.SearchPlacesOnlineAsync(searchText);
                
                // Only update if the text hasn't changed during the async operation
                if (textBox.Text == currentText)
                {
                    cmbBirthPlace.ItemsSource = filteredPlaces;
                }
            }
            else
            {
                // Fallback to local search
                var filteredPlaces = _birthPlaceService.SearchPlaces(searchText);
                cmbBirthPlace.ItemsSource = filteredPlaces;
            }
        }
        catch
        {
            // On error, use local search
            var filteredPlaces = _birthPlaceService.SearchPlaces(searchText);
            cmbBirthPlace.ItemsSource = filteredPlaces;
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

    private void BtnCalculate_Click(object sender, RoutedEventArgs e)
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

            // Calculate horoscope with optional features based on UI settings
            bool includeDasa = chkCalculateDasa.IsChecked == true;
            bool includeNavamsa = chkCalculateNavamsa.IsChecked == true;
            int dasaYears = cmbDasaYears.SelectedIndex switch
            {
                0 => 10,
                1 => 25,
                2 => 50,
                3 => 120,
                _ => 50
            };

            _currentHoroscope = _calculator.CalculateHoroscope(birthDetails, includeDasa, includeNavamsa, dasaYears);

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

        // Display Planets
        dgPlanets.ItemsSource = horoscope.Planets;

        // Display Houses
        var housesDisplay = horoscope.Houses.Select(h => new
        {
            h.HouseNumber,
            h.RasiName,
            h.TamilRasiName,
            h.Lord,
            PlanetsDisplay = h.Planets.Count > 0 ? string.Join(", ", h.Planets) : "-"
        }).ToList();
        dgHouses.ItemsSource = housesDisplay;

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

        // Show/hide Dasa section
        if (chkCalculateDasa.IsChecked == true && horoscope.VimshottariDasas != null && horoscope.VimshottariDasas.Count > 0)
        {
            dasaSection.Visibility = Visibility.Visible;
            
            // Display Dasa information
            var dasaText = "Vimshottari Dasa Periods:\n\n";
            
            // Find current dasa
            var currentDasa = horoscope.VimshottariDasas.FirstOrDefault(d => 
                d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);
            
            if (currentDasa != null)
            {
                dasaText += $"CURRENT DASA: {currentDasa.Lord} ({currentDasa.TamilLord})\n";
                dasaText += $"Period: {currentDasa.StartDate:yyyy-MM-dd} to {currentDasa.EndDate:yyyy-MM-dd}\n\n";
                
                // Show current Bhukti
                var currentBhukti = currentDasa.Bhuktis.FirstOrDefault(b => 
                    b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);
                if (currentBhukti != null)
                {
                    dasaText += $"Current Bhukti: {currentBhukti.Lord} ({currentBhukti.TamilLord})\n";
                    dasaText += $"Period: {currentBhukti.StartDate:yyyy-MM-dd} to {currentBhukti.EndDate:yyyy-MM-dd}\n\n";
                }
            }
            
            // Show next few dasas
            dasaText += "Upcoming Dasa Periods:\n";
            foreach (var dasa in horoscope.VimshottariDasas.Take(10))
            {
                var isCurrent = dasa == currentDasa ? " ← CURRENT" : "";
                dasaText += $"{dasa.Lord,-10} ({dasa.TamilLord,-10}): {dasa.StartDate:yyyy-MM-dd} to {dasa.EndDate:yyyy-MM-dd}{isCurrent}\n";
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

        // Add Planets table (Rasi Chart - D1) - Simplified to match screen display
        document.Add(new Paragraph("Navagraha Positions (Rasi Chart - D1) - நவகிரக நிலைகள்", headerFont));
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

        // Add Houses table
        document.Add(new Paragraph("Houses (Bhavas) - பாவங்கள்", headerFont));
        var housesTable = new PdfPTable(5);
        housesTable.WidthPercentage = 100;
        housesTable.SetWidths(new float[] { 1f, 2f, 2f, 2f, 3f });

        // Table headers
        housesTable.AddCell(CreateHeaderCell("House", cellFont));
        housesTable.AddCell(CreateHeaderCell("Rasi", cellFont));
        housesTable.AddCell(CreateHeaderCell("Tamil Rasi", cellFont));
        housesTable.AddCell(CreateHeaderCell("Lord", cellFont));
        housesTable.AddCell(CreateHeaderCell("Planets", cellFont));

        // Table data
        foreach (var house in _currentHoroscope.Houses)
        {
            housesTable.AddCell(new PdfPCell(new Phrase(house.HouseNumber.ToString(), dataCellFont)));
            housesTable.AddCell(new PdfPCell(new Phrase(house.RasiName, dataCellFont)));
            housesTable.AddCell(new PdfPCell(new Phrase(house.TamilRasiName, dataCellFont)));
            housesTable.AddCell(new PdfPCell(new Phrase(house.Lord, dataCellFont)));
            var planets = house.Planets.Count > 0 ? string.Join(", ", house.Planets) : "-";
            housesTable.AddCell(new PdfPCell(new Phrase(planets, smallFont)));
        }

        document.Add(housesTable);
        document.Add(new Paragraph("\n"));

        // Add Vimshottari Dasa if calculated
        if (_currentHoroscope.VimshottariDasas != null && _currentHoroscope.VimshottariDasas.Count > 0)
        {
            document.NewPage(); // New page for Dasa
            
            document.Add(new Paragraph("Vimshottari Dasa / Bhukti - விம்சோத்தரி தசா", headerFont));
            document.Add(new Paragraph("\n"));

            // Find current dasa
            var currentDasa = _currentHoroscope.VimshottariDasas.FirstOrDefault(d =>
                d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);

            if (currentDasa != null)
            {
                document.Add(new Paragraph($"CURRENT DASA: {currentDasa.Lord} ({currentDasa.TamilLord})", subHeaderFont));
                document.Add(new Paragraph($"Period: {currentDasa.StartDate:yyyy-MM-dd} to {currentDasa.EndDate:yyyy-MM-dd}", normalFont));
                document.Add(new Paragraph("\n"));

                // Show current Bhukti
                var currentBhukti = currentDasa.Bhuktis.FirstOrDefault(b =>
                    b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);
                if (currentBhukti != null)
                {
                    document.Add(new Paragraph($"Current Bhukti: {currentBhukti.Lord} ({currentBhukti.TamilLord})", normalFont));
                    document.Add(new Paragraph($"Period: {currentBhukti.StartDate:yyyy-MM-dd} to {currentBhukti.EndDate:yyyy-MM-dd}", normalFont));
                    document.Add(new Paragraph("\n"));
                }
            }

            // Show upcoming dasas
            document.Add(new Paragraph("Upcoming Dasa Periods:", subHeaderFont));
            var dasaTable = new PdfPTable(5);
            dasaTable.WidthPercentage = 100;
            dasaTable.SetWidths(new float[] { 2f, 2f, 2f, 2f, 1f });

            dasaTable.AddCell(CreateHeaderCell("Dasa Lord", cellFont));
            dasaTable.AddCell(CreateHeaderCell("Tamil Name", cellFont));
            dasaTable.AddCell(CreateHeaderCell("Start Date", cellFont));
            dasaTable.AddCell(CreateHeaderCell("End Date", cellFont));
            dasaTable.AddCell(CreateHeaderCell("Years", cellFont));

            foreach (var dasa in _currentHoroscope.VimshottariDasas.Take(20))
            {
                var isCurrent = dasa == currentDasa;
                var font = isCurrent ? subHeaderFont : dataCellFont;
                var bgColor = isCurrent ? new BaseColor(255, 255, 200) : new BaseColor(255, 255, 255);
                
                var durationYears = (dasa.EndDate - dasa.StartDate).Days / 365.25;

                dasaTable.AddCell(new PdfPCell(new Phrase(dasa.Lord, font)) { BackgroundColor = bgColor });
                dasaTable.AddCell(new PdfPCell(new Phrase(dasa.TamilLord, font)) { BackgroundColor = bgColor });
                dasaTable.AddCell(new PdfPCell(new Phrase(dasa.StartDate.ToString("yyyy-MM-dd"), font)) { BackgroundColor = bgColor });
                dasaTable.AddCell(new PdfPCell(new Phrase(dasa.EndDate.ToString("yyyy-MM-dd"), font)) { BackgroundColor = bgColor });
                dasaTable.AddCell(new PdfPCell(new Phrase(durationYears.ToString("F1"), font)) { BackgroundColor = bgColor });
            }

            document.Add(dasaTable);
        }

        // Add Charts on new page
        document.NewPage();
        document.Add(new Paragraph("Birth Charts - ஜாதக கட்டம்", headerFont));
        document.Add(new Paragraph("\n"));

        // Create Rasi Chart as text representation
        document.Add(new Paragraph("Rasi Chart (D-1) - ராசி கட்டம்", subHeaderFont));
        var rasiChartText = GenerateTextChart(_currentHoroscope, isNavamsa: false);
        document.Add(new Paragraph(rasiChartText, FontFactory.GetFont(FontFactory.COURIER, 8)));
        document.Add(new Paragraph("\n"));

        // Create Navamsa Chart if calculated
        if (_currentHoroscope.NavamsaPlanets != null && _currentHoroscope.NavamsaPlanets.Count > 0)
        {
            document.Add(new Paragraph("Navamsa Chart (D-9) - நவாம்சம் கட்டம்", subHeaderFont));
            var navamsaChartText = GenerateTextChart(_currentHoroscope, isNavamsa: true);
            document.Add(new Paragraph(navamsaChartText, FontFactory.GetFont(FontFactory.COURIER, 8)));
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
}