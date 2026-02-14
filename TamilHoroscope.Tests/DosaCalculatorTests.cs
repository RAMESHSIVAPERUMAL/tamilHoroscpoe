using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Tests;

public class DosaCalculatorTests
{
    [Theory]
    [InlineData(1, true)]  // Mars in 1st house
    [InlineData(2, true)]  // Mars in 2nd house
    [InlineData(4, true)]  // Mars in 4th house
    [InlineData(7, true)]  // Mars in 7th house
    [InlineData(8, true)]  // Mars in 8th house
    [InlineData(12, true)] // Mars in 12th house
    [InlineData(3, false)] // Mars in 3rd house (no dosha)
    [InlineData(5, false)] // Mars in 5th house (no dosha)
    public void DetectMangalDosha_MarsInVariousHouses_ReturnsCorrectResult(int marsHouse, bool shouldHaveDosha)
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Mars").House = marsHouse;

        var calculator = new DosaCalculator();

        // Act
        var dosas = calculator.DetectDosas(horoscope);

        // Assert
        if (shouldHaveDosha)
        {
            Assert.Contains(dosas, d => d.Name == "Mangal Dosha (Kuja Dosha)");
        }
        else
        {
            Assert.DoesNotContain(dosas, d => d.Name == "Mangal Dosha (Kuja Dosha)");
        }
    }

    [Fact]
    public void DetectMangalDosha_SeverityHigherIn7thAnd8th()
    {
        // Arrange
        var horoscope1 = CreateTestHoroscope();
        horoscope1.Planets.First(p => p.Name == "Mars").House = 7;

        var horoscope2 = CreateTestHoroscope();
        horoscope2.Planets.First(p => p.Name == "Mars").House = 2;

        var calculator = new DosaCalculator();

        // Act
        var dosas1 = calculator.DetectDosas(horoscope1);
        var dosas2 = calculator.DetectDosas(horoscope2);

        // Assert
        var dosha1 = dosas1.First(d => d.Name == "Mangal Dosha (Kuja Dosha)");
        var dosha2 = dosas2.First(d => d.Name == "Mangal Dosha (Kuja Dosha)");

        Assert.True(dosha1.Severity > dosha2.Severity);
    }

    [Fact]
    public void DetectKaalSarpDosha_AllPlanetsBetweenRahuKetu_ReturnsDosha()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        // Place Rahu in house 3 and Ketu in house 9
        horoscope.Planets.First(p => p.Name == "Rahu").House = 3;
        horoscope.Planets.First(p => p.Name == "Ketu").House = 9;

        // Place all other planets between houses 3 and 9
        horoscope.Planets.First(p => p.Name == "Sun").House = 4;
        horoscope.Planets.First(p => p.Name == "Moon").House = 5;
        horoscope.Planets.First(p => p.Name == "Mars").House = 6;
        horoscope.Planets.First(p => p.Name == "Mercury").House = 7;
        horoscope.Planets.First(p => p.Name == "Jupiter").House = 8;
        horoscope.Planets.First(p => p.Name == "Venus").House = 4;
        horoscope.Planets.First(p => p.Name == "Saturn").House = 5;

        var calculator = new DosaCalculator();

        // Act
        var dosas = calculator.DetectDosas(horoscope);

        // Assert
        Assert.Contains(dosas, d => d.Name == "Kaal Sarp Dosha");
    }

    [Fact]
    public void DetectKaalSarpDosha_PlanetOutsideRahuKetuAxis_NoDosha()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        // Place Rahu in house 3 and Ketu in house 9
        horoscope.Planets.First(p => p.Name == "Rahu").House = 3;
        horoscope.Planets.First(p => p.Name == "Ketu").House = 9;

        // Place one planet outside the Rahu-Ketu axis
        horoscope.Planets.First(p => p.Name == "Sun").House = 1; // Outside

        var calculator = new DosaCalculator();

        // Act
        var dosas = calculator.DetectDosas(horoscope);

        // Assert
        Assert.DoesNotContain(dosas, d => d.Name == "Kaal Sarp Dosha");
    }

    [Fact]
    public void DetectPitraDosha_SunWithRahu_ReturnsDosha()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Sun").Rasi = 5;
        horoscope.Planets.First(p => p.Name == "Rahu").Rasi = 5; // Same rasi as Sun

        var calculator = new DosaCalculator();

        // Act
        var dosas = calculator.DetectDosas(horoscope);

        // Assert
        Assert.Contains(dosas, d => d.Name == "Pitra Dosha");
    }

    [Fact]
    public void DetectPitraDosha_RahuIn9thHouse_ReturnsDosha()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Rahu").House = 9;

        var calculator = new DosaCalculator();

        // Act
        var dosas = calculator.DetectDosas(horoscope);

        // Assert
        Assert.Contains(dosas, d => d.Name == "Pitra Dosha");
    }

    [Fact]
    public void DetectShakatDosha_MoonIn6thFromJupiter_ReturnsDosha()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Jupiter").House = 1;
        horoscope.Planets.First(p => p.Name == "Moon").House = 7; // 6th from Jupiter (wraps around)

        var calculator = new DosaCalculator();

        // Act
        var dosas = calculator.DetectDosas(horoscope);

        // Assert
        Assert.Contains(dosas, d => d.Name == "Shakat Dosha");
    }

    [Fact]
    public void DetectKemadrumaDosha_MoonAlone_ReturnsDosha()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Moon").Rasi = 5; // Leo

        // Make sure no planets in 2nd (Virgo) or 12th (Cancer) from Moon
        foreach (var planet in horoscope.Planets.Where(p => p.Name != "Moon"))
        {
            if (planet.Rasi == 6 || planet.Rasi == 4) // Virgo or Cancer
            {
                planet.Rasi = 1; // Move to Aries
            }
        }

        var calculator = new DosaCalculator();

        // Act
        var dosas = calculator.DetectDosas(horoscope);

        // Assert
        Assert.Contains(dosas, d => d.Name == "Kemadruma Dosha");
    }

    [Fact]
    public void DetectDosas_MultiLanguageSupport_ReturnsLocalizedNames()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Mars").House = 7; // Mangal dosha

        var calculator = new DosaCalculator();

        // Act
        var dosasTamil = calculator.DetectDosas(horoscope, "Tamil");
        var dosasTelugu = calculator.DetectDosas(horoscope, "Telugu");
        var dosasKannada = calculator.DetectDosas(horoscope, "Kannada");
        var dosasMalayalam = calculator.DetectDosas(horoscope, "Malayalam");

        // Assert
        var tamilDosa = dosasTamil.FirstOrDefault(d => d.Name == "Mangal Dosha (Kuja Dosha)");
        Assert.NotNull(tamilDosa);
        Assert.Equal("மங்கள தோஷம்", tamilDosa.LocalName);

        var teluguDosa = dosasTelugu.FirstOrDefault(d => d.Name == "Mangal Dosha (Kuja Dosha)");
        Assert.NotNull(teluguDosa);
        Assert.Equal("మంగళ దోషం", teluguDosa.LocalName);

        var kannadaDosa = dosasKannada.FirstOrDefault(d => d.Name == "Mangal Dosha (Kuja Dosha)");
        Assert.NotNull(kannadaDosa);
        Assert.Equal("ಮಂಗಳ ದೋಷ", kannadaDosa.LocalName);

        var malayalamDosa = dosasMalayalam.FirstOrDefault(d => d.Name == "Mangal Dosha (Kuja Dosha)");
        Assert.NotNull(malayalamDosa);
        Assert.Equal("മംഗള ദോഷം", malayalamDosa.LocalName);
    }

    [Fact]
    public void DetectDosas_AllRemediesProvided()
    {
        // Arrange
        var horoscope = CreateTestHoroscope();
        horoscope.Planets.First(p => p.Name == "Mars").House = 7; // Mangal dosha

        var calculator = new DosaCalculator();

        // Act
        var dosas = calculator.DetectDosas(horoscope);

        // Assert
        var mangalDosa = dosas.First(d => d.Name == "Mangal Dosha (Kuja Dosha)");
        Assert.NotEmpty(mangalDosa.Remedies);
        Assert.True(mangalDosa.Remedies.Count >= 3); // At least 3 remedies
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
