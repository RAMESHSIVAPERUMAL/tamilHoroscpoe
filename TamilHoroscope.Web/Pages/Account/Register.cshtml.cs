using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AuthService = TamilHoroscope.Web.Services.Interfaces.IAuthenticationService;

namespace TamilHoroscope.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly AuthService _authService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        AuthService authService,
        ILogger<RegisterModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email (Optional)")]
        public string? Email { get; set; }

        [Display(Name = "Mobile Number (Optional)")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit Indian mobile number starting with 6-9")]
        public string? MobileNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The password must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Validate that either email or mobile is provided
        var hasEmail = !string.IsNullOrWhiteSpace(Input.Email);
        var hasMobile = !string.IsNullOrWhiteSpace(Input.MobileNumber);

        if (!hasEmail && !hasMobile)
        {
            ModelState.AddModelError(string.Empty, "Either email or mobile number must be provided.");
            return Page();
        }

        try
        {
            // Register user using authentication service
            // Service handles transaction management internally
            var (success, message, user) = await _authService.RegisterUserAsync(
                Input.Email ?? string.Empty,
                Input.MobileNumber,
                Input.FullName,
                Input.Password);

            if (!success || user == null)
            {
                ModelState.AddModelError(string.Empty, message);
                _logger.LogWarning("Registration failed: {Message}", message);
                return Page();
            }

            _logger.LogInformation("User registered successfully: UserId={UserId}", user.UserId);

            // TODO: Implement session/cookie-based authentication
            // For now, redirect to login page
            return LocalRedirect("/Account/Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            return Page();
        }
    }
}
