using TamilHoroscope.Core.Data;

namespace TamilHoroscope.Core.Models;

/// <summary>
/// Represents a Bhukti (sub-period) within a Dasa
/// </summary>
public class BhuktiData
{
    /// <summary>
    /// Name of the Bhukti lord (planet)
    /// </summary>
    public string Lord { get; set; } = string.Empty;

    /// <summary>
    /// Language for localized names (English, Tamil, Telugu, Kannada, Malayalam)
    /// </summary>
    public string Language { get; set; } = "English";

    /// <summary>
    /// Tamil name of the Bhukti lord (deprecated - use LocalizedLord)
    /// </summary>
    [Obsolete("Use LocalizedLord property instead")]
    public string TamilLord { get; set; } = string.Empty;

    /// <summary>
    /// Localized name of the Bhukti lord based on Language property
    /// </summary>
    public string LocalizedLord => LocalizedWordings.GetPlanetName(Lord, Language);

    /// <summary>
    /// Start date of the Bhukti period
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date of the Bhukti period
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Duration of the Bhukti in days
    /// </summary>
    public double DurationDays { get; set; }
}
