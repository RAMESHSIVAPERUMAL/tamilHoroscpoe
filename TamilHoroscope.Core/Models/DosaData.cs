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
    /// Description of the dosha and its effects
    /// </summary>
    public string Description { get; set; } = string.Empty;

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
    /// Suggested remedies for the dosha
    /// </summary>
    public List<string> Remedies { get; set; } = new();
}
