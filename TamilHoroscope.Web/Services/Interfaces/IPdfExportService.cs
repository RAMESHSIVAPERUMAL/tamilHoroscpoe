using TamilHoroscope.Core.Models;

namespace TamilHoroscope.Web.Services.Interfaces;

/// <summary>
/// Service for exporting horoscope data to PDF format
/// </summary>
public interface IPdfExportService
{
    /// <summary>
    /// Generates a PDF document from horoscope data
    /// </summary>
    /// <param name="horoscope">The horoscope data to export</param>
    /// <param name="personName">Name of the person</param>
    /// <param name="language">Display language (Tamil, Telugu, Kannada, Malayalam)</param>
    /// <param name="isTrialUser">Whether the user is in trial mode</param>
    /// <returns>PDF file as byte array</returns>
    byte[] GeneratePdf(HoroscopeData horoscope, string personName, string language, bool isTrialUser);

    /// <summary>
    /// Generates a PDF document from horoscope data with embedded chart images
    /// </summary>
    /// <param name="horoscope">The horoscope data to export</param>
    /// <param name="personName">Name of the person</param>
    /// <param name="language">Display language (Tamil, Telugu, Kannada, Malayalam)</param>
    /// <param name="isTrialUser">Whether the user is in trial mode</param>
    /// <param name="rasiChartImage">Base64-encoded PNG image of Rasi chart</param>
    /// <param name="navamsaChartImage">Base64-encoded PNG image of Navamsa chart (optional)</param>
    /// <returns>PDF file as byte array</returns>
    byte[] GeneratePdfWithChartImages(HoroscopeData horoscope, string personName, string language, bool isTrialUser, string? rasiChartImage, string? navamsaChartImage);

    /// <summary>
    /// Generates a PDF document with full-page capture (entire horoscope results as image)
    /// </summary>
    /// <param name="horoscope">The horoscope data to export</param>
    /// <param name="personName">Name of the person</param>
    /// <param name="language">Display language</param>
    /// <param name="isTrialUser">Whether the user is in trial mode</param>
    /// <param name="fullPageImage">Base64-encoded PNG image of entire horoscope page</param>
    /// <returns>PDF file as byte array</returns>
    byte[] GeneratePdfWithFullPageImage(HoroscopeData horoscope, string personName, string language, bool isTrialUser, string fullPageImage);

    /// <summary>
    /// Generates a PDF document with section-by-section images (one section per PDF page)
    /// This approach provides better readability than full-page capture
    /// </summary>
    /// <param name="horoscope">The horoscope data to export</param>
    /// <param name="personName">Name of the person</param>
    /// <param name="language">Display language</param>
    /// <param name="isTrialUser">Whether the user is in trial mode</param>
    /// <param name="sectionImages">Dictionary of section names to base64-encoded PNG images</param>
    /// <returns>PDF file as byte array</returns>
    byte[] GeneratePdfWithSections(HoroscopeData horoscope, string personName, string language, bool isTrialUser, Dictionary<string, string> sectionImages);

    /// <summary>
    /// Gets the suggested filename for the PDF export
    /// </summary>
    /// <param name="personName">Name of the person</param>
    /// <returns>Suggested filename</returns>
    string GetSuggestedFilename(string personName);
}
