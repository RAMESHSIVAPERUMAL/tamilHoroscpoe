using TamilHoroscope.Core.Data;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Core.Calculators;

/// <summary>
/// Calculator for detecting astrological doshas in a horoscope
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
                Description = $"Mars in {mars.House}th house. May cause delays or difficulties in marriage and marital harmony.",
                InvolvedPlanets = new List<string> { "Mars" },
                InvolvedHouses = new List<int> { mars.House },
                Severity = severity,
                Remedies = new List<string>
                {
                    "Chant Hanuman Chalisa regularly",
                    "Worship Lord Hanuman on Tuesdays",
                    "Recite Mangal mantra: Om Angarakaya Namaha",
                    "Donate red lentils on Tuesdays",
                    "Wear red coral gemstone after consultation"
                }
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
                existingDosa.Description += $" Note: Dosha is partially cancelled - {string.Join(", ", cancellationReasons)}";
                existingDosa.Severity = Math.Max(1, existingDosa.Severity - 4);
            }
        }
    }

    /// <summary>
    /// Check for Kaal Sarp Dosha (all planets between Rahu and Ketu)
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
                Description = "All planets are positioned between Rahu and Ketu. May cause obstacles, delays, and mental anxiety.",
                InvolvedPlanets = new List<string> { "Rahu", "Ketu" },
                InvolvedHouses = new List<int> { rahuHouse, ketuHouse },
                Severity = 9,
                Remedies = new List<string>
                {
                    "Visit Kaal Sarp Dosha temples",
                    "Perform Rahu-Ketu puja on special days",
                    "Recite Maha Mrityunjaya mantra",
                    "Donate to snake sanctuaries",
                    "Observe fasts on Nag Panchami"
                }
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
                Description = $"Ancestral affliction detected: {reason}. May cause family issues and obstacles in life.",
                InvolvedPlanets = new List<string> { "Sun" },
                InvolvedHouses = new List<int> { sun.House },
                Severity = severity,
                Remedies = new List<string>
                {
                    "Perform Shraddha rituals for ancestors",
                    "Feed Brahmins on Amavasya",
                    "Donate food on Saturdays",
                    "Recite Gayatri mantra daily",
                    "Plant a Peepal tree and water it regularly"
                }
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
                Description = $"Moon in {houseFromJupiter}th house from Jupiter. May cause financial instability and ups and downs in life.",
                InvolvedPlanets = new List<string> { "Moon", "Jupiter" },
                InvolvedHouses = new List<int> { moon.House, jupiter.House },
                Severity = 6,
                Remedies = new List<string>
                {
                    "Worship Lord Vishnu on Thursdays",
                    "Recite Guru mantra: Om Gurave Namaha",
                    "Donate yellow items on Thursdays",
                    "Wear yellow sapphire after consultation",
                    "Feed cows regularly"
                }
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
                Description = "Moon has no planets in adjacent houses. May cause poverty, mental stress, and lack of support.",
                InvolvedPlanets = new List<string> { "Moon" },
                InvolvedHouses = new List<int> { moon.House },
                Severity = 7,
                Remedies = new List<string>
                {
                    "Wear pearl or moonstone after consultation",
                    "Recite Chandra mantra on Mondays",
                    "Donate white items on Mondays",
                    "Worship Lord Shiva regularly",
                    "Maintain good relationship with mother"
                }
            };
            dosas.Add(dosa);
        }
    }

    // Helper methods

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
