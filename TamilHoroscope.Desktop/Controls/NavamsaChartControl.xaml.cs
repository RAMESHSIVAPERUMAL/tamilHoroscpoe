using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Desktop.Controls;

/// <summary>
/// South Indian Style Navamsa Chart (D-9) - Based on traditional HTML/CSS reference
/// </summary>
public partial class NavamsaChartControl : UserControl
{
    public NavamsaChartControl()
    {
        InitializeComponent();
    }

    public void DrawChart(HoroscopeData horoscope)
    {
        ChartCanvas.Children.Clear();

        // Check if Navamsa planets are available
        if (horoscope.NavamsaPlanets == null || horoscope.NavamsaPlanets.Count == 0)
        {
            DrawPlaceholder();
            return;
        }

        // Define chart dimensions (4x4 grid layout)
        double boxSize = 100; // Each box is 100x100
        
        // Border color matching reference CSS (#F88857 - coral/orange)
        var borderBrush = new SolidColorBrush(Color.FromRgb(0xF8, 0x88, 0x57));
        
        // Background gradient matching reference CSS
        var backgroundBrush = new RadialGradientBrush(
            Color.FromRgb(0xFF, 0xFD, 0xE9),
            Color.FromRgb(0xFF, 0xFC, 0xD5)
        );

        // Create a map of Rasi to Planets (using Navamsa planets)
        var rasiToPlanets = new Dictionary<int, List<string>>();
        for (int i = 1; i <= 12; i++)
        {
            rasiToPlanets[i] = new List<string>();
        }

        // Group Navamsa planets by their Rasi
        foreach (var planet in horoscope.NavamsaPlanets)
        {
            rasiToPlanets[planet.Rasi].Add(GetPlanetAbbreviation(planet.Name));
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

            // Add planets in this Rasi
            var planets = rasiToPlanets[rasiNum];
            if (planets.Count > 0)
            {
                // Create a wrap panel for planets
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
                        Foreground = new SolidColorBrush(Color.FromRgb(0x80, 0x00, 0x80)), // Purple color
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

        // Add center title with Tamil text
        var centerTitle = new TextBlock
        {
            Text = "நவாம்சம்",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x80, 0x00, 0x80)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };
        Canvas.SetLeft(centerTitle, boxSize + 35);
        Canvas.SetTop(centerTitle, boxSize + 65);
        ChartCanvas.Children.Add(centerTitle);
        
        System.Diagnostics.Debug.WriteLine(">>> South Indian style Navamsa Chart completed!");
    }

    private void DrawPlaceholder()
    {
        var placeholder = new TextBlock
        {
            Text = "Navamsa Chart\nNot Yet Calculated",
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };
        Canvas.SetLeft(placeholder, 100);
        Canvas.SetTop(placeholder, 180);
        ChartCanvas.Children.Add(placeholder);
    }

    private string GetPlanetAbbreviation(string planetName)
    {
        return planetName switch
        {
            "Sun" => "சூரி",
            "Moon" => "சந்",
            "Mars" => "செவ்",
            "Mercury" => "புத",
            "Jupiter" => "குரு",
            "Venus" => "சுக்",
            "Saturn" => "சனி",
            "Rahu" => "ராகு",
            "Ketu" => "கேது",
            _ => planetName.Substring(0, Math.Min(2, planetName.Length))
        };
    }
}
