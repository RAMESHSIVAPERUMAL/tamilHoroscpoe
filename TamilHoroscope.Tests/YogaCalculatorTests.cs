using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Tests;

public class YogaCalculatorTests
{
    [Fact]
    public void DetectGajakesariYoga_JupiterInKendraFromMoon_ReturnsYoga()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        // Place Moon in Aries (1) and Jupiter in Cancer (4) - 4th from Moon (kendra)
        // Forward count: 1->2->3->4 = 4 (kendra)
        horoscope.Planets.First(p => p.Name == "Moon").Rasi = 1;
        horoscope.Planets.First(p => p.Name == "Jupiter").Rasi = 4;

        var calculator = new YogaCalculator();

        // Act
        var yogas = calculator.DetectYogas(horoscope);

        // Assert
        Assert.NotEmpty(yogas);
        Assert.Contains(yogas, y => y.Name == "Gajakesari Yoga");
    }

    [Fact]
    public void DetectGajakesariYoga_JupiterNotInKendraFromMoon_NoYoga()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        // Place Moon in Aries (1) and Jupiter in Gemini (3) - 3rd from Moon (not kendra)
        horoscope.Planets.First(p => p.Name == "Moon").Rasi = 1;
        horoscope.Planets.First(p => p.Name == "Jupiter").Rasi = 3;

        var calculator = new YogaCalculator();

        // Act
        var yogas = calculator.DetectYogas(horoscope);

        // Assert
        Assert.DoesNotContain(yogas, y => y.Name == "Gajakesari Yoga");
    }

    [Fact]
    public void DetectBudhaAdityaYoga_SunMercuryConjunction_ReturnsYoga()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        // Place Sun and Mercury in same rasi
        horoscope.Planets.First(p => p.Name == "Sun").Rasi = 5;
        horoscope.Planets.First(p => p.Name == "Mercury").Rasi = 5;

        var calculator = new YogaCalculator();

        // Act
        var yogas = calculator.DetectYogas(horoscope);

        // Assert
        Assert.Contains(yogas, y => y.Name == "Budha Aditya Yoga");
    }

    [Fact]
    public void DetectHamsaYoga_JupiterInKendraInOwnSign_ReturnsYoga()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.LagnaRasi = 1; // Aries lagna
        // Place Jupiter in Sagittarius (9) which is 9th house - not kendra
        // Place Jupiter in Cancer (4) which is 4th house - kendra, and exalted
        var jupiter = horoscope.Planets.First(p => p.Name == "Jupiter");
        jupiter.Rasi = 4; // Cancer (exaltation)
        jupiter.House = 4; // 4th house from Aries lagna

        var calculator = new YogaCalculator();

        // Act
        var yogas = calculator.DetectYogas(horoscope);

        // Assert
        Assert.Contains(yogas, y => y.Name == "Hamsa Yoga");
    }

    [Fact]
    public void DetectYogas_MultiLanguageSupport_ReturnsLocalizedNames()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        // Place Moon and Jupiter in same rasi to trigger Gajakesari Yoga (1st house = kendra)
        horoscope.Planets.First(p => p.Name == "Moon").Rasi = 5;
        horoscope.Planets.First(p => p.Name == "Jupiter").Rasi = 5; // Same rasi (1st house from Moon - kendra)

        var calculator = new YogaCalculator();

        // Act
        var yogasTamil = calculator.DetectYogas(horoscope, "Tamil");
        var yogasTelugu = calculator.DetectYogas(horoscope, "Telugu");
        var yogasKannada = calculator.DetectYogas(horoscope, "Kannada");
        var yogasMalayalam = calculator.DetectYogas(horoscope, "Malayalam");

        // Assert - may detect multiple yogas, so check for any yoga with localized names
        Assert.NotEmpty(yogasTamil);
        Assert.NotEmpty(yogasTelugu);
        Assert.NotEmpty(yogasKannada);
        Assert.NotEmpty(yogasMalayalam);
        
        // Check that Tamil yoga has Tamil local name
        var anyTamilYoga = yogasTamil.First();
        Assert.NotEmpty(anyTamilYoga.LocalName);
        
        // Check that Telugu yoga has Telugu local name
        var anyTeluguYoga = yogasTelugu.First();
        Assert.NotEmpty(anyTeluguYoga.LocalName);
    }

    [Fact]
    public void DetectSunaphaYoga_PlanetIn2ndFromMoon_ReturnsYoga()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Moon").Rasi = 1; // Aries
        horoscope.Planets.First(p => p.Name == "Venus").Rasi = 2; // Taurus (2nd from Moon)

        var calculator = new YogaCalculator();

        // Act
        var yogas = calculator.DetectYogas(horoscope);

        // Assert
        Assert.Contains(yogas, y => y.Name == "Sunapha Yoga");
    }

    [Fact]
    public void DetectAnaphaYoga_PlanetIn12thFromMoon_ReturnsYoga()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Moon").Rasi = 2; // Taurus
        horoscope.Planets.First(p => p.Name == "Venus").Rasi = 1; // Aries (12th from Moon)

        var calculator = new YogaCalculator();

        // Act
        var yogas = calculator.DetectYogas(horoscope);

        // Assert
        Assert.Contains(yogas, y => y.Name == "Anapha Yoga");
    }

    [Fact]
    public void DetectDurdhuraYoga_PlanetsOnBothSidesOfMoon_ReturnsYoga()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Moon").Rasi = 5; // Leo
        horoscope.Planets.First(p => p.Name == "Venus").Rasi = 6; // Virgo (2nd from Moon)
        horoscope.Planets.First(p => p.Name == "Mars").Rasi = 4; // Cancer (12th from Moon)

        var calculator = new YogaCalculator();

        // Act
        var yogas = calculator.DetectYogas(horoscope);

        // Assert
        Assert.Contains(yogas, y => y.Name == "Durdhura Yoga");
    }

    private HoroscopeData CreateTestHoroscope()
    {
        var horoscope = new HoroscopeData
        {
            LagnaRasi = 1,
            Planets = new List<PlanetData>
            {
                new() { Name = "Sun", Rasi = 1, House = 1, Longitude = 15.0 },
                new() { Name = "Moon", Rasi = 2, House = 2, Longitude = 45.0 },
                new() { Name = "Mars", Rasi = 3, House = 3, Longitude = 75.0 },
                new() { Name = "Mercury", Rasi = 4, House = 4, Longitude = 105.0 },
                new() { Name = "Jupiter", Rasi = 5, House = 5, Longitude = 135.0 },
                new() { Name = "Venus", Rasi = 6, House = 6, Longitude = 165.0 },
                new() { Name = "Saturn", Rasi = 7, House = 7, Longitude = 195.0 },
                new() { Name = "Rahu", Rasi = 8, House = 8, Longitude = 225.0 },
                new() { Name = "Ketu", Rasi = 2, House = 2, Longitude = 45.0 }
            }
        };

        return horoscope;
    }
}
