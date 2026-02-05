using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Desktop.Controls;

/// <summary>
/// South Indian Style Rasi Chart - FIXED VERSION with all 4 lines
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

        // Define chart dimensions
        double width = 400;
        double height = 400;
        double centerX = width / 2;
        double centerY = height / 2;
        double size = 170; // Half size of the square
        double margin = 20;

        // ???????????????????????????????????????????????????????????
        // DRAW THE OUTER SQUARE BORDER
        // ???????????????????????????????????????????????????????????
        var outerRect = new Rectangle
        {
            Width = size * 2,
            Height = size * 2,
            Stroke = Brushes.Black,
            StrokeThickness = 3,
            Fill = Brushes.White
        };
        Canvas.SetLeft(outerRect, centerX - size);
        Canvas.SetTop(outerRect, centerY - size);
        ChartCanvas.Children.Add(outerRect);

        // ???????????????????????????????????????????????????????????
        // DRAW ALL 4 LINES - THIS IS THE FIX!
        // ???????????????????????????????????????????????????????????
        System.Diagnostics.Debug.WriteLine(">>> Drawing Rasi Chart with ALL 4 lines...");
        
        // LINE 1: Horizontal (left to right)
        DrawLine(centerX - size, centerY, centerX + size, centerY, 3);
        System.Diagnostics.Debug.WriteLine(">>> Drew HORIZONTAL line");
        
        // LINE 2: Vertical (top to bottom)  
        DrawLine(centerX, centerY - size, centerX, centerY + size, 3);
        System.Diagnostics.Debug.WriteLine(">>> Drew VERTICAL line");
        
        // LINE 3: Diagonal \ (top-left to bottom-right)
        DrawLine(centerX - size, centerY - size, centerX + size, centerY + size, 3);
        System.Diagnostics.Debug.WriteLine(">>> Drew DIAGONAL \\ line");
        
        // LINE 4: Diagonal / (top-right to bottom-left)
        DrawLine(centerX + size, centerY - size, centerX - size, centerY + size, 3);
        System.Diagnostics.Debug.WriteLine(">>> Drew DIAGONAL / line");

        System.Diagnostics.Debug.WriteLine(">>> All 4 lines drawn successfully!");

        // ???????????????????????????????????????????????????????????
        // TRADITIONAL SOUTH INDIAN LAYOUT
        // ???????????????????????????????????????????????????????????
        
        // Calculate which house contains the Lagna (Ascendant)
        int lagnaHouse = 1; // Default to house 1
        
        // Find which house has the same Rasi as Lagna
        for (int i = 0; i < horoscope.Houses.Count; i++)
        {
            if (horoscope.Houses[i].Rasi == horoscope.LagnaRasi)
            {
                lagnaHouse = horoscope.Houses[i].HouseNumber;
                break;
            }
        }

        // Define positions for the 12 sections in traditional South Indian chart
        var sectionCenters = new (double X, double Y, double W, double H)[]
        {
            // Top row (4 sections)
            (centerX - size * 0.75, centerY - size * 0.75, size * 0.5, size * 0.5),  // 1: Top-Left
            (centerX - size * 0.25, centerY - size * 0.75, size * 0.5, size * 0.5),  // 2: Top-Center-Left
            (centerX + size * 0.25, centerY - size * 0.75, size * 0.5, size * 0.5),  // 3: Top-Center-Right
            (centerX + size * 0.75, centerY - size * 0.75, size * 0.5, size * 0.5),  // 4: Top-Right
            
            // Middle rows (2 sections each side)
            (centerX + size * 0.75, centerY - size * 0.25, size * 0.5, size * 0.5),  // 5: Right-Top
            (centerX + size * 0.75, centerY + size * 0.25, size * 0.5, size * 0.5),  // 6: Right-Bottom
            
            // Bottom row (4 sections)
            (centerX + size * 0.75, centerY + size * 0.75, size * 0.5, size * 0.5),  // 7: Bottom-Right
            (centerX + size * 0.25, centerY + size * 0.75, size * 0.5, size * 0.5),  // 8: Bottom-Center-Right
            (centerX - size * 0.25, centerY + size * 0.75, size * 0.5, size * 0.5),  // 9: Bottom-Center-Left
            (centerX - size * 0.75, centerY + size * 0.75, size * 0.5, size * 0.5),  // 10: Bottom-Left
            
            // Middle left (2 sections)
            (centerX - size * 0.75, centerY + size * 0.25, size * 0.5, size * 0.5),  // 11: Left-Bottom
            (centerX - size * 0.75, centerY - size * 0.25, size * 0.5, size * 0.5),  // 12: Left-Top
        };

        // Map Rasis to sections (TRADITIONAL South Indian chart layout)
        var rasiToSection = new Dictionary<int, int>
        {
            { 12, 1 },  // Pisces ? Top-Left
            { 1, 2 },   // Aries ? Top-Center-Left (TRADITIONAL)
            { 2, 3 },   // Taurus ? Top-Center-Right
            { 3, 4 },   // Gemini ? Top-Right
            { 4, 5 },   // Cancer ? Right-Top
            { 5, 6 },   // Leo ? Right-Bottom
            { 6, 7 },   // Virgo ? Bottom-Right
            { 7, 8 },   // Libra ? Bottom-Center-Right
            { 8, 9 },   // Scorpio ? Bottom-Center-Left
            { 9, 10 },  // Sagittarius ? Bottom-Left
            { 10, 11 }, // Capricorn ? Left-Bottom
            { 11, 12 }  // Aquarius ? Left-Top
        };

        // Create a map of Rasi to Planets
        var rasiToPlanets = new Dictionary<int, List<string>>();
        for (int i = 1; i <= 12; i++)
        {
            rasiToPlanets[i] = new List<string>();
        }

        // Group planets by their Rasi
        foreach (var planet in horoscope.Planets)
        {
            rasiToPlanets[planet.Rasi].Add(GetPlanetAbbreviation(planet.Name));
        }

        // Draw each Rasi section with its contents
        for (int rasiNum = 1; rasiNum <= 12; rasiNum++)
        {
            var sectionIndex = rasiToSection[rasiNum] - 1;
            var section = sectionCenters[sectionIndex];
            
            // Get Rasi info
            var rasiInfo = TamilHoroscope.Core.Data.TamilNames.Rasis[rasiNum];
            var planets = rasiToPlanets[rasiNum];
            
            // Determine if this Rasi contains the Lagna
            bool isLagna = (rasiNum == horoscope.LagnaRasi);

            // Create content for this section
            var stack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add Rasi number
            var rasiNumText = new TextBlock
            {
                Text = rasiNum.ToString(),
                FontSize = 9,
                FontWeight = FontWeights.Normal,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stack.Children.Add(rasiNumText);

            // Add Lagna marker if this is the Lagna Rasi
            if (isLagna)
            {
                var lagnaMarker = new TextBlock
                {
                    Text = "? As",  // Sun symbol + As for Ascendant
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Green,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                stack.Children.Add(lagnaMarker);
            }

            // Add planets in this Rasi
            if (planets.Count > 0)
            {
                var planetsText = new TextBlock
                {
                    Text = string.Join("\n", planets),
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.DarkRed,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                };
                stack.Children.Add(planetsText);
            }

            // Position the stack in the center of the section
            Canvas.SetLeft(stack, section.X - 20);
            Canvas.SetTop(stack, section.Y - 20);
            ChartCanvas.Children.Add(stack);
        }

        // Add chart title
        var title = new TextBlock
        {
            Text = "Rasi Chart (Jathagam)",
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.Black
        };
        Canvas.SetLeft(title, centerX - 90);
        Canvas.SetTop(title, margin);
        ChartCanvas.Children.Add(title);
        
        System.Diagnostics.Debug.WriteLine(">>> Chart drawing completed!");
    }

    private void DrawLine(double x1, double y1, double x2, double y2, double thickness = 1.5)
    {
        var line = new Line
        {
            X1 = x1,
            Y1 = y1,
            X2 = x2,
            Y2 = y2,
            Stroke = Brushes.Black,
            StrokeThickness = thickness
        };
        ChartCanvas.Children.Add(line);
    }

    private string GetPlanetAbbreviation(string planetName)
    {
        return planetName switch
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
            _ => planetName.Substring(0, Math.Min(2, planetName.Length))
        };
    }

    private string GetRasiAbbreviation(string rasiName)
    {
        return rasiName switch
        {
            "Aries" => "Ar",
            "Taurus" => "Ta",
            "Gemini" => "Ge",
            "Cancer" => "Cn",
            "Leo" => "Le",
            "Virgo" => "Vi",
            "Libra" => "Li",
            "Scorpio" => "Sc",
            "Sagittarius" => "Sg",
            "Capricorn" => "Cp",
            "Aquarius" => "Aq",
            "Pisces" => "Pi",
            _ => rasiName.Substring(0, Math.Min(2, rasiName.Length))
        };
    }
}
