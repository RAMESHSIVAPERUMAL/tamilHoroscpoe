using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Desktop.Controls;

/// <summary>
/// Displays planetary strength as a bar chart
/// </summary>
public partial class PlanetStrengthChartControl : UserControl
{
    public PlanetStrengthChartControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Display the planetary strength chart
    /// </summary>
    public void DisplayStrengths(List<PlanetStrengthData> strengths)
    {
        ChartPanel.Children.Clear();

        if (strengths == null || strengths.Count == 0)
        {
            var noDataText = new TextBlock
            {
                Text = "No planetary strength data available",
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.Gray
            };
            ChartPanel.Children.Add(noDataText);
            dgDetailsTable.ItemsSource = null;
            return;
        }

        // Find maximum strength for scaling
        double maxStrength = strengths.Max(s => s.TotalStrength);
        maxStrength = Math.Max(maxStrength, 390); // Theoretical maximum

        // Create bar for each planet
        foreach (var strength in strengths)
        {
            var planetBar = CreatePlanetStrengthBar(strength, maxStrength);
            ChartPanel.Children.Add(planetBar);
        }

        // Populate the detailed components table
        dgDetailsTable.ItemsSource = strengths;

        // Add summary
        var strongPlanets = strengths.Where(s => s.StrengthPercentage >= 60).Count();
        var weakPlanets = strengths.Where(s => s.StrengthPercentage < 40).Count();
        
        SummaryText.Text = $"Summary: {strongPlanets} strong planets (?60%), {weakPlanets} weak planets (<40%)\n" +
                          $"Note: Rahu and Ketu are excluded as they don't have Shadbala in traditional astrology.\n" +
                          $"Minimum required strength varies by planet. Strength above required minimum indicates positive results.";
    }

    /// <summary>
    /// Create a bar display for one planet's strength
    /// </summary>
    private Grid CreatePlanetStrengthBar(PlanetStrengthData strength, double maxStrength)
    {
        var grid = new Grid
        {
            Margin = new Thickness(0, 0, 0, 12)
        };

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

        // Planet name and Tamil name
        var namePanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };
        var planetName = new TextBlock
        {
            Text = strength.Name,
            FontWeight = FontWeights.Bold,
            FontSize = 12
        };
        var tamilName = new TextBlock
        {
            Text = strength.TamilName,
            FontSize = 10,
            Foreground = Brushes.Gray
        };
        namePanel.Children.Add(planetName);
        namePanel.Children.Add(tamilName);
        Grid.SetColumn(namePanel, 0);
        grid.Children.Add(namePanel);

        // Bar container
        var barContainer = new Grid
        {
            Height = 30,
            Margin = new Thickness(5, 0, 5, 0)
        };

        // Background bar (light gray)
        var backgroundBar = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
            CornerRadius = new CornerRadius(3),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        barContainer.Children.Add(backgroundBar);

        // Strength bar (colored)
        var strengthPercentage = (strength.TotalStrength / maxStrength) * 100.0;
        var strengthBar = new Border
        {
            Background = GetStrengthColor(strength.StrengthPercentage),
            CornerRadius = new CornerRadius(3),
            HorizontalAlignment = HorizontalAlignment.Left,
            Width = (barContainer.ActualWidth > 0 ? barContainer.ActualWidth : 500) * (strengthPercentage / 100.0)
        };

        // Required strength indicator (vertical line)
        var requiredIndicator = new Border
        {
            Background = Brushes.Red,
            Width = 2,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness((strength.RequiredStrength / maxStrength) * (barContainer.ActualWidth > 0 ? barContainer.ActualWidth : 500), 0, 0, 0)
        };

        barContainer.Children.Add(strengthBar);
        barContainer.Children.Add(requiredIndicator);

        // Add strength text inside the bar
        var strengthText = new TextBlock
        {
            Text = $"{strength.TotalStrength:F1}R ({strength.TotalStrengthVirupas:F0}V)",
            Foreground = Brushes.White,
            FontWeight = FontWeights.Bold,
            FontSize = 11,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5, 0, 0, 0)
        };
        barContainer.Children.Add(strengthText);

        Grid.SetColumn(barContainer, 1);
        grid.Children.Add(barContainer);

        // Strength grade
        var gradePanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        var gradeText = new TextBlock
        {
            Text = strength.StrengthGrade,
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = GetStrengthColor(strength.StrengthPercentage)
        };
        var percentText = new TextBlock
        {
            Text = $"{strength.StrengthPercentage:F0}%",
            FontSize = 10,
            Foreground = Brushes.Gray
        };
        gradePanel.Children.Add(gradeText);
        gradePanel.Children.Add(percentText);
        Grid.SetColumn(gradePanel, 2);
        grid.Children.Add(gradePanel);

        return grid;
    }

    /// <summary>
    /// Get color based on strength percentage
    /// </summary>
    private SolidColorBrush GetStrengthColor(double percentage)
    {
        return percentage switch
        {
            >= 80 => new SolidColorBrush(Color.FromRgb(34, 139, 34)),   // Forest Green
            >= 60 => new SolidColorBrush(Color.FromRgb(50, 205, 50)),   // Lime Green
            >= 40 => new SolidColorBrush(Color.FromRgb(255, 215, 0)),   // Gold
            >= 20 => new SolidColorBrush(Color.FromRgb(255, 140, 0)),   // Dark Orange
            _ => new SolidColorBrush(Color.FromRgb(220, 20, 60))        // Crimson
        };
    }
}
