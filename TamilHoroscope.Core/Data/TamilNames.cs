namespace TamilHoroscope.Core.Data;

/// <summary>
/// Tamil names and mappings for astrological elements
/// </summary>
public static class TamilNames
{
    /// <summary>
    /// Nakshatra (star) names in Tamil
    /// </summary>
    public static readonly Dictionary<int, (string English, string Tamil)> Nakshatras = new()
    {
        { 1, ("Ashwini", "அஸ்வினி") },
        { 2, ("Bharani", "பரணி") },
        { 3, ("Krittika", "கிருத்திகை") },
        { 4, ("Rohini", "ரோகிணி") },
        { 5, ("Mrigashira", "மிருகசீரிடம்") },
        { 6, ("Ardra", "திருவாதிரை") },
        { 7, ("Punarvasu", "புனர்பூசம்") },
        { 8, ("Pushya", "பூசம்") },
        { 9, ("Ashlesha", "ஆயில்யம்") },
        { 10, ("Magha", "மகம்") },
        { 11, ("Purva Phalguni", "பூரம்") },
        { 12, ("Uttara Phalguni", "உத்திரம்") },
        { 13, ("Hasta", "அஸ்தம்") },
        { 14, ("Chitra", "சித்திரை") },
        { 15, ("Swati", "சுவாதி") },
        { 16, ("Vishakha", "விசாகம்") },
        { 17, ("Anuradha", "அனுஷம்") },
        { 18, ("Jyeshtha", "கேட்டை") },
        { 19, ("Mula", "மூலம்") },
        { 20, ("Purva Ashadha", "பூராடம்") },
        { 21, ("Uttara Ashadha", "உத்திராடம்") },
        { 22, ("Shravana", "திருவோணம்") },
        { 23, ("Dhanishta", "அவிட்டம்") },
        { 24, ("Shatabhisha", "சதயம்") },
        { 25, ("Purva Bhadrapada", "பூரட்டாதி") },
        { 26, ("Uttara Bhadrapada", "உத்திரட்டாதி") },
        { 27, ("Revati", "ரேவதி") }
    };

    /// <summary>
    /// Rasi (zodiac sign) names in Tamil
    /// </summary>
    public static readonly Dictionary<int, (string English, string Tamil)> Rasis = new()
    {
        { 1, ("Aries", "மேஷம்") },
        { 2, ("Taurus", "ரிஷபம்") },
        { 3, ("Gemini", "மிதுனம்") },
        { 4, ("Cancer", "கடகம்") },
        { 5, ("Leo", "சிம்மம்") },
        { 6, ("Virgo", "கன்னி") },
        { 7, ("Libra", "துலாம்") },
        { 8, ("Scorpio", "விருச்சிகம்") },
        { 9, ("Sagittarius", "தனுசு") },
        { 10, ("Capricorn", "மகரம்") },
        { 11, ("Aquarius", "கும்பம்") },
        { 12, ("Pisces", "மீனம்") }
    };

    /// <summary>
    /// Planet names in Tamil (Navagraha)
    /// </summary>
    public static readonly Dictionary<string, string> Planets = new()
    {
        { "Sun", "சூரியன்" },
        { "Moon", "சந்திரன்" },
        { "Mars", "செவ்வாய்" },
        { "Mercury", "புதன்" },
        { "Jupiter", "குரு" },
        { "Venus", "சுக்கிரன்" },
        { "Saturn", "சனி" },
        { "Rahu", "ராகு" },
        { "Ketu", "கேது" }
    };

    /// <summary>
    /// Vara (weekday) names in Tamil
    /// </summary>
    public static readonly Dictionary<int, (string English, string Tamil)> Varas = new()
    {
        { 0, ("Sunday", "ஞாயிறு") },
        { 1, ("Monday", "திங்கள்") },
        { 2, ("Tuesday", "செவ்வாய்") },
        { 3, ("Wednesday", "புதன்") },
        { 4, ("Thursday", "வியாழன்") },
        { 5, ("Friday", "வெள்ளி") },
        { 6, ("Saturday", "சனி") }
    };

    /// <summary>
    /// Tamil month names
    /// </summary>
    public static readonly Dictionary<int, string> TamilMonths = new()
    {
        { 1, "சித்திரை" },    // Chithirai (Apr-May)
        { 2, "வைகாசி" },      // Vaikasi (May-Jun)
        { 3, "ஆனி" },         // Aani (Jun-Jul)
        { 4, "ஆடி" },         // Aadi (Jul-Aug)
        { 5, "ஆவணி" },       // Aavani (Aug-Sep)
        { 6, "புரட்டாசி" },  // Purattasi (Sep-Oct)
        { 7, "ஐப்பசி" },     // Aippasi (Oct-Nov)
        { 8, "கார்த்திகை" }, // Karthikai (Nov-Dec)
        { 9, "மார்கழி" },    // Margazhi (Dec-Jan)
        { 10, "தை" },         // Thai (Jan-Feb)
        { 11, "மாசி" },       // Maasi (Feb-Mar)
        { 12, "பங்குனி" }    // Panguni (Mar-Apr)
    };

    /// <summary>
    /// Tithi names in Tamil
    /// </summary>
    public static readonly Dictionary<int, (string English, string Tamil)> Tithis = new()
    {
        { 1, ("Pratipada", "பிரதமை") },
        { 2, ("Dwitiya", "துவிதியை") },
        { 3, ("Tritiya", "திருதியை") },
        { 4, ("Chaturthi", "சதுர்த்தி") },
        { 5, ("Panchami", "பஞ்சமி") },
        { 6, ("Shashthi", "சஷ்டி") },
        { 7, ("Saptami", "சப்தமி") },
        { 8, ("Ashtami", "அஷ்டமி") },
        { 9, ("Navami", "நவமி") },
        { 10, ("Dasami", "தசமி") },
        { 11, ("Ekadashi", "ஏகாதசி") },
        { 12, ("Dwadashi", "துவாதசி") },
        { 13, ("Trayodashi", "திரயோதசி") },
        { 14, ("Chaturdashi", "சதுர்த்தசி") },
        { 15, ("Purnima/Amavasya", "பௌர்ணமி/அமாவாசை") },
        { 16, ("Pratipada", "பிரதமை") },
        { 17, ("Dwitiya", "துவிதியை") },
        { 18, ("Tritiya", "திருதியை") },
        { 19, ("Chaturthi", "சதுர்த்தி") },
        { 20, ("Panchami", "பஞ்சமி") },
        { 21, ("Shashthi", "சஷ்டி") },
        { 22, ("Saptami", "சப்தமி") },
        { 23, ("Ashtami", "அஷ்டமி") },
        { 24, ("Navami", "நவமி") },
        { 25, ("Dasami", "தசமி") },
        { 26, ("Ekadashi", "ஏகாதசி") },
        { 27, ("Dwadashi", "துவாதசி") },
        { 28, ("Trayodashi", "திரயோதசி") },
        { 29, ("Chaturdashi", "சதுர்த்தசி") },
        { 30, ("Purnima/Amavasya", "பௌர்ணமி/அமாவாசை") }
    };

    /// <summary>
    /// Yoga names in Tamil
    /// </summary>
    public static readonly Dictionary<int, (string English, string Tamil)> Yogas = new()
    {
        { 1, ("Vishkambha", "விஷ்கம்பம்") },
        { 2, ("Priti", "பிரீதி") },
        { 3, ("Ayushman", "ஆயுஷ்மான்") },
        { 4, ("Saubhagya", "சௌபாக்யம்") },
        { 5, ("Shobhana", "ஷோபனம்") },
        { 6, ("Atiganda", "அதிகண்டம்") },
        { 7, ("Sukarma", "சுகர்மா") },
        { 8, ("Dhriti", "த்ருதி") },
        { 9, ("Shula", "சூலம்") },
        { 10, ("Ganda", "கண்டம்") },
        { 11, ("Vriddhi", "விருத்தி") },
        { 12, ("Dhruva", "துருவம்") },
        { 13, ("Vyaghata", "வியாகாதம்") },
        { 14, ("Harshana", "ஹர்ஷணம்") },
        { 15, ("Vajra", "வஜ்ரம்") },
        { 16, ("Siddhi", "சித்தி") },
        { 17, ("Vyatipata", "வியதீபாதம்") },
        { 18, ("Variyan", "வரியான்") },
        { 19, ("Parigha", "பரிகம்") },
        { 20, ("Shiva", "சிவம்") },
        { 21, ("Siddha", "சித்தம்") },
        { 22, ("Sadhya", "சாத்யம்") },
        { 23, ("Shubha", "சுபம்") },
        { 24, ("Shukla", "சுக்லம்") },
        { 25, ("Brahma", "பிரம்மம்") },
        { 26, ("Indra", "இந்திரம்") },
        { 27, ("Vaidhriti", "வைத்ருதி") }
    };

    /// <summary>
    /// Karana names in Tamil
    /// </summary>
    public static readonly Dictionary<int, (string English, string Tamil)> Karanas = new()
    {
        { 1, ("Bava", "பவ") },
        { 2, ("Balava", "பாலவ") },
        { 3, ("Kaulava", "கௌலவ") },
        { 4, ("Taitila", "தைதில") },
        { 5, ("Garaja", "கரஜ") },
        { 6, ("Vanija", "வணிஜ") },
        { 7, ("Vishti", "விஷ்டி") },
        { 8, ("Shakuni", "சகுனி") },
        { 9, ("Chatushpada", "சதுஷ்பாத") },
        { 10, ("Naga", "நாக") },
        { 11, ("Kimstughna", "கிம்ஸ்துக்ன") }
    };

    /// <summary>
    /// Rasi lords (planet ruling each sign)
    /// </summary>
    public static readonly Dictionary<int, string> RasiLords = new()
    {
        { 1, "Mars" },      // Aries
        { 2, "Venus" },     // Taurus
        { 3, "Mercury" },   // Gemini
        { 4, "Moon" },      // Cancer
        { 5, "Sun" },       // Leo
        { 6, "Mercury" },   // Virgo
        { 7, "Venus" },     // Libra
        { 8, "Mars" },      // Scorpio
        { 9, "Jupiter" },   // Sagittarius
        { 10, "Saturn" },   // Capricorn
        { 11, "Saturn" },   // Aquarius
        { 12, "Jupiter" }   // Pisces
    };

    /// <summary>
    /// Vimshottari Dasa durations in years for each planet
    /// </summary>
    public static readonly Dictionary<string, int> DasaDurations = new()
    {
        { "Sun", 6 },
        { "Moon", 10 },
        { "Mars", 7 },
        { "Mercury", 17 },
        { "Jupiter", 16 },
        { "Venus", 20 },
        { "Saturn", 19 },
        { "Rahu", 18 },
        { "Ketu", 7 }
    };

    /// <summary>
    /// Vimshottari Dasa sequence starting from each nakshatra
    /// The sequence is: Ketu, Venus, Sun, Moon, Mars, Rahu, Jupiter, Saturn, Mercury
    /// </summary>
    public static readonly Dictionary<int, string> NakshatraDasaLord = new()
    {
        { 1, "Ketu" },      // Ashwini
        { 2, "Venus" },     // Bharani
        { 3, "Sun" },       // Krittika
        { 4, "Moon" },      // Rohini
        { 5, "Mars" },      // Mrigashira
        { 6, "Rahu" },      // Ardra
        { 7, "Jupiter" },   // Punarvasu
        { 8, "Saturn" },    // Pushya
        { 9, "Mercury" },   // Ashlesha
        { 10, "Ketu" },     // Magha
        { 11, "Venus" },    // Purva Phalguni
        { 12, "Sun" },      // Uttara Phalguni
        { 13, "Moon" },     // Hasta
        { 14, "Mars" },     // Chitra
        { 15, "Rahu" },     // Swati
        { 16, "Jupiter" },  // Vishakha
        { 17, "Saturn" },   // Anuradha
        { 18, "Mercury" },  // Jyeshtha
        { 19, "Ketu" },     // Mula
        { 20, "Venus" },    // Purva Ashadha
        { 21, "Sun" },      // Uttara Ashadha
        { 22, "Moon" },     // Shravana
        { 23, "Mars" },     // Dhanishta
        { 24, "Rahu" },     // Shatabhisha
        { 25, "Jupiter" },  // Purva Bhadrapada
        { 26, "Saturn" },   // Uttara Bhadrapada
        { 27, "Mercury" }   // Revati
    };

    /// <summary>
    /// Dasa sequence order (used for calculating subsequent dasas)
    /// </summary>
    public static readonly string[] DasaSequence = 
    {
        "Ketu", "Venus", "Sun", "Moon", "Mars", "Rahu", "Jupiter", "Saturn", "Mercury"
    };

    /// <summary>
    /// Multi-language support for planet names
    /// </summary>
    public static readonly Dictionary<string, Dictionary<string, string>> PlanetNames = new()
    {
        ["Sun"] = new() { ["English"] = "Sun", ["Tamil"] = "சூரியன்", ["Telugu"] = "సూర్యుడు", ["Kannada"] = "ಸೂರ್ಯ", ["Malayalam"] = "സൂര്യൻ" },
        ["Moon"] = new() { ["English"] = "Moon", ["Tamil"] = "சந்திரன்", ["Telugu"] = "చంద్రుడు", ["Kannada"] = "ಚಂದ್ರ", ["Malayalam"] = "ചന്ദ്രൻ" },
        ["Mars"] = new() { ["English"] = "Mars", ["Tamil"] = "செவ்வாய்", ["Telugu"] = "కుజుడు", ["Kannada"] = "ಮಂಗಳ", ["Malayalam"] = "ചൊവ്വ" },
        ["Mercury"] = new() { ["English"] = "Mercury", ["Tamil"] = "புதன்", ["Telugu"] = "బుధుడు", ["Kannada"] = "ಬುಧ", ["Malayalam"] = "ബുധൻ" },
        ["Jupiter"] = new() { ["English"] = "Jupiter", ["Tamil"] = "குரு", ["Telugu"] = "గురుడు", ["Kannada"] = "ಗುರು", ["Malayalam"] = "ഗുരു" },
        ["Venus"] = new() { ["English"] = "Venus", ["Tamil"] = "சுக்கிரன்", ["Telugu"] = "శుక్రుడు", ["Kannada"] = "ಶುಕ್ರ", ["Malayalam"] = "ശുക്രൻ" },
        ["Saturn"] = new() { ["English"] = "Saturn", ["Tamil"] = "சனி", ["Telugu"] = "శనుడు", ["Kannada"] = "ಶನಿ", ["Malayalam"] = "ശനി" },
        ["Rahu"] = new() { ["English"] = "Rahu", ["Tamil"] = "ராகு", ["Telugu"] = "రాహువు", ["Kannada"] = "ರಾಹು", ["Malayalam"] = "രാഹു" },
        ["Ketu"] = new() { ["English"] = "Ketu", ["Tamil"] = "கேது", ["Telugu"] = "కేతువు", ["Kannada"] = "ಕೇತು", ["Malayalam"] = "കേതു" }
    };

    /// <summary>
    /// Multi-language support for Rasi (zodiac) names
    /// </summary>
    public static readonly Dictionary<int, Dictionary<string, string>> RasiNames = new()
    {
        [1] = new() { ["English"] = "Aries", ["Tamil"] = "மேஷம்", ["Telugu"] = "మేషం", ["Kannada"] = "ಮೇಷ", ["Malayalam"] = "മേടം" },
        [2] = new() { ["English"] = "Taurus", ["Tamil"] = "ரிஷபம்", ["Telugu"] = "వృషభం", ["Kannada"] = "ವೃಷಭ", ["Malayalam"] = "ഇടവം" },
        [3] = new() { ["English"] = "Gemini", ["Tamil"] = "மிதுனம்", ["Telugu"] = "మిథునం", ["Kannada"] = "ಮಿಥುನ", ["Malayalam"] = "മിഥുനം" },
        [4] = new() { ["English"] = "Cancer", ["Tamil"] = "கடகம்", ["Telugu"] = "కర్కాటకం", ["Kannada"] = "ಕರ್ಕಾಟಕ", ["Malayalam"] = "കർക്കിടകം" },
        [5] = new() { ["English"] = "Leo", ["Tamil"] = "சிம்மம்", ["Telugu"] = "సింహం", ["Kannada"] = "ಸಿಂಹ", ["Malayalam"] = "ചിങ്ങം" },
        [6] = new() { ["English"] = "Virgo", ["Tamil"] = "கன்னி", ["Telugu"] = "కన్య", ["Kannada"] = "ಕನ್ಯಾ", ["Malayalam"] = "കന്നി" },
        [7] = new() { ["English"] = "Libra", ["Tamil"] = "துலாம்", ["Telugu"] = "తుల", ["Kannada"] = "ತುಲಾ", ["Malayalam"] = "തുലാം" },
        [8] = new() { ["English"] = "Scorpio", ["Tamil"] = "விருச்சிகம்", ["Telugu"] = "వృశ్చికం", ["Kannada"] = "ವೃಶ್ಚಿಕ", ["Malayalam"] = "വൃശ്ചികം" },
        [9] = new() { ["English"] = "Sagittarius", ["Tamil"] = "தனுசு", ["Telugu"] = "ధనస్సు", ["Kannada"] = "ಧನು", ["Malayalam"] = "ധനു" },
        [10] = new() { ["English"] = "Capricorn", ["Tamil"] = "மகரம்", ["Telugu"] = "మకరం", ["Kannada"] = "ಮಕರ", ["Malayalam"] = "മകരം" },
        [11] = new() { ["English"] = "Aquarius", ["Tamil"] = "கும்பம்", ["Telugu"] = "కుంభం", ["Kannada"] = "ಕುಂಭ", ["Malayalam"] = "കുംഭം" },
        [12] = new() { ["English"] = "Pisces", ["Tamil"] = "மீனம்", ["Telugu"] = "మీనం", ["Kannada"] = "ಮೀನ", ["Malayalam"] = "മീനം" }
    };

    /// <summary>
    /// Multi-language support for Nakshatra (star) names
    /// </summary>
    public static readonly Dictionary<int, Dictionary<string, string>> NakshatraNames = new()
    {
        [1] = new() { ["English"] = "Ashwini", ["Tamil"] = "அஸ்வினி", ["Telugu"] = "అశ్విని", ["Kannada"] = "ಅಶ್ವಿನಿ", ["Malayalam"] = "അശ്വതി" },
        [2] = new() { ["English"] = "Bharani", ["Tamil"] = "பரணி", ["Telugu"] = "భరణి", ["Kannada"] = "ಭರಣಿ", ["Malayalam"] = "ഭരണി" },
        [3] = new() { ["English"] = "Krittika", ["Tamil"] = "கிருத்திகை", ["Telugu"] = "కృత్తిక", ["Kannada"] = "ಕೃತ್ತಿಕಾ", ["Malayalam"] = "കാർത്തിക" },
        [4] = new() { ["English"] = "Rohini", ["Tamil"] = "ரோகிணி", ["Telugu"] = "రోహిణి", ["Kannada"] = "ರೋಹಿಣಿ", ["Malayalam"] = "രോഹിണി" },
        [5] = new() { ["English"] = "Mrigashira", ["Tamil"] = "மிருகசீரிடம்", ["Telugu"] = "మృగశిర", ["Kannada"] = "ಮೃಗಶಿರ", ["Malayalam"] = "മകയിരം" },
        [6] = new() { ["English"] = "Ardra", ["Tamil"] = "திருவாதிரை", ["Telugu"] = "ఆరుద్ర", ["Kannada"] = "ಆರ್ದ್ರಾ", ["Malayalam"] = "തിരുവാതിര" },
        [7] = new() { ["English"] = "Punarvasu", ["Tamil"] = "புனர்பூசம்", ["Telugu"] = "పునర్వసు", ["Kannada"] = "ಪುನರ್ವಸು", ["Malayalam"] = "പുണർതം" },
        [8] = new() { ["English"] = "Pushya", ["Tamil"] = "பூசம்", ["Telugu"] = "పుష్యమి", ["Kannada"] = "ಪುಷ್ಯ", ["Malayalam"] = "പൂയം" },
        [9] = new() { ["English"] = "Ashlesha", ["Tamil"] = "ஆயில்யம்", ["Telugu"] = "ఆశ్లేష", ["Kannada"] = "ಆಶ್ಲೇಷಾ", ["Malayalam"] = "ആയില്യം" },
        [10] = new() { ["English"] = "Magha", ["Tamil"] = "மகம்", ["Telugu"] = "మఖ", ["Kannada"] = "ಮಖ", ["Malayalam"] = "മകം" },
        [11] = new() { ["English"] = "Purva Phalguni", ["Tamil"] = "பூரம்", ["Telugu"] = "పుబ్బ", ["Kannada"] = "ಪೂರ್ವಾ ಫಲ್ಗುಣಿ", ["Malayalam"] = "പൂരം" },
        [12] = new() { ["English"] = "Uttara Phalguni", ["Tamil"] = "உத்திரம்", ["Telugu"] = "ఉత్తర", ["Kannada"] = "ಉತ್ತರಾ ಫಲ್ಗುಣಿ", ["Malayalam"] = "ഉത്രം" },
        [13] = new() { ["English"] = "Hasta", ["Tamil"] = "அஸ்தம்", ["Telugu"] = "హస్త", ["Kannada"] = "ಹಸ್ತ", ["Malayalam"] = "അത്തം" },
        [14] = new() { ["English"] = "Chitra", ["Tamil"] = "சித்திரை", ["Telugu"] = "చిత్త", ["Kannada"] = "ಚಿತ್ರಾ", ["Malayalam"] = "ചിത്തിര" },
        [15] = new() { ["English"] = "Swati", ["Tamil"] = "சுவாதி", ["Telugu"] = "స్వాతి", ["Kannada"] = "ಸ್ವಾತಿ", ["Malayalam"] = "ചോതി" },
        [16] = new() { ["English"] = "Vishakha", ["Tamil"] = "விசாகம்", ["Telugu"] = "విశాఖ", ["Kannada"] = "ವಿಶಾಖಾ", ["Malayalam"] = "വിശാഖം" },
        [17] = new() { ["English"] = "Anuradha", ["Tamil"] = "அனுஷம்", ["Telugu"] = "అనూరాధ", ["Kannada"] = "ಅನೂರಾಧಾ", ["Malayalam"] = "അനിഴം" },
        [18] = new() { ["English"] = "Jyeshtha", ["Tamil"] = "கேட்டை", ["Telugu"] = "జ్యేష్ఠ", ["Kannada"] = "ಜ್ಯೇಷ್ಠಾ", ["Malayalam"] = "തൃക്കേട്ട" },
        [19] = new() { ["English"] = "Mula", ["Tamil"] = "மூலம்", ["Telugu"] = "మూల", ["Kannada"] = "ಮೂಲಾ", ["Malayalam"] = "മൂലം" },
        [20] = new() { ["English"] = "Purva Ashadha", ["Tamil"] = "பூராடம்", ["Telugu"] = "పూర్వాషాఢ", ["Kannada"] = "ಪೂರ್ವಾಷಾಢಾ", ["Malayalam"] = "പൂരാടം" },
        [21] = new() { ["English"] = "Uttara Ashadha", ["Tamil"] = "உத்திராடம்", ["Telugu"] = "ఉత్తరాషాఢ", ["Kannada"] = "ಉತ್ತರಾಷಾಢಾ", ["Malayalam"] = "ഉത്രാടം" },
        [22] = new() { ["English"] = "Shravana", ["Tamil"] = "திருவோணம்", ["Telugu"] = "శ్రవణం", ["Kannada"] = "ಶ್ರವಣಾ", ["Malayalam"] = "തിരുവോണം" },
        [23] = new() { ["English"] = "Dhanishta", ["Tamil"] = "அவிட்டம்", ["Telugu"] = "ధనిష్ట", ["Kannada"] = "ಧನಿಷ್ಠಾ", ["Malayalam"] = "അവിട്ടം" },
        [24] = new() { ["English"] = "Shatabhisha", ["Tamil"] = "சதயம்", ["Telugu"] = "శతభిషం", ["Kannada"] = "ಶತಭಿಷಾ", ["Malayalam"] = "ചതയം" },
        [25] = new() { ["English"] = "Purva Bhadrapada", ["Tamil"] = "பூரட்டாதி", ["Telugu"] = "పూర్వాభాద్ర", ["Kannada"] = "ಪೂರ್ವಾಭಾದ್ರಪದ", ["Malayalam"] = "പൂരുരുട്ടാതി" },
        [26] = new() { ["English"] = "Uttara Bhadrapada", ["Tamil"] = "உத்திரட்டாதி", ["Telugu"] = "ఉత్తరాభాద్ర", ["Kannada"] = "ಉತ್ತರಾಭಾದ್ರಪದ", ["Malayalam"] = "ഉത്രട്ടാതി" },
        [27] = new() { ["English"] = "Revati", ["Tamil"] = "ரேவதி", ["Telugu"] = "రేవతి", ["Kannada"] = "ರೇವತಿ", ["Malayalam"] = "രേവതി" }
    };

    /// <summary>
    /// Get localized planet name
    /// </summary>
    public static string GetPlanetName(string englishName, string language = "Tamil")
    {
        if (PlanetNames.ContainsKey(englishName) && PlanetNames[englishName].ContainsKey(language))
        {
            return PlanetNames[englishName][language];
        }
        return englishName;
    }

    /// <summary>
    /// Get localized Rasi name
    /// </summary>
    public static string GetRasiName(int rasiNumber, string language = "Tamil")
    {
        if (RasiNames.ContainsKey(rasiNumber) && RasiNames[rasiNumber].ContainsKey(language))
        {
            return RasiNames[rasiNumber][language];
        }
        return Rasis.ContainsKey(rasiNumber) ? Rasis[rasiNumber].English : "";
    }

    /// <summary>
    /// Get localized Nakshatra name
    /// </summary>
    public static string GetNakshatraName(int nakshatraNumber, string language = "Tamil")
    {
        if (NakshatraNames.ContainsKey(nakshatraNumber) && NakshatraNames[nakshatraNumber].ContainsKey(language))
        {
            return NakshatraNames[nakshatraNumber][language];
        }
        return Nakshatras.ContainsKey(nakshatraNumber) ? Nakshatras[nakshatraNumber].English : "";
    }
}
