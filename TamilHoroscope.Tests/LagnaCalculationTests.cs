using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;
using Xunit;

namespace TamilHoroscope.Tests;

public class LagnaCalculationTests
{
    [Fact]
    public void TestRameshBirthChart_ShouldShowCancerAscendant()
    {
        // Arrange
        var calculator = new PanchangCalculator();
        
        // Birth details from the document:
        // Date: July 18, 1983, Monday
        // Time: 6:35 AM IST (+5:30)
        // Place: Kumbakonam, Tamil Nadu, India
        // Coordinates: Approximately 10.9601°N, 79.3845°E
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(1983, 7, 18, 6, 35, 0),
            Latitude = 10.9601,
            Longitude = 79.3845,
            TimeZoneOffset = 5.5,
            PlaceName = "Kumbakonam"
        };

        // Act
        var horoscope = calculator.CalculateHoroscope(birthDetails, includeDasa: false, includeNavamsa: false);

        // Assert
        // Expected: Cancer (Karka) ascendant
        Assert.Equal("Cancer", horoscope.LagnaRasiName);
        Assert.Equal(4, horoscope.LagnaRasi); // Cancer is the 4th sign
        
        // Verify the Tamil name is present (encoding may vary in test output)
        Assert.NotNull(horoscope.TamilLagnaRasiName);
        Assert.NotEmpty(horoscope.TamilLagnaRasiName);
        
        // Verify the longitude is in Cancer range (90° - 120°)
        Assert.InRange(horoscope.LagnaLongitude, 90.0, 120.0);
        
        // Expected Moon in Libra (Tula) according to the document
        var moon = horoscope.Planets.FirstOrDefault(p => p.Name == "Moon");
        Assert.NotNull(moon);
        Assert.Equal("Libra", moon.RasiName);
        Assert.Equal("Swati", moon.NakshatraName);
    }

    [Fact]
    public void TestRameshBirthChart_VerifyNakshatra()
    {
        // Arrange
        var calculator = new PanchangCalculator();
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(1983, 7, 18, 6, 35, 0),
            Latitude = 10.9601,
            Longitude = 79.3845,
            TimeZoneOffset = 5.5,
            PlaceName = "Kumbakonam"
        };

        // Act
        var horoscope = calculator.CalculateHoroscope(birthDetails);

        // Assert - Expected: Swati nakshatra (from document)
        Assert.Equal("Swati", horoscope.Panchang.NakshatraName);
        Assert.Equal("Monday", horoscope.Panchang.VaraName);
    }

    [Fact]
    public void TestRameshBirthChart_VerifyPlanetPositions()
    {
        // Arrange
        var calculator = new PanchangCalculator();
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(1983, 7, 18, 6, 35, 0),
            Latitude = 10.9601,
            Longitude = 79.3845,
            TimeZoneOffset = 5.5,
            PlaceName = "Kumbakonam"
        };

        // Act
        var horoscope = calculator.CalculateHoroscope(birthDetails);

        // Assert - Verify key planet positions from the document
        var sun = horoscope.Planets.FirstOrDefault(p => p.Name == "Sun");
        Assert.NotNull(sun);
        Assert.Equal("Cancer", sun.RasiName); // Sun at 1° 14' Cancer
        
        var moon = horoscope.Planets.FirstOrDefault(p => p.Name == "Moon");
        Assert.NotNull(moon);
        Assert.Equal("Libra", moon.RasiName); // Moon at 12° 40' Libra
        
        var mercury = horoscope.Planets.FirstOrDefault(p => p.Name == "Mercury");
        Assert.NotNull(mercury);
        Assert.Equal("Cancer", mercury.RasiName); // Mercury at 10° 45' Cancer
        
        var venus = horoscope.Planets.FirstOrDefault(p => p.Name == "Venus");
        Assert.NotNull(venus);
        Assert.Equal("Leo", venus.RasiName); // Venus at 11° 6' Leo
        
        var mars = horoscope.Planets.FirstOrDefault(p => p.Name == "Mars");
        Assert.NotNull(mars);
        Assert.Equal("Gemini", mars.RasiName); // Mars at 18° 55' Gemini
        
        var jupiter = horoscope.Planets.FirstOrDefault(p => p.Name == "Jupiter");
        Assert.NotNull(jupiter);
        Assert.Equal("Scorpio", jupiter.RasiName); // Jupiter at 7° 38' Scorpio
        
        var saturn = horoscope.Planets.FirstOrDefault(p => p.Name == "Saturn");
        Assert.NotNull(saturn);
        Assert.Equal("Libra", saturn.RasiName); // Saturn at 4° 19' Libra
    }
}
