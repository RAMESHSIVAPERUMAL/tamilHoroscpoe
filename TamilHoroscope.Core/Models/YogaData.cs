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
    /// Description of the yoga and its effects
    /// </summary>
    public string Description { get; set; } = string.Empty;

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
