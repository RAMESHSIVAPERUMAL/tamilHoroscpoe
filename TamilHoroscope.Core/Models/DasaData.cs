namespace TamilHoroscope.Core.Models;

/// <summary>
/// Represents a Vimshottari Dasa period
/// </summary>
public class DasaData
{
    /// <summary>
    /// Name of the Dasa lord (planet)
    /// </summary>
    public string Lord { get; set; } = string.Empty;

    /// <summary>
    /// Tamil name of the Dasa lord
    /// </summary>
    public string TamilLord { get; set; } = string.Empty;

    /// <summary>
    /// Start date of the Dasa period
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date of the Dasa period
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Duration of the Dasa in years
    /// </summary>
    public int DurationYears { get; set; }

    /// <summary>
    /// List of Bhukti (sub-periods) within this Dasa
    /// </summary>
    public List<BhuktiData> Bhuktis { get; set; } = new();
}
