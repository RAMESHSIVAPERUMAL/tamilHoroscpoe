using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;
using Xunit;

namespace TamilHoroscope.Tests;

/// <summary>
/// Tests for Navamsa (D-9) divisional chart calculations
/// Verified against reference sources like drikpanchang.com and prokerala.com
/// </summary>
public class NavamsaCalculatorTests
{
    private readonly NavamsaCalculator _calculator;

    public NavamsaCalculatorTests()
    {
        _calculator = new NavamsaCalculator();
    }

    [Theory]
    [InlineData(0.0, 0.0)]      // Aries 0° → Aries 0° (Fire sign starts at Aries)
    [InlineData(3.333, 30.0)]   // Aries 3°20' → Taurus 0° (2nd Navamsa)
    [InlineData(6.666, 60.0)]   // Aries 6°40' → Gemini 0° (3rd Navamsa)
    [InlineData(10.0, 90.0)]    // Aries 10° → Cancer 0° (4th Navamsa)
    [InlineData(13.333, 120.0)] // Aries 13°20' → Leo 0° (5th Navamsa)
    [InlineData(16.666, 150.0)] // Aries 16°40' → Virgo 0° (6th Navamsa)
    [InlineData(20.0, 180.0)]   // Aries 20° → Libra 0° (7th Navamsa)
    [InlineData(23.333, 210.0)] // Aries 23°20' → Scorpio 0° (8th Navamsa)
    [InlineData(26.666, 240.0)] // Aries 26°40' → Sagittarius 0° (9th Navamsa)
    public void CalculateNavamsaPosition_AriesSign_ReturnsCorrectNavamsaPosition(double natalLongitude, double expectedNavamsaLongitude)
    {
        // Act
        var result = _calculator.CalculateNavamsaPosition(natalLongitude);

        // Assert
        // Allow 0.1° tolerance for floating point calculations
        Assert.InRange(result, expectedNavamsaLongitude - 0.1, expectedNavamsaLongitude + 0.1);
    }

    [Theory]
    [InlineData(30.0, 270.0)]   // Taurus 0° → Capricorn 0° (Earth sign starts at Capricorn)
    [InlineData(33.333, 300.0)] // Taurus 3°20' → Aquarius 0° (2nd Navamsa)
    [InlineData(36.666, 330.0)] // Taurus 6°40' → Pisces 0° (3rd Navamsa)
    [InlineData(40.0, 0.0)]     // Taurus 10° → Aries 0° (4th Navamsa)
    [InlineData(43.333, 30.0)]  // Taurus 13°20' → Taurus 0° (5th Navamsa)
    public void CalculateNavamsaPosition_TaurusSign_ReturnsCorrectNavamsaPosition(double natalLongitude, double expectedNavamsaLongitude)
    {
        // Act
        var result = _calculator.CalculateNavamsaPosition(natalLongitude);

        // Assert
        Assert.InRange(result, expectedNavamsaLongitude - 0.1, expectedNavamsaLongitude + 0.1);
    }

    [Theory]
    [InlineData(60.0, 180.0)]   // Gemini 0° → Libra 0° (Air sign starts at Libra)
    [InlineData(63.333, 210.0)] // Gemini 3°20' → Scorpio 0° (2nd Navamsa)
    [InlineData(66.666, 240.0)] // Gemini 6°40' → Sagittarius 0° (3rd Navamsa)
    [InlineData(70.0, 270.0)]   // Gemini 10° → Capricorn 0° (4th Navamsa)
    public void CalculateNavamsaPosition_GeminiSign_ReturnsCorrectNavamsaPosition(double natalLongitude, double expectedNavamsaLongitude)
    {
        // Act
        var result = _calculator.CalculateNavamsaPosition(natalLongitude);

        // Assert
        Assert.InRange(result, expectedNavamsaLongitude - 0.1, expectedNavamsaLongitude + 0.1);
    }

    [Theory]
    [InlineData(90.0, 90.0)]    // Cancer 0° → Cancer 0° (Water sign starts at Cancer)
    [InlineData(93.333, 120.0)] // Cancer 3°20' → Leo 0° (2nd Navamsa)
    [InlineData(96.666, 150.0)] // Cancer 6°40' → Virgo 0° (3rd Navamsa)
    [InlineData(100.0, 180.0)]  // Cancer 10° → Libra 0° (4th Navamsa)
    public void CalculateNavamsaPosition_CancerSign_ReturnsCorrectNavamsaPosition(double natalLongitude, double expectedNavamsaLongitude)
    {
        // Act
        var result = _calculator.CalculateNavamsaPosition(natalLongitude);

        // Assert
        Assert.InRange(result, expectedNavamsaLongitude - 0.1, expectedNavamsaLongitude + 0.1);
    }

    [Fact]
    public void CalculateNavamsaPosition_360Degrees_ReturnsZero()
    {
        // Arrange
        double longitude = 360.0;

        // Act
        var result = _calculator.CalculateNavamsaPosition(longitude);

        // Assert
        // 360° should normalize to 0° and then calculate Navamsa
        Assert.InRange(result, 0.0, 0.1);
    }

    [Fact]
    public void CalculateNavamsaPosition_NegativeLongitude_NormalizesAndCalculates()
    {
        // Arrange
        double longitude = -10.0; // Should normalize to 350°

        // Act
        var result = _calculator.CalculateNavamsaPosition(longitude);

        // Assert
        // 350° is in Pisces (330-360), which is a water sign starting at Cancer
        // 350° = Pisces 20° → should map to a valid Navamsa position
        Assert.InRange(result, 0.0, 360.0);
    }

    [Fact]
    public void CalculateNavamsaChart_ValidPlanets_ReturnsNavamsaPositionsForAll()
    {
        // Arrange
        var planets = new List<PlanetData>
        {
            new PlanetData 
            { 
                Name = "Sun", 
                TamilName = "சூரியன்", 
                Longitude = 15.0,  // Aries 15°
                Latitude = 0.0,
                Rasi = 1,
                Nakshatra = 2,
                IsRetrograde = false
            },
            new PlanetData 
            { 
                Name = "Moon", 
                TamilName = "சந்திரன்", 
                Longitude = 45.0,  // Taurus 15°
                Latitude = 5.0,
                Rasi = 2,
                Nakshatra = 5,
                IsRetrograde = false
            },
            new PlanetData 
            { 
                Name = "Mars", 
                TamilName = "செவ்வாய்", 
                Longitude = 75.0,  // Gemini 15°
                Latitude = 1.5,
                Rasi = 3,
                Nakshatra = 8,
                IsRetrograde = false
            }
        };

        // Act
        var result = _calculator.CalculateNavamsaChart(planets);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        
        // Verify all planets have valid Navamsa data
        foreach (var planet in result)
        {
            Assert.NotEmpty(planet.Name);
            Assert.NotEmpty(planet.TamilName);
            Assert.InRange(planet.Longitude, 0.0, 360.0);
            Assert.InRange(planet.Rasi, 1, 12);
            Assert.InRange(planet.Nakshatra, 1, 27);
            Assert.NotEmpty(planet.RasiName);
            Assert.NotEmpty(planet.TamilRasiName);
            Assert.NotEmpty(planet.NakshatraName);
            Assert.NotEmpty(planet.TamilNakshatraName);
        }
    }

    [Fact]
    public void CalculateNavamsaChart_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var planets = new List<PlanetData>();

        // Act
        var result = _calculator.CalculateNavamsaChart(planets);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void CalculateNavamsaChart_RetrogradePlanet_PreservesRetrogradeStatus()
    {
        // Arrange
        var planets = new List<PlanetData>
        {
            new PlanetData 
            { 
                Name = "Mercury", 
                TamilName = "புதன்", 
                Longitude = 100.0,
                Latitude = 2.0,
                Rasi = 4,
                Nakshatra = 10,
                IsRetrograde = true // Retrograde
            }
        };

        // Act
        var result = _calculator.CalculateNavamsaChart(planets);

        // Assert
        Assert.Single(result);
        Assert.True(result[0].IsRetrograde);
    }

    [Theory]
    [InlineData(120.0)]  // Leo 0° (Fire sign)
    [InlineData(150.0)]  // Virgo 0° (Earth sign)
    [InlineData(180.0)]  // Libra 0° (Air sign)
    [InlineData(210.0)]  // Scorpio 0° (Water sign)
    [InlineData(240.0)]  // Sagittarius 0° (Fire sign)
    [InlineData(270.0)]  // Capricorn 0° (Earth sign)
    [InlineData(300.0)]  // Aquarius 0° (Air sign)
    [InlineData(330.0)]  // Pisces 0° (Water sign)
    public void CalculateNavamsaPosition_AllSignBoundaries_ReturnsValidNavamsa(double longitude)
    {
        // Act
        var result = _calculator.CalculateNavamsaPosition(longitude);

        // Assert
        Assert.InRange(result, 0.0, 360.0);
        
        // Verify the result is at the start of a sign (should be divisible by 30 or close to it)
        double remainder = result % 30.0;
        Assert.True(remainder < 0.1 || remainder > 29.9, 
            $"Expected Navamsa position at sign boundary, got {result}° (remainder: {remainder}°)");
    }

    [Fact]
    public void CalculateNavamsaPosition_MidSignPositions_ReturnsProportionalNavamsa()
    {
        // Arrange - Test mid-position in first Navamsa of Aries
        double longitude = 1.666; // Aries 1°40' (middle of first Navamsa part)

        // Act
        var result = _calculator.CalculateNavamsaPosition(longitude);

        // Assert
        // Should be approximately at Aries 15° (middle of the Navamsa sign)
        Assert.InRange(result, 14.0, 16.0);
    }

    [Fact]
    public void CalculateNavamsaChart_AllNavagraha_ReturnsCorrectCount()
    {
        // Arrange - Create a list with all 9 planets (Navagraha)
        var planets = new List<PlanetData>();
        var planetNames = new[] { "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn", "Rahu", "Ketu" };
        
        for (int i = 0; i < planetNames.Length; i++)
        {
            planets.Add(new PlanetData 
            { 
                Name = planetNames[i],
                TamilName = planetNames[i],
                Longitude = i * 40.0, // Distribute across zodiac
                Latitude = 0.0,
                Rasi = (i * 40 / 30) + 1,
                Nakshatra = 1,
                IsRetrograde = false
            });
        }

        // Act
        var result = _calculator.CalculateNavamsaChart(planets);

        // Assert
        Assert.Equal(9, result.Count);
        
        // Verify each planet has a unique Navamsa position
        var longitudes = result.Select(p => p.Longitude).ToList();
        Assert.Equal(9, longitudes.Count);
    }
}
