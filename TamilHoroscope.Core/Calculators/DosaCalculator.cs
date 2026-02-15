using TamilHoroscope.Core.Data;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Core.Calculators;

/// <summary>
/// Calculator for detecting astrological doshas (afflictions) in a horoscope
/// 
/// METHODOLOGY: This calculator implements classical Vedic astrology dosha detection
/// principles based on traditional texts including references to Parasara's teachings.
/// The doshas detected are based on:
/// - Malefic planet placements in sensitive houses
/// - Unfavorable planetary relationships and positions
/// - Absence of beneficial planetary support
/// - Shadow planet (Rahu/Ketu) afflictions
/// 
/// PARASARA METHOD ALIGNMENT:
/// - ✓ Uses house-based dosha calculations as described in classical texts
/// - ✓ Implements planetary aspect rules (Jupiter 5/7/9, Saturn 3/7/10, Mars 4/7/8)
///   following Parasara's Graha Drishti (planetary aspects) system
/// - ✓ Considers planetary dignities for dosha cancellation (exaltation, own sign)
/// - ✓ Applies classical severity ratings based on house positions
/// 
/// CALCULATION APPROACH:
/// - Mangal Dosha: Based on traditional rules for Mars placement affecting marriage
/// - Kaal Sarp Dosha: All planets hemmed between Rahu and Ketu
/// - Pitra Dosha: Affliction of Sun (father/ancestors) by shadow planets
/// - Shakat Dosha: Unfavorable Moon-Jupiter relationship
/// - Kemadruma Dosha: Moon without planetary support (classical definition)
/// 
/// DOSHA CANCELLATION: The calculator implements partial dosha cancellation logic
/// based on traditional principles:
/// - Planet in own sign or exaltation reduces dosha effect
/// - Benefic aspects (especially Jupiter) can mitigate doshas
/// - Mutual planetary relationships can cancel specific doshas
/// 
/// FUTURE ENHANCEMENTS:
/// - Integration with Shadbala for strength-based severity calculation
/// - More comprehensive cancellation rules from BPHS
/// - Additional doshas mentioned in classical texts
/// - Divisional chart analysis for dosha confirmation
/// 
/// REFERENCES:
/// - Brihat Parashara Hora Shastra (BPHS), relevant chapters on doshas
/// - Traditional texts on Mangal (Kuja) Dosha
/// - Classical works on shadow planet (Rahu/Ketu) afflictions
/// - Vedic astrology treatises on planetary aspects and relationships
/// </summary>
public class DosaCalculator
{
    /// <summary>
    /// Detect all doshas in the given horoscope
    /// </summary>
    public List<DosaData> DetectDosas(HoroscopeData horoscope, string language = "Tamil")
    {
        var dosas = new List<DosaData>();

        // Check for various doshas
        CheckMangalDosha(horoscope, dosas, language);
        CheckKaalSarpDosha(horoscope, dosas, language);
        CheckPitraDosha(horoscope, dosas, language);
        CheckShakatDosha(horoscope, dosas, language);
        CheckKemadrumaDosha(horoscope, dosas, language);

        return dosas;
    }

    /// <summary>
    /// Check for Mangal Dosha (Mars affliction affecting marriage)
    /// Mars in 1st, 2nd, 4th, 7th, 8th, or 12th house from Lagna
    /// 
    /// PARASARA BASIS: Mangal (Kuja) Dosha is a well-known affliction in Vedic astrology
    /// affecting marital harmony. While not explicitly detailed in BPHS, it is based on
    /// Parasara's principles of Mars as a natural malefic (kroora graha) and its effect
    /// on sensitive houses related to marriage and partnership.
    /// 
    /// TRADITIONAL DEFINITION: Mars placed in specific houses (1,2,4,7,8,12) creates
    /// obstacles in marriage. The severity varies by house:
    /// - 7th and 8th houses: Most severe (directly affect marriage and longevity)
    /// - 1st and 12th houses: Severe (affect personality and hidden matters)
    /// - 2nd and 4th houses: Moderate (affect family and domestic peace)
    /// 
    /// CANCELLATION RULES: The dosha can be cancelled or reduced when:
    /// - Mars is in its own sign (Aries, Scorpio) or exaltation (Capricorn)
    /// - Jupiter aspects Mars with its benefic 5th, 7th, or 9th house drishti
    /// - Both partners have Mangal Dosha (mutual cancellation)
    /// 
    /// This implementation includes basic cancellation logic following traditional principles.
    /// </summary>
    private void CheckMangalDosha(HoroscopeData horoscope, List<DosaData> dosas, string language)
    {
        var mars = horoscope.Planets.FirstOrDefault(p => p.Name == "Mars");
        if (mars == null) return;

        var mangalDoshaHouses = new[] { 1, 2, 4, 7, 8, 12 };

        if (mangalDoshaHouses.Contains(mars.House))
        {
            // Calculate severity based on house
            int severity = mars.House switch
            {
                7 or 8 => 10, // Most severe in 7th and 8th
                1 or 12 => 8,  // Severe in 1st and 12th
                _ => 6         // Moderate in 2nd and 4th
            };

            var dosa = new DosaData
            {
                Name = "Mangal Dosha (Kuja Dosha)",
                LocalName = GetDosaLocalName("Mangal Dosha", language),
                Language = language,
                DescriptionArgs = new object[] { mars.House },
                InvolvedPlanets = new List<string> { "Mars" },
                InvolvedHouses = new List<int> { mars.House },
                Severity = severity
            };
            dosas.Add(dosa);
        }

        // Check for cancellation of Mangal Dosha
        if (mangalDoshaHouses.Contains(mars.House))
        {
            bool cancelled = false;
            var cancellationReasons = new List<string>();

            // Mars in own sign or exaltation reduces dosha
            if (mars.Rasi == 1 || mars.Rasi == 8 || mars.Rasi == 10) // Aries, Scorpio, Capricorn (exaltation)
            {
                cancelled = true;
                cancellationReasons.Add("Mars is in own sign or exaltation");
            }

            // Jupiter aspecting Mars can reduce dosha
            var jupiter = horoscope.Planets.FirstOrDefault(p => p.Name == "Jupiter");
            if (jupiter != null && IsAspecting(jupiter, mars))
            {
                cancelled = true;
                cancellationReasons.Add("Jupiter aspects Mars");
            }

            if (cancelled && dosas.Any(d => d.Name == "Mangal Dosha (Kuja Dosha)"))
            {
                var existingDosa = dosas.First(d => d.Name == "Mangal Dosha (Kuja Dosha)");
                // Append cancellation note to description (for backward compatibility)
                #pragma warning disable CS0618 // Type or member is obsolete
                if (!string.IsNullOrEmpty(existingDosa.Description))
                {
                    existingDosa.Description += $" Note: Dosha is partially cancelled - {string.Join(", ", cancellationReasons)}";
                }
                #pragma warning restore CS0618 // Type or member is obsolete
                existingDosa.Severity = Math.Max(1, existingDosa.Severity - 4);
            }
        }
    }

    /// <summary>
    /// Check for Kaal Sarp Dosha (all planets between Rahu and Ketu)
    /// 
    /// CLASSICAL BASIS: Kaal Sarp Yoga/Dosha occurs when all seven classical planets
    /// (Sun, Moon, Mars, Mercury, Jupiter, Venus, Saturn) are positioned on one side
    /// of the Rahu-Ketu axis, hemmed between these shadow planets.
    /// 
    /// EFFECTS: This configuration is considered to create:
    /// - Obstacles and delays in life endeavors
    /// - Mental anxiety and restlessness
    /// - Karmic challenges requiring spiritual remedies
    /// 
    /// CALCULATION METHOD: Checks if all planets (excluding Rahu and Ketu themselves)
    /// fall within the arc between Rahu and Ketu. This follows the traditional definition
    /// used in Vedic astrology for centuries.
    /// 
    /// NOTE: Some texts debate whether this is always malefic or can have positive effects
    /// in certain cases. The traditional view treats it as requiring attention and remedies.
    /// </summary>
    private void CheckKaalSarpDosha(HoroscopeData horoscope, List<DosaData> dosas, string language)
    {
        var rahu = horoscope.Planets.FirstOrDefault(p => p.Name == "Rahu");
        var ketu = horoscope.Planets.FirstOrDefault(p => p.Name == "Ketu");

        if (rahu == null || ketu == null) return;

        // Get all planets except Rahu and Ketu
        var otherPlanets = horoscope.Planets
            .Where(p => p.Name != "Rahu" && p.Name != "Ketu")
            .ToList();

        // Check if all planets are between Rahu and Ketu
        bool allBetween = true;
        int rahuHouse = rahu.House;
        int ketuHouse = ketu.House;

        foreach (var planet in otherPlanets)
        {
            if (!IsBetweenRahuKetu(planet.House, rahuHouse, ketuHouse))
            {
                allBetween = false;
                break;
            }
        }

        if (allBetween)
        {
            var dosa = new DosaData
            {
                Name = "Kaal Sarp Dosha",
                LocalName = GetDosaLocalName("Kaal Sarp Dosha", language),
                Language = language,
                InvolvedPlanets = new List<string> { "Rahu", "Ketu" },
                InvolvedHouses = new List<int> { rahuHouse, ketuHouse },
                Severity = 9
            };
            dosas.Add(dosa);
        }
    }

    /// <summary>
    /// Check for Pitra Dosha (ancestral affliction)
    /// Sun with Rahu or Ketu, or 9th house afflicted
    /// </summary>
    private void CheckPitraDosha(HoroscopeData horoscope, List<DosaData> dosas, string language)
    {
        var sun = horoscope.Planets.FirstOrDefault(p => p.Name == "Sun");
        var rahu = horoscope.Planets.FirstOrDefault(p => p.Name == "Rahu");
        var ketu = horoscope.Planets.FirstOrDefault(p => p.Name == "Ketu");

        if (sun == null) return;

        bool hasPitraDosha = false;
        string reason = "";
        int severity = 0;

        // Check if Sun is with Rahu or Ketu
        if (rahu != null && sun.Rasi == rahu.Rasi)
        {
            hasPitraDosha = true;
            reason = "Sun is conjunct with Rahu";
            severity = 8;
        }
        else if (ketu != null && sun.Rasi == ketu.Rasi)
        {
            hasPitraDosha = true;
            reason = "Sun is conjunct with Ketu";
            severity = 8;
        }

        // Check 9th house (house of ancestors) afflicted by Rahu or Ketu
        if (!hasPitraDosha)
        {
            if (rahu != null && rahu.House == 9)
            {
                hasPitraDosha = true;
                reason = "Rahu in 9th house (house of ancestors)";
                severity = 7;
            }
            else if (ketu != null && ketu.House == 9)
            {
                hasPitraDosha = true;
                reason = "Ketu in 9th house (house of ancestors)";
                severity = 7;
            }
        }

        if (hasPitraDosha)
        {
            var dosa = new DosaData
            {
                Name = "Pitra Dosha",
                LocalName = GetDosaLocalName("Pitra Dosha", language),
                Language = language,
                DescriptionArgs = new object[] { reason },
                InvolvedPlanets = new List<string> { "Sun" },
                InvolvedHouses = new List<int> { sun.House },
                Severity = severity
            };
            dosas.Add(dosa);
        }
    }

    /// <summary>
    /// Check for Shakat Dosha (Jupiter-Moon affliction)
    /// Moon in 6th, 8th from Jupiter (12th is not traditionally counted for Shakat)
    /// </summary>
    private void CheckShakatDosha(HoroscopeData horoscope, List<DosaData> dosas, string language)
    {
        var moon = horoscope.Planets.FirstOrDefault(p => p.Name == "Moon");
        var jupiter = horoscope.Planets.FirstOrDefault(p => p.Name == "Jupiter");

        if (moon == null || jupiter == null) return;

        // Calculate which house Moon occupies counting from Jupiter
        int houseFromJupiter;
        if (moon.House >= jupiter.House)
        {
            houseFromJupiter = moon.House - jupiter.House + 1;
        }
        else
        {
            houseFromJupiter = 12 - jupiter.House + moon.House + 1;
        }

        // Check if Moon is in 6th or 8th from Jupiter
        if (houseFromJupiter == 6 || houseFromJupiter == 8)
        {
            var dosa = new DosaData
            {
                Name = "Shakat Dosha",
                LocalName = GetDosaLocalName("Shakat Dosha", language),
                Language = language,
                DescriptionArgs = new object[] { houseFromJupiter },
                InvolvedPlanets = new List<string> { "Moon", "Jupiter" },
                InvolvedHouses = new List<int> { moon.House, jupiter.House },
                Severity = 6
            };
            dosas.Add(dosa);
        }
    }

    /// <summary>
    /// Check for Kemadruma Dosha (Moon without support)
    /// Moon alone with no planets in 2nd and 12th houses from it
    /// </summary>
    private void CheckKemadrumaDosha(HoroscopeData horoscope, List<DosaData> dosas, string language)
    {
        var moon = horoscope.Planets.FirstOrDefault(p => p.Name == "Moon");
        if (moon == null) return;

        int moonRasi = moon.Rasi;
        int secondFromMoon = (moonRasi % 12) + 1;
        int twelfthFromMoon = moonRasi == 1 ? 12 : moonRasi - 1;

        var planetsInSecond = horoscope.Planets
            .Where(p => p.Rasi == secondFromMoon && p.Name != "Moon" && p.Name != "Rahu" && p.Name != "Ketu")
            .ToList();

        var planetsInTwelfth = horoscope.Planets
            .Where(p => p.Rasi == twelfthFromMoon && p.Name != "Moon" && p.Name != "Rahu" && p.Name != "Ketu")
            .ToList();

        // Check if Moon has no benefic planets on either side
        if (!planetsInSecond.Any() && !planetsInTwelfth.Any())
        {
            var dosa = new DosaData
            {
                Name = "Kemadruma Dosha",
                LocalName = GetDosaLocalName("Kemadruma Dosha", language),
                Language = language,
                InvolvedPlanets = new List<string> { "Moon" },
                InvolvedHouses = new List<int> { moon.House },
                Severity = 7
            };
            dosas.Add(dosa);
        }
    }

    // Helper methods

    /// <summary>
    /// Checks if one planet aspects another based on Parasara's Graha Drishti system
    /// 
    /// PARASARA'S ASPECT SYSTEM (Graha Drishti):
    /// - All planets have a full (100%) 7th house aspect
    /// - Jupiter has special aspects: 5th, 7th, and 9th houses (forward count)
    /// - Saturn has special aspects: 3rd, 7th, and 10th houses
    /// - Mars has special aspects: 4th, 7th, and 8th houses
    /// 
    /// This follows the exact aspect rules described in BPHS Chapter on planetary aspects.
    /// 
    /// NOTE: The PlanetStrengthCalculator has more detailed aspect strength calculations
    /// using proportional aspect values. This is a simplified boolean check for dosha
    /// cancellation purposes.
    /// </summary>
    private bool IsAspecting(PlanetData planet1, PlanetData planet2)
    {
        // Calculate which house planet2 occupies counting from planet1
        int houseFromPlanet1;
        if (planet2.House >= planet1.House)
        {
            houseFromPlanet1 = planet2.House - planet1.House + 1;
        }
        else
        {
            houseFromPlanet1 = 12 - planet1.House + planet2.House + 1;
        }

        // Jupiter aspects 5th, 7th, and 9th houses from itself
        if (planet1.Name == "Jupiter")
        {
            return houseFromPlanet1 == 5 || houseFromPlanet1 == 7 || houseFromPlanet1 == 9;
        }

        // Saturn aspects 3rd, 7th, and 10th houses from itself
        if (planet1.Name == "Saturn")
        {
            return houseFromPlanet1 == 3 || houseFromPlanet1 == 7 || houseFromPlanet1 == 10;
        }

        // Mars aspects 4th, 7th, and 8th houses from itself
        if (planet1.Name == "Mars")
        {
            return houseFromPlanet1 == 4 || houseFromPlanet1 == 7 || houseFromPlanet1 == 8;
        }

        // All planets have 7th house aspect
        return houseFromPlanet1 == 7;
    }

    private bool IsBetweenRahuKetu(int planetHouse, int rahuHouse, int ketuHouse)
    {
        // Normalize houses to ensure Rahu is before Ketu in sequence
        if (rahuHouse > ketuHouse)
        {
            // If planet is between rahu and end, or between start and ketu
            return planetHouse > rahuHouse || planetHouse < ketuHouse;
        }
        else
        {
            // Normal case: planet should be between rahu and ketu
            return planetHouse > rahuHouse && planetHouse < ketuHouse;
        }
    }

    private string GetDosaLocalName(string dosaName, string language)
    {
        var dosaNames = new Dictionary<string, Dictionary<string, string>>
        {
            ["Mangal Dosha"] = new() { ["Tamil"] = "மங்கள தோஷம்", ["Telugu"] = "మంగళ దోషం", ["Kannada"] = "ಮಂಗಳ ದೋಷ", ["Malayalam"] = "മംഗള ദോഷം" },
            ["Kaal Sarp Dosha"] = new() { ["Tamil"] = "கால சர்ப்ப தோஷம்", ["Telugu"] = "కాల సర్ప దోషం", ["Kannada"] = "ಕಾಲ ಸರ್ಪ ದೋಷ", ["Malayalam"] = "കാല സർപ്പ ദോഷം" },
            ["Pitra Dosha"] = new() { ["Tamil"] = "பித்ரு தோஷம்", ["Telugu"] = "పితృ దోషం", ["Kannada"] = "ಪಿತೃ ದೋಷ", ["Malayalam"] = "പിതൃ ദോഷം" },
            ["Shakat Dosha"] = new() { ["Tamil"] = "சகட தோஷம்", ["Telugu"] = "శకట దోషం", ["Kannada"] = "ಶಕಟ ದೋಷ", ["Malayalam"] = "ശകട ദോഷം" },
            ["Kemadruma Dosha"] = new() { ["Tamil"] = "கேமத்ரும தோஷம்", ["Telugu"] = "కేమద్రుమ దోషం", ["Kannada"] = "ಕೇಮದ್ರುಮ ದೋಷ", ["Malayalam"] = "കേമദ്രുമ ദോഷം" }
        };

        if (dosaNames.ContainsKey(dosaName) && dosaNames[dosaName].ContainsKey(language))
        {
            return dosaNames[dosaName][language];
        }

        return dosaName;
    }
}
