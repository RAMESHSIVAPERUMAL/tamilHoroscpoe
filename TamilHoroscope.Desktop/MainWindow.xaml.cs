using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;
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

    public MainWindow()
    {
        InitializeComponent();
        _calculator = new PanchangCalculator();

        // Set default date to today
        dpBirthDate.SelectedDate = DateTime.Today;

        // Add keyboard shortcuts
        this.KeyDown += MainWindow_KeyDown;
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

            // Calculate horoscope
            _currentHoroscope = _calculator.CalculateHoroscope(birthDetails);

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

        return new BirthDetails
        {
            DateTime = dateTime,
            Latitude = double.Parse(txtLatitude.Text, CultureInfo.InvariantCulture),
            Longitude = double.Parse(txtLongitude.Text, CultureInfo.InvariantCulture),
            TimeZoneOffset = double.Parse(txtTimezone.Text, CultureInfo.InvariantCulture),
            PlaceName = txtPlaceName.Text
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

        // Show/hide Navamsa section
        if (chkCalculateNavamsa.IsChecked == true)
        {
            navamsaSection.Visibility = Visibility.Visible;
            if (horoscope.NavamsaPlanets != null && horoscope.NavamsaPlanets.Count > 0)
            {
                txtNavamsa.Text = "Navamsa positions:\n" +
                                 string.Join("\n", horoscope.NavamsaPlanets.Select(p => 
                                     $"{p.Name} ({p.TamilName}): {p.RasiName} - {p.NakshatraName}"));
                txtNavamsa.FontStyle = FontStyles.Normal;
                txtNavamsa.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
        else
        {
            navamsaSection.Visibility = Visibility.Collapsed;
        }

        // Show/hide Dasa section
        if (chkCalculateDasa.IsChecked == true)
        {
            dasaSection.Visibility = Visibility.Visible;
            var dasaYears = cmbDasaYears.SelectedIndex switch
            {
                0 => "10",
                1 => "25",
                2 => "50",
                3 => "120",
                _ => "50"
            };
            txtDasa.Text = $"Vimshottari Dasa calculation for {dasaYears} years will be displayed here.\n" +
                          "This feature is ready for implementation in Phase 3.";
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
        // Create PDF document
        var document = new Document(PageSize.A4, 50, 50, 50, 50);
        var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
        document.Open();

        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 0, 255));
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

        // Add birth details
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

        document.Add(new Paragraph("Birth Details", headerFont));
        document.Add(new Paragraph($"Date: {_currentBirthDetails!.DateTime:yyyy-MM-dd}", normalFont));
        document.Add(new Paragraph($"Time: {_currentBirthDetails.DateTime:HH:mm:ss}", normalFont));
        document.Add(new Paragraph($"Place: {_currentBirthDetails.PlaceName}", normalFont));
        document.Add(new Paragraph($"Coordinates: {_currentBirthDetails.Latitude:F4}°N, {_currentBirthDetails.Longitude:F4}°E", normalFont));
        document.Add(new Paragraph("\n"));

        // Add Panchangam
        document.Add(new Paragraph("Panchangam - பஞ்சாங்கம்", headerFont));
        document.Add(new Paragraph($"Tamil Month: {_currentHoroscope!.Panchang.TamilMonth}", normalFont));
        document.Add(new Paragraph($"Vara: {_currentHoroscope.Panchang.VaraName} ({_currentHoroscope.Panchang.TamilVaraName})", normalFont));
        document.Add(new Paragraph($"Tithi: {_currentHoroscope.Panchang.TithiName} ({_currentHoroscope.Panchang.TamilTithiName})", normalFont));
        document.Add(new Paragraph($"Nakshatra: {_currentHoroscope.Panchang.NakshatraName} ({_currentHoroscope.Panchang.TamilNakshatraName})", normalFont));
        document.Add(new Paragraph($"Yoga: {_currentHoroscope.Panchang.YogaName} ({_currentHoroscope.Panchang.TamilYogaName})", normalFont));
        document.Add(new Paragraph($"Karana: {_currentHoroscope.Panchang.KaranaName} ({_currentHoroscope.Panchang.TamilKaranaName})", normalFont));
        document.Add(new Paragraph("\n"));

        // Add Lagna
        document.Add(new Paragraph("Lagna (Ascendant) - லக்னம்", headerFont));
        document.Add(new Paragraph($"Rasi: {_currentHoroscope.LagnaRasiName} ({_currentHoroscope.TamilLagnaRasiName})", normalFont));
        document.Add(new Paragraph($"Longitude: {_currentHoroscope.LagnaLongitude:F2}°", normalFont));
        document.Add(new Paragraph("\n"));

        // Add Planets table
        document.Add(new Paragraph("Navagraha Positions - நவகிரக நிலைகள்", headerFont));
        var planetsTable = new PdfPTable(5);
        planetsTable.WidthPercentage = 100;
        planetsTable.SetWidths(new float[] { 2f, 2f, 2f, 2f, 1.5f });

        // Table headers
        var cellFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
        planetsTable.AddCell(new PdfPCell(new Phrase("Planet", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });
        planetsTable.AddCell(new PdfPCell(new Phrase("Rasi", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });
        planetsTable.AddCell(new PdfPCell(new Phrase("Nakshatra", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });
        planetsTable.AddCell(new PdfPCell(new Phrase("House", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });
        planetsTable.AddCell(new PdfPCell(new Phrase("Retro", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });

        // Table data
        var dataCellFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        foreach (var planet in _currentHoroscope.Planets)
        {
            planetsTable.AddCell(new PdfPCell(new Phrase($"{planet.Name}\n{planet.TamilName}", dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.RasiName, dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraName, dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.House.ToString(), dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.IsRetrograde ? "Yes" : "No", dataCellFont)));
        }

        document.Add(planetsTable);
        document.Add(new Paragraph("\n"));

        // Add Houses table
        document.Add(new Paragraph("Houses (Bhavas) - பாவங்கள்", headerFont));
        var housesTable = new PdfPTable(4);
        housesTable.WidthPercentage = 100;
        housesTable.SetWidths(new float[] { 1f, 2f, 2f, 3f });

        // Table headers
        housesTable.AddCell(new PdfPCell(new Phrase("House", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });
        housesTable.AddCell(new PdfPCell(new Phrase("Rasi", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });
        housesTable.AddCell(new PdfPCell(new Phrase("Lord", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });
        housesTable.AddCell(new PdfPCell(new Phrase("Planets", cellFont)) { BackgroundColor = new BaseColor(211, 211, 211) });

        // Table data
        foreach (var house in _currentHoroscope.Houses)
        {
            housesTable.AddCell(new PdfPCell(new Phrase(house.HouseNumber.ToString(), dataCellFont)));
            housesTable.AddCell(new PdfPCell(new Phrase($"{house.RasiName}\n{house.TamilRasiName}", dataCellFont)));
            housesTable.AddCell(new PdfPCell(new Phrase(house.Lord, dataCellFont)));
            var planets = house.Planets.Count > 0 ? string.Join(", ", house.Planets) : "-";
            housesTable.AddCell(new PdfPCell(new Phrase(planets, dataCellFont)));
        }

        document.Add(housesTable);

        // Add footer
        document.Add(new Paragraph("\n\n"));
        var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, new BaseColor(128, 128, 128));
        var footer = new Paragraph($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss} using Tamil Horoscope Calculator\nSwiss Ephemeris for Astronomical Accuracy", footerFont);
        footer.Alignment = Element.ALIGN_CENTER;
        document.Add(footer);

        document.Close();
    }

    private void UpdateStatus(string message, bool isSuccess)
    {
        txtStatus.Text = message;
        txtStatus.Foreground = isSuccess ? 
            System.Windows.Media.Brushes.Green : 
            System.Windows.Media.Brushes.Red;
    }
}