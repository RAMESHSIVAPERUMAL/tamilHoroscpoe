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
    /// Tamil name of the Bhukti lord
    /// </summary>
    public string TamilLord { get; set; } = string.Empty;

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
