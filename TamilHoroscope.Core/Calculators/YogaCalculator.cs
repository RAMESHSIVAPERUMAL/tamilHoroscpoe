using TamilHoroscope.Core.Data;
using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Core.Calculators;

/// <summary>
/// Calculator for detecting astrological yogas in a horoscope
/// 
/// METHODOLOGY: This calculator implements classical Vedic astrology yogas as described in
/// traditional texts including Brihat Parashara Hora Shastra (BPHS). The yogas detected are
/// based on Parasara's foundational principles of:
/// - Rasi-based planetary positions (sign placements)
/// - House-based planetary lordships and relationships
/// - Kendra (angular houses: 1,4,7,10) and Trikona (trinal houses: 1,5,9) principles
/// - Planetary conjunctions and relative positions
/// 
/// PARASARA METHOD ALIGNMENT:
/// - ✓ Uses classical yoga definitions from BPHS and related texts
/// - ✓ Implements rasi-based counting from Moon and Lagna as per Parasara
/// - ✓ Considers planetary dignities (own signs, exaltation) for Mahapurusha yogas
/// - ✓ Applies house lordship rules for Raja and Dhana yogas
/// 
/// FUTURE ENHANCEMENTS:
/// - Integration with Shadbala (6-fold strength) for yoga strength validation
/// - Divisional chart (D-9 Navamsa) confirmation of yoga potency
/// - Aspect strength calculations using Parasara's Drishti system
/// - Additional yogas from BPHS chapters on planetary combinations
/// 
/// REFERENCES:
/// - Brihat Parashara Hora Shastra (BPHS), Chapters on Yogas
/// - Classical texts on Pancha Mahapurusha Yogas
/// - Traditional Vedic astrology treatises on Chandra (Moon) yogas
/// </summary>
public class YogaCalculator
{
    /// <summary>
    /// Detect all yogas in the given horoscope
    /// </summary>
    public List<YogaData> DetectYogas(HoroscopeData horoscope, string language = "Tamil")
    {
        var yogas = new List<YogaData>();

        // Get planet positions by house
        var planetsByHouse = GetPlanetsByHouse(horoscope.Planets);
        var planetsByRasi = GetPlanetsByRasi(horoscope.Planets);

        // Check for various yogas
        CheckGajakesariYoga(horoscope, planetsByRasi, yogas, language);
        CheckRajaYoga(horoscope, planetsByHouse, yogas, language);
        CheckDhanaYoga(horoscope, planetsByHouse, yogas, language);
        CheckSunaphaYoga(horoscope, planetsByRasi, yogas, language);
        CheckAnaphaYoga(horoscope, planetsByRasi, yogas, language);
        CheckDurdhuraYoga(horoscope, planetsByRasi, yogas, language);
        CheckBudhaAdityaYoga(horoscope, planetsByRasi, yogas, language);
        CheckMahapurushYogas(horoscope, planetsByHouse, planetsByRasi, yogas, language);

        return yogas;
    }

    /// <summary>
    /// Check for Gajakesari Yoga (Jupiter-Moon combination)
    /// Jupiter in kendra (1,4,7,10) from Moon
    /// 
    /// PARASARA BASIS: This yoga is described in BPHS as a highly auspicious combination.
    /// When Jupiter (Guru) occupies a kendra (angular house) from the Moon (Chandra),
    /// it creates this beneficial yoga that brings wisdom, wealth, fame, and good character.
    /// 
    /// CALCULATION METHOD: Uses rasi-based counting from Moon's position to determine
    /// Jupiter's placement in 1st, 4th, 7th, or 10th house from Moon, following traditional
    /// Vedic astrology methodology.
    /// </summary>
    private void CheckGajakesariYoga(HoroscopeData horoscope, Dictionary<int, List<PlanetData>> planetsByRasi, List<YogaData> yogas, string language)
    {
        var moon = horoscope.Planets.FirstOrDefault(p => p.Name == "Moon");
        var jupiter = horoscope.Planets.FirstOrDefault(p => p.Name == "Jupiter");

        if (moon == null || jupiter == null) return;

        // Calculate which house Jupiter occupies counting from Moon's rasi
        int houseFromMoon;
        if (jupiter.Rasi >= moon.Rasi)
        {
            houseFromMoon = jupiter.Rasi - moon.Rasi + 1;
        }
        else
        {
            houseFromMoon = 12 - moon.Rasi + jupiter.Rasi + 1;
        }

        // Check if Jupiter is in kendra from Moon (1st, 4th, 7th, 10th house)
        if (houseFromMoon == 1 || houseFromMoon == 4 || houseFromMoon == 7 || houseFromMoon == 10)
        {
            var yoga = new YogaData
            {
                Name = "Gajakesari Yoga",
                LocalName = GetYogaLocalName("Gajakesari Yoga", language),
                Description = "Jupiter in kendra from Moon. Brings wisdom, wealth, fame, and good character.",
                InvolvedPlanets = new List<string> { "Jupiter", "Moon" },
                InvolvedHouses = new List<int> { moon.House, jupiter.House },
                IsBeneficial = true,
                Strength = houseFromMoon == 1 ? 10 : 8
            };
            yogas.Add(yoga);
        }
    }

    /// <summary>
    /// Check for Raja Yoga (association of lords of kendras and trikonas)
    /// 
    /// PARASARA BASIS: Raja Yogas are extensively described in BPHS as combinations that
    /// confer power, authority, and royal status. The fundamental principle is that when
    /// the lords of angular houses (kendras: 1,4,7,10) and trinal houses (trikonas: 1,5,9)
    /// form relationships, they create yogas for leadership and success.
    /// 
    /// CALCULATION METHOD: Evaluates each planet's lordship of houses from Lagna and
    /// checks if it simultaneously rules both kendra and trikona houses. Uses classical
    /// planetary lordship rules (e.g., Mars rules Aries and Scorpio).
    /// 
    /// NOTE: This is a simplified implementation. Full Parasara method would also consider:
    /// - Conjunction or mutual aspect between kendra and trikona lords
    /// - Strength of the planets involved (Shadbala)
    /// - Position in divisional charts for confirmation
    /// </summary>
    private void CheckRajaYoga(HoroscopeData horoscope, Dictionary<int, List<PlanetData>> planetsByHouse, List<YogaData> yogas, string language)
    {
        // Kendra houses: 1, 4, 7, 10
        // Trikona houses: 1, 5, 9
        var kendraHouses = new[] { 1, 4, 7, 10 };
        var trikonaHouses = new[] { 1, 5, 9 };

        var lagnaRasi = horoscope.LagnaRasi;

        foreach (var planet in horoscope.Planets.Where(p => p.Name != "Rahu" && p.Name != "Ketu"))
        {
            // Get the houses ruled by this planet from Lagna
            var ruledHouses = GetRuledHouses(planet.Name, lagnaRasi);

            bool rulesKendra = ruledHouses.Any(h => kendraHouses.Contains(h));
            bool rulesTrikona = ruledHouses.Any(h => trikonaHouses.Contains(h));

            if (rulesKendra && rulesTrikona)
            {
                var yoga = new YogaData
                {
                    Name = "Raja Yoga",
                    LocalName = GetYogaLocalName("Raja Yoga", language),
                    Description = $"{planet.Name} rules both kendra and trikona houses. Brings power, authority, and success.",
                    InvolvedPlanets = new List<string> { planet.Name },
                    InvolvedHouses = ruledHouses,
                    IsBeneficial = true,
                    Strength = 9
                };
                yogas.Add(yoga);
            }
        }
    }

    /// <summary>
    /// Check for Dhana Yoga (wealth combination)
    /// Lord of 2nd or 11th in connection with 9th or 5th
    /// </summary>
    private void CheckDhanaYoga(HoroscopeData horoscope, Dictionary<int, List<PlanetData>> planetsByHouse, List<YogaData> yogas, string language)
    {
        var lagnaRasi = horoscope.LagnaRasi;
        var wealthHouses = new[] { 2, 11 }; // Houses of wealth
        var fortuneHouses = new[] { 5, 9 }; // Houses of fortune

        foreach (var planet in horoscope.Planets.Where(p => p.Name != "Rahu" && p.Name != "Ketu"))
        {
            var ruledHouses = GetRuledHouses(planet.Name, lagnaRasi);

            bool rulesWealth = ruledHouses.Any(h => wealthHouses.Contains(h));
            bool rulesFortune = ruledHouses.Any(h => fortuneHouses.Contains(h));

            if (rulesWealth && rulesFortune)
            {
                var yoga = new YogaData
                {
                    Name = "Dhana Yoga",
                    LocalName = GetYogaLocalName("Dhana Yoga", language),
                    Description = $"{planet.Name} creates wealth yoga. Brings prosperity and financial gains.",
                    InvolvedPlanets = new List<string> { planet.Name },
                    InvolvedHouses = ruledHouses,
                    IsBeneficial = true,
                    Strength = 8
                };
                yogas.Add(yoga);
            }
        }
    }

    /// <summary>
    /// Check for Sunapha Yoga (planets in 2nd from Moon)
    /// </summary>
    private void CheckSunaphaYoga(HoroscopeData horoscope, Dictionary<int, List<PlanetData>> planetsByRasi, List<YogaData> yogas, string language)
    {
        var moon = horoscope.Planets.FirstOrDefault(p => p.Name == "Moon");
        if (moon == null) return;

        int secondFromMoon = (moon.Rasi % 12) + 1;
        
        if (planetsByRasi.ContainsKey(secondFromMoon))
        {
            var planetsInSecond = planetsByRasi[secondFromMoon]
                .Where(p => p.Name != "Sun" && p.Name != "Moon" && p.Name != "Rahu" && p.Name != "Ketu")
                .ToList();

            if (planetsInSecond.Any())
            {
                var yoga = new YogaData
                {
                    Name = "Sunapha Yoga",
                    LocalName = GetYogaLocalName("Sunapha Yoga", language),
                    Description = "Planet(s) in 2nd house from Moon. Brings wealth, intelligence, and good reputation.",
                    InvolvedPlanets = planetsInSecond.Select(p => p.Name).ToList(),
                    InvolvedHouses = planetsInSecond.Select(p => p.House).Distinct().ToList(),
                    IsBeneficial = true,
                    Strength = 7
                };
                yogas.Add(yoga);
            }
        }
    }

    /// <summary>
    /// Check for Anapha Yoga (planets in 12th from Moon)
    /// </summary>
    private void CheckAnaphaYoga(HoroscopeData horoscope, Dictionary<int, List<PlanetData>> planetsByRasi, List<YogaData> yogas, string language)
    {
        var moon = horoscope.Planets.FirstOrDefault(p => p.Name == "Moon");
        if (moon == null) return;

        int twelfthFromMoon = moon.Rasi == 1 ? 12 : moon.Rasi - 1;
        
        if (planetsByRasi.ContainsKey(twelfthFromMoon))
        {
            var planetsInTwelfth = planetsByRasi[twelfthFromMoon]
                .Where(p => p.Name != "Sun" && p.Name != "Moon" && p.Name != "Rahu" && p.Name != "Ketu")
                .ToList();

            if (planetsInTwelfth.Any())
            {
                var yoga = new YogaData
                {
                    Name = "Anapha Yoga",
                    LocalName = GetYogaLocalName("Anapha Yoga", language),
                    Description = "Planet(s) in 12th house from Moon. Brings fame, good health, and spiritual inclination.",
                    InvolvedPlanets = planetsInTwelfth.Select(p => p.Name).ToList(),
                    InvolvedHouses = planetsInTwelfth.Select(p => p.House).Distinct().ToList(),
                    IsBeneficial = true,
                    Strength = 7
                };
                yogas.Add(yoga);
            }
        }
    }

    /// <summary>
    /// Check for Durdhura Yoga (planets on both sides of Moon)
    /// </summary>
    private void CheckDurdhuraYoga(HoroscopeData horoscope, Dictionary<int, List<PlanetData>> planetsByRasi, List<YogaData> yogas, string language)
    {
        var moon = horoscope.Planets.FirstOrDefault(p => p.Name == "Moon");
        if (moon == null) return;

        int secondFromMoon = (moon.Rasi % 12) + 1;
        int twelfthFromMoon = moon.Rasi == 1 ? 12 : moon.Rasi - 1;

        bool hasSecond = planetsByRasi.ContainsKey(secondFromMoon) &&
            planetsByRasi[secondFromMoon].Any(p => p.Name != "Sun" && p.Name != "Moon" && p.Name != "Rahu" && p.Name != "Ketu");
        
        bool hasTwelfth = planetsByRasi.ContainsKey(twelfthFromMoon) &&
            planetsByRasi[twelfthFromMoon].Any(p => p.Name != "Sun" && p.Name != "Moon" && p.Name != "Rahu" && p.Name != "Ketu");

        if (hasSecond && hasTwelfth)
        {
            var involvedPlanets = new List<string>();
            if (planetsByRasi.ContainsKey(secondFromMoon))
                involvedPlanets.AddRange(planetsByRasi[secondFromMoon].Where(p => p.Name != "Sun" && p.Name != "Moon").Select(p => p.Name));
            if (planetsByRasi.ContainsKey(twelfthFromMoon))
                involvedPlanets.AddRange(planetsByRasi[twelfthFromMoon].Where(p => p.Name != "Sun" && p.Name != "Moon").Select(p => p.Name));

            var yoga = new YogaData
            {
                Name = "Durdhura Yoga",
                LocalName = GetYogaLocalName("Durdhura Yoga", language),
                Description = "Planets on both sides of Moon. Brings prosperity, conveyances, and comforts.",
                InvolvedPlanets = involvedPlanets,
                InvolvedHouses = new List<int>(),
                IsBeneficial = true,
                Strength = 8
            };
            yogas.Add(yoga);
        }
    }

    /// <summary>
    /// Check for Budha Aditya Yoga (Sun-Mercury conjunction)
    /// </summary>
    private void CheckBudhaAdityaYoga(HoroscopeData horoscope, Dictionary<int, List<PlanetData>> planetsByRasi, List<YogaData> yogas, string language)
    {
        var sun = horoscope.Planets.FirstOrDefault(p => p.Name == "Sun");
        var mercury = horoscope.Planets.FirstOrDefault(p => p.Name == "Mercury");

        if (sun == null || mercury == null) return;

        if (sun.Rasi == mercury.Rasi)
        {
            var yoga = new YogaData
            {
                Name = "Budha Aditya Yoga",
                LocalName = GetYogaLocalName("Budha Aditya Yoga", language),
                Description = "Sun and Mercury conjunction. Brings intelligence, communication skills, and analytical ability.",
                InvolvedPlanets = new List<string> { "Sun", "Mercury" },
                InvolvedHouses = new List<int> { sun.House },
                IsBeneficial = true,
                Strength = 7
            };
            yogas.Add(yoga);
        }
    }

    /// <summary>
    /// Check for Mahapurusha Yogas (5 great person yogas)
    /// 
    /// PARASARA BASIS: The Pancha Mahapurusha Yogas are one of the most celebrated yoga
    /// combinations in BPHS. These yogas are formed when the five tara grahas (Mars, Mercury,
    /// Jupiter, Venus, Saturn) occupy their own signs or exaltation signs in kendra houses.
    /// Each yoga bestows specific noble qualities:
    /// 
    /// - Hamsa Yoga (Jupiter): Wisdom, spirituality, righteousness
    /// - Malavya Yoga (Venus): Beauty, luxury, artistic talents
    /// - Sasa Yoga (Saturn): Discipline, authority, longevity
    /// - Ruchaka Yoga (Mars): Courage, leadership, military prowess
    /// - Bhadra Yoga (Mercury): Intelligence, communication, business acumen
    /// 
    /// CALCULATION METHOD: Checks if each qualifying planet is in:
    /// 1. A kendra house (1,4,7,10) from Lagna
    /// 2. Its own sign (Moolatrikona) or exaltation sign
    /// 
    /// This follows the classical Parasara definition precisely.
    /// </summary>
    private void CheckMahapurushYogas(HoroscopeData horoscope, Dictionary<int, List<PlanetData>> planetsByHouse, Dictionary<int, List<PlanetData>> planetsByRasi, List<YogaData> yogas, string language)
    {
        // Hamsa Yoga: Jupiter in kendra in own sign or exaltation
        CheckHamsaYoga(horoscope, yogas, language);
        
        // Malavya Yoga: Venus in kendra in own sign or exaltation
        CheckMalavyaYoga(horoscope, yogas, language);
        
        // Sasa Yoga: Saturn in kendra in own sign or exaltation
        CheckSasaYoga(horoscope, yogas, language);
        
        // Ruchaka Yoga: Mars in kendra in own sign or exaltation
        CheckRuchakaYoga(horoscope, yogas, language);
        
        // Bhadra Yoga: Mercury in kendra in own sign or exaltation
        CheckBhadraYoga(horoscope, yogas, language);
    }

    private void CheckHamsaYoga(HoroscopeData horoscope, List<YogaData> yogas, string language)
    {
        var jupiter = horoscope.Planets.FirstOrDefault(p => p.Name == "Jupiter");
        if (jupiter == null) return;

        var kendraHouses = new[] { 1, 4, 7, 10 };
        var jupiterOwnSigns = new[] { 9, 12 }; // Sagittarius, Pisces
        int jupiterExaltation = 4; // Cancer

        if (kendraHouses.Contains(jupiter.House) && 
            (jupiterOwnSigns.Contains(jupiter.Rasi) || jupiter.Rasi == jupiterExaltation))
        {
            var yoga = new YogaData
            {
                Name = "Hamsa Yoga",
                LocalName = GetYogaLocalName("Hamsa Yoga", language),
                Description = "Jupiter in kendra in own sign or exaltation. Brings wisdom, spirituality, and prosperity.",
                InvolvedPlanets = new List<string> { "Jupiter" },
                InvolvedHouses = new List<int> { jupiter.House },
                IsBeneficial = true,
                Strength = 9
            };
            yogas.Add(yoga);
        }
    }

    private void CheckMalavyaYoga(HoroscopeData horoscope, List<YogaData> yogas, string language)
    {
        var venus = horoscope.Planets.FirstOrDefault(p => p.Name == "Venus");
        if (venus == null) return;

        var kendraHouses = new[] { 1, 4, 7, 10 };
        var venusOwnSigns = new[] { 2, 7 }; // Taurus, Libra
        int venusExaltation = 12; // Pisces

        if (kendraHouses.Contains(venus.House) && 
            (venusOwnSigns.Contains(venus.Rasi) || venus.Rasi == venusExaltation))
        {
            var yoga = new YogaData
            {
                Name = "Malavya Yoga",
                LocalName = GetYogaLocalName("Malavya Yoga", language),
                Description = "Venus in kendra in own sign or exaltation. Brings beauty, luxury, and artistic talents.",
                InvolvedPlanets = new List<string> { "Venus" },
                InvolvedHouses = new List<int> { venus.House },
                IsBeneficial = true,
                Strength = 9
            };
            yogas.Add(yoga);
        }
    }

    private void CheckSasaYoga(HoroscopeData horoscope, List<YogaData> yogas, string language)
    {
        var saturn = horoscope.Planets.FirstOrDefault(p => p.Name == "Saturn");
        if (saturn == null) return;

        var kendraHouses = new[] { 1, 4, 7, 10 };
        var saturnOwnSigns = new[] { 10, 11 }; // Capricorn, Aquarius
        int saturnExaltation = 7; // Libra

        if (kendraHouses.Contains(saturn.House) && 
            (saturnOwnSigns.Contains(saturn.Rasi) || saturn.Rasi == saturnExaltation))
        {
            var yoga = new YogaData
            {
                Name = "Sasa Yoga",
                LocalName = GetYogaLocalName("Sasa Yoga", language),
                Description = "Saturn in kendra in own sign or exaltation. Brings discipline, longevity, and authority.",
                InvolvedPlanets = new List<string> { "Saturn" },
                InvolvedHouses = new List<int> { saturn.House },
                IsBeneficial = true,
                Strength = 9
            };
            yogas.Add(yoga);
        }
    }

    private void CheckRuchakaYoga(HoroscopeData horoscope, List<YogaData> yogas, string language)
    {
        var mars = horoscope.Planets.FirstOrDefault(p => p.Name == "Mars");
        if (mars == null) return;

        var kendraHouses = new[] { 1, 4, 7, 10 };
        var marsOwnSigns = new[] { 1, 8 }; // Aries, Scorpio
        int marsExaltation = 10; // Capricorn

        if (kendraHouses.Contains(mars.House) && 
            (marsOwnSigns.Contains(mars.Rasi) || mars.Rasi == marsExaltation))
        {
            var yoga = new YogaData
            {
                Name = "Ruchaka Yoga",
                LocalName = GetYogaLocalName("Ruchaka Yoga", language),
                Description = "Mars in kendra in own sign or exaltation. Brings courage, leadership, and military prowess.",
                InvolvedPlanets = new List<string> { "Mars" },
                InvolvedHouses = new List<int> { mars.House },
                IsBeneficial = true,
                Strength = 9
            };
            yogas.Add(yoga);
        }
    }

    private void CheckBhadraYoga(HoroscopeData horoscope, List<YogaData> yogas, string language)
    {
        var mercury = horoscope.Planets.FirstOrDefault(p => p.Name == "Mercury");
        if (mercury == null) return;

        var kendraHouses = new[] { 1, 4, 7, 10 };
        var mercuryOwnSigns = new[] { 3, 6 }; // Gemini, Virgo
        int mercuryExaltation = 6; // Virgo

        if (kendraHouses.Contains(mercury.House) && 
            (mercuryOwnSigns.Contains(mercury.Rasi) || mercury.Rasi == mercuryExaltation))
        {
            var yoga = new YogaData
            {
                Name = "Bhadra Yoga",
                LocalName = GetYogaLocalName("Bhadra Yoga", language),
                Description = "Mercury in kendra in own sign or exaltation. Brings intelligence, communication, and business acumen.",
                InvolvedPlanets = new List<string> { "Mercury" },
                InvolvedHouses = new List<int> { mercury.House },
                IsBeneficial = true,
                Strength = 9
            };
            yogas.Add(yoga);
        }
    }

    // Helper methods

    private Dictionary<int, List<PlanetData>> GetPlanetsByHouse(List<PlanetData> planets)
    {
        return planets.GroupBy(p => p.House).ToDictionary(g => g.Key, g => g.ToList());
    }

    private Dictionary<int, List<PlanetData>> GetPlanetsByRasi(List<PlanetData> planets)
    {
        return planets.GroupBy(p => p.Rasi).ToDictionary(g => g.Key, g => g.ToList());
    }

    private List<int> GetRuledHouses(string planetName, int lagnaRasi)
    {
        var houses = new List<int>();

        // Get signs ruled by planet
        var ruledSigns = GetRuledSigns(planetName);

        // Convert signs to houses from lagna
        foreach (var sign in ruledSigns)
        {
            int house = sign - lagnaRasi + 1;
            if (house <= 0) house += 12;
            houses.Add(house);
        }

        return houses;
    }

    private List<int> GetRuledSigns(string planetName)
    {
        return planetName switch
        {
            "Sun" => new List<int> { 5 },           // Leo
            "Moon" => new List<int> { 4 },          // Cancer
            "Mars" => new List<int> { 1, 8 },       // Aries, Scorpio
            "Mercury" => new List<int> { 3, 6 },    // Gemini, Virgo
            "Jupiter" => new List<int> { 9, 12 },   // Sagittarius, Pisces
            "Venus" => new List<int> { 2, 7 },      // Taurus, Libra
            "Saturn" => new List<int> { 10, 11 },   // Capricorn, Aquarius
            _ => new List<int>()
        };
    }

    private string GetYogaLocalName(string yogaName, string language)
    {
        // For now, return the same name. This will be expanded with multi-language support
        var yogaNames = new Dictionary<string, Dictionary<string, string>>
        {
            ["Gajakesari Yoga"] = new() { ["Tamil"] = "கஜகேசரி யோகம்", ["Telugu"] = "గజకేసరి యోగం", ["Kannada"] = "ಗಜಕೇಸರಿ ಯೋಗ", ["Malayalam"] = "ഗജകേസരി യോഗം" },
            ["Raja Yoga"] = new() { ["Tamil"] = "ராஜ யோகம்", ["Telugu"] = "రాజ యోగం", ["Kannada"] = "ರಾಜ ಯೋಗ", ["Malayalam"] = "രാജ യോഗം" },
            ["Dhana Yoga"] = new() { ["Tamil"] = "தன யோகம்", ["Telugu"] = "ధన యోగం", ["Kannada"] = "ಧನ ಯೋಗ", ["Malayalam"] = "ധന യോഗം" },
            ["Sunapha Yoga"] = new() { ["Tamil"] = "சுனபா யோகம்", ["Telugu"] = "సునఫ యోగం", ["Kannada"] = "ಸುನಫ ಯೋಗ", ["Malayalam"] = "സുനഫ യോഗം" },
            ["Anapha Yoga"] = new() { ["Tamil"] = "அனபா யோகம்", ["Telugu"] = "అనఫ యోగం", ["Kannada"] = "ಅನಫ ಯೋಗ", ["Malayalam"] = "അനഫ യോഗം" },
            ["Durdhura Yoga"] = new() { ["Tamil"] = "துர்துரா யோகம்", ["Telugu"] = "దుర్ధర యోగం", ["Kannada"] = "ದುರ್ಧರ ಯೋಗ", ["Malayalam"] = "ദുർധര യോഗം" },
            ["Budha Aditya Yoga"] = new() { ["Tamil"] = "புத ஆதித்ய யோகம்", ["Telugu"] = "బుధ ఆదిత్య యోగం", ["Kannada"] = "ಬುಧ ಆದಿತ್ಯ ಯೋಗ", ["Malayalam"] = "ബുധ ആദിത്യ യോഗം" },
            ["Hamsa Yoga"] = new() { ["Tamil"] = "அம்ச யோகம்", ["Telugu"] = "హంస యోగం", ["Kannada"] = "ಹಂಸ ಯೋಗ", ["Malayalam"] = "ഹംസ യോഗം" },
            ["Malavya Yoga"] = new() { ["Tamil"] = "மாளவ்ய யோகம்", ["Telugu"] = "మాళవ్య యోగం", ["Kannada"] = "ಮಾಳವ್ಯ ಯೋಗ", ["Malayalam"] = "മാളവ്യ യോഗം" },
            ["Sasa Yoga"] = new() { ["Tamil"] = "சச யோகம்", ["Telugu"] = "శశ యోగం", ["Kannada"] = "ಶಶ ಯೋಗ", ["Malayalam"] = "ശശ യോഗം" },
            ["Ruchaka Yoga"] = new() { ["Tamil"] = "ருசக யோகம்", ["Telugu"] = "రుచక యోగం", ["Kannada"] = "ರುಚಕ ಯೋಗ", ["Malayalam"] = "രുചക യോഗം" },
            ["Bhadra Yoga"] = new() { ["Tamil"] = "பத்ர யோகம்", ["Telugu"] = "భద్ర యోగం", ["Kannada"] = "ಭದ್ರ ಯೋಗ", ["Malayalam"] = "ഭദ്ര യോഗം" }
        };

        if (yogaNames.ContainsKey(yogaName) && yogaNames[yogaName].ContainsKey(language))
        {
            return yogaNames[yogaName][language];
        }

        return yogaName;
    }
}
