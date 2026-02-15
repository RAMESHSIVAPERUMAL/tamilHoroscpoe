using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PdfExportController : ControllerBase
{
    private readonly IPdfExportService _pdfExportService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<PdfExportController> _logger;

    public PdfExportController(
        IPdfExportService pdfExportService,
        ISubscriptionService subscriptionService,
        ILogger<PdfExportController> logger)
    {
        _pdfExportService = pdfExportService;
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    [HttpPost("export")]
    [RequestSizeLimit(100_000_000)] // 100 MB limit for PDF export with section images
    [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)] // For IIS compatibility
    public async Task<IActionResult> ExportPdf([FromBody] PdfExportRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isTrialUser = await _subscriptionService.IsUserInTrialAsync(userId);

            // Deserialize horoscope data
            var horoscope = JsonSerializer.Deserialize<HoroscopeData>(request.HoroscopeJson);
            
            if (horoscope == null)
            {
                return BadRequest(new { error = "Invalid horoscope data" });
            }

            byte[] pdfBytes;
            
            // PRIORITY 1: Section-by-section mode (BEST QUALITY)
            if (request.UseSectionMode && request.SectionImages != null && request.SectionImages.Any())
            {
                _logger.LogInformation("Generating PDF with section-by-section capture for user {UserId}", userId);
                pdfBytes = _pdfExportService.GeneratePdfWithSections(
                    horoscope,
                    request.PersonName,
                    request.Language,
                    isTrialUser,
                    request.SectionImages);
            }
            // PRIORITY 2: Full-page mode
            else if (request.UseFullPageMode && !string.IsNullOrEmpty(request.FullPageImage))
            {
                _logger.LogInformation("Generating PDF with full-page capture for user {UserId}", userId);
                pdfBytes = _pdfExportService.GeneratePdfWithFullPageImage(
                    horoscope, 
                    request.PersonName, 
                    request.Language, 
                    isTrialUser,
                    request.FullPageImage);
            }
            // FALLBACK: Chart images mode
            else
            {
                _logger.LogInformation("Generating PDF with chart images for user {UserId}", userId);
                pdfBytes = _pdfExportService.GeneratePdfWithChartImages(
                    horoscope, 
                    request.PersonName, 
                    request.Language, 
                    isTrialUser,
                    request.RasiChartImage,
                    request.NavamsaChartImage);
            }
                
            var filename = _pdfExportService.GetSuggestedFilename(request.PersonName);

            _logger.LogInformation("PDF exported successfully for user {UserId}, person: {PersonName}, filename: {Filename}", 
                userId, request.PersonName, filename);

            // Return PDF with properly formatted Content-Disposition header
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{filename}\"");
            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting PDF");
            return StatusCode(500, new { error = "Failed to generate PDF", details = ex.Message });
        }
    }
}

public class PdfExportRequest
{
    public string HoroscopeJson { get; set; } = string.Empty;
    public string PersonName { get; set; } = string.Empty;
    public string Language { get; set; } = "Tamil";
    public string? RasiChartImage { get; set; } // Base64 PNG image
    public string? NavamsaChartImage { get; set; } // Base64 PNG image
    public string? FullPageImage { get; set; } // Full page capture (fallback)
    public bool UseFullPageMode { get; set; } = false; // Flag for full-page rendering
    
    // NEW: Section-by-section export (BEST QUALITY)
    public bool UseSectionMode { get; set; } = false; // Flag for section-by-section export
    public Dictionary<string, string>? SectionImages { get; set; } // Section name -> Base64 image
}
