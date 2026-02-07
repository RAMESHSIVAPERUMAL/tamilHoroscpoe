using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Core.Calculators;

/// <summary>
/// Calculator for planetary strength (simplified Shadbala)
/// Based on key factors: Positional, Directional, Motional, Natural, Temporal, and Aspectual strength
/// </summary>
public class PlanetStrengthCalculator
{
    // Natural strength values (Naisargika Bala) in Rupas - based on luminosity
    private static readonly Dictionary<string, double> NaturalStrengths = new()
    {
        { "Sun", 60.0 },
        { "Moon", 51.43 },
        { "Venus", 42.86 },
        { "Jupiter", 34.29 },
        { "Mercury", 25.71 },
        { "Mars", 17.14 },
        { "Saturn", 8.57 },
        { "Rahu", 8.57 },
        { "Ketu", 8.57 }
    };

    // Required minimum strength for benefic results (in Rupas)
    // Updated for complete Shadbala calculation
    private static readonly Dictionary<string, double> RequiredStrengths = new()
    {
        { "Sun", 390.0 },
        { "Moon", 360.0 },
        { "Mars", 300.0 },
        { "Mercury", 420.0 },
        { "Jupiter", 390.0 },
        { "Venus", 330.0 },
        { "Saturn", 300.0 },
        { "Rahu", 300.0 },
        { "Ketu", 300.0 }
    };

    /// <summary>
    /// Calculate strength for all planets in the horoscope
    /// Rahu and Ketu are excluded as they don't have Shadbala in traditional astrology
    /// </summary>
    public List<PlanetStrengthData> CalculatePlanetaryStrengths(HoroscopeData horoscope)
    {
        var strengths = new List<PlanetStrengthData>();

        // Exclude Rahu and Ketu - they don't have Shadbala in traditional astrology
        foreach (var planet in horoscope.Planets.Where(p => p.Name != "Rahu" && p.Name != "Ketu"))
        {
            var strength = new PlanetStrengthData
            {
                Name = planet.Name,
                TamilName = planet.TamilName,
                RequiredStrength = RequiredStrengths.GetValueOrDefault(planet.Name, 5.0)
            };

            // Calculate individual strength components
            strength.PositionalStrength = CalculatePositionalStrength(planet, horoscope);
            strength.DirectionalStrength = CalculateDirectionalStrength(planet);
            strength.MotionalStrength = CalculateMotionalStrength(planet);
            strength.NaturalStrength = NaturalStrengths.GetValueOrDefault(planet.Name, 30.0);
            strength.TemporalStrength = CalculateTemporalStrength(planet, horoscope);
            strength.AspectualStrength = CalculateAspectualStrength(planet, horoscope);

            // Total strength is the sum of all components
            strength.TotalStrength = 
                strength.PositionalStrength +
                strength.DirectionalStrength +
                strength.MotionalStrength +
                strength.NaturalStrength +
                strength.TemporalStrength +
                strength.AspectualStrength;

            // Calculate percentage (maximum possible with accurate Parasara method)
            // Positional: 290, Directional: 60, Motional: 60, Natural: 60, Temporal: ~112, Aspectual: 60 = ~642 max
            strength.StrengthPercentage = (strength.TotalStrength / 642.0) * 100.0;

            strengths.Add(strength);
        }

        return strengths;
    }

    /// <summary>
    /// Calculate positional strength (Sthana Bala) with all 5 traditional sub-components
    /// 1. Uchcha Bala (Exaltation Strength)
    /// 2. Saptavargaja Bala (Seven Divisional Strength)
    /// 3. Ojhayugma Bala (Odd/Even Sign Strength)
    /// 4. Kendra Bala (Angular House Strength)
    /// 5. Drekkana Bala (Decanate Strength)
    /// </summary>
    private double CalculatePositionalStrength(PlanetData planet, HoroscopeData horoscope)
    {
        double totalSthanaBala = 0.0;
        
        // 1. Uchcha Bala (Exaltation/Debilitation Strength) - Maximum 60 Rupas
        totalSthanaBala += CalculateUchchaBala(planet);
        
        // 2. Saptavargaja Bala (Seven Divisional Strength) - Maximum 20 × 7 = 140 Rupas
        totalSthanaBala += CalculateSaptavargajaBala(planet);
        
        // 3. Ojhayugma Bala (Odd/Even Sign Strength) - Maximum 15 Rupas
        totalSthanaBala += CalculateOjhayugmaBala(planet);
        
        // 4. Kendra Bala (Angular House Strength) - Maximum 60 Rupas
        totalSthanaBala += CalculateKendraBala(planet);
        
        // 5. Drekkana Bala (Decanate Strength) - Maximum 15 Rupas
        totalSthanaBala += CalculateDrekkanaBala(planet);
        
        return totalSthanaBala;
    }
    
    /// <summary>
    /// Calculate Uchcha Bala (Exaltation Strength)
    /// Based on exact degrees from exaltation/debilitation points
    /// Maximum: 60 Rupas
    /// </summary>
    private double CalculateUchchaBala(PlanetData planet)
    {
        // Get exact exaltation degrees for each planet
        var (exaltationSign, exaltationDegree) = GetExaltationPoint(planet.Name);
        var (debilitationSign, debilitationDegree) = GetDebilitationPoint(planet.Name);
        
        // Calculate absolute longitude of debilitation point
        double debilitationLongitude = (debilitationSign - 1) * 30.0 + debilitationDegree;
        
        // Normalize planet longitude (0-360)
        double planetLongitude = planet.Longitude;
        while (planetLongitude < 0) planetLongitude += 360.0;
        while (planetLongitude >= 360.0) planetLongitude -= 360.0;
        
        // Calculate distance from debilitation point
        double distanceFromDebilitation = planetLongitude - debilitationLongitude;
        
        // Normalize to 0-360 range
        while (distanceFromDebilitation < 0) distanceFromDebilitation += 360.0;
        while (distanceFromDebilitation >= 360.0) distanceFromDebilitation -= 360.0;
        
        // If distance > 180, take the shorter arc
        if (distanceFromDebilitation > 180.0)
        {
            distanceFromDebilitation = 360.0 - distanceFromDebilitation;
        }
        
        // Calculate Uchcha Bala: (distance from debilitation / 180) * 60 Rupas
        double uchchaBala = (distanceFromDebilitation / 180.0) * 60.0;
        
        return uchchaBala;
    }
    
    /// <summary>
    /// Calculate Saptavargaja Bala (Seven Divisional Strength) - Parasara method
    /// Based on planet's dignity in 7 divisional charts (D-1, D-2, D-3, D-7, D-9, D-12, D-30)
    /// Maximum: 20 Rupas per division × 7 = 140 Rupas
    /// Uses accurate Parashara divisional calculation formulas
    /// </summary>
    private double CalculateSaptavargajaBala(PlanetData planet)
    {
        double totalBala = 0.0;
        
        // D-1 (Rasi) - Main birth chart
        totalBala += GetDivisionalStrength(planet.Name, planet.Rasi, 1);
        
        // D-2 (Hora) - Parasara Hora calculation
        int horaSign = CalculateHoraSign(planet.Longitude);
        totalBala += GetDivisionalStrength(planet.Name, horaSign, 2);
        
        // D-3 (Drekkana) - Parasara Drekkana calculation
        int drekkanaSign = CalculateDrekkanaSign(planet.Longitude);
        totalBala += GetDivisionalStrength(planet.Name, drekkanaSign, 3);
        
        // D-7 (Saptamsa) - Parasara Saptamsa calculation
        int saptamsaSign = CalculateSaptamsaSign(planet.Longitude);
        totalBala += GetDivisionalStrength(planet.Name, saptamsaSign, 7);
        
        // D-9 (Navamsa) - Parasara Navamsa calculation
        int navamsaSign = CalculateNavamsaSign(planet.Longitude);
        totalBala += GetDivisionalStrength(planet.Name, navamsaSign, 9);
        
        // D-12 (Dwadasamsa) - Parasara Dwadasamsa calculation
        int dwadasamsaSign = CalculateDwadasamsaSign(planet.Longitude);
        totalBala += GetDivisionalStrength(planet.Name, dwadasamsaSign, 12);
        
        // D-30 (Trimshamsa) - Parasara Trimshamsa calculation
        int trimshamsaSign = CalculateTrimshamsaSign(planet.Longitude);
        totalBala += GetDivisionalStrength(planet.Name, trimshamsaSign, 30);
        
        return totalBala;
    }
    
    /// <summary>
    /// Calculate Hora (D-2) sign - Parasara method
    /// Odd signs: First 15° = Sun (Leo), Last 15° = Moon (Cancer)
    /// Even signs: First 15° = Moon (Cancer), Last 15° = Sun (Leo)
    /// </summary>
    private int CalculateHoraSign(double longitude)
    {
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        int sign = (int)(longitude / 30.0) + 1;
        double positionInSign = longitude % 30.0;
        
        bool isOddSign = sign % 2 == 1;
        bool isFirstHalf = positionInSign < 15.0;
        
        if (isOddSign)
        {
            return isFirstHalf ? 5 : 4; // Leo : Cancer
        }
        else
        {
            return isFirstHalf ? 4 : 5; // Cancer : Leo
        }
    }
    
    /// <summary>
    /// Calculate Drekkana (D-3) sign - Parasara method
    /// Each sign divided into 3 parts of 10° each
    /// Starts from own sign, then 5th, then 9th from it
    /// </summary>
    private int CalculateDrekkanaSign(double longitude)
    {
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        int sign = (int)(longitude / 30.0) + 1;
        double positionInSign = longitude % 30.0;
        
        int drekkanaNumber = (int)(positionInSign / 10.0); // 0, 1, or 2
        
        // Drekkanas start from own sign, 5th from it, 9th from it
        int drekkanaSign = ((sign - 1) + (drekkanaNumber * 4)) % 12 + 1;
        
        return drekkanaSign;
    }
    
    /// <summary>
    /// Calculate Saptamsa (D-7) sign - Parasara method
    /// Each sign divided into 7 parts
    /// Odd signs: Start from own sign
    /// Even signs: Start from 7th sign
    /// </summary>
    private int CalculateSaptamsaSign(double longitude)
    {
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        int sign = (int)(longitude / 30.0) + 1;
        double positionInSign = longitude % 30.0;
        
        int saptamsaNumber = (int)(positionInSign / (30.0 / 7.0)); // 0-6
        
        bool isOddSign = sign % 2 == 1;
        int startSign = isOddSign ? sign : ((sign + 6) % 12);
        if (startSign == 0) startSign = 12;
        
        int saptamsaSign = ((startSign - 1) + saptamsaNumber) % 12 + 1;
        
        return saptamsaSign;
    }
    
    /// <summary>
    /// Calculate Navamsa (D-9) sign - Parasara method
    /// Each sign divided into 9 parts of 3°20' each
    /// Fire signs (Aries, Leo, Sag): Start from Aries
    /// Earth signs (Taurus, Virgo, Cap): Start from Capricorn
    /// Air signs (Gemini, Libra, Aqua): Start from Libra
    /// Water signs (Cancer, Scorpio, Pisces): Start from Cancer
    /// </summary>
    private int CalculateNavamsaSign(double longitude)
    {
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        int sign = (int)(longitude / 30.0) + 1;
        double positionInSign = longitude % 30.0;
        
        int navamsaNumber = (int)(positionInSign / (30.0 / 9.0)); // 0-8
        
        // Determine element and starting sign
        int element = (sign - 1) % 3; // 0=Fire/Water, 1=Earth, 2=Air (adjusted)
        int startSign;
        
        if (sign == 1 || sign == 5 || sign == 9) // Fire signs
            startSign = 1; // Aries
        else if (sign == 2 || sign == 6 || sign == 10) // Earth signs
            startSign = 10; // Capricorn
        else if (sign == 3 || sign == 7 || sign == 11) // Air signs
            startSign = 7; // Libra
        else // Water signs (4, 8, 12)
            startSign = 4; // Cancer
        
        int navamsaSign = ((startSign - 1) + navamsaNumber) % 12 + 1;
        
        return navamsaSign;
    }
    
    /// <summary>
    /// Calculate Dwadasamsa (D-12) sign - Parasara method
    /// Each sign divided into 12 parts of 2°30' each
    /// Starts from own sign
    /// </summary>
    private int CalculateDwadasamsaSign(double longitude)
    {
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        int sign = (int)(longitude / 30.0) + 1;
        double positionInSign = longitude % 30.0;
        
        int dwadasamsaNumber = (int)(positionInSign / 2.5); // 0-11
        
        int dwadasamsaSign = ((sign - 1) + dwadasamsaNumber) % 12 + 1;
        
        return dwadasamsaSign;
    }
    
    /// <summary>
    /// Calculate Trimshamsa (D-30) sign - Parasara method
    /// Special unequal division for malefics
    /// Odd signs: Mars(5°), Saturn(5°), Jupiter(8°), Mercury(7°), Venus(5°)
    /// Even signs: Venus(5°), Mercury(7°), Jupiter(8°), Saturn(5°), Mars(5°)
    /// </summary>
    private int CalculateTrimshamsaSign(double longitude)
    {
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        int sign = (int)(longitude / 30.0) + 1;
        double positionInSign = longitude % 30.0;
        
        bool isOddSign = sign % 2 == 1;
        
        // Trimshamsa divisions (unequal)
        if (isOddSign)
        {
            // Odd signs: Mars, Saturn, Jupiter, Mercury, Venus
            if (positionInSign < 5.0)
                return 1; // Mars - Aries
            else if (positionInSign < 10.0)
                return 10; // Saturn - Capricorn
            else if (positionInSign < 18.0)
                return 9; // Jupiter - Sagittarius
            else if (positionInSign < 25.0)
                return 6; // Mercury - Virgo
            else
                return 7; // Venus - Libra
        }
        else
        {
            // Even signs: Venus, Mercury, Jupiter, Saturn, Mars
            if (positionInSign < 5.0)
                return 7; // Venus - Libra
            else if (positionInSign < 12.0)
                return 6; // Mercury - Virgo
            else if (positionInSign < 20.0)
                return 9; // Jupiter - Sagittarius
            else if (positionInSign < 25.0)
                return 10; // Saturn - Capricorn
            else
                return 1; // Mars - Aries
        }
    }
    
    /// <summary>
    /// Get strength value for a planet in a divisional sign
    /// </summary>
    private double GetDivisionalStrength(string planetName, int sign, int division)
    {
        // Check dignity in the divisional sign
        var (exaltationSign, _) = GetExaltationPoint(planetName);
        var (debilitationSign, _) = GetDebilitationPoint(planetName);
        
        // Get own signs
        var ownSigns = GetOwnSigns(planetName);
        
        // Get friend/enemy relationships (simplified)
        var signLord = GetSignLord(sign);
        var relationship = GetPlanetRelationship(planetName, signLord);
        
        // Assign strength based on dignity
        if (sign == exaltationSign)
            return 20.0; // Exalted
        else if (ownSigns.Contains(sign))
            return 15.0; // Own sign
        else if (relationship == "Friend")
            return 10.0; // Friend's sign
        else if (relationship == "Neutral")
            return 7.5; // Neutral sign
        else if (relationship == "Enemy")
            return 3.75; // Enemy's sign
        else if (sign == debilitationSign)
            return 0.0; // Debilitated
        else
            return 7.5; // Default neutral
    }
    
    /// <summary>
    /// Calculate Ojhayugma Bala (Odd/Even Sign Strength)
    /// Masculine planets strong in odd signs, Feminine planets strong in even signs
    /// Maximum: 15 Rupas
    /// </summary>
    private double CalculateOjhayugmaBala(PlanetData planet)
    {
        // Masculine planets: Sun, Mars, Jupiter
        // Feminine planets: Moon, Venus, Saturn
        // Mercury: Neutral (strong in both)
        
        bool isMasculine = planet.Name is "Sun" or "Mars" or "Jupiter";
        bool isFeminine = planet.Name is "Moon" or "Venus" or "Saturn";
        bool isNeutral = planet.Name == "Mercury";
        
        // Odd signs: 1, 3, 5, 7, 9, 11 (Aries, Gemini, Leo, Libra, Sagittarius, Aquarius)
        bool isOddSign = planet.Rasi % 2 == 1;
        
        if (isNeutral)
            return 15.0; // Mercury gets full strength in any sign
        else if ((isMasculine && isOddSign) || (isFeminine && !isOddSign))
            return 15.0; // Full strength
        else
            return 0.0; // No strength
    }
    
    /// <summary>
    /// Calculate Kendra Bala (Angular House Strength)
    /// Strength based on placement in Kendra (1, 4, 7, 10) or Trikona (1, 5, 9) houses
    /// Maximum: 60 Rupas
    /// </summary>
    private double CalculateKendraBala(PlanetData planet)
    {
        int house = planet.House;
        
        // Kendra houses (Angular): 1, 4, 7, 10 - Full strength (60 Rupas)
        if (house == 1 || house == 4 || house == 7 || house == 10)
            return 60.0;
        
        // Panaphara houses (Succedent): 2, 5, 8, 11 - Half strength (30 Rupas)
        else if (house == 2 || house == 5 || house == 8 || house == 11)
            return 30.0;
        
        // Apoklima houses (Cadent): 3, 6, 9, 12 - Quarter strength (15 Rupas)
        else
            return 15.0;
    }
    
    /// <summary>
    /// Calculate Drekkana Bala (Decanate Strength)
    /// Based on the decanate (10° division) of the sign
    /// Maximum: 15 Rupas
    /// </summary>
    private double CalculateDrekkanaBala(PlanetData planet)
    {
        // Get position within the sign (0-30)
        double positionInSign = planet.Longitude % 30.0;
        
        // Determine which decanate (0-10, 10-20, 20-30)
        int decanate = (int)(positionInSign / 10.0) + 1; // 1, 2, or 3
        
        // Masculine planets strong in 1st decanate
        // Feminine planets strong in 3rd decanate
        // Mercury (neutral) gets equal strength
        
        bool isMasculine = planet.Name is "Sun" or "Mars" or "Jupiter";
        bool isFeminine = planet.Name is "Moon" or "Venus" or "Saturn";
        bool isNeutral = planet.Name == "Mercury";
        
        if (isNeutral)
            return 15.0;
        else if (isMasculine && decanate == 1)
            return 15.0;
        else if (isFeminine && decanate == 3)
            return 15.0;
        else if (decanate == 2) // Middle decanate
            return 7.5;
        else
            return 5.0; // Weak position
    }
    
    /// <summary>
    /// Get own signs for a planet
    /// </summary>
    private int[] GetOwnSigns(string planetName)
    {
        return planetName switch
        {
            "Sun" => new[] { 5 },          // Leo
            "Moon" => new[] { 4 },         // Cancer
            "Mars" => new[] { 1, 8 },      // Aries, Scorpio
            "Mercury" => new[] { 3, 6 },   // Gemini, Virgo
            "Jupiter" => new[] { 9, 12 },  // Sagittarius, Pisces
            "Venus" => new[] { 2, 7 },     // Taurus, Libra
            "Saturn" => new[] { 10, 11 },  // Capricorn, Aquarius
            _ => Array.Empty<int>()
        };
    }
    
    /// <summary>
    /// Get lord of a sign
    /// </summary>
    private string GetSignLord(int sign)
    {
        return sign switch
        {
            1 => "Mars",      // Aries
            2 => "Venus",     // Taurus
            3 => "Mercury",   // Gemini
            4 => "Moon",      // Cancer
            5 => "Sun",       // Leo
            6 => "Mercury",   // Virgo
            7 => "Venus",     // Libra
            8 => "Mars",      // Scorpio
            9 => "Jupiter",   // Sagittarius
            10 => "Saturn",   // Capricorn
            11 => "Saturn",   // Aquarius
            12 => "Jupiter",  // Pisces
            _ => "Sun"
        };
    }
    
    /// <summary>
    /// Get relationship between two planets (simplified)
    /// </summary>
    private string GetPlanetRelationship(string planet1, string planet2)
    {
        // Simplified friendship table
        var friends = new Dictionary<string, string[]>
        {
            { "Sun", new[] { "Moon", "Mars", "Jupiter" } },
            { "Moon", new[] { "Sun", "Mercury" } },
            { "Mars", new[] { "Sun", "Moon", "Jupiter" } },
            { "Mercury", new[] { "Sun", "Venus" } },
            { "Jupiter", new[] { "Sun", "Moon", "Mars" } },
            { "Venus", new[] { "Mercury", "Saturn" } },
            { "Saturn", new[] { "Mercury", "Venus" } }
        };
        
        var enemies = new Dictionary<string, string[]>
        {
            { "Sun", new[] { "Venus", "Saturn" } },
            { "Moon", new[] { "None" } },
            { "Mars", new[] { "Mercury" } },
            { "Mercury", new[] { "Moon" } },
            { "Jupiter", new[] { "Mercury", "Venus" } },
            { "Venus", new[] { "Sun", "Moon" } },
            { "Saturn", new[] { "Sun", "Moon", "Mars" } }
        };
        
        if (planet1 == planet2)
            return "Own";
        else if (friends.ContainsKey(planet1) && friends[planet1].Contains(planet2))
            return "Friend";
        else if (enemies.ContainsKey(planet1) && enemies[planet1].Contains(planet2))
            return "Enemy";
        else
            return "Neutral";
    }
    
    /// <summary>
    /// Get exact exaltation point (sign and degree) for a planet
    /// </summary>
    private (int sign, double degree) GetExaltationPoint(string planetName)
    {
        return planetName switch
        {
            "Sun" => (1, 10.0),      // 10° Aries
            "Moon" => (2, 3.0),      // 3° Taurus
            "Mars" => (10, 28.0),    // 28° Capricorn
            "Mercury" => (6, 15.0),  // 15° Virgo
            "Jupiter" => (4, 5.0),   // 5° Cancer
            "Venus" => (12, 27.0),   // 27° Pisces
            "Saturn" => (7, 20.0),   // 20° Libra
            _ => (1, 0.0)
        };
    }
    
    /// <summary>
    /// Get exact debilitation point (sign and degree) for a planet
    /// </summary>
    private (int sign, double degree) GetDebilitationPoint(string planetName)
    {
        return planetName switch
        {
            "Sun" => (7, 10.0),      // 10° Libra (opposite of exaltation)
            "Moon" => (8, 3.0),      // 3° Scorpio
            "Mars" => (4, 28.0),     // 28° Cancer
            "Mercury" => (12, 15.0), // 15° Pisces
            "Jupiter" => (10, 5.0),  // 5° Capricorn
            "Venus" => (6, 27.0),    // 27° Virgo
            "Saturn" => (1, 20.0),   // 20° Aries
            _ => (7, 0.0)
        };
    }

    /// <summary>
    /// Calculate directional strength (Dig Bala) - Parasara method
    /// Based on house placement and proportional distance from ideal direction
    /// Maximum: 60 Rupas
    /// Formula: 60 × (1 - angular_distance / 180°)
    /// </summary>
    private double CalculateDirectionalStrength(PlanetData planet)
    {
        // Each planet is strong in specific houses (directions):
        // Jupiter & Mercury: 1st house (East - Ascendant)
        // Sun & Mars: 10th house (South - Midheaven)
        // Saturn: 7th house (West - Descendant)
        // Moon & Venus: 4th house (North - IC/Nadir)
        
        int house = planet.House;
        
        // Get ideal house for this planet
        int idealHouse = planet.Name switch
        {
            "Jupiter" or "Mercury" => 1,
            "Sun" or "Mars" => 10,
            "Saturn" => 7,
            "Moon" or "Venus" => 4,
            _ => 1
        };
        
        // Calculate angular distance from ideal house
        int distance = Math.Abs(house - idealHouse);
        
        // Take the shorter arc (through 12 houses)
        if (distance > 6)
            distance = 12 - distance;
        
        // Parasara formula: Proportional strength based on angular distance
        // Maximum 60 Rupas at ideal house, decreasing linearly to 0 at opposite house
        double digBala = 60.0 * (1.0 - (distance / 6.0));
        
        return Math.Max(0, digBala);
    }

    /// <summary>
    /// Calculate motional strength (Chesta Bala) - Parasara method
    /// Based on planet's speed, direction, and retrograde status
    /// Maximum: 60 Rupas
    /// Formula varies by planet type and motion state
    /// </summary>
    private double CalculateMotionalStrength(PlanetData planet)
    {
        // Sun and Moon don't have Chesta Bala (always direct, constant relative speed)
        if (planet.Name == "Sun" || planet.Name == "Moon")
        {
            return 30.0; // Fixed value per Parasara
        }

        double chestaBala = 0.0;

        // Retrograde motion gives maximum strength per Parasara
        if (planet.IsRetrograde)
        {
            chestaBala = 60.0; // Maximum strength when retrograde
        }
        else
        {
            // Direct motion: Strength based on speed relative to mean motion
            // Faster than average = stronger (planet is eager/active)
            // Slower than average = weaker (planet is lazy/passive)
            
            double normalSpeed = GetNormalSpeed(planet.Name);
            double currentSpeed = Math.Abs(planet.Speed);
            
            if (normalSpeed > 0)
            {
                // Parasara formula: Proportional to speed ratio
                // Max 60 when moving at 2x normal speed
                double speedRatio = currentSpeed / normalSpeed;
                chestaBala = Math.Min(60.0, speedRatio * 30.0);
            }
            else
            {
                chestaBala = 30.0; // Default middle value
            }
        }

        return chestaBala;
    }
    
    /// <summary>
    /// Get normal average speed for a planet (degrees per day) - Parasara values
    /// </summary>
    private double GetNormalSpeed(string planetName)
    {
        return planetName switch
        {
            "Moon" => 13.176, // Fast moving
            "Mercury" => 1.383, // Variable due to proximity to Sun
            "Venus" => 1.602, // Similar to Mercury
            "Sun" => 0.986, // Mean solar motion
            "Mars" => 0.524, // Medium speed
            "Jupiter" => 0.083, // Slow
            "Saturn" => 0.033, // Very slow
            _ => 0.5
        };
    }

    /// <summary>
    /// Calculate temporal strength (Kala Bala) using accurate Parasara methodology
    /// Total maximum: ~112 Rupas (not 450 as previously calculated)
    /// Components: Nathonnatha (30), Paksha (30), Tribhaga (20), Varsha/Masa/Vara/Hora (15+10+8+8=41), Ayana (20), Yuddha (variable)
    /// </summary>
    private double CalculateTemporalStrength(PlanetData planet, HoroscopeData horoscope)
    {
        double kalaBala = 0.0;
        
        var birthDateTime = horoscope.BirthDetails?.DateTime ?? DateTime.Now;
        var moonLongitude = horoscope.Panchang.MoonLongitude;
        var sunLongitude = horoscope.Panchang.SunLongitude;
        
        // 1. Nathonnatha Bala (Day/Night strength based on distance from Noon/Midnight) - Max 30 Rupas
        kalaBala += CalculateNathonnathaBalaAccurate(planet.Name, birthDateTime, sunLongitude);
        
        // 2. Paksha Bala (Lunar phase/Tithi strength) - Max 30 Rupas
        kalaBala += CalculatePakshaBalaAccurate(planet.Name, moonLongitude, sunLongitude);
        
        // 3. Tribhaga Bala (Day/Night third division) - Max 20 Rupas
        kalaBala += CalculateTribhagaBalaAccurate(planet.Name, birthDateTime, sunLongitude);
        
        // 4. Varsha Bala (Year Lord) - Max 15 Rupas
        kalaBala += CalculateVarshaBala(planet.Name, birthDateTime);
        
        // 5. Masa Bala (Month Lord) - Max 10 Rupas
        kalaBala += CalculateMasaBalaAccurate(planet.Name, sunLongitude);
        
        // 6. Vara Bala (Weekday Lord) - Max 8 Rupas
        kalaBala += CalculateVaraBalaAccurate(planet.Name, birthDateTime);
        
        // 7. Hora Bala (Hour Lord) - Max 8 Rupas
        kalaBala += CalculateHoraBalaAccurate(planet.Name, birthDateTime, sunLongitude);
        
        // 8. Ayana Bala (Declination strength) - Max 20 Rupas
        kalaBala += CalculateAyanaBalaAccurate(planet.Name, sunLongitude, planet.Latitude);
        
        // 9. Yuddha Bala (Planetary war - when planets within 1°) - Variable
        kalaBala += CalculateYuddhaBala(planet, horoscope);
        
        return kalaBala;
    }
    
    /// <summary>
    /// Calculate Nathonnatha Bala (Day/Night strength) - Accurate Parasara method
    /// Based on distance from Noon (for benefics) or Midnight (for malefics)
    /// Maximum: 30 Rupas
    /// </summary>
    private double CalculateNathonnathaBalaAccurate(string planetName, DateTime dateTime, double sunLongitude)
    {
        // Determine if planet is benefic or malefic
        bool isBenefic = planetName is "Moon" or "Mercury" or "Jupiter" or "Venus";
        
        // Calculate local hour (0-24)
        double localHour = dateTime.Hour + dateTime.Minute / 60.0 + dateTime.Second / 3600.0;
        
        // Calculate distance from noon (12:00) or midnight (0:00/24:00)
        double distanceFromReference;
        
        if (isBenefic)
        {
            // Benefics strong at noon
            distanceFromReference = Math.Abs(localHour - 12.0);
        }
        else
        {
            // Malefics strong at midnight
            distanceFromReference = Math.Min(localHour, 24.0 - localHour);
        }
        
        // Maximum distance is 12 hours
        // Strength = (12 - distance) / 12 * 30 Rupas
        double strength = ((12.0 - distanceFromReference) / 12.0) * 30.0;
        
        return Math.Max(0, Math.Min(30.0, strength));
    }
    
    /// <summary>
    /// Calculate Paksha Bala (Lunar fortnight strength) - Accurate Parasara method
    /// Based on Moon-Sun elongation and planet nature
    /// Maximum: 30 Rupas
    /// </summary>
    private double CalculatePakshaBalaAccurate(string planetName, double moonLongitude, double sunLongitude)
    {
        // Calculate Moon-Sun elongation (Tithi angle)
        double elongation = moonLongitude - sunLongitude;
        while (elongation < 0) elongation += 360.0;
        while (elongation >= 360.0) elongation -= 360.0;
        
        // Benefics: Moon, Mercury, Jupiter, Venus
        bool isBenefic = planetName is "Moon" or "Mercury" or "Jupiter" or "Venus";
        
        // For benefics: Strength increases from New Moon (0°) to Full Moon (180°)
        // For malefics: Strength increases from Full Moon (180°) to New Moon (360°/0°)
        
        double strength;
        if (isBenefic)
        {
            // Benefics: Linear from 0 at New Moon to 30 at Full Moon
            if (elongation <= 180.0)
            {
                strength = (elongation / 180.0) * 30.0;
            }
            else
            {
                strength = ((360.0 - elongation) / 180.0) * 30.0;
            }
        }
        else
        {
            // Malefics: Linear from 30 at New Moon to 0 at Full Moon
            if (elongation <= 180.0)
            {
                strength = ((180.0 - elongation) / 180.0) * 30.0;
            }
            else
            {
                strength = ((elongation - 180.0) / 180.0) * 30.0;
            }
        }
        
        return Math.Max(0, Math.Min(30.0, strength));
    }
    
    /// <summary>
    /// Calculate Tribhaga Bala (Day/Night third division) - Accurate Parasara method
    /// Divide day and night into 3 parts each, assign specific planets to each part
    /// Maximum: 20 Rupas
    /// </summary>
    private double CalculateTribhagaBalaAccurate(string planetName, DateTime dateTime, double sunLongitude)
    {
        // Simplified: Using 6 AM as sunrise, 6 PM as sunset
        // In full implementation, would calculate actual sunrise/sunset times
        double hour = dateTime.Hour + dateTime.Minute / 60.0;
        
        // Determine which Tribhaga period (1-6)
        int tribhagaPeriod;
        
        if (hour >= 6 && hour < 10)
            tribhagaPeriod = 1; // Morning first part - Mercury
        else if (hour >= 10 && hour < 14)
            tribhagaPeriod = 2; // Morning second part - Sun
        else if (hour >= 14 && hour < 18)
            tribhagaPeriod = 3; // Morning third part - Saturn
        else if (hour >= 18 && hour < 22)
            tribhagaPeriod = 4; // Night first part - Moon
        else if (hour >= 22 || hour < 2)
            tribhagaPeriod = 5; // Night second part - Venus
        else // hour >= 2 && hour < 6
            tribhagaPeriod = 6; // Night third part - Mars
        
        // Assign ruling planet for each period
        var rulingPlanet = tribhagaPeriod switch
        {
            1 => "Mercury",
            2 => "Sun",
            3 => "Saturn",
            4 => "Moon",
            5 => "Venus",
            6 => "Mars",
            _ => "Jupiter" // Default
        };
        
        // If planet matches the ruling planet of current Tribhaga, give 20 Rupas
        return rulingPlanet == planetName ? 20.0 : 0.0;
    }
    
    /// <summary>
    /// Calculate Varsha Bala (Year Lord strength) - Accurate method
    /// Maximum: 15 Rupas
    /// </summary>
    private double CalculateVarshaBala(string planetName, DateTime dateTime)
    {
        // Simplified: Use year mod 7 cycle
        // In full implementation, would use Samvatsara cycle
        int yearIndex = dateTime.Year % 7;
        
        var yearLords = new[] { "Saturn", "Jupiter", "Mars", "Sun", "Venus", "Mercury", "Moon" };
        var yearLord = yearLords[yearIndex];
        
        return yearLord == planetName ? 15.0 : 0.0;
    }
    
    /// <summary>
    /// Calculate Masa Bala (Month Lord strength) - Accurate Parasara method
    /// Based on which sign the Sun is transiting
    /// Maximum: 10 Rupas
    /// </summary>
    private double CalculateMasaBalaAccurate(string planetName, double sunLongitude)
    {
        // Get current solar month (sign Sun is in)
        int sunSign = (int)(sunLongitude / 30.0) + 1;
        if (sunSign > 12) sunSign = 12;
        
        // Lord of the sign where Sun is transiting
        var monthLord = GetSignLord(sunSign);
        
        return monthLord == planetName ? 10.0 : 0.0;
    }
    
    /// <summary>
    /// Calculate Vara Bala (Weekday Lord strength) - Accurate Parasara method
    /// Maximum: 8 Rupas
    /// </summary>
    private double CalculateVaraBalaAccurate(string planetName, DateTime dateTime)
    {
        var dayOfWeek = dateTime.DayOfWeek;
        var weekdayLord = GetPlanetForWeekday(dayOfWeek);
        
        return weekdayLord == planetName ? 8.0 : 0.0;
    }
    
    /// <summary>
    /// Calculate Hora Bala (Hour Lord strength) - Accurate Parasara method
    /// Based on planetary hour system starting from sunrise
    /// Maximum: 8 Rupas
    /// </summary>
    private double CalculateHoraBalaAccurate(string planetName, DateTime dateTime, double sunLongitude)
    {
        // Simplified: Assuming 6 AM sunrise
        // In full implementation, calculate actual sunrise time based on location and date
        
        double hoursSinceSunrise = dateTime.Hour - 6.0 + dateTime.Minute / 60.0;
        if (hoursSinceSunrise < 0) hoursSinceSunrise += 24.0;
        
        // Planetary hour sequence starting from day lord
        var horaSequence = new[] { "Saturn", "Jupiter", "Mars", "Sun", "Venus", "Mercury", "Moon" };
        
        // Get day lord
        var dayLord = GetPlanetForWeekday(dateTime.DayOfWeek);
        int dayLordIndex = Array.IndexOf(horaSequence, dayLord);
        
        // Calculate current hora lord
        int horasSinceSunrise = (int)hoursSinceSunrise;
        int horaLordIndex = (dayLordIndex + horasSinceSunrise) % 7;
        var horaLord = horaSequence[horaLordIndex];
        
        return horaLord == planetName ? 8.0 : 0.0;
    }
    
    /// <summary>
    /// Calculate Ayana Bala (Declination strength) - Accurate Parasara method
    /// Based on Sun's declination (Kranti) and planet's nature
    /// Maximum: 20 Rupas
    /// </summary>
    private double CalculateAyanaBalaAccurate(string planetName, double sunLongitude, double planetLatitude)
    {
        // Calculate Sun's declination (Kranti)
        // Approximate formula: declination ? 23.45° × sin(sunLongitude - 80°)
        double sunDeclination = 23.45 * Math.Sin((sunLongitude - 80.0) * Math.PI / 180.0);
        
        // Determine if Sun is in Uttarayana (North) or Dakshinayana (South)
        // Uttarayana: Capricorn to Gemini (270° to 90°) - declination increasing
        // Dakshinayana: Cancer to Sagittarius (90° to 270°) - declination decreasing
        
        bool isUttarayana = sunLongitude >= 270.0 || sunLongitude < 90.0;
        
        // Benefics strong during Uttarayana, Malefics during Dakshinayana
        bool isBenefic = planetName is "Moon" or "Mercury" or "Jupiter" or "Venus";
        
        // Calculate strength based on declination magnitude
        double declinationStrength = Math.Abs(sunDeclination) / 23.45; // 0 to 1
        
        double ayanaBala;
        if (isUttarayana)
        {
            // Uttarayana period
            ayanaBala = isBenefic ? declinationStrength * 20.0 : (1.0 - declinationStrength) * 20.0;
        }
        else
        {
            // Dakshinayana period
            ayanaBala = isBenefic ? (1.0 - declinationStrength) * 20.0 : declinationStrength * 20.0;
        }
        
        return Math.Max(0, Math.Min(20.0, ayanaBala));
    }
    
    /// <summary>
    /// Calculate Yuddha Bala (Planetary War strength)
    /// When two planets are within 1° of each other, they are in war
    /// The planet with higher longitude wins and gets additional strength
    /// </summary>
    private double CalculateYuddhaBala(PlanetData planet, HoroscopeData horoscope)
    {
        // Yuddha only applies to non-luminaries (not Sun/Moon)
        if (planet.Name == "Sun" || planet.Name == "Moon")
            return 0.0;
        
        double yuddhaBala = 0.0;
        
        // Check for planets within 1° (excluding Sun, Moon, Rahu, Ketu)
        var eligiblePlanets = horoscope.Planets
            .Where(p => p.Name != "Sun" && p.Name != "Moon" && p.Name != "Rahu" && p.Name != "Ketu" && p.Name != planet.Name)
            .ToList();
        
        foreach (var otherPlanet in eligiblePlanets)
        {
            double angularDistance = Math.Abs(planet.Longitude - otherPlanet.Longitude);
            
            // Normalize to 0-180 range
            if (angularDistance > 180.0)
                angularDistance = 360.0 - angularDistance;
            
            // Check if within 1°
            if (angularDistance <= 1.0)
            {
                // Planetary war! Determine winner based on longitude
                // The planet ahead (higher longitude in same sign) wins
                
                // Get planet sizes (apparent diameter)
                double planetSize = GetPlanetApparentSize(planet.Name);
                double otherSize = GetPlanetApparentSize(otherPlanet.Name);
                
                // Winner gets strength bonus, loser loses strength
                if (planetSize > otherSize)
                {
                    // This planet wins - gains strength
                    yuddhaBala += 10.0;
                }
                else if (planetSize < otherSize)
                {
                    // This planet loses - loses strength
                    yuddhaBala -= 10.0;
                }
                // If equal size, no change
            }
        }
        
        return yuddhaBala;
    }
    
    /// <summary>
    /// Get apparent size/diameter of planet (for Yuddha Bala)
    /// Larger planets win in planetary war
    /// </summary>
    private double GetPlanetApparentSize(string planetName)
    {
        // Relative apparent sizes (arbitrary units)
        return planetName switch
        {
            "Jupiter" => 5.0,  // Largest
            "Venus" => 4.0,
            "Mars" => 3.0,
            "Mercury" => 2.0,
            "Saturn" => 1.5,   // Smallest (far away)
            _ => 1.0
        };
    }
    
    /// <summary>
    /// Get planet lord for a weekday
    /// </summary>
    private string GetPlanetForWeekday(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => "Sun",
            DayOfWeek.Monday => "Moon",
            DayOfWeek.Tuesday => "Mars",
            DayOfWeek.Wednesday => "Mercury",
            DayOfWeek.Thursday => "Jupiter",
            DayOfWeek.Friday => "Venus",
            DayOfWeek.Saturday => "Saturn",
            _ => "Sun"
        };
    }

    /// <summary>
    /// Calculate aspectual strength (Drik Bala) - Parasara method
    /// Based on aspects received from other planets
    /// Includes special aspects: Jupiter (5th, 7th, 9th), Saturn (3rd, 7th, 10th), Mars (4th, 7th, 8th)
    /// Maximum: 60 Rupas, Can be negative if heavily afflicted
    /// </summary>
    private double CalculateAspectualStrength(PlanetData planet, HoroscopeData horoscope)
    {
        double drikBala = 0.0; // Start from 0, aspects add or subtract
        
        foreach (var otherPlanet in horoscope.Planets)
        {
            if (otherPlanet.Name == planet.Name) continue;
            if (otherPlanet.Name == "Rahu" || otherPlanet.Name == "Ketu") continue; // Nodes don't aspect in Parasara
            
            // Check if otherPlanet aspects this planet
            if (DoesAspect(otherPlanet, planet))
            {
                // Calculate aspect strength based on orb (exactness)
                double aspectStrength = GetAspectStrength(otherPlanet, planet);
                
                // Determine if benefic or malefic
                bool isBenefic = otherPlanet.Name is "Jupiter" or "Venus" or "Mercury" or "Moon";
                
                // Natural benefics add strength, malefics subtract
                if (isBenefic)
                {
                    drikBala += aspectStrength * 10.0; // Benefic aspect adds strength
                }
                else
                {
                    drikBala -= aspectStrength * 5.0; // Malefic aspect reduces strength
                }
            }
        }
        
        // Normalize to 0-60 range per Parasara
        // Base strength is 30 (neutral), aspects modify it
        double finalDrikBala = 30.0 + drikBala;
        
        return Math.Max(0, Math.Min(60.0, finalDrikBala));
    }
    
    /// <summary>
    /// Check if a planet aspects another - Parasara method with special aspects
    /// </summary>
    private bool DoesAspect(PlanetData aspectingPlanet, PlanetData aspectedPlanet)
    {
        int houseDifference = Math.Abs(aspectingPlanet.House - aspectedPlanet.House);
        if (houseDifference > 6) houseDifference = 12 - houseDifference;
        
        // All planets aspect 7th house (opposition)
        if (houseDifference == 6) // 7th house
            return true;
        
        // Special aspects per Parasara
        switch (aspectingPlanet.Name)
        {
            case "Jupiter":
                // Jupiter aspects 5th, 7th, 9th houses
                return houseDifference == 4 || houseDifference == 6 || houseDifference == 8;
                
            case "Saturn":
                // Saturn aspects 3rd, 7th, 10th houses
                return houseDifference == 2 || houseDifference == 6 || houseDifference == 9;
                
            case "Mars":
                // Mars aspects 4th, 7th, 8th houses
                return houseDifference == 3 || houseDifference == 6 || houseDifference == 7;
                
            default:
                // Other planets only aspect 7th house
                return houseDifference == 6;
        }
    }
    
    /// <summary>
    /// Calculate aspect strength based on orb (exactness) - Parasara method
    /// Closer to exact = stronger aspect
    /// </summary>
    private double GetAspectStrength(PlanetData aspectingPlanet, PlanetData aspectedPlanet)
    {
        // Calculate angular distance
        double angularDistance = Math.Abs(aspectingPlanet.Longitude - aspectedPlanet.Longitude);
        if (angularDistance > 180.0) angularDistance = 360.0 - angularDistance;
        
        // Find nearest aspect angle (180° for 7th, etc.)
        double nearestAspectAngle = 180.0; // Default 7th house aspect
        
        // Adjust for special aspects
        if (aspectingPlanet.Name == "Jupiter")
        {
            // Check 120° (5th), 180° (7th), 240° (9th)
            double[] angles = { 120.0, 180.0, 240.0 };
            nearestAspectAngle = angles.OrderBy(a => Math.Abs(angularDistance - a)).First();
        }
        else if (aspectingPlanet.Name == "Saturn")
        {
            // Check 60° (3rd), 180° (7th), 270° (10th)
            double[] angles = { 60.0, 180.0, 270.0 };
            nearestAspectAngle = angles.OrderBy(a => Math.Abs(angularDistance - a)).First();
        }
        else if (aspectingPlanet.Name == "Mars")
        {
            // Check 90° (4th), 180° (7th), 210° (8th)
            double[] angles = { 90.0, 180.0, 210.0 };
            nearestAspectAngle = angles.OrderBy(a => Math.Abs(angularDistance - a)).First();
        }
        
        // Calculate orb (deviation from exact aspect)
        double orb = Math.Abs(angularDistance - nearestAspectAngle);
        if (orb > 180.0) orb = 360.0 - orb;
        
        // Aspect is stronger when orb is smaller
        // Full strength within 5° orb, decreasing to 0 at 15° orb
        double maxOrb = 15.0; // Parasara allows up to 15° orb
        if (orb > maxOrb) return 0.0;
        
        double strength = (maxOrb - orb) / maxOrb; // 1.0 at exact, 0.0 at max orb
        
        return strength;
    }

    /// <summary>
    /// Get sign relationship for a planet in a sign - Used for reference
    /// </summary>
    private string GetSignRelationship(string planetName, int rasi)
    {
        var exaltation = planetName switch
        {
            "Sun" => 1, "Moon" => 2, "Mars" => 10, "Mercury" => 6,
            "Jupiter" => 4, "Venus" => 12, "Saturn" => 7, _ => 0
        };
        if (rasi == exaltation) return "Exaltation";

        var debilitation = exaltation <= 6 ? exaltation + 6 : exaltation - 6;
        if (rasi == debilitation) return "Debilitation";

        var ownSigns = GetOwnSigns(planetName);
        if (ownSigns.Contains(rasi)) return "OwnSign";

        return "Neutral";
    }
}
