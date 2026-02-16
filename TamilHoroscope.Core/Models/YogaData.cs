using TamilHoroscope.Core.Data;

namespace TamilHoroscope.Core.Models;

/// <summary>
/// Represents an astrological yoga (planetary combination) detected in a horoscope
/// </summary>
public class YogaData
{
    /// <summary>
    /// Name of the yoga in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Name of the yoga in the selected regional language
    /// </summary>
    public string LocalName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the yoga and its effects (English only, deprecated - use LocalizedDescription)
    /// </summary>
    [Obsolete("Use LocalizedDescription property instead for multi-language support")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Language for localization (Tamil, Telugu, Kannada, Malayalam, English)
    /// </summary>
    public string Language { get; set; } = "English";

    /// <summary>
    /// Arguments for parameterized descriptions (e.g., planet names)
    /// </summary>
    public object[] DescriptionArgs { get; set; } = Array.Empty<object>();

    /// <summary>
    /// Gets the localized description based on the Language property
    /// </summary>
    public string LocalizedDescription
    {
        get
        {
            // If Description is set (backward compatibility), return it
            #pragma warning disable CS0618 // Type or member is obsolete
            if (!string.IsNullOrEmpty(Description))
            {
                return Description;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            // Otherwise, try to get from LocalizedWordings dictionary
            return LocalizedWordings.GetYogaDescription(Name, Language, DescriptionArgs);
        }
    }

    /// <summary>
    /// Planets involved in forming the yoga
    /// </summary>
    public List<string> InvolvedPlanets { get; set; } = new();

    /// <summary>
    /// Houses involved in forming the yoga
    /// </summary>
    public List<int> InvolvedHouses { get; set; } = new();

    /// <summary>
    /// Whether the yoga is beneficial (true) or malefic (false)
    /// </summary>
    public bool IsBeneficial { get; set; }

    /// <summary>
    /// Strength of the yoga (1-10 scale, 10 being strongest)
    /// </summary>
    public int Strength { get; set; }
}
