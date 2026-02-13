using Microsoft.AspNetCore.Mvc;
using TamilHoroscope.Web.Services;

namespace TamilHoroscope.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BirthPlaceController : ControllerBase
{
    private readonly BirthPlaceService _birthPlaceService;
    private readonly ILogger<BirthPlaceController> _logger;

    public BirthPlaceController(BirthPlaceService birthPlaceService, ILogger<BirthPlaceController> logger)
    {
        _birthPlaceService = birthPlaceService;
        _logger = logger;
    }

    /// <summary>
    /// Search birth places by name
    /// </summary>
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return Ok(new { places = Array.Empty<object>() });
            }

            var places = _birthPlaceService.SearchPlaces(q);
            
            var result = places.Select(p => new
            {
                name = p.Name,
                displayName = p.DisplayName,
                latitude = p.Latitude,
                longitude = p.Longitude,
                timeZone = p.TimeZone,
                state = p.State,
                country = p.Country
            });

            return Ok(new { places = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching birth places for query: {Query}", q);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get place by exact name
    /// </summary>
    [HttpGet("get/{name}")]
    public IActionResult GetByName(string name)
    {
        try
        {
            var place = _birthPlaceService.GetPlaceByName(name);
            
            if (place == null)
            {
                return NotFound(new { error = "Place not found" });
            }

            return Ok(new
            {
                name = place.Name,
                displayName = place.DisplayName,
                latitude = place.Latitude,
                longitude = place.Longitude,
                timeZone = place.TimeZone,
                state = place.State,
                country = place.Country
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting birth place: {Name}", name);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
