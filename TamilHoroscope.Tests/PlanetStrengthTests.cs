using TamilHoroscope.Core.Calculators;
using TamilHoroscope.Core.Models;
using Xunit;

namespace TamilHoroscope.Tests;

public class PlanetStrengthTests
{
    [Fact]
    public void TestRameshBirthChart_PlanetaryStrength()
    {
        // Arrange
        var calculator = new PanchangCalculator();
        
        // Birth details from the test:
        // Date: July 18, 1983, Monday
        // Time: 6:35 AM IST (+5:30)
        // Place: Kumbakonam, Tamil Nadu, India
        var birthDetails = new BirthDetails
        {
            DateTime = new DateTime(1983, 7, 18, 6, 35, 0),
            Latitude = 10.9601,
            Longitude = 79.3845,
            TimeZoneOffset = 5.5,
            PlaceName = "Kumbakonam"
        };

        // Act
        var horoscope = calculator.CalculateHoroscope(
            birthDetails, 
            includeDasa: false, 
            includeNavamsa: false,
            dasaYears: 0,
            includeStrength: true);

        // Assert
        Assert.NotNull(horoscope.PlanetStrengths);
        Assert.NotEmpty(horoscope.PlanetStrengths);
        
        // Should have 7 planets (excluding Rahu and Ketu)
        Assert.Equal(7, horoscope.PlanetStrengths.Count);
        
        // Verify no Rahu or Ketu
        Assert.DoesNotContain(horoscope.PlanetStrengths, s => s.Name == "Rahu");
        Assert.DoesNotContain(horoscope.PlanetStrengths, s => s.Name == "Ketu");
        
        // Verify all 7 planets are present
        var planetNames = horoscope.PlanetStrengths.Select(s => s.Name).ToList();
        Assert.Contains("Sun", planetNames);
        Assert.Contains("Moon", planetNames);
        Assert.Contains("Mars", planetNames);
        Assert.Contains("Mercury", planetNames);
        Assert.Contains("Jupiter", planetNames);
        Assert.Contains("Venus", planetNames);
        Assert.Contains("Saturn", planetNames);
        
        // Verify all planets have positive total strength
        foreach (var strength in horoscope.PlanetStrengths)
        {
            Assert.True(strength.TotalStrength > 0, $"{strength.Name} should have positive total strength");
            
            // Verify all components are present
            Assert.True(strength.PositionalStrength >= 0, $"{strength.Name} should have non-negative positional strength");
            Assert.True(strength.DirectionalStrength >= 0, $"{strength.Name} should have non-negative directional strength");
            Assert.True(strength.MotionalStrength >= 0, $"{strength.Name} should have non-negative motional strength");
            Assert.True(strength.NaturalStrength > 0, $"{strength.Name} should have positive natural strength");
            Assert.True(strength.TemporalStrength >= 0, $"{strength.Name} should have non-negative temporal strength");
            Assert.True(strength.AspectualStrength >= 0, $"{strength.Name} should have non-negative aspectual strength");
            
            // Verify total equals sum of components
            var sum = strength.PositionalStrength + strength.DirectionalStrength + 
                     strength.MotionalStrength + strength.NaturalStrength + 
                     strength.TemporalStrength + strength.AspectualStrength;
            Assert.Equal(sum, strength.TotalStrength, 2); // Allow 2 decimal places tolerance
        }
    }

    [Fact]
    public void TestPlanetStrength_ComponentsAreReasonable()
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
        var horoscope = calculator.CalculateHoroscope(
            birthDetails, 
            includeDasa: false, 
            includeNavamsa: false,
            dasaYears: 0,
            includeStrength: true);

        // Assert - Check reasonable ranges for components
        foreach (var strength in horoscope.PlanetStrengths!)
        {
            // Positional strength: 0-290 Rupas (complex calculation with multiple sub-components)
            Assert.InRange(strength.PositionalStrength, 0, 300);
            
            // Directional strength: 0-60 Rupas
            Assert.InRange(strength.DirectionalStrength, 0, 60);
            
            // Motional strength: 0-60 Rupas (Sun/Moon should be ~30)
            Assert.InRange(strength.MotionalStrength, 0, 60);
            if (strength.Name == "Sun" || strength.Name == "Moon")
            {
                Assert.Equal(30.0, strength.MotionalStrength, 1);
            }
            
            // Natural strength: Fixed values (Sun highest at 60)
            Assert.InRange(strength.NaturalStrength, 0, 65);
            if (strength.Name == "Sun")
            {
                Assert.Equal(60.0, strength.NaturalStrength, 1);
            }
            
            // Temporal strength: 0-112 Rupas (sum of all temporal components)
            Assert.InRange(strength.TemporalStrength, 0, 120);
            
            // Aspectual strength: 0-60 Rupas
            Assert.InRange(strength.AspectualStrength, 0, 60);
            
            // Total strength: Should be reasonable (0-642 theoretical max)
            Assert.InRange(strength.TotalStrength, 0, 650);
            
            // Percentage should be 0-100
            Assert.InRange(strength.StrengthPercentage, 0, 100);
            
            // Required strength should be positive
            Assert.True(strength.RequiredStrength > 0);
            
            // Grade should not be empty
            Assert.NotEmpty(strength.StrengthGrade);
            Assert.NotEmpty(strength.TamilStrengthGrade);
        }
    }

    [Fact]
    public void TestPlanetStrength_DisplayAllComponents()
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
        var horoscope = calculator.CalculateHoroscope(
            birthDetails, 
            includeDasa: false, 
            includeNavamsa: false,
            dasaYears: 0,
            includeStrength: true);

        // Assert - Display for debugging/verification
        Console.WriteLine("Planetary Strength Components:");
        Console.WriteLine("Planet\tPositional\tDirectional\tMotional\tNatural\tTemporal\tAspectual\tTotal\tRequired\tGrade");
        Console.WriteLine(new string('-', 120));
        
        foreach (var strength in horoscope.PlanetStrengths!)
        {
            Console.WriteLine($"{strength.Name}\t" +
                            $"{strength.PositionalStrength:F1}\t\t" +
                            $"{strength.DirectionalStrength:F1}\t\t" +
                            $"{strength.MotionalStrength:F1}\t\t" +
                            $"{strength.NaturalStrength:F1}\t" +
                            $"{strength.TemporalStrength:F1}\t\t" +
                            $"{strength.AspectualStrength:F1}\t\t" +
                            $"{strength.TotalStrength:F1}\t" +
                            $"{strength.RequiredStrength:F1}\t\t" +
                            $"{strength.StrengthGrade}");
        }
    }
}
