namespace TamilHoroscope.Web.Data.Entities;

/// <summary>
/// Represents a horoscope generation record for daily deduction tracking
/// </summary>
public class HoroscopeGeneration
{
    /// <summary>
    /// Unique identifier for the generation record
    /// </summary>
    public int GenerationId { get; set; }

    /// <summary>
    /// Foreign key to the User
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Date of generation (date only, no time) for daily tracking
    /// </summary>
    public DateTime GenerationDate { get; set; }

    /// <summary>
    /// Person name for whom the horoscope was generated
    /// </summary>
    public string? PersonName { get; set; }

    /// <summary>
    /// Birth date and time for the horoscope
    /// </summary>
    public DateTime BirthDateTime { get; set; }

    /// <summary>
    /// Place name where the person was born
    /// </summary>
    public string? PlaceName { get; set; }

    /// <summary>
    /// Latitude of the birth location
    /// </summary>
    public decimal Latitude { get; set; }

    /// <summary>
    /// Longitude of the birth location
    /// </summary>
    public decimal Longitude { get; set; }

    /// <summary>
    /// Amount deducted from wallet (0 for trial users)
    /// </summary>
    public decimal AmountDeducted { get; set; }

    /// <summary>
    /// Whether this generation was during trial period
    /// </summary>
    public bool WasTrialPeriod { get; set; }

    /// <summary>
    /// Date and time when the record was created
    /// </summary>
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
