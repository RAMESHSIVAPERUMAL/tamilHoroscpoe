using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TamilHoroscope.Web.Services.Implementations;

namespace TamilHoroscope.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        IAuthenticationService authService,
        ILogger<LoginModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Email or mobile number is required")]
        [Display(Name = "Email or Mobile Number")]
        public string EmailOrMobile { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Authenticate user
            var (success, user) = await _authService.AuthenticateAsync(Input.EmailOrMobile, Input.Password);

            if (!success || user == null)
            {
                _logger.LogWarning("Failed login attempt for: {EmailOrMobile}", Input.EmailOrMobile);
                ModelState.AddModelError(string.Empty, "Invalid email/mobile or password.");
                return Page();
            }

            // Store user ID in session
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email ?? string.Empty);
            HttpContext.Session.SetString("UserFullName", user.FullName);

            _logger.LogInformation("User {UserId} logged in successfully", user.UserId);

            // Validate and redirect to return URL or home page
            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for: {EmailOrMobile}", Input.EmailOrMobile);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return Page();
        }
    }
}
