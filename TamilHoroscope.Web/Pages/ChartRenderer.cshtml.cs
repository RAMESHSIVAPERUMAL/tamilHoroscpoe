using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TamilHoroscope.Web.Pages;

public class ChartRendererModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string ChartDataJson { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string ChartType { get; set; } = "rasi"; // "rasi" or "navamsa"

    [BindProperty(SupportsGet = true)]
    public string Language { get; set; } = "Tamil";

    public void OnGet()
    {
        // Page model is set from query parameters
    }
}
