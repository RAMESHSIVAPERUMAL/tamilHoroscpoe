using TamilHoroscope.Core.Data;

namespace TamilHoroscope.Core.Models;

/// <summary>
/// Represents an astrological dosha (affliction) detected in a horoscope
/// </summary>
public class DosaData
{
    /// <summary>
    /// Name of the dosha in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Name of the dosha in the selected regional language
    /// </summary>
    public string LocalName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the dosha and its effects (English only, deprecated - use LocalizedDescription)
    /// </summary>
    [Obsolete("Use LocalizedDescription property instead for multi-language support")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Language for localization (Tamil, Telugu, Kannada, Malayalam, English)
    /// </summary>
    public string Language { get; set; } = "Tamil";

    /// <summary>
    /// Arguments for parameterized descriptions (e.g., house numbers, reasons)
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
            if (!string.IsNullOrEmpty(Description) && Language == "Tamil")
            {
                return Description;
            }
            
            // Otherwise, try to get from TamilNames dictionary
            return TamilNames.GetDosaDescription(Name, Language, DescriptionArgs);
        }
    }

    /// <summary>
    /// Planets involved in forming the dosha
    /// </summary>
    public List<string> InvolvedPlanets { get; set; } = new();

    /// <summary>
    /// Houses involved in forming the dosha
    /// </summary>
    public List<int> InvolvedHouses { get; set; } = new();

    /// <summary>
    /// Severity of the dosha (1-10 scale, 10 being most severe)
    /// </summary>
    public int Severity { get; set; }

    /// <summary>
    /// Suggested remedies for the dosha (English only, deprecated - use LocalizedRemedies)
    /// </summary>
    [Obsolete("Use LocalizedRemedies property instead for multi-language support")]
    public List<string> Remedies { get; set; } = new();

    /// <summary>
    /// Gets the localized remedies based on the Language property
    /// </summary>
    public List<string> LocalizedRemedies
    {
        get
        {
            // If Remedies is set (backward compatibility), return it
            if (Remedies != null && Remedies.Any() && Language == "Tamil")
            {
                return Remedies;
            }
            
            // Otherwise, try to get from TamilNames dictionary
            return TamilNames.GetDosaRemedies(Name, Language);
        }
    }
}
