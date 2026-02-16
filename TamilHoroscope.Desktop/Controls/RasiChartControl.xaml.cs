using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Desktop.Controls;

/// <summary>
/// South Indian Style Rasi Chart - Based on traditional HTML/CSS reference
/// </summary>
public partial class RasiChartControl : UserControl
{
    public RasiChartControl()
    {
        InitializeComponent();
    }

    public void DrawChart(HoroscopeData horoscope)
    {
        ChartCanvas.Children.Clear();

        // Get language from horoscope planets (fallback to Tamil if not set)
        string language = horoscope.Planets.Any() ? horoscope.Planets.First().Language : "Tamil";

        // Define chart dimensions (4x4 grid layout)
        double boxSize = 100; // Each box is 100x100
        
        // South Indian chart uses a 4x4 grid
        // Rasi positions (fixed clockwise from top-left):
        // 12  1   2   3
        // 11  TITLE   4
        // 10  TITLE   5
        // 9   8   7   6
        
        // Border color matching reference CSS (#F88857 - coral/orange)
        var borderBrush = new SolidColorBrush(Color.FromRgb(0xF8, 0x88, 0x57));
        
        // Background gradient matching reference CSS (radial gradient effect)
        var backgroundBrush = new RadialGradientBrush(
            Color.FromRgb(0xFF, 0xFD, 0xE9),
            Color.FromRgb(0xFF, 0xFC, 0xD5)
        );

        // Create a map of Rasi to Planets
        var rasiToPlanets = new Dictionary<int, List<string>>();
        for (int i = 1; i <= 12; i++)
        {
            rasiToPlanets[i] = new List<string>();
        }

        // Group planets by their Rasi
        foreach (var planet in horoscope.Planets)
        {
            rasiToPlanets[planet.Rasi].Add(GetPlanetAbbreviation(planet.Name, language));
        }

        // Define the 12 box positions in the 4x4 grid
        var boxPositions = new[]
        {
            new { Row = 0, Col = 0, Rasi = 12 },   // Pisces (top-left)
            new { Row = 0, Col = 1, Rasi = 1 },    // Aries
            new { Row = 0, Col = 2, Rasi = 2 },    // Taurus
            new { Row = 0, Col = 3, Rasi = 3 },    // Gemini
            new { Row = 1, Col = 3, Rasi = 4 },    // Cancer
            new { Row = 2, Col = 3, Rasi = 5 },    // Leo
            new { Row = 3, Col = 3, Rasi = 6 },    // Virgo
            new { Row = 3, Col = 2, Rasi = 7 },    // Libra
            new { Row = 3, Col = 1, Rasi = 8 },    // Scorpio
            new { Row = 3, Col = 0, Rasi = 9 },    // Sagittarius
            new { Row = 2, Col = 0, Rasi = 10 },   // Capricorn
            new { Row = 1, Col = 0, Rasi = 11 }    // Aquarius
        };

        // Draw all 12 boxes
        foreach (var position in boxPositions)
        {
            double left = position.Col * boxSize;
            double top = position.Row * boxSize;
            int rasiNum = position.Rasi;

            // Create border for each box
            var border = new Border
            {
                Width = boxSize,
                Height = boxSize,
                BorderBrush = borderBrush,
                BorderThickness = new Thickness(1),
                Background = backgroundBrush
            };
            Canvas.SetLeft(border, left);
            Canvas.SetTop(border, top);
            ChartCanvas.Children.Add(border);

            // Create content stack for this rasi
            var contentStack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add Lagna marker if this is the Lagna Rasi
            bool isLagna = (rasiNum == horoscope.LagnaRasi);
            if (isLagna)
            {
                var lagnaMarker = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0xDC, 0x35, 0x45)), // Bootstrap danger red
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(6, 2, 6, 2),
                    Margin = new Thickness(0, 2, 0, 2),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                var lagnaText = new TextBlock
                {
                    Text = "லக்",  // Tamil "Lak" for Lagna
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                lagnaMarker.Child = lagnaText;
                contentStack.Children.Add(lagnaMarker);
            }

            // Add planets in this Rasi
            var planets = rasiToPlanets[rasiNum];
            if (planets.Count > 0)
            {
                // Create a wrap panel for planets (similar to HTML span.pad1)
                var planetsPanel = new WrapPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = boxSize - 10
                };

                foreach (var planetAbbrev in planets)
                {
                    var planetText = new TextBlock
                    {
                        Text = planetAbbrev,
                        FontSize = 10,
                        FontWeight = FontWeights.Normal,
                        Foreground = new SolidColorBrush(Color.FromRgb(0x80, 0x00, 0x80)), // Purple color from CSS
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        Width = 30,
                        Margin = new Thickness(1)
                    };
                    planetsPanel.Children.Add(planetText);
                }

                contentStack.Children.Add(planetsPanel);
            }

            // Position the content stack
            Canvas.SetLeft(contentStack, left + 5);
            Canvas.SetTop(contentStack, top + 20);
            ChartCanvas.Children.Add(contentStack);
        }

        // Draw center title area (2x2 grid in the middle)
        var centerBorder = new Border
        {
            Width = boxSize * 2,
            Height = boxSize * 2,
            BorderBrush = borderBrush,
            BorderThickness = new Thickness(1),
            Background = Brushes.White
        };
        Canvas.SetLeft(centerBorder, boxSize);
        Canvas.SetTop(centerBorder, boxSize);
        ChartCanvas.Children.Add(centerBorder);

        // Add center title with localized text
        var centerTitle = new TextBlock
        {
            Text = TamilHoroscope.Core.Data.LocalizedWordings.GetSectionName("Rasi", language),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x80, 0x00, 0x80)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };
        Canvas.SetLeft(centerTitle, boxSize + 50);
        Canvas.SetTop(centerTitle, boxSize + 75);
        ChartCanvas.Children.Add(centerTitle);
        
        System.Diagnostics.Debug.WriteLine(">>> South Indian style Rasi Chart completed!");
    }

    private string GetPlanetAbbreviation(string planetName, string language)
    {
        // Get localized name and take first few characters
        var localizedName = TamilHoroscope.Core.Data.LocalizedWordings.GetPlanetName(planetName, language);
        
        // For Tamil/Telugu/Kannada/Malayalam, take first 2-4 characters depending on name length
        if (localizedName.Length <= 4)
            return localizedName;
        else if (localizedName.Length <= 6)
            return localizedName.Substring(0, Math.Min(4, localizedName.Length));
        else
            return localizedName.Substring(0, Math.Min(3, localizedName.Length));
    }
}
