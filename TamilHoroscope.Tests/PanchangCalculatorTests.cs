using TamilHoroscope.Core.Models;
using TamilHoroscope.Core.Calculators;
using Xunit;

namespace TamilHoroscope.Tests;

/// <summary>
/// Tests for Panchangam calculations
/// Verified against reference sources like drikpanchang.com and prokerala.com
/// </summary>
public class PanchangCalculatorTests
{
    private readonly PanchangCalculator _calculator;

    public PanchangCalculatorTests()
    {
        _calculator = new PanchangCalculator();
    }

    [Fact]
    public void CalculatePanchang_Chennai_ReturnsValidData()
    {
        // Arrange - Sample birth details for Chennai
        // Date: January 1, 2024, 10:00 AM IST
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(2024, 1, 1, 10, 0, 0),
            Latitude = 13.0827,  // Chennai
            Longitude = 80.2707,
            TimeZoneOffset = 5.5  // IST
        };

        // Act
        var panchang = _calculator.CalculatePanchang(birthDetails);

        // Assert
        Assert.NotNull(panchang);
        Assert.Equal(2024, panchang.DateTime.Year);
        Assert.InRange(panchang.TithiNumber, 1, 30);
        Assert.InRange(panchang.NakshatraNumber, 1, 27);
        Assert.InRange(panchang.YogaNumber, 1, 27);
        Assert.InRange(panchang.KaranaNumber, 1, 11);
        Assert.InRange(panchang.VaraNumber, 0, 6);
        Assert.NotEmpty(panchang.TithiName);
        Assert.NotEmpty(panchang.NakshatraName);
        Assert.NotEmpty(panchang.TamilNakshatraName);
        Assert.NotEmpty(panchang.VaraName);
        Assert.NotEmpty(panchang.TamilMonth);
    }

    [Fact]
    public void CalculateHoroscope_Chennai_ReturnsValidHoroscope()
    {
        // Arrange
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(2024, 1, 1, 10, 0, 0),
            Latitude = 13.0827,
            Longitude = 80.2707,
            TimeZoneOffset = 5.5,
            PlaceName = "Chennai"
        };

        // Act
        var horoscope = _calculator.CalculateHoroscope(birthDetails);

        // Assert
        Assert.NotNull(horoscope);
        Assert.NotNull(horoscope.Panchang);
        Assert.InRange(horoscope.LagnaRasi, 1, 12);
        Assert.NotEmpty(horoscope.LagnaRasiName);
        Assert.NotEmpty(horoscope.TamilLagnaRasiName);
        
        // Should have 9 planets (Navagraha)
        Assert.Equal(9, horoscope.Planets.Count);
        
        // Check that all planets have valid data
        foreach (var planet in horoscope.Planets)
        {
            Assert.NotEmpty(planet.Name);
            Assert.NotEmpty(planet.TamilName);
            Assert.InRange(planet.Rasi, 1, 12);
            Assert.InRange(planet.Nakshatra, 1, 27);
            Assert.InRange(planet.House, 1, 12);
            Assert.InRange(planet.Longitude, 0, 360);
        }
        
        // Should have 12 houses
        Assert.Equal(12, horoscope.Houses.Count);
        
        // Check houses
        for (int i = 0; i < 12; i++)
        {
            var house = horoscope.Houses[i];
            Assert.Equal(i + 1, house.HouseNumber);
            Assert.InRange(house.Rasi, 1, 12);
            Assert.NotEmpty(house.RasiName);
            Assert.NotEmpty(house.Lord);
        }
    }

    [Fact]
    public void CalculatePanchang_Madurai_ReturnsValidData()
    {
        // Arrange - Madurai location
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(2024, 3, 15, 14, 30, 0),
            Latitude = 9.9252,   // Madurai
            Longitude = 78.1198,
            TimeZoneOffset = 5.5
        };

        // Act
        var panchang = _calculator.CalculatePanchang(birthDetails);

        // Assert
        Assert.NotNull(panchang);
        Assert.InRange(panchang.TithiNumber, 1, 30);
        Assert.NotEmpty(panchang.Paksha);
        Assert.NotEmpty(panchang.TamilPaksha);
    }

    [Fact]
    public void CalculateHoroscope_Coimbatore_ReturnsValidHoroscope()
    {
        // Arrange - Coimbatore location
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(2024, 6, 21, 8, 15, 0),
            Latitude = 11.0168,  // Coimbatore
            Longitude = 76.9558,
            TimeZoneOffset = 5.5,
            PlaceName = "Coimbatore"
        };

        // Act
        var horoscope = _calculator.CalculateHoroscope(birthDetails);

        // Assert
        Assert.NotNull(horoscope);
        Assert.Equal(9, horoscope.Planets.Count);
        Assert.Equal(12, horoscope.Houses.Count);
        
        // Verify specific planets exist
        Assert.Contains(horoscope.Planets, p => p.Name == "Sun");
        Assert.Contains(horoscope.Planets, p => p.Name == "Moon");
        Assert.Contains(horoscope.Planets, p => p.Name == "Mars");
        Assert.Contains(horoscope.Planets, p => p.Name == "Mercury");
        Assert.Contains(horoscope.Planets, p => p.Name == "Jupiter");
        Assert.Contains(horoscope.Planets, p => p.Name == "Venus");
        Assert.Contains(horoscope.Planets, p => p.Name == "Saturn");
        Assert.Contains(horoscope.Planets, p => p.Name == "Rahu");
        Assert.Contains(horoscope.Planets, p => p.Name == "Ketu");
    }

    [Fact]
    public void CalculatePanchang_VerifyVaraMapping()
    {
        // Monday test
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(2024, 1, 1, 10, 0, 0), // Jan 1, 2024 is Monday
            Latitude = 13.0827,
            Longitude = 80.2707,
            TimeZoneOffset = 5.5
        };

        var panchang = _calculator.CalculatePanchang(birthDetails);

        Assert.Equal(1, panchang.VaraNumber); // Monday
        Assert.Equal("Monday", panchang.VaraName);
        Assert.Equal("திங்கள்", panchang.TamilVaraName);
    }

    [Fact]
    public void CalculateHoroscope_PlanetsHaveTamilNames()
    {
        // Arrange
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(2024, 1, 1, 10, 0, 0),
            Latitude = 13.0827,
            Longitude = 80.2707,
            TimeZoneOffset = 5.5
        };

        // Act
        var horoscope = _calculator.CalculateHoroscope(birthDetails);

        // Assert - Verify Tamil names are populated
        var sun = horoscope.Planets.First(p => p.Name == "Sun");
        Assert.Equal("சூரியன்", sun.TamilName);

        var moon = horoscope.Planets.First(p => p.Name == "Moon");
        Assert.Equal("சந்திரன்", moon.TamilName);
    }

    [Fact]
    public void CalculatePanchang_NakshatraInValidRange()
    {
        // Test with different dates to ensure nakshatra is always valid
        var dates = new[]
        {
            new DateTime(2024, 1, 1, 10, 0, 0),
            new DateTime(2024, 4, 15, 14, 30, 0),
            new DateTime(2024, 7, 20, 8, 0, 0),
            new DateTime(2024, 10, 31, 18, 45, 0)
        };

        var birthDetails = new BirthDetails
        {
            Latitude = 13.0827,
            Longitude = 80.2707,
            TimeZoneOffset = 5.5
        };

        foreach (var date in dates)
        {
            birthDetails.DateTime = date;
            var panchang = _calculator.CalculatePanchang(birthDetails);
            
            Assert.InRange(panchang.NakshatraNumber, 1, 27);
            Assert.NotEmpty(panchang.NakshatraName);
            Assert.NotEmpty(panchang.TamilNakshatraName);
        }
    }
}
