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

    /// <summary>
    /// Section/label names in multiple languages
    /// </summary>
    public static readonly Dictionary<string, Dictionary<string, string>> SectionNames = new()
    {
        ["RasiChart"] = new() { ["English"] = "Rasi Chart (Birth Chart)", ["Tamil"] = "ராசி கட்டம்", ["Telugu"] = "రాశి చక్రం", ["Kannada"] = "ರಾಶಿ ಚಕ್ರ", ["Malayalam"] = "രാശി ചക്രം" },
        ["NavamsaChart"] = new() { ["English"] = "Navamsa Chart (D-9)", ["Tamil"] = "நவாம்ச கட்டம்", ["Telugu"] = "నవాంశ చక్రం", ["Kannada"] = "ನವಾಂಶ ಚಕ್ರ", ["Malayalam"] = "നവാംശ ചക്രം" },
        ["PlanetaryStrength"] = new() { ["English"] = "Planetary Strength", ["Tamil"] = "கிரக பலம்", ["Telugu"] = "గ్రహ బలం", ["Kannada"] = "ಗ್ರಹ ಬಲ", ["Malayalam"] = "ഗ്രഹ ബലം" },
        ["VimshottariDasa"] = new() { ["English"] = "Vimshottari Dasa Periods", ["Tamil"] = "விம்சோத்தரி தசை", ["Telugu"] = "విమ్శోత్తరి దశ", ["Kannada"] = "ವಿಂಶೋತ್ತರಿ ದಶಾ", ["Malayalam"] = "വിംശോത്തരി ദശ" },
        ["AstrologicalYogas"] = new() { ["English"] = "Astrological Yogas", ["Tamil"] = "ஜோதிட யோகங்கள்", ["Telugu"] = "జ్యోతిష యోగాలు", ["Kannada"] = "ಜ್ಯೋತಿಷ ಯೋಗಗಳು", ["Malayalam"] = "ജ്യോതിഷ യോഗങ്ങൾ" },
        ["AstrologicalDoshas"] = new() { ["English"] = "Astrological Doshas", ["Tamil"] = "ஜோதிட தோஷங்கள்", ["Telugu"] = "జ్యోతిష దోషాలు", ["Kannada"] = "ಜ್ಯೋತಿಷ ದೋಷಗಳು", ["Malayalam"] = "ജ്യോതിഷ ദോഷങ്ങൾ" },
        ["Planet"] = new() { ["English"] = "Planet", ["Tamil"] = "கிரகம்", ["Telugu"] = "గ్రహం", ["Kannada"] = "ಗ್ರಹ", ["Malayalam"] = "ഗ്രഹം" },
        ["Rasi"] = new() { ["English"] = "Rasi", ["Tamil"] = "ராசி", ["Telugu"] = "రాశి", ["Kannada"] = "ರಾಶಿ", ["Malayalam"] = "രാശി" },
        ["Nakshatra"] = new() { ["English"] = "Nakshatra", ["Tamil"] = "நட்சத்திரம்", ["Telugu"] = "నక్షత్రం", ["Kannada"] = "ನಕ್ಷತ್ರ", ["Malayalam"] = "നക്ഷത്രം" },
        ["Navamsa"] = new() { ["English"] = "Navamsa", ["Tamil"] = "நவாம்சம்", ["Telugu"] = "నవాంశం", ["Kannada"] = "ನವಾಂಶ", ["Malayalam"] = "നവാംശം" }
    };

    /// <summary>
    /// Get localized section/label name
    /// </summary>
    public static string GetSectionName(string sectionKey, string language = "Tamil")
    {
        if (SectionNames.ContainsKey(sectionKey) && SectionNames[sectionKey].ContainsKey(language))
        {
            return SectionNames[sectionKey][language];
        }
        return sectionKey;
    }

    /// <summary>
    /// Yoga descriptions in multiple languages
    /// </summary>
    public static readonly Dictionary<string, Dictionary<string, string>> YogaDescriptions = new()
    {
        ["Gajakesari Yoga"] = new()
        {
            ["English"] = "Jupiter in kendra from Moon. Brings wisdom, wealth, fame, and good character.",
            ["Tamil"] = "சந்திரனிலிருந்து குரு கேந்திரத்தில். ஞானம், செல்வம், புகழ் மற்றும் நல்ல குணத்தை அளிக்கிறது.",
            ["Telugu"] = "చంద్రుడి నుండి గురువు కేంద్రంలో. జ్ఞానం, సంపద, కీర్తి మరియు మంచి స్వభావాన్ని ఇస్తుంది.",
            ["Kannada"] = "ಚಂದ್ರನಿಂದ ಗುರು ಕೇಂದ್ರದಲ್ಲಿ. ಜ್ಞಾನ, ಸಂಪತ್ತು, ಖ್ಯಾತಿ ಮತ್ತು ಉತ್ತಮ ಪಾತ್ರವನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "ചന്ദ്രനിൽ നിന്ന് വ്യാഴം കേന്ദ്രത്തിൽ. ജ്ഞാനം, സമ്പത്ത്, പ്രശസ്തി, നല്ല സ്വഭാവം എന്നിവ നൽകുന്നു."
        },
        ["Raja Yoga"] = new()
        {
            ["English"] = "{0} rules both kendra and trikona houses. Brings power, authority, and success.",
            ["Tamil"] = "{0} கேந்திரம் மற்றும் திரிகோண வீடுகளை ஆளுகிறது. அதிகாரம், சக்தி மற்றும் வெற்றியை அளிக்கிறது.",
            ["Telugu"] = "{0} కేంద్రం మరియు త్రికోణ గృహాలను పాలిస్తుంది. శక్తి, అధికారం మరియు విజయాన్ని ఇస్తుంది.",
            ["Kannada"] = "{0} ಕೇಂದ್ರ ಮತ್ತು ತ್ರಿಕೋಣ ಮನೆಗಳನ್ನು ಆಳುತ್ತದೆ. ಶಕ್ತಿ, ಅಧಿಕಾರ ಮತ್ತು ಯಶಸ್ಸು ತರುತ್ತದೆ.",
            ["Malayalam"] = "{0} കേന്ദ്ര, ത്രികോണ ഗൃഹങ്ങളെ ഭരിക്കുന്നു. ശക്തി, അധികാരം, വിജയം എന്നിവ നൽകുന്നു."
        },
        ["Dhana Yoga"] = new()
        {
            ["English"] = "{0} creates wealth yoga. Brings prosperity and financial gains.",
            ["Tamil"] = "{0} செல்வ யோகத்தை உருவாக்குகிறது. செழிப்பு மற்றும் நிதி ஆதாயங்களை அளிக்கிறது.",
            ["Telugu"] = "{0} ధన యోగాన్ని సృష్టిస్తుంది. శ్రేయస్సు మరియు ఆర్థిక లాభాలను ఇస్తుంది.",
            ["Kannada"] = "{0} ಸಂಪತ್ತು ಯೋಗವನ್ನು ರಚಿಸುತ್ತದೆ. ಸಮೃದ್ಧಿ ಮತ್ತು ಆರ್ಥಿಕ ಲಾಭಗಳನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "{0} ധനയോഗം സൃഷ്ടിക്കുന്നു. സമൃദ്ധി, സാമ്പത്തിക നേട്ടങ്ങൾ നൽകുന്നു."
        },
        ["Sunapha Yoga"] = new()
        {
            ["English"] = "Planet(s) in 2nd house from Moon. Brings wealth, intelligence, and good reputation.",
            ["Tamil"] = "சந்திரனிலிருந்து 2-ஆம் வீட்டில் கிரகங்கள். செல்வம், அறிவு மற்றும் நல்ல புகழை அளிக்கிறது.",
            ["Telugu"] = "చంద్రుడి నుండి 2వ ఇంట్లో గ్రహాలు. సంపద, తెలివితేటలు మరియు మంచి పేరును ఇస్తుంది.",
            ["Kannada"] = "ಚಂದ್ರನಿಂದ 2ನೇ ಮನೆಯಲ್ಲಿ ಗ್ರಹಗಳು. ಸಂಪತ್ತು, ಬುದ್ಧಿವಂತಿಕೆ ಮತ್ತು ಉತ್ತಮ ಖ್ಯಾತಿಯನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "ചന്ദ്രനിൽ നിന്ന് 2-ാം ഭവനത്തിൽ ഗ്രഹങ്ങൾ. സമ്പത്ത്, ബുദ്ധി, നല്ല പ്രശസ്തി നൽകുന്നു."
        },
        ["Anapha Yoga"] = new()
        {
            ["English"] = "Planet(s) in 12th house from Moon. Brings fame, good health, and spiritual inclination.",
            ["Tamil"] = "சந்திரனிலிருந்து 12-ஆம் வீட்டில் கிரகங்கள். புகழ், நல்ல ஆரோக்கியம் மற்றும் ஆன்மீக விருப்பத்தை அளிக்கிறது.",
            ["Telugu"] = "చంద్రుడి నుండి 12వ ఇంట్లో గ్రహాలు. కీర్తి, మంచి ఆరోగ్యం మరియు ఆధ్యాత్మిక మొగ్గును ఇస్తుంది.",
            ["Kannada"] = "ಚಂದ್ರನಿಂದ 12ನೇ ಮನೆಯಲ್ಲಿ ಗ್ರಹಗಳು. ಖ್ಯಾತಿ, ಉತ್ತಮ ಆರೋಗ್ಯ ಮತ್ತು ಆಧ್ಯಾತ್ಮಿಕ ಒಲವು ತರುತ್ತದೆ.",
            ["Malayalam"] = "ചന്ദ്രനിൽ നിന്ന് 12-ാം ഭവനത്തിൽ ഗ്രഹങ്ങൾ. പ്രശസ്തി, നല്ല ആരോഗ്യം, ആത്മീയ താത്പര്യം നൽകുന്നു."
        },
        ["Durdhura Yoga"] = new()
        {
            ["English"] = "Planets on both sides of Moon. Brings prosperity, conveyances, and comforts.",
            ["Tamil"] = "சந்திரனின் இரு பக்கங்களிலும் கிரகங்கள். செழிப்பு, வாகனங்கள் மற்றும் வசதிகளை அளிக்கிறது.",
            ["Telugu"] = "చంద్రుడికి రెండు వైపులా గ్రహాలు. శ్రేయస్సు, వాహనాలు మరియు సౌకర్యాలను ఇస్తుంది.",
            ["Kannada"] = "ಚಂದ್ರನ ಎರಡೂ ಬದಿಗಳಲ್ಲಿ ಗ್ರಹಗಳು. ಸಮೃದ್ಧಿ, ವಾಹನಗಳು ಮತ್ತು ಸೌಕರ್ಯಗಳನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "ചന്ദ്രന്റെ ഇരുവശത്തും ഗ്രഹങ്ങൾ. സമൃദ്ധി, വാഹനങ്ങൾ, സുഖസൗകര്യങ്ങൾ നൽകുന്നു."
        },
        ["Budha Aditya Yoga"] = new()
        {
            ["English"] = "Sun and Mercury conjunction. Brings intelligence, communication skills, and analytical ability.",
            ["Tamil"] = "சூரியன் மற்றும் புதன் இணைவு. அறிவு, தொடர்பு திறன் மற்றும் பகுப்பாய்வு திறனை அளிக்கிறது.",
            ["Telugu"] = "సూర్యుడు మరియు బుధుడు కలయిక. తెలివితేటలు, కమ్యూనికేషన్ నైపుణ్యాలు మరియు విశ్లేషణాత్మక సామర్థ్యాన్ని ఇస్తుంది.",
            ["Kannada"] = "ಸೂರ್ಯ ಮತ್ತು ಬುಧ ಸಂಯೋಗ. ಬುದ್ಧಿವಂತಿಕೆ, ಸಂವಹನ ಕೌಶಲ್ಯ ಮತ್ತು ವಿಶ್ಲೇಷಣಾತ್ಮಕ ಸಾಮರ್ಥ್ಯವನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "സൂര്യ-ബുധ സംയോഗം. ബുദ്ധി, ആശയവിനിമയ കഴിവ്, വിശകലന ശേഷി നൽകുന്നു."
        },
        ["Hamsa Yoga"] = new()
        {
            ["English"] = "Jupiter in kendra in own sign or exaltation. Brings wisdom, spirituality, and prosperity.",
            ["Tamil"] = "குரு கேந்திரத்தில் தன் ராசியில் அல்லது உச்சத்தில். ஞானம், ஆன்மீகம் மற்றும் செழிப்பை அளிக்கிறது.",
            ["Telugu"] = "గురువు కేంద్రంలో స్వరాశిలో లేదా ఉచ్చంలో. జ్ఞానం, ఆధ్యాత్మికత మరియు శ్రేయస్సును ఇస్తుంది.",
            ["Kannada"] = "ಗುರು ಕೇಂದ್ರದಲ್ಲಿ ಸ್ವರಾಶಿಯಲ್ಲಿ ಅಥವಾ ಉಚ್ಚದಲ್ಲಿ. ಜ್ಞಾನ, ಆಧ್ಯಾತ್ಮಿಕತೆ ಮತ್ತು ಸಮೃದ್ಧಿಯನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "വ്യാഴം കേന്ദ്രത്തിൽ സ്വരാശിയിലോ ഉച്ചത്തിലോ. ജ്ഞാനം, ആത്മീയത, സമൃദ്ധി നൽകുന്നു."
        },
        ["Malavya Yoga"] = new()
        {
            ["English"] = "Venus in kendra in own sign or exaltation. Brings beauty, luxury, and artistic talents.",
            ["Tamil"] = "சுக்கிரன் கேந்திரத்தில் தன் ராசியில் அல்லது உச்சத்தில். அழகு, ஆடம்பரம் மற்றும் கலை திறமைகளை அளிக்கிறது.",
            ["Telugu"] = "శుక్రుడు కేంద్రంలో స్వరాశిలో లేదా ఉచ్చంలో. అందం, విలాసం మరియు కళాత్మక ప్రతిభను ఇస్తుంది.",
            ["Kannada"] = "ಶುಕ್ರ ಕೇಂದ್ರದಲ್ಲಿ ಸ್ವರಾಶಿಯಲ್ಲಿ ಅಥವಾ ಉಚ್ಚದಲ್ಲಿ. ಸೌಂದರ್ಯ, ಐಷಾರಾಮಿ ಮತ್ತು ಕಲಾತ್ಮಕ ಪ್ರತಿಭೆಗಳನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "ശുക്രൻ കേന്ദ്രത്തിൽ സ്വരാശിയിലോ ഉച്ചത്തിലോ. സൗന്ദര്യം, ആഡംബരം, കലാപ്രതിഭ നൽകുന്നു."
        },
        ["Sasa Yoga"] = new()
        {
            ["English"] = "Saturn in kendra in own sign or exaltation. Brings discipline, longevity, and authority.",
            ["Tamil"] = "சனி கேந்திரத்தில் தன் ராசியில் அல்லது உச்சத்தில். ஒழுக்கம், நீண்ட ஆயுள் மற்றும் அதிகாரத்தை அளிக்கிறது.",
            ["Telugu"] = "శని కేంద్రంలో స్వరాశిలో లేదా ఉచ్చంలో. క్రమశిక్షణ, దీర్ఘాయువు మరియు అధికారాన్ని ఇస్తుంది.",
            ["Kannada"] = "ಶನಿ ಕೇಂದ್ರದಲ್ಲಿ ಸ್ವರಾಶಿಯಲ್ಲಿ ಅಥವಾ ಉಚ್ಚದಲ್ಲಿ. ಶಿಸ್ತು, ದೀರ್ಘಾಯುಷ್ಯ ಮತ್ತು ಅಧಿಕಾರವನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "ശനി കേന്ദ്രത്തിൽ സ്വരാശിയിലോ ഉച്ചത്തിലോ. അച്ചടക്കം, ദീർഘായുസ്സ്, അധികാരം നൽകുന്നു."
        },
        ["Ruchaka Yoga"] = new()
        {
            ["English"] = "Mars in kendra in own sign or exaltation. Brings courage, leadership, and military prowess.",
            ["Tamil"] = "செவ்வாய் கேந்திரத்தில் தன் ராசியில் அல்லது உச்சத்தில். தைரியம், தலைமைத்துவம் மற்றும் படை வலிமையை அளிக்கிறது.",
            ["Telugu"] = "కుజుడు కేంద్రంలో స్వరాశిలో లేదా ఉచ్చంలో. ధైర్యం, నాయకత్వం మరియు సైనిక పరాక్రమాన్ని ఇస్తుంది.",
            ["Kannada"] = "ಮಂಗಳ ಕೇಂದ್ರದಲ್ಲಿ ಸ್ವರಾಶಿಯಲ್ಲಿ ಅಥವಾ ಉಚ್ಚದಲ್ಲಿ. ಧೈರ್ಯ, ನಾಯಕತ್ವ ಮತ್ತು ಮಿಲಿಟರಿ ಪರಾಕ್ರಮವನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "ചൊവ്വ കേന്ദ്രത്തിൽ സ്വരാശിയിലോ ഉച്ചത്തിലോ. ധൈര്യം, നേതൃത്വം, സൈനിക വൈദഗ്ധ്യം നൽകുന്നു."
        },
        ["Bhadra Yoga"] = new()
        {
            ["English"] = "Mercury in kendra in own sign or exaltation. Brings intelligence, communication, and business acumen.",
            ["Tamil"] = "புதன் கேந்திரத்தில் தன் ராசியில் அல்லது உச்சத்தில். அறிவு, தொடர்பு மற்றும் வணிக புத்திசாலித்தனத்தை அளிக்கிறது.",
            ["Telugu"] = "బుధుడు కేంద్రంలో స్వరాశిలో లేదా ఉచ్చంలో. తెలివితేటలు, కమ్యూనికేషన్ మరియు వ్యాపార తెలివిని ఇస్తుంది.",
            ["Kannada"] = "ಬುಧ ಕೇಂದ್ರದಲ್ಲಿ ಸ್ವರಾಶಿಯಲ್ಲಿ ಅಥವಾ ಉಚ್ಚದಲ್ಲಿ. ಬುದ್ಧಿವಂತಿಕೆ, ಸಂವಹನ ಮತ್ತು ವ್ಯಾಪಾರ ಪರಿಣತಿಯನ್ನು ತರುತ್ತದೆ.",
            ["Malayalam"] = "ബുധൻ കേന്ദ്രത്തിൽ സ്വരാശിയിലോ ഉച്ചത്തിലോ. ബുദ്ധി, ആശയവിനിമയം, ബിസിനസ് കഴിവ് നൽകുന്നു."
        }
    };

    /// <summary>
    /// Dosa descriptions in multiple languages
    /// </summary>
    public static readonly Dictionary<string, Dictionary<string, string>> DosaDescriptions = new()
    {
        ["Mangal Dosha (Kuja Dosha)"] = new()
        {
            ["English"] = "Mars in {0}th house. May cause delays or difficulties in marriage and marital harmony.",
            ["Tamil"] = "செவ்வாய் {0}-ஆம் வீட்டில். திருமணம் மற்றும் திருமண இணக்கத்தில் தாமதங்கள் அல்லது சிரமங்களை ஏற்படுத்தலாம்.",
            ["Telugu"] = "కుజుడు {0}వ ఇంట్లో. వివాహం మరియు వైవాహిక సామరస్యంలో ఆలస్యాలు లేదా ఇబ్బందులను కలిగిస్తుంది.",
            ["Kannada"] = "ಮಂಗಳ {0}ನೇ ಮನೆಯಲ್ಲಿ. ಮದುವೆ ಮತ್ತು ವೈವಾಹಿಕ ಸಾಮರಸ್ಯದಲ್ಲಿ ವಿಳಂಬಗಳು ಅಥವಾ ತೊಂದರೆಗಳನ್ನು ಉಂಟುಮಾಡಬಹುದು.",
            ["Malayalam"] = "ചൊവ്വ {0}-ാം ഭവനത്തിൽ. വിവാഹത്തിലും വൈവാഹിക സൗഹാർദ്ദത്തിലും കാലതാമസം അല്ലെങ്കിൽ ബുദ്ധിമുട്ടുകൾ ഉണ്ടാക്കാം."
        },
        ["Kaal Sarp Dosha"] = new()
        {
            ["English"] = "All planets are positioned between Rahu and Ketu. May cause obstacles, delays, and mental anxiety.",
            ["Tamil"] = "அனைத்து கிரகங்களும் ராகு மற்றும் கேது இடையே உள்ளன. தடைகள், தாமதங்கள் மற்றும் மன கவலைகளை ஏற்படுத்தலாம்.",
            ["Telugu"] = "అన్ని గ్రహాలు రాహు మరియు కేతువుల మధ్య ఉన్నాయి. అడ్డంకులు, ఆలస్యాలు మరియు మానసిక ఆందోళనను కలిగిస్తుంది.",
            ["Kannada"] = "ಎಲ್ಲಾ ಗ್ರಹಗಳು ರಾಹು ಮತ್ತು ಕೇತುಗಳ ನಡುವೆ ಇವೆ. ಅಡೆತಡೆಗಳು, ವಿಳಂಬಗಳು ಮತ್ತು ಮಾನಸಿಕ ಆತಂಕವನ್ನು ಉಂಟುಮಾಡಬಹುದು.",
            ["Malayalam"] = "എല്ലാ ഗ്രഹങ്ങളും രാഹു-കേതുക്കൾക്കിടയിൽ. പ്രതിബന്ധങ്ങൾ, കാലതാമസം, മാനസിക ഉത്കണ്ഠ ഉണ്ടാക്കാം."
        },
        ["Pitra Dosha"] = new()
        {
            ["English"] = "Ancestral affliction detected: {0}. May cause family issues and obstacles in life.",
            ["Tamil"] = "மூதாதையர் பாதிப்பு கண்டறியப்பட்டது: {0}. குடும்ப பிரச்சினைகள் மற்றும் வாழ்க்கை தடைகளை ஏற்படுத்தலாம்.",
            ["Telugu"] = "పూర్వీకుల బాధ కనుగొనబడింది: {0}. కుటుంబ సమస్యలు మరియు జీవితంలో అడ్డంకులను కలిగిస్తుంది.",
            ["Kannada"] = "ಪಿತೃ ದೋಷ ಪತ್ತೆಯಾಗಿದೆ: {0}. ಕುಟುಂಬ ಸಮಸ್ಯೆಗಳು ಮತ್ತು ಜೀವನದಲ್ಲಿ ಅಡೆತಡೆಗಳನ್ನು ಉಂಟುಮಾಡಬಹುದು.",
            ["Malayalam"] = "പിതൃദോഷം കണ്ടെത്തി: {0}. കുടുംബ പ്രശ്നങ്ങളും ജീവിതത്തിൽ തടസ്സങ്ങളും ഉണ്ടാക്കാം."
        },
        ["Shakat Dosha"] = new()
        {
            ["English"] = "Moon in {0}th house from Jupiter. May cause financial instability and ups and downs in life.",
            ["Tamil"] = "குருவிலிருந்து {0}-ஆம் வீட்டில் சந்திரன். நிதி ஸ்திரமின்மை மற்றும் வாழ்க்கையில் ஏற்ற இறக்கங்களை ஏற்படுத்தலாம்.",
            ["Telugu"] = "గురువు నుండి {0}వ ఇంట్లో చంద్రుడు. ఆర్థిక అస్థిరత మరియు జీవితంలో హెచ్చు తగ్గులను కలిగిస్తుంది.",
            ["Kannada"] = "ಗುರುವಿನಿಂದ {0}ನೇ ಮನೆಯಲ್ಲಿ ಚಂದ್ರ. ಆರ್ಥಿಕ ಅಸ್ಥಿರತೆ ಮತ್ತು ಜೀವನದಲ್ಲಿ ಏರುಪೇರುಗಳನ್ನು ಉಂಟುಮಾಡಬಹುದು.",
            ["Malayalam"] = "വ്യാഴത്തിൽ നിന്ന് {0}-ാം ഭവനത്തിൽ ചന്ദ്രൻ. സാമ്പത്തിക അസ്ഥിരത, ജീവിതത്തിൽ ഉയർച്ച താഴ്ചകൾ ഉണ്ടാക്കാം."
        },
        ["Kemadruma Dosha"] = new()
        {
            ["English"] = "Moon has no planets in adjacent houses. May cause poverty, mental stress, and lack of support.",
            ["Tamil"] = "சந்திரனுக்கு அருகில் கிரகங்கள் இல்லை. வறுமை, மன அழுத்தம் மற்றும் ஆதரவின்மையை ஏற்படுத்தலாம்.",
            ["Telugu"] = "చంద్రుడికి ప్రక్క ఇళ్లలో గ్రహాలు లేవు. పేదరికం, మానసిక ఒత్తిడి మరియు మద్దతు లేకపోవడం కలిగిస్తుంది.",
            ["Kannada"] = "ಚಂದ್ರನಿಗೆ ಪಕ್ಕದ ಮನೆಗಳಲ್ಲಿ ಗ್ರಹಗಳಿಲ್ಲ. ಬಡತನ, ಮಾನಸಿಕ ಒತ್ತಡ ಮತ್ತು ಬೆಂಬಲದ ಕೊರತೆಯನ್ನು ಉಂಟುಮಾಡಬಹುದು.",
            ["Malayalam"] = "ചന്ദ്രന് അടുത്ത ഭവനങ്ങളിൽ ഗ്രഹങ്ങളില്ല. ദാരിദ്ര്യം, മാനസിക സമ്മർദ്ദം, പിന്തുണയില്ലായ്മ ഉണ്ടാക്കാം."
        }
    };

    /// <summary>
    /// Dosa remedies in multiple languages
    /// </summary>
    public static readonly Dictionary<string, Dictionary<string, List<string>>> DosaRemedies = new()
    {
        ["Mangal Dosha (Kuja Dosha)"] = new()
        {
            ["English"] = new List<string>
            {
                "Chant Hanuman Chalisa regularly",
                "Worship Lord Hanuman on Tuesdays",
                "Recite Mangal mantra: Om Angarakaya Namaha",
                "Donate red lentils on Tuesdays",
                "Wear red coral gemstone after consultation"
            },
            ["Tamil"] = new List<string>
            {
                "ஹனுமான் சாலிசா தினமும் சொல்லுங்கள்",
                "செவ்வாய்க்கிழமைகளில் ஹனுமானை வணங்குங்கள்",
                "மங்கள மந்திரம் சொல்லுங்கள்: ஓம் அங்காரகாய நம:",
                "செவ்வாய்க்கிழமைகளில் சிவப்பு பருப்பு தானம் செய்யுங்கள்",
                "ஆலோசனைக்குப் பிறகு சிவப்பு பவளம் அணியுங்கள்"
            },
            ["Telugu"] = new List<string>
            {
                "హనుమాన్ చాలీసా రోజూ చదవండి",
                "మంగళవారాలలో హనుమంతుడిని పూజించండి",
                "మంగళ మంత్రం చదవండి: ఓం అంగారకాయ నమః",
                "మంగళవారాలలో ఎరుపు పప్పు దానం చేయండి",
                "సలహా తర్వాత ఎరుపు పగడం ధరించండి"
            },
            ["Kannada"] = new List<string>
            {
                "ಹನುಮಾನ್ ಚಾಲೀಸಾ ನಿಯಮಿತವಾಗಿ ಪಠಿಸಿ",
                "ಮಂಗಳವಾರಗಳಲ್ಲಿ ಹನುಮಂತನನ್ನು ಪೂಜಿಸಿ",
                "ಮಂಗಳ ಮಂತ್ರ ಓದಿ: ಓಂ ಅಂಗಾರಕಾಯ ನಮಃ",
                "ಮಂಗಳವಾರಗಳಲ್ಲಿ ಕೆಂಪು ದಾಲ್ ದಾನ ಮಾಡಿ",
                "ಸಲಹೆ ನಂತರ ಕೆಂಪು ಹವಳವನ್ನು ಧರಿಸಿ"
            },
            ["Malayalam"] = new List<string>
            {
                "ഹനുമാൻ ചാലീസ പതിവായി പാരായണം ചെയ്യുക",
                "ചൊവ്വാഴ്ചകളിൽ ഹനുമാനെ ആരാധിക്കുക",
                "മംഗള മന്ത്രം ജപിക്കുക: ഓം അംഗാരകായ നമഃ",
                "ചൊവ്വാഴ്ചകളിൽ ചുവന്ന പരിപ്പ് ദാനം ചെയ്യുക",
                "കൂടിയാലോചന കഴിഞ്ഞ് ചുവന്ന പവിഴം ധരിക്കുക"
            }
        },
        ["Kaal Sarp Dosha"] = new()
        {
            ["English"] = new List<string>
            {
                "Visit Kaal Sarp Dosha temples",
                "Perform Rahu-Ketu puja on special days",
                "Recite Maha Mrityunjaya mantra",
                "Donate to snake sanctuaries",
                "Observe fasts on Nag Panchami"
            },
            ["Tamil"] = new List<string>
            {
                "கால சர்ப்ப தோஷ கோவில்களுக்குச் செல்லுங்கள்",
                "சிறப்பு நாட்களில் ராகு-கேது பூஜை செய்யுங்கள்",
                "மகா மிருத்யுஞ்சய மந்திரம் சொல்லுங்கள்",
                "பாம்பு சரணாலயங்களுக்கு தானம் செய்யுங்கள்",
                "நாக பஞ்சமியில் விரதம் இருங்கள்"
            },
            ["Telugu"] = new List<string>
            {
                "కాల సర్ప దోష దేవాలయాలను సందర్శించండి",
                "ప్రత్యేక రోజులలో రాహు-కేతు పూజ చేయండి",
                "మహా మృత్యుంజయ మంత్రం చదవండి",
                "పాము సంరక్షణాలయాలకు దానం చేయండి",
                "నాగ పంచమిలో ఉపవాసం ఉండండి"
            },
            ["Kannada"] = new List<string>
            {
                "ಕಾಲ ಸರ್ಪ ದೋಷ ದೇವಾಲಯಗಳಿಗೆ ಭೇಟಿ ನೀಡಿ",
                "ವಿಶೇಷ ದಿನಗಳಲ್ಲಿ ರಾಹು-ಕೇತು ಪೂಜೆ ಮಾಡಿ",
                "ಮಹಾ ಮೃತ್ಯುಂಜಯ ಮಂತ್ರ ಓದಿ",
                "ಹಾವು ಅಭಯಾರಣ್ಯಗಳಿಗೆ ದಾನ ಮಾಡಿ",
                "ನಾಗ ಪಂಚಮಿಯಲ್ಲಿ ಉಪವಾಸ ಮಾಡಿ"
            },
            ["Malayalam"] = new List<string>
            {
                "കാല സർപ്പ ദോഷ ക്ഷേത്രങ്ങൾ സന്ദർശിക്കുക",
                "പ്രത്യേക ദിവസങ്ങളിൽ രാഹു-കേതു പൂജ നടത്തുക",
                "മഹാ മൃത്യുഞ്ജയ മന്ത്രം ജപിക്കുക",
                "സർപ്പ സങ്കേതങ്ങൾക്ക് ദാനം നൽകുക",
                "നാഗപഞ്ചമിയിൽ ഉപവസിക്കുക"
            }
        },
        ["Pitra Dosha"] = new()
        {
            ["English"] = new List<string>
            {
                "Perform Shraddha rituals for ancestors",
                "Feed Brahmins on Amavasya",
                "Donate food on Saturdays",
                "Recite Gayatri mantra daily",
                "Plant a Peepal tree and water it regularly"
            },
            ["Tamil"] = new List<string>
            {
                "மூதாதையர்களுக்கு ஸ்ராத்த சடங்குகள் செய்யுங்கள்",
                "அமாவாசையில் பிராமணர்களுக்கு உணவளியுங்கள்",
                "சனிக்கிழமைகளில் உணவு தானம் செய்யுங்கள்",
                "தினமும் காயத்ரி மந்திரம் சொல்லுங்கள்",
                "அரச மரம் நட்டு தொடர்ந்து நீர் ஊற்றுங்கள்"
            },
            ["Telugu"] = new List<string>
            {
                "పూర్వీకుల కోసం శ్రాద్ధం చేయండి",
                "అమావాస్యలో బ్రాహ్మణులకు భోజనం పెట్టండి",
                "శనివారాలలో ఆహార దానం చేయండి",
                "ప్రతిరోజు గాయత్రీ మంత్రం చదవండి",
                "రావి చెట్టు నాటి నిత్యం నీళ్లు పోయండి"
            },
            ["Kannada"] = new List<string>
            {
                "ಪೂರ್ವಜರಿಗಾಗಿ ಶ್ರಾದ್ಧ ಕಾರ್ಯಗಳನ್ನು ಮಾಡಿ",
                "ಅಮಾವಾಸ್ಯೆಯಲ್ಲಿ ಬ್ರಾಹ್ಮಣರಿಗೆ ಅನ್ನ ದಾನ ಮಾಡಿ",
                "ಶನಿವಾರಗಳಲ್ಲಿ ಆಹಾರ ದಾನ ಮಾಡಿ",
                "ಪ್ರತಿದಿನ ಗಾಯತ್ರಿ ಮಂತ್ರ ಓದಿ",
                "ಅರಳಿ ಮರ ನೆಡಿ ನಿಯಮಿತವಾಗಿ ನೀರು ಹಾಕಿ"
            },
            ["Malayalam"] = new List<string>
            {
                "പിതൃക്കൾക്കായി ശ്രാദ്ധം നടത്തുക",
                "അമാവാസിയിൽ ബ്രാഹ്മണർക്ക് ഭക്ഷണം നൽകുക",
                "ശനിയാഴ്ചകളിൽ ഭക്ഷണ ദാനം ചെയ്യുക",
                "ദിവസേന ഗായത്രി മന്ത്രം ജപിക്കുക",
                "അരയാൽ നട്ട് പതിവായി നനയ്ക്കുക"
            }
        },
        ["Shakat Dosha"] = new()
        {
            ["English"] = new List<string>
            {
                "Worship Lord Vishnu on Thursdays",
                "Recite Guru mantra: Om Gurave Namaha",
                "Donate yellow items on Thursdays",
                "Wear yellow sapphire after consultation",
                "Feed cows regularly"
            },
            ["Tamil"] = new List<string>
            {
                "வியாழக்கிழமைகளில் விஷ்ணுவை வணங்குங்கள்",
                "குரு மந்திரம் சொல்லுங்கள்: ஓம் குரவே நம:",
                "வியாழக்கிழமைகளில் மஞ்சள் பொருட்கள் தானம் செய்யுங்கள்",
                "ஆலோசனைக்குப் பிறகு மஞ்சள் நீலம் அணியுங்கள்",
                "பசுக்களுக்கு தொடர்ந்து உணவளியுங்கள்"
            },
            ["Telugu"] = new List<string>
            {
                "గురువారాలలో విష్ణువును పూజించండి",
                "గురు మంత్రం చదవండి: ఓం గురవే నమః",
                "గురువారాలలో పసుపు వస్తువులు దానం చేయండి",
                "సలహా తర్వాత పసుపు రంగు నీలం ధరించండి",
                "ఆవులకు నిత్యం ఆహారం పెట్టండి"
            },
            ["Kannada"] = new List<string>
            {
                "ಗುರುವಾರಗಳಲ್ಲಿ ವಿಷ್ಣುವನ್ನು ಪೂಜಿಸಿ",
                "ಗುರು ಮಂತ್ರ ಓದಿ: ಓಂ ಗುರವೇ ನಮಃ",
                "ಗುರುವಾರಗಳಲ್ಲಿ ಹಳದಿ ವಸ್ತುಗಳನ್ನು ದಾನ ಮಾಡಿ",
                "ಸಲಹೆ ನಂತರ ಹಳದಿ ನೀಲಮಣಿ ಧರಿಸಿ",
                "ಹಸುಗಳಿಗೆ ನಿಯಮಿತವಾಗಿ ಆಹಾರ ನೀಡಿ"
            },
            ["Malayalam"] = new List<string>
            {
                "വ്യാഴാഴ്ചകളിൽ വിഷ്ണുവിനെ ആരാധിക്കുക",
                "ഗുരു മന്ത്രം ജപിക്കുക: ഓം ഗുരവേ നമഃ",
                "വ്യാഴാഴ്ചകളിൽ മഞ്ഞ വസ്തുക്കൾ ദാനം ചെയ്യുക",
                "കൂടിയാലോചന കഴിഞ്ഞ് മഞ്ഞ നീലക്കല്ല് ധരിക്കുക",
                "പശുക്കൾക്ക് പതിവായി ആഹാരം നൽകുക"
            }
        },
        ["Kemadruma Dosha"] = new()
        {
            ["English"] = new List<string>
            {
                "Wear pearl or moonstone after consultation",
                "Recite Chandra mantra on Mondays",
                "Donate white items on Mondays",
                "Worship Lord Shiva regularly",
                "Maintain good relationship with mother"
            },
            ["Tamil"] = new List<string>
            {
                "ஆலோசனைக்குப் பிறகு முத்து அல்லது சந்திர கல் அணியுங்கள்",
                "திங்கட்கிழமைகளில் சந்திர மந்திரம் சொல்லுங்கள்",
                "திங்கட்கிழமைகளில் வெள்ளை பொருட்கள் தானம் செய்யுங்கள்",
                "சிவனை தொடர்ந்து வணங்குங்கள்",
                "தாயுடன் நல்ல உறவை பேணுங்கள்"
            },
            ["Telugu"] = new List<string>
            {
                "సలహా తర్వాత ముత్యం లేదా చంద్రకాంత మణి ధరించండి",
                "సోమవారాలలో చంద్ర మంత్రం చదవండి",
                "సోమవారాలలో తెల్లని వస్తువులు దానం చేయండి",
                "శివుడిని నిత్యం పూజించండి",
                "తల్లితో మంచి సంబంధం ఉంచండి"
            },
            ["Kannada"] = new List<string>
            {
                "ಸಲಹೆ ನಂತರ ಮುತ್ತು ಅಥವಾ ಚಂದ್ರಕಾಂತ ಮಣಿ ಧರಿಸಿ",
                "ಸೋಮವಾರಗಳಲ್ಲಿ ಚಂದ್ರ ಮಂತ್ರ ಓದಿ",
                "ಸೋಮವಾರಗಳಲ್ಲಿ ಬಿಳಿ ವಸ್ತುಗಳನ್ನು ದಾನ ಮಾಡಿ",
                "ಶಿವನನ್ನು ನಿಯಮಿತವಾಗಿ ಪೂಜಿಸಿ",
                "ತಾಯಿಯೊಂದಿಗೆ ಉತ್ತಮ ಸಂಬಂಧ ಇಟ್ಟುಕೊಳ್ಳಿ"
            },
            ["Malayalam"] = new List<string>
            {
                "കൂടിയാലോചന കഴിഞ്ഞ് മുത്ത് അല്ലെങ്കിൽ ചന്ദ്രകാന്തം ധരിക്കുക",
                "തിങ്കളാഴ്ചകളിൽ ചന്ദ്ര മന്ത്രം ജപിക്കുക",
                "തിങ്കളാഴ്ചകളിൽ വെള്ള വസ്തുക്കൾ ദാനം ചെയ്യുക",
                "ശിവനെ പതിവായി ആരാധിക്കുക",
                "അമ്മയുമായി നല്ല ബന്ധം നിലനിർത്തുക"
            }
        }
    };

    /// <summary>
    /// Get localized yoga description
    /// </summary>
    public static string GetYogaDescription(string yogaName, string language, params object[] args)
    {
        if (YogaDescriptions.ContainsKey(yogaName) && YogaDescriptions[yogaName].ContainsKey(language))
        {
            var description = YogaDescriptions[yogaName][language];
            return args.Length > 0 ? string.Format(description, args) : description;
        }
        return yogaName;
    }

    /// <summary>
    /// Get localized dosa description
    /// </summary>
    public static string GetDosaDescription(string dosaName, string language, params object[] args)
    {
        if (DosaDescriptions.ContainsKey(dosaName) && DosaDescriptions[dosaName].ContainsKey(language))
        {
            var description = DosaDescriptions[dosaName][language];
            return args.Length > 0 ? string.Format(description, args) : description;
        }
        return dosaName;
    }

    /// <summary>
    /// Get localized dosa remedies
    /// </summary>
    public static List<string> GetDosaRemedies(string dosaName, string language)
    {
        if (DosaRemedies.ContainsKey(dosaName) && DosaRemedies[dosaName].ContainsKey(language))
        {
            return DosaRemedies[dosaName][language];
        }
        return new List<string>();
    }
}
