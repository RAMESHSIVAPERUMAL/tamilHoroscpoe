namespace TamilHoroscope.Web.Data.Entities;

/// <summary>
/// Represents system configuration parameters
/// </summary>
public class SystemConfig
{
    /// <summary>
    /// Unique identifier for the configuration
    /// </summary>
    public int ConfigId { get; set; }

    /// <summary>
    /// Configuration key (unique)
    /// </summary>
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// Configuration value
    /// </summary>
    public string ConfigValue { get; set; } = string.Empty;

    /// <summary>
    /// Description of the configuration parameter
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Data type: decimal, int, string, or bool
    /// </summary>
    public string DataType { get; set; } = "string";

    /// <summary>
    /// Date when the configuration was last modified
    /// </summary>
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the configuration is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
