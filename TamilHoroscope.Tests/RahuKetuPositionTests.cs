using TamilHoroscope.Core.Models;
using TamilHoroscope.Core.Calculators;
using Xunit;

namespace TamilHoroscope.Tests;

/// <summary>
/// Tests for Rahu and Ketu position calculations
/// Verifies Mean Node is used (traditional Vedic astrology) instead of True Node
/// </summary>
public class RahuKetuPositionTests
{
    private readonly PanchangCalculator _calculator;

    public RahuKetuPositionTests()
    {
        _calculator = new PanchangCalculator();
    }

    [Fact]
    public void CalculateHoroscope_Kumbakonam1983_RahuInTaurus_KetuInScorpio()
    {
        // Arrange - Example from user
        // Date: July 18, 1983, 6:35 AM
        // Location: Kumbakonam
        // Expected: Rahu in Taurus (Rasi 2), Ketu in Scorpio (Rasi 8)
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(1983, 7, 18, 6, 35, 0),
            Latitude = 10.9601,
            Longitude = 79.3845,
            TimeZoneOffset = 5.5,
            PlaceName = "Kumbakonam"
        };

        // Act
        var horoscope = _calculator.CalculateHoroscope(birthDetails);

        // Assert
        var rahu = horoscope.Planets.First(p => p.Name == "Rahu");
        var ketu = horoscope.Planets.First(p => p.Name == "Ketu");

        // Rahu should be in Taurus (Rasi 2)
        Assert.Equal(2, rahu.Rasi);
        Assert.Equal("Taurus", rahu.RasiName);
        // Tamil name check - just verify it's not empty
        Assert.False(string.IsNullOrEmpty(rahu.TamilRasiName));

        // Ketu should be in Scorpio (Rasi 8) - 180° opposite to Rahu
        Assert.Equal(8, ketu.Rasi);
        Assert.Equal("Scorpio", ketu.RasiName);
        Assert.False(string.IsNullOrEmpty(ketu.TamilRasiName));

        // Additional validation: Longitude check
        // Taurus is 30° to 60°, so Rahu longitude should be in this range
        Assert.InRange(rahu.Longitude, 30.0, 60.0);

        // Scorpio is 210° to 240°, so Ketu longitude should be in this range
        Assert.InRange(ketu.Longitude, 210.0, 240.0);

        // Verify Ketu is exactly 180° opposite to Rahu
        var expectedKetuLongitude = (rahu.Longitude + 180.0) % 360.0;
        Assert.Equal(expectedKetuLongitude, ketu.Longitude, 0.01); // Within 0.01 degree
    }

    [Fact]
    public void CalculateHoroscope_MeanNodeVsTrueNode_DifferentResults()
    {
        // This test verifies we're using Mean Node
        // Mean Node and True Node can differ by up to ~1.5 degrees
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(1983, 7, 18, 6, 35, 0),
            Latitude = 10.9601,
            Longitude = 79.3845,
            TimeZoneOffset = 5.5,
            PlaceName = "Kumbakonam"
        };

        var horoscope = _calculator.CalculateHoroscope(birthDetails);
        var rahu = horoscope.Planets.First(p => p.Name == "Rahu");

        // Mean Node for this date should place Rahu in Taurus
        // (True Node would place it in Gemini, which was the previous bug)
        Assert.Equal(2, rahu.Rasi); // Should be Taurus (2), not Gemini (3)
    }

    [Fact]
    public void CalculateHoroscope_RahuKetu_Always180DegreesApart()
    {
        // Test multiple dates to ensure Rahu and Ketu are always opposite
        var testDates = new[]
        {
            new DateTime(1983, 7, 18, 6, 35, 0),
            new DateTime(2000, 1, 1, 12, 0, 0),
            new DateTime(2024, 3, 15, 14, 30, 0),
            new DateTime(1990, 6, 21, 8, 0, 0)
        };

        var birthDetails = new BirthDetails
        {
            Latitude = 13.0827,
            Longitude = 80.2707,
            TimeZoneOffset = 5.5,
            PlaceName = "Chennai"
        };

        foreach (var date in testDates)
        {
            birthDetails.DateTime = date;
            var horoscope = _calculator.CalculateHoroscope(birthDetails);

            var rahu = horoscope.Planets.First(p => p.Name == "Rahu");
            var ketu = horoscope.Planets.First(p => p.Name == "Ketu");

            // Calculate expected Ketu longitude
            var expectedKetuLong = (rahu.Longitude + 180.0) % 360.0;

            // Verify Ketu is 180° opposite
            Assert.Equal(expectedKetuLong, ketu.Longitude, 0.01);

            // Verify Rasis are 6 signs apart
            var rasiDifference = Math.Abs(rahu.Rasi - ketu.Rasi);
            Assert.True(rasiDifference == 6 || rasiDifference == -6, 
                $"Rahu and Ketu should be 6 signs apart. Rahu: {rahu.Rasi}, Ketu: {ketu.Rasi}");
        }
    }

    [Fact]
    public void CalculateHoroscope_RahuIsRetrograde()
    {
        // Rahu and Ketu always move in retrograde motion
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(1983, 7, 18, 6, 35, 0),
            Latitude = 10.9601,
            Longitude = 79.3845,
            TimeZoneOffset = 5.5,
            PlaceName = "Kumbakonam"
        };

        var horoscope = _calculator.CalculateHoroscope(birthDetails);

        var rahu = horoscope.Planets.First(p => p.Name == "Rahu");
        var ketu = horoscope.Planets.First(p => p.Name == "Ketu");

        // Rahu and Ketu are always retrograde (negative speed)
        Assert.True(rahu.IsRetrograde, "Rahu should always be retrograde");
        Assert.True(ketu.IsRetrograde, "Ketu should always be retrograde");
        Assert.True(rahu.Speed < 0, "Rahu speed should be negative");
        Assert.True(ketu.Speed < 0, "Ketu speed should be negative");
    }
}
