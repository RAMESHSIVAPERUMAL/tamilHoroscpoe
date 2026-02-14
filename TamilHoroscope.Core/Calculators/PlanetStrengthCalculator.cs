using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Core.Calculators;

/// <summary>
/// Calculator for planetary strength (Shadbala) per Brihat Parashara Hora Shastra.
/// All internal calculations are in Virupas (Shashtiamsas).
/// Final results are stored in Rupas (1 Rupa = 60 Virupas).
/// </summary>
public class PlanetStrengthCalculator
{
    // Naisargika Bala (Natural Strength) in Virupas - BPHS fixed values
    // Sun is brightest, Saturn is dimmest. Separated by 1/7 of 60 Virupas.
    private static readonly Dictionary<string, double> NaisargikaBalaVirupas = new()
    {
        { "Sun", 60.0 },
        { "Moon", 51.43 },
        { "Venus", 42.86 },
        { "Jupiter", 34.29 },
        { "Mercury", 25.71 },
        { "Mars", 17.14 },
        { "Saturn", 8.57 }
    };

    // Required minimum total Shadbala for benefic results (in Rupas) - BPHS Chapter 27
    private static readonly Dictionary<string, double> RequiredStrengths = new()
    {
        { "Sun", 6.5 },
        { "Moon", 6.0 },
        { "Mars", 5.0 },
        { "Mercury", 7.0 },
        { "Jupiter", 6.5 },
        { "Venus", 5.5 },
        { "Saturn", 5.0 }
    };

    /// <summary>
    /// Calculate Shadbala for all planets (excluding Rahu and Ketu).
    /// Returns results in Rupas with individual components.
    /// </summary>
    /// <param name="horoscope">Horoscope data</param>
    /// <param name="language">Language for localized names (Tamil, Telugu, Kannada, Malayalam)</param>
    public List<PlanetStrengthData> CalculatePlanetaryStrengths(HoroscopeData horoscope, string language = "Tamil")
    {
        var strengths = new List<PlanetStrengthData>();

        foreach (var planet in horoscope.Planets.Where(p => p.Name != "Rahu" && p.Name != "Ketu"))
        {
            var strength = new PlanetStrengthData
            {
                Name = planet.Name,
                Language = language,
#pragma warning disable CS0618
                TamilName = planet.TamilName,
#pragma warning restore CS0618
                RequiredStrength = RequiredStrengths.GetValueOrDefault(planet.Name, 5.0)
            };

            // Pre-calculate Moon's Paksha Bala (needed for Chesta Bala)
            if (planet.Name == "Moon")
            {
                double moonLong = horoscope.Panchang.MoonLongitude;
                double sunLong = horoscope.Panchang.SunLongitude;
                _currentPakshaBalaForMoon = CalculatePakshaBala("Moon", moonLong, sunLong);
            }

            // Calculate each component in Virupas, convert to Rupas for storage
            double sthanaBalaV = CalculateSthanaBala(planet, horoscope);
            double digBalaV = CalculateDigBala(planet);
            double kalaBalaV = CalculateKalaBala(planet, horoscope);
            double chestaBalaV = CalculateChestaBala(planet);
            double naisargikaBalaV = NaisargikaBalaVirupas.GetValueOrDefault(planet.Name, 25.0);
            double drikBalaV = CalculateDrikBala(planet, horoscope);

            // Store in Rupas (divide by 60)
            strength.PositionalStrength = sthanaBalaV / 60.0;
            strength.DirectionalStrength = digBalaV / 60.0;
            strength.TemporalStrength = kalaBalaV / 60.0;
            strength.MotionalStrength = chestaBalaV / 60.0;
            strength.NaturalStrength = naisargikaBalaV / 60.0;
            strength.AspectualStrength = drikBalaV / 60.0;

            strength.TotalStrength =
                strength.PositionalStrength +
                strength.DirectionalStrength +
                strength.TemporalStrength +
                strength.MotionalStrength +
                strength.NaturalStrength +
                strength.AspectualStrength;

            // Strength ratio = Total / Required. Ratio >= 1.0 means sufficient.
            double ratio = strength.RequiredStrength > 0
                ? strength.TotalStrength / strength.RequiredStrength
                : 1.0;

            // Scale: ratio 0 -> 0%, ratio 1 -> 50%, ratio 2 -> 100%
            strength.StrengthPercentage = Math.Max(0, Math.Min(100, ratio * 50.0));

            strengths.Add(strength);
        }

        return strengths;
    }

    // ----------------------------------------------------------------
    // 1. STHANA BALA (Positional Strength) - all in Virupas
    // ----------------------------------------------------------------

    private double CalculateSthanaBala(PlanetData planet, HoroscopeData horoscope)
    {
        double total = 0.0;
        total += CalculateUchchaBala(planet);          // max 60
        total += CalculateSaptavargajaBala(planet);    // max ~337 (sum of 7 divisions)
        total += CalculateOjhayugmaBala(planet);       // max 30 (Rasi 15 + Navamsa 15)
        total += CalculateKendraBala(planet);           // max 60
        total += CalculateDrekkanaBala(planet);         // max 15
        return total;
    }

    /// <summary>
    /// Uchcha Bala - max 60 Virupas.
    /// = (arc from debilitation point / 3).
    /// At exaltation point -> 60V, at debilitation -> 0V.
    /// </summary>
    private double CalculateUchchaBala(PlanetData planet)
    {
        var (debSign, debDeg) = GetDebilitationPoint(planet.Name);
        double debLong = (debSign - 1) * 30.0 + debDeg;

        double pLong = NormalizeDeg(planet.Longitude);
        double dist = NormalizeDeg(pLong - debLong);
        if (dist > 180.0) dist = 360.0 - dist;

        // dist / 3 gives Virupas, max = 180/3 = 60
        return dist / 3.0;
    }

    /// <summary>
    /// Saptavargaja Bala - dignity in 7 divisional charts.
    /// BPHS assigns points per dignity level:
    ///   Moolatrikona=45, Own=30, Great Friend=22.5, Friend=15, Neutral=7.5, Enemy=3.75, Great Enemy=1.875
    /// The 7 divisions are: D-1, D-2, D-3, D-7, D-9, D-12, D-30.
    /// </summary>
    private double CalculateSaptavargajaBala(PlanetData planet)
    {
        double total = 0.0;

        // D-1 (Rasi)
        total += GetDignityVirupas(planet.Name, planet.Rasi);

        // D-2 (Hora)
        total += GetDignityVirupas(planet.Name, CalculateHoraSign(planet.Longitude));

        // D-3 (Drekkana)
        total += GetDignityVirupas(planet.Name, CalculateDrekkanaSign(planet.Longitude));

        // D-7 (Saptamsa)
        total += GetDignityVirupas(planet.Name, CalculateSaptamsaSign(planet.Longitude));

        // D-9 (Navamsa)
        total += GetDignityVirupas(planet.Name, CalculateNavamsaSign(planet.Longitude));

        // D-12 (Dwadasamsa)
        total += GetDignityVirupas(planet.Name, CalculateDwadasamsaSign(planet.Longitude));

        // D-30 (Trimshamsa)
        total += GetDignityVirupas(planet.Name, CalculateTrimshamsaSign(planet.Longitude));

        return total;
    }

    /// <summary>
    /// Get Saptavargaja dignity Virupas for a planet in a given sign.
    /// </summary>
    private double GetDignityVirupas(string planetName, int sign)
    {
        var (exSign, _) = GetExaltationPoint(planetName);
        var (debSign, _) = GetDebilitationPoint(planetName);
        var ownSigns = GetOwnSigns(planetName);
        int moolTriSign = GetMoolatrikonaSign(planetName);
        var signLord = GetSignLord(sign);

        if (sign == debSign) return 1.875;                   // Debilitated
        if (sign == exSign) return 45.0;                     // Exalted (treated as Moolatrikona-level)
        if (moolTriSign == sign) return 45.0;                // Moolatrikona
        if (ownSigns.Contains(sign)) return 30.0;            // Own sign

        var rel = GetPlanetRelationship(planetName, signLord);
        return rel switch
        {
            "Own" => 30.0,
            "GreatFriend" => 22.5,
            "Friend" => 15.0,
            "Neutral" => 7.5,
            "Enemy" => 3.75,
            "GreatEnemy" => 1.875,
            _ => 7.5
        };
    }

    /// <summary>
    /// Ojhayugma Bala - Rasi component (15V) + Navamsa component (15V) = max 30V.
    /// Moon/Venus: strong in even signs. Others: strong in odd signs.
    /// </summary>
    private double CalculateOjhayugmaBala(PlanetData planet)
    {
        double bala = 0.0;
        bool isFeminine = planet.Name is "Moon" or "Venus";

        // Rasi component
        bool rasiIsOdd = planet.Rasi % 2 == 1;
        if (isFeminine && !rasiIsOdd) bala += 15.0;
        else if (!isFeminine && rasiIsOdd) bala += 15.0;

        // Navamsa component
        int navSign = CalculateNavamsaSign(planet.Longitude);
        bool navIsOdd = navSign % 2 == 1;
        if (isFeminine && !navIsOdd) bala += 15.0;
        else if (!isFeminine && navIsOdd) bala += 15.0;

        return bala;
    }

    /// <summary>
    /// Kendra Bala - 60/30/15 Virupas for Kendra/Panaphara/Apoklima houses.
    /// </summary>
    private double CalculateKendraBala(PlanetData planet)
    {
        int h = planet.House;
        if (h == 1 || h == 4 || h == 7 || h == 10) return 60.0;
        if (h == 2 || h == 5 || h == 8 || h == 11) return 30.0;
        return 15.0;
    }

    /// <summary>
    /// Drekkana Bala - 15 Virupas.
    /// Male planets (Su/Ma/Ju) strong in 1st decanate.
    /// Neutral (Me) strong in 2nd decanate.
    /// Female planets (Mo/Ve/Sa) strong in 3rd decanate.
    /// Gets 15V if in matching decanate, 0 otherwise.
    /// </summary>
    private double CalculateDrekkanaBala(PlanetData planet)
    {
        double posInSign = NormalizeDeg(planet.Longitude) % 30.0;
        int decanate = (int)(posInSign / 10.0) + 1; // 1, 2, or 3

        bool isMale = planet.Name is "Sun" or "Mars" or "Jupiter";
        bool isFemale = planet.Name is "Moon" or "Venus" or "Saturn";
        bool isNeutral = planet.Name == "Mercury";

        if (isMale && decanate == 1) return 15.0;
        if (isNeutral && decanate == 2) return 15.0;
        if (isFemale && decanate == 3) return 15.0;
        return 0.0;
    }

    // ----------------------------------------------------------------
    // 2. DIG BALA (Directional Strength) - max 60 Virupas
    // ----------------------------------------------------------------

    /// <summary>
    /// Dig Bala per BPHS - linear interpolation.
    /// Strongest at ideal house (60V), weakest at opposite house (0V).
    /// Ideal houses: Ju/Me -> 1, Su/Ma -> 10, Sa -> 7, Mo/Ve -> 4.
    /// Formula: DigBala = 60 - (distance x 10) where distance is in houses (0..6).
    /// </summary>
    private double CalculateDigBala(PlanetData planet)
    {
        int house = planet.House;
        int idealHouse = planet.Name switch
        {
            "Jupiter" or "Mercury" => 1,
            "Sun" or "Mars" => 10,
            "Saturn" => 7,
            "Moon" or "Venus" => 4,
            _ => 1
        };

        // Forward distance from ideal house to actual house (0..11)
        int fwd = ((house - idealHouse) % 12 + 12) % 12;
        // Convert to 0..6 range (take shorter arc)
        if (fwd > 6) fwd = 12 - fwd;

        // Linear: 60V at ideal (fwd=0), 0V at opposite (fwd=6)
        double digBala = 60.0 - (fwd * 10.0);
        return digBala;
    }

    // ----------------------------------------------------------------
    // 3. KALA BALA (Temporal Strength) - all in Virupas
    // ----------------------------------------------------------------

    private double CalculateKalaBala(PlanetData planet, HoroscopeData horoscope)
    {
        var dt = horoscope.BirthDetails?.DateTime ?? DateTime.Now;
        double moonLong = horoscope.Panchang.MoonLongitude;
        double sunLong = horoscope.Panchang.SunLongitude;

        double total = 0.0;
        total += CalculateNathonnathaBala(planet.Name, dt);                 // max 60V

        // Paksha Bala: per BPHS, Moon's Paksha Bala goes to Chesta Bala instead
        if (planet.Name != "Moon")
            total += CalculatePakshaBala(planet.Name, moonLong, sunLong);   // max 60V

        total += CalculateTribhagaBala(planet.Name, dt);                    // max 60V
        total += CalculateAbdaBala(planet.Name, dt);                        // max 15V
        total += CalculateMasaBala(planet.Name, sunLong);                   // max 30V
        total += CalculateVaraBala(planet.Name, dt);                        // max 45V
        total += CalculateHoraBala(planet.Name, dt);                        // max 60V

        // Ayana Bala: per BPHS, Sun's Ayana Bala goes to Chesta Bala instead
        if (planet.Name != "Sun")
            total += CalculateAyanaBala(planet.Name, planet.Longitude);     // max 60V

        total += CalculateYuddhaBala(planet, horoscope);                    // variable
        return total;
    }

    /// <summary>
    /// Nathonnatha Bala - max 60 Virupas.
    /// Diurnal planets (Su/Ju/Ve) strong at noon, nocturnal (Mo/Ma/Sa) at midnight.
    /// Mercury always 60V.
    /// </summary>
    private double CalculateNathonnathaBala(string planetName, DateTime dt)
    {
        if (planetName == "Mercury") return 60.0; // Always strong

        double hour = dt.Hour + dt.Minute / 60.0 + dt.Second / 3600.0;

        // Diurnal planets: Sun, Jupiter, Venus - strong at noon (hour=12)
        // Nocturnal planets: Moon, Mars, Saturn - strong at midnight (hour=0/24)
        bool isDiurnal = planetName is "Sun" or "Jupiter" or "Venus";

        double distFromRef;
        if (isDiurnal)
        {
            // Distance from noon
            distFromRef = Math.Abs(hour - 12.0);
        }
        else
        {
            // Distance from midnight
            distFromRef = hour <= 12.0 ? hour : 24.0 - hour;
        }

        // At reference point -> 60V, at 12 hours away -> 0V
        double bala = ((12.0 - distFromRef) / 12.0) * 60.0;
        return Math.Max(0, bala);
    }

    /// <summary>
    /// Paksha Bala - max 60 Virupas.
    /// Benefics (Ju/Ve/Mo/Me) strong in Shukla Paksha (waxing).
    /// Malefics (Su/Ma/Sa) strong in Krishna Paksha (waning).
    /// </summary>
    private double CalculatePakshaBala(string planetName, double moonLong, double sunLong)
    {
        double elongation = NormalizeDeg(moonLong - sunLong);
        bool isBenefic = planetName is "Moon" or "Mercury" or "Jupiter" or "Venus";

        double bala;
        if (isBenefic)
        {
            // Strongest at Full Moon (180 deg), weakest at New Moon (0 deg)
            bala = (elongation <= 180.0)
                ? (elongation / 180.0) * 60.0
                : ((360.0 - elongation) / 180.0) * 60.0;
        }
        else
        {
            // Strongest at New Moon, weakest at Full Moon
            bala = (elongation <= 180.0)
                ? ((180.0 - elongation) / 180.0) * 60.0
                : ((elongation - 180.0) / 180.0) * 60.0;
        }

        return Math.Max(0, Math.Min(60.0, bala));
    }

    /// <summary>
    /// Tribhaga Bala - 60 Virupas if planet rules current third of day/night.
    /// Day thirds: Mercury, Sun, Saturn. Night thirds: Moon, Venus, Mars.
    /// Jupiter always gets 60V.
    /// </summary>
    private double CalculateTribhagaBala(string planetName, DateTime dt)
    {
        if (planetName == "Jupiter") return 60.0; // Always strong

        double hour = dt.Hour + dt.Minute / 60.0;

        // Approximate: day 6AM-6PM, night 6PM-6AM
        // Day: 1st third (6-10) = Mercury, 2nd (10-14) = Sun, 3rd (14-18) = Saturn
        // Night: 1st third (18-22) = Moon, 2nd (22-2) = Venus, 3rd (2-6) = Mars
        string ruler;
        if (hour >= 6 && hour < 10) ruler = "Mercury";
        else if (hour >= 10 && hour < 14) ruler = "Sun";
        else if (hour >= 14 && hour < 18) ruler = "Saturn";
        else if (hour >= 18 && hour < 22) ruler = "Moon";
        else if (hour >= 22 || hour < 2) ruler = "Venus";
        else ruler = "Mars"; // 2-6

        return ruler == planetName ? 60.0 : 0.0;
    }

    /// <summary>
    /// Abda Bala (Year Lord) - 15 Virupas to the lord of the year.
    /// </summary>
    private double CalculateAbdaBala(string planetName, DateTime dt)
    {
        var yearLords = new[] { "Sun", "Venus", "Mercury", "Moon", "Saturn", "Jupiter", "Mars" };
        int idx = dt.Year % 7;
        return yearLords[idx] == planetName ? 15.0 : 0.0;
    }

    /// <summary>
    /// Masa Bala (Month Lord) - 30 Virupas to the lord of the lunar month.
    /// Approximated by sign lord of Sun's position.
    /// </summary>
    private double CalculateMasaBala(string planetName, double sunLongitude)
    {
        int sunSign = (int)(NormalizeDeg(sunLongitude) / 30.0) + 1;
        if (sunSign > 12) sunSign = 12;
        return GetSignLord(sunSign) == planetName ? 30.0 : 0.0;
    }

    /// <summary>
    /// Vara Bala (Weekday Lord) - 45 Virupas to the lord of the weekday.
    /// </summary>
    private double CalculateVaraBala(string planetName, DateTime dt)
    {
        return GetPlanetForWeekday(dt.DayOfWeek) == planetName ? 45.0 : 0.0;
    }

    /// <summary>
    /// Hora Bala (Hora Lord) - 60 Virupas to the lord of the planetary hour.
    /// Uses the Chaldean order descending:
    /// Saturn, Jupiter, Mars, Sun, Venus, Mercury, Moon (repeat).
    /// First hora of the day belongs to the weekday lord.
    /// </summary>
    private double CalculateHoraBala(string planetName, DateTime dt)
    {
        // Chaldean descending order
        var chaldeanOrder = new[] { "Saturn", "Jupiter", "Mars", "Sun", "Venus", "Mercury", "Moon" };

        // Approximate sunrise at 6:00 AM
        double hoursSinceSunrise = dt.Hour - 6.0 + dt.Minute / 60.0;
        if (hoursSinceSunrise < 0) hoursSinceSunrise += 24.0;

        string dayLord = GetPlanetForWeekday(dt.DayOfWeek);
        int dayLordIdx = Array.IndexOf(chaldeanOrder, dayLord);

        int horaNumber = (int)hoursSinceSunrise; // 0-based hora count
        int horaLordIdx = ((dayLordIdx + horaNumber) % 7 + 7) % 7;
        return chaldeanOrder[horaLordIdx] == planetName ? 60.0 : 0.0;
    }

    /// <summary>
    /// Ayana Bala - max 60 Virupas per BPHS.
    /// Based on planet's own declination (approximated from its longitude).
    /// Per B.V. Raman's standard formula (BPHS Ch.27 v.22):
    ///   AyanaBala = (declination + 23.45) / 46.9 * 60
    /// All planets gain more strength with northern declination.
    /// This is the most widely used interpretation in standard Shadbala software.
    /// </summary>
    private double CalculateAyanaBala(string planetName, double planetLongitude)
    {
        // Approximate the planet's declination from its sidereal longitude
        // declination ~ 23.45 * sin(longitude) for ecliptic bodies near 0 latitude
        double declination = 23.45 * Math.Sin(NormalizeDeg(planetLongitude) * Math.PI / 180.0);

        // B.V. Raman formula: (declination + 24) / 48 * 60
        // Using 23.45 for obliquity of ecliptic:
        // At max north declination (+23.45): 60V
        // At equator (0 declination): 30V
        // At max south declination (-23.45): 0V
        double bala = ((declination + 23.45) / 46.9) * 60.0;

        return Math.Max(0, Math.Min(60.0, bala));
    }

    /// <summary>
    /// Yuddha Bala - planetary war when two Tara grahas are within 1 degree.
    /// Winner (northernmost declination) gains, loser loses.
    /// </summary>
    private double CalculateYuddhaBala(PlanetData planet, HoroscopeData horoscope)
    {
        if (planet.Name is "Sun" or "Moon") return 0.0;

        double bala = 0.0;
        var eligible = horoscope.Planets
            .Where(p => p.Name != "Sun" && p.Name != "Moon" &&
                        p.Name != "Rahu" && p.Name != "Ketu" &&
                        p.Name != planet.Name);

        foreach (var other in eligible)
        {
            double dist = Math.Abs(planet.Longitude - other.Longitude);
            if (dist > 180.0) dist = 360.0 - dist;

            if (dist <= 1.0)
            {
                // Compare by latitude (declination proxy) - northernmost wins
                if (planet.Latitude > other.Latitude)
                    bala += 30.0;   // Winner bonus
                else if (planet.Latitude < other.Latitude)
                    bala -= 30.0;   // Loser penalty
            }
        }
        return bala;
    }

    // ----------------------------------------------------------------
    // 4. CHESTA BALA (Motional Strength) - max 60 Virupas
    // ----------------------------------------------------------------

    /// <summary>
    /// Chesta Bala per BPHS.
    /// Sun: Chesta Bala = Ayana Bala (declination-based, max 60V).
    /// Moon: Chesta Bala = Paksha Bala (elongation-based, max 60V).
    /// Others: Based on speed relative to mean. Retrograde = 60V.
    /// </summary>
    private double CalculateChestaBala(PlanetData planet)
    {
        // Sun's Chesta Bala = its Ayana Bala per BPHS
        if (planet.Name == "Sun")
            return CalculateAyanaBala(planet.Name, planet.Longitude);

        // Moon's Chesta Bala = its Paksha Bala per BPHS
        // We need Sun longitude for this; store it temporarily
        if (planet.Name == "Moon")
            return _currentPakshaBalaForMoon;

        if (planet.IsRetrograde)
            return 60.0;

        double normalSpeed = GetNormalSpeed(planet.Name);
        double currentSpeed = Math.Abs(planet.Speed);

        if (normalSpeed <= 0) return 30.0;

        // Speed ratio determines strength: at mean speed -> 30V, at 2x -> 60V, at 0 -> 0V
        double ratio = currentSpeed / normalSpeed;
        double bala = ratio * 30.0;
        return Math.Max(0, Math.Min(60.0, bala));
    }

    // Temporary storage for Moon's Paksha Bala to use as Chesta Bala
    private double _currentPakshaBalaForMoon;

    private double GetNormalSpeed(string planetName)
    {
        return planetName switch
        {
            "Mercury" => 1.383,
            "Venus" => 1.202,
            "Mars" => 0.524,
            "Jupiter" => 0.083,
            "Saturn" => 0.033,
            _ => 0.5
        };
    }

    // ----------------------------------------------------------------
    // 5. DRIK BALA (Aspectual Strength) - in Virupas, can be negative
    // ----------------------------------------------------------------

    /// <summary>
    /// Drik Bala per BPHS.
    /// Each aspecting planet contributes based on its aspect strength,
    /// positive for benefics, negative for malefics.
    /// Uses Parasara's Graha Drishti (planetary aspect) values.
    /// </summary>
    private double CalculateDrikBala(PlanetData planet, HoroscopeData horoscope)
    {
        double bala = 0.0;

        foreach (var other in horoscope.Planets)
        {
            if (other.Name == planet.Name) continue;
            if (other.Name is "Rahu" or "Ketu") continue;

            // Directional house difference (from aspecting to aspected)
            int diff = ((planet.House - other.House) % 12 + 12) % 12;
            if (diff == 0) diff = 12; // Same house -> treat as 12th from

            double aspectValue = GetParasaraAspectValue(other.Name, diff);
            if (aspectValue <= 0) continue;

            // Benefics add, malefics subtract
            bool isBenefic = other.Name is "Jupiter" or "Venus" or "Moon" or "Mercury";
            double contribution = aspectValue * 15.0; // Full aspect = 15V contribution

            bala += isBenefic ? contribution : -contribution;
        }

        return bala; // Can be negative - this is correct per BPHS
    }

    /// <summary>
    /// Parasara Graha Drishti (aspect) value.
    /// Returns aspect strength as fraction (0 to 1).
    /// All planets: 3rd/10th = 1/4, 4th/8th = 3/4, 5th/9th = 1/2, 7th = full.
    /// Special aspects: Mars 4th/8th=full, Jupiter 5th/9th=full, Saturn 3rd/10th=full.
    /// </summary>
    private double GetParasaraAspectValue(string planetName, int houseDiff)
    {
        // Base Parasara aspect values (for all planets)
        double baseAspect = houseDiff switch
        {
            3 or 10 => 0.25,  // Quarter aspect
            4 or 8 => 0.75,   // Three-quarter aspect
            5 or 9 => 0.50,   // Half aspect
            7 => 1.0,         // Full aspect (all planets)
            _ => 0.0
        };

        // Special (full) aspects override
        if (planetName == "Mars" && (houseDiff == 4 || houseDiff == 8))
            return 1.0;
        if (planetName == "Jupiter" && (houseDiff == 5 || houseDiff == 9))
            return 1.0;
        if (planetName == "Saturn" && (houseDiff == 3 || houseDiff == 10))
            return 1.0;

        return baseAspect;
    }

    // ----------------------------------------------------------------
    // DIVISIONAL CHART CALCULATIONS
    // ----------------------------------------------------------------

    private int CalculateHoraSign(double longitude)
    {
        longitude = NormalizeDeg(longitude);
        int sign = (int)(longitude / 30.0) + 1;
        double pos = longitude % 30.0;
        bool isOdd = sign % 2 == 1;
        bool isFirst = pos < 15.0;

        if (isOdd)
            return isFirst ? 5 : 4; // Leo : Cancer
        else
            return isFirst ? 4 : 5; // Cancer : Leo
    }

    private int CalculateDrekkanaSign(double longitude)
    {
        longitude = NormalizeDeg(longitude);
        int sign = (int)(longitude / 30.0) + 1;
        int drek = (int)((longitude % 30.0) / 10.0); // 0, 1, 2
        return ((sign - 1) + drek * 4) % 12 + 1;
    }

    private int CalculateSaptamsaSign(double longitude)
    {
        longitude = NormalizeDeg(longitude);
        int sign = (int)(longitude / 30.0) + 1;
        int part = (int)((longitude % 30.0) / (30.0 / 7.0)); // 0..6
        bool isOdd = sign % 2 == 1;
        int start;
        if (isOdd)
        {
            start = sign;
        }
        else
        {
            start = (sign + 6) % 12;
            if (start == 0) start = 12;
        }
        return ((start - 1) + part) % 12 + 1;
    }

    private int CalculateNavamsaSign(double longitude)
    {
        longitude = NormalizeDeg(longitude);
        int sign = (int)(longitude / 30.0) + 1;
        int nav = (int)((longitude % 30.0) / (30.0 / 9.0)); // 0..8

        int startSign = sign switch
        {
            1 or 5 or 9 => 1,     // Fire -> Aries
            2 or 6 or 10 => 10,   // Earth -> Capricorn
            3 or 7 or 11 => 7,    // Air -> Libra
            _ => 4                  // Water -> Cancer
        };

        return ((startSign - 1) + nav) % 12 + 1;
    }

    private int CalculateDwadasamsaSign(double longitude)
    {
        longitude = NormalizeDeg(longitude);
        int sign = (int)(longitude / 30.0) + 1;
        int part = (int)((longitude % 30.0) / 2.5); // 0..11
        return ((sign - 1) + part) % 12 + 1;
    }

    private int CalculateTrimshamsaSign(double longitude)
    {
        longitude = NormalizeDeg(longitude);
        int sign = (int)(longitude / 30.0) + 1;
        double pos = longitude % 30.0;
        bool isOdd = sign % 2 == 1;

        if (isOdd)
        {
            if (pos < 5.0) return 1;       // Mars -> Aries
            if (pos < 10.0) return 10;     // Saturn -> Capricorn
            if (pos < 18.0) return 9;      // Jupiter -> Sagittarius
            if (pos < 25.0) return 6;      // Mercury -> Virgo
            return 7;                       // Venus -> Libra
        }
        else
        {
            if (pos < 5.0) return 7;       // Venus -> Libra
            if (pos < 12.0) return 6;      // Mercury -> Virgo
            if (pos < 20.0) return 9;      // Jupiter -> Sagittarius
            if (pos < 25.0) return 10;     // Saturn -> Capricorn
            return 1;                       // Mars -> Aries
        }
    }

    // ----------------------------------------------------------------
    // REFERENCE DATA
    // ----------------------------------------------------------------

    private int[] GetOwnSigns(string planetName) => planetName switch
    {
        "Sun" => new[] { 5 },
        "Moon" => new[] { 4 },
        "Mars" => new[] { 1, 8 },
        "Mercury" => new[] { 3, 6 },
        "Jupiter" => new[] { 9, 12 },
        "Venus" => new[] { 2, 7 },
        "Saturn" => new[] { 10, 11 },
        _ => Array.Empty<int>()
    };

    /// <summary>
    /// Moolatrikona sign for each planet (single sign).
    /// </summary>
    private int GetMoolatrikonaSign(string planetName) => planetName switch
    {
        "Sun" => 5,       // Leo
        "Moon" => 2,      // Taurus (first 3 deg is exaltation, rest is Moolatrikona per some)
        "Mars" => 1,      // Aries
        "Mercury" => 6,   // Virgo
        "Jupiter" => 9,   // Sagittarius
        "Venus" => 7,     // Libra
        "Saturn" => 11,   // Aquarius
        _ => 0
    };

    private string GetSignLord(int sign) => sign switch
    {
        1 => "Mars", 2 => "Venus", 3 => "Mercury", 4 => "Moon",
        5 => "Sun", 6 => "Mercury", 7 => "Venus", 8 => "Mars",
        9 => "Jupiter", 10 => "Saturn", 11 => "Saturn", 12 => "Jupiter",
        _ => "Sun"
    };

    private (int sign, double degree) GetExaltationPoint(string p) => p switch
    {
        "Sun" => (1, 10.0),
        "Moon" => (2, 3.0),
        "Mars" => (10, 28.0),
        "Mercury" => (6, 15.0),
        "Jupiter" => (4, 5.0),
        "Venus" => (12, 27.0),
        "Saturn" => (7, 20.0),
        _ => (1, 0.0)
    };

    private (int sign, double degree) GetDebilitationPoint(string p) => p switch
    {
        "Sun" => (7, 10.0),
        "Moon" => (8, 3.0),
        "Mars" => (4, 28.0),
        "Mercury" => (12, 15.0),
        "Jupiter" => (10, 5.0),
        "Venus" => (6, 27.0),
        "Saturn" => (1, 20.0),
        _ => (7, 0.0)
    };

    /// <summary>
    /// Naisargika (permanent) planetary relationships per BPHS.
    /// Returns: Own, Friend, Neutral, Enemy.
    /// </summary>
    private string GetPlanetRelationship(string planet, string signLord)
    {
        if (planet == signLord) return "Own";

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
            { "Moon", Array.Empty<string>() },
            { "Mars", new[] { "Mercury" } },
            { "Mercury", new[] { "Moon" } },
            { "Jupiter", new[] { "Mercury", "Venus" } },
            { "Venus", new[] { "Sun", "Moon" } },
            { "Saturn", new[] { "Sun", "Moon", "Mars" } }
        };

        bool isFriend = friends.ContainsKey(planet) && friends[planet].Contains(signLord);
        bool isEnemy = enemies.ContainsKey(planet) && enemies[planet].Contains(signLord);

        if (isFriend) return "Friend";
        if (isEnemy) return "Enemy";
        return "Neutral";
    }

    private string GetPlanetForWeekday(DayOfWeek day) => day switch
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

    private static double NormalizeDeg(double deg)
    {
        deg %= 360.0;
        if (deg < 0) deg += 360.0;
        return deg;
    }
}
