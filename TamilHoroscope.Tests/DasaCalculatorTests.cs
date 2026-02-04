using TamilHoroscope.Core.Models;
using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Data;
using Xunit;

namespace TamilHoroscope.Tests;

/// <summary>
/// Tests for Vimshottari Dasa calculations
/// </summary>
public class DasaCalculatorTests
{
    private readonly DasaCalculator _dasaCalculator;
    private readonly PanchangCalculator _panchangCalculator;

    public DasaCalculatorTests()
    {
        _dasaCalculator = new DasaCalculator();
        _panchangCalculator = new PanchangCalculator();
    }

    [Fact]
    public void CalculateVimshottariDasa_ValidInput_ReturnsCorrectNumberOfDasas()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1, 10, 0, 0);
        int moonNakshatra = 10; // Magha - Ketu Dasa
        double moonLongitude = 126.0; // Within Magha range (120° - 133.33°)

        // Act
        var dasas = _dasaCalculator.CalculateVimshottariDasa(birthDate, moonNakshatra, moonLongitude, 120);

        // Assert
        Assert.NotNull(dasas);
        Assert.NotEmpty(dasas);
        
        // Should cover 120 years (may slightly exceed due to balance calculation)
        var totalYears = dasas.Sum(d => d.DurationYears);
        Assert.InRange(totalYears, 118, 125); // Allow slight variance due to balance and rounding
    }

    [Fact]
    public void CalculateVimshottariDasa_MaghaNakshatra_StartsWithKetuDasa()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1, 10, 0, 0);
        int moonNakshatra = 10; // Magha - Ketu Dasa
        double moonLongitude = 126.0; // Within Magha

        // Act
        var dasas = _dasaCalculator.CalculateVimshottariDasa(birthDate, moonNakshatra, moonLongitude, 50);

        // Assert
        Assert.Equal("Ketu", dasas[0].Lord);
        Assert.Equal(TamilNames.Planets["Ketu"], dasas[0].TamilLord);
    }

    [Fact]
    public void CalculateVimshottariDasa_BharaniNakshatra_StartsWithVenusDasa()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1, 10, 0, 0);
        int moonNakshatra = 2; // Bharani - Venus Dasa
        double moonLongitude = 20.0; // Within Bharani range (13.33° - 26.67°)

        // Act
        var dasas = _dasaCalculator.CalculateVimshottariDasa(birthDate, moonNakshatra, moonLongitude, 50);

        // Assert
        Assert.Equal("Venus", dasas[0].Lord);
        Assert.Equal(20, TamilNames.DasaDurations["Venus"]); // Venus is 20 years
    }

    [Fact]
    public void CalculateVimshottariDasa_FollowsCorrectSequence()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1, 10, 0, 0);
        int moonNakshatra = 1; // Ashwini - Ketu Dasa
        double moonLongitude = 0.5; // Beginning of Ashwini

        // Act
        var dasas = _dasaCalculator.CalculateVimshottariDasa(birthDate, moonNakshatra, moonLongitude, 120);

        // Assert - Verify the sequence follows: Ketu, Venus, Sun, Moon, Mars, Rahu, Jupiter, Saturn, Mercury
        var expectedSequence = new[] { "Ketu", "Venus", "Sun", "Moon", "Mars", "Rahu", "Jupiter", "Saturn", "Mercury" };
        
        for (int i = 0; i < Math.Min(9, dasas.Count); i++)
        {
            int sequenceIndex = i % 9;
            Assert.Equal(expectedSequence[sequenceIndex], dasas[i].Lord);
        }
    }

    [Fact]
    public void CalculateVimshottariDasa_DasaPeriodsAreContinuous()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1, 10, 0, 0);
        int moonNakshatra = 5; // Mrigashira - Mars Dasa
        double moonLongitude = 60.0;

        // Act
        var dasas = _dasaCalculator.CalculateVimshottariDasa(birthDate, moonNakshatra, moonLongitude, 50);

        // Assert - Each Dasa should start where the previous one ends
        for (int i = 1; i < dasas.Count; i++)
        {
            Assert.Equal(dasas[i - 1].EndDate, dasas[i].StartDate);
        }
    }

    [Fact]
    public void CalculateVimshottariDasa_EachDasaHasNineBhuktis()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1, 10, 0, 0);
        int moonNakshatra = 7; // Punarvasu - Jupiter Dasa
        double moonLongitude = 86.67;

        // Act
        var dasas = _dasaCalculator.CalculateVimshottariDasa(birthDate, moonNakshatra, moonLongitude, 40);

        // Assert - Each Dasa should have 9 Bhuktis
        foreach (var dasa in dasas)
        {
            Assert.Equal(9, dasa.Bhuktis.Count);
        }
    }

    [Fact]
    public void CalculateVimshottariDasa_BhuktisAreContinuous()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1, 10, 0, 0);
        int moonNakshatra = 8; // Pushya - Saturn Dasa
        double moonLongitude = 100.0;

        // Act
        var dasas = _dasaCalculator.CalculateVimshottariDasa(birthDate, moonNakshatra, moonLongitude, 40);

        // Assert - Each Bhukti should start where the previous one ends
        foreach (var dasa in dasas)
        {
            for (int i = 1; i < dasa.Bhuktis.Count; i++)
            {
                Assert.Equal(dasa.Bhuktis[i - 1].EndDate, dasa.Bhuktis[i].StartDate);
            }
        }
    }

    [Fact]
    public void CalculateVimshottariDasa_BhuktiSequenceStartsWithDasaLord()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1, 10, 0, 0);
        int moonNakshatra = 9; // Ashlesha - Mercury Dasa
        double moonLongitude = 113.33;

        // Act
        var dasas = _dasaCalculator.CalculateVimshottariDasa(birthDate, moonNakshatra, moonLongitude, 40);

        // Assert - First Bhukti should be of the same lord as Dasa
        foreach (var dasa in dasas.Take(3)) // Check first 3 dasas
        {
            Assert.Equal(dasa.Lord, dasa.Bhuktis[0].Lord);
        }
    }

    [Fact]
    public void CalculateHoroscopeWithDasa_Chennai_ReturnsValidDasaData()
    {
        // Arrange
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(2000, 1, 1, 10, 0, 0),
            Latitude = 13.0827,
            Longitude = 80.2707,
            TimeZoneOffset = 5.5,
            PlaceName = "Chennai"
        };

        // Act
        var horoscope = _panchangCalculator.CalculateHoroscope(birthDetails, includeDasa: true, dasaYears: 120);

        // Assert
        Assert.NotNull(horoscope.VimshottariDasas);
        Assert.NotEmpty(horoscope.VimshottariDasas);
        
        // Verify Dasa is based on Moon's nakshatra
        var expectedDasaLord = TamilNames.NakshatraDasaLord[horoscope.Panchang.NakshatraNumber];
        Assert.Equal(expectedDasaLord, horoscope.VimshottariDasas[0].Lord);
    }

    [Fact]
    public void CalculateHoroscopeWithoutDasa_DoesNotIncludeDasaData()
    {
        // Arrange
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(2000, 1, 1, 10, 0, 0),
            Latitude = 13.0827,
            Longitude = 80.2707,
            TimeZoneOffset = 5.5,
            PlaceName = "Chennai"
        };

        // Act
        var horoscope = _panchangCalculator.CalculateHoroscope(birthDetails, includeDasa: false);

        // Assert
        Assert.Null(horoscope.VimshottariDasas);
    }

    [Theory]
    [InlineData(1, "Ketu")]
    [InlineData(2, "Venus")]
    [InlineData(3, "Sun")]
    [InlineData(4, "Moon")]
    [InlineData(5, "Mars")]
    [InlineData(6, "Rahu")]
    [InlineData(7, "Jupiter")]
    [InlineData(8, "Saturn")]
    [InlineData(9, "Mercury")]
    [InlineData(10, "Ketu")]
    [InlineData(27, "Mercury")]
    public void NakshatraDasaLord_ReturnsCorrectLord(int nakshatra, string expectedLord)
    {
        // Act
        var lord = TamilNames.NakshatraDasaLord[nakshatra];

        // Assert
        Assert.Equal(expectedLord, lord);
    }

    [Theory]
    [InlineData("Ketu", 7)]
    [InlineData("Venus", 20)]
    [InlineData("Sun", 6)]
    [InlineData("Moon", 10)]
    [InlineData("Mars", 7)]
    [InlineData("Rahu", 18)]
    [InlineData("Jupiter", 16)]
    [InlineData("Saturn", 19)]
    [InlineData("Mercury", 17)]
    public void DasaDurations_ReturnsCorrectYears(string planet, int expectedYears)
    {
        // Act
        var years = TamilNames.DasaDurations[planet];

        // Assert
        Assert.Equal(expectedYears, years);
    }

    [Fact]
    public void DasaDurations_SumEquals120Years()
    {
        // Act
        var totalYears = TamilNames.DasaDurations.Values.Sum();

        // Assert
        Assert.Equal(120, totalYears);
    }
}
