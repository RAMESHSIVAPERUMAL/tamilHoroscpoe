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
}
