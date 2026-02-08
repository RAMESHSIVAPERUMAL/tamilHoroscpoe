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
        
        // Verify all planets have positive total strength (in Rupas)
        foreach (var strength in horoscope.PlanetStrengths)
        {
            Assert.True(strength.TotalStrength > 0, $"{strength.Name} should have positive total strength");
            
            // Positional and Motional are always >= 0
            Assert.True(strength.PositionalStrength >= 0, $"{strength.Name} should have non-negative positional strength");
            Assert.True(strength.DirectionalStrength >= 0, $"{strength.Name} should have non-negative directional strength (Dig Bala)");
            Assert.True(strength.MotionalStrength >= 0, $"{strength.Name} should have non-negative motional strength");
            Assert.True(strength.NaturalStrength > 0, $"{strength.Name} should have positive natural strength");
            Assert.True(strength.TemporalStrength >= 0, $"{strength.Name} should have non-negative temporal strength");
            // Drik Bala (Aspectual) CAN be negative per Parasara
            
            // Verify total equals sum of components
            var sum = strength.PositionalStrength + strength.DirectionalStrength + 
                     strength.MotionalStrength + strength.NaturalStrength + 
                     strength.TemporalStrength + strength.AspectualStrength;
            Assert.Equal(sum, strength.TotalStrength, 2);
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

        // Assert - Check reasonable ranges for components (all in Rupas now)
        foreach (var strength in horoscope.PlanetStrengths!)
        {
            // Sthana Bala (Positional): max ~8 Rupas (Uchcha 1R + Sapta ~5.6R + Ojha 0.5R + Kendra 1R + Drekk 0.25R)
            Assert.InRange(strength.PositionalStrength, 0, 10);
            
            // Dig Bala (Directional): 0-1 Rupa (60V / 60)
            Assert.InRange(strength.DirectionalStrength, 0, 1.01);
            
            // Chesta Bala (Motional): 0-1 Rupa.
            // Sun's Chesta = Ayana Bala, Moon's Chesta = Paksha Bala (both 0-1R)
            Assert.InRange(strength.MotionalStrength, 0, 1.01);
            
            // Naisargika Bala (Natural): Sun = 60V/60 = 1.0 Rupa
            Assert.InRange(strength.NaturalStrength, 0, 1.01);
            if (strength.Name == "Sun")
            {
                Assert.Equal(1.0, strength.NaturalStrength, 1);
            }
            
            // Kala Bala (Temporal): max ~6 Rupas
            Assert.InRange(strength.TemporalStrength, 0, 7);
            
            // Drik Bala (Aspectual): can be negative (range roughly -1 to +1 Rupa)
            Assert.InRange(strength.AspectualStrength, -2, 2);
            
            // Total strength in Rupas: typical range 3-12 Rupas
            Assert.InRange(strength.TotalStrength, 0, 20);
            
            // Percentage should be 0-100
            Assert.InRange(strength.StrengthPercentage, 0, 100);
            
            // Required strength should be positive (5-7 Rupas per BPHS)
            Assert.True(strength.RequiredStrength > 0);
            Assert.InRange(strength.RequiredStrength, 4.0, 8.0);
            
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
        Console.WriteLine("Shadbala Components (all values in Rupas, 1 Rupa = 60 Virupas):");
        Console.WriteLine("Planet\tSthana\tDig\tChesta\tNaisarg\tKala\tDrik\tTotal\tReqd\tRatio\tGrade");
        Console.WriteLine(new string('-', 110));
        
        foreach (var strength in horoscope.PlanetStrengths!)
        {
            double ratio = strength.RequiredStrength > 0 ? strength.TotalStrength / strength.RequiredStrength : 0;
            Console.WriteLine($"{strength.Name}\t" +
                            $"{strength.PositionalStrength:F2}\t" +
                            $"{strength.DirectionalStrength:F2}\t" +
                            $"{strength.MotionalStrength:F2}\t" +
                            $"{strength.NaturalStrength:F2}\t" +
                            $"{strength.TemporalStrength:F2}\t" +
                            $"{strength.AspectualStrength:F2}\t" +
                            $"{strength.TotalStrength:F2}\t" +
                            $"{strength.RequiredStrength:F1}\t" +
                            $"{ratio:F2}\t" +
                            $"{strength.StrengthGrade}");
        }

        Console.WriteLine();
        Console.WriteLine("Virupas breakdown:");
        foreach (var strength in horoscope.PlanetStrengths!)
        {
            Console.WriteLine($"{strength.Name}: " +
                            $"Sthana={strength.PositionalStrength * 60:F0}V, " +
                            $"Dig={strength.DirectionalStrength * 60:F0}V, " +
                            $"Chesta={strength.MotionalStrength * 60:F0}V, " +
                            $"Naisarg={strength.NaturalStrength * 60:F0}V, " +
                            $"Kala={strength.TemporalStrength * 60:F0}V, " +
                            $"Drik={strength.AspectualStrength * 60:F0}V, " +
                            $"Total={strength.TotalStrength * 60:F0}V ({strength.TotalStrength:F2}R)");
        }
    }
}
