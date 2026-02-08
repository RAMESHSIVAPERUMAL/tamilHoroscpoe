using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IWalletService _walletService;
    private readonly IConfigService _configService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IWalletService walletService,
        IConfigService configService,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _walletService = walletService;
        _configService = configService;
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
        
        if (ModelState.IsValid)
        {
            // Validate that either email or mobile is provided
            var hasEmail = !string.IsNullOrWhiteSpace(Input.Email);
            var hasMobile = !string.IsNullOrWhiteSpace(Input.MobileNumber);

            if (!hasEmail && !hasMobile)
            {
                ModelState.AddModelError(string.Empty, "Either email or mobile number must be provided.");
                return Page();
            }

            // Check if email already exists (if provided)
            if (hasEmail)
            {
                var existingEmailUser = await _userManager.FindByEmailAsync(Input.Email!);
                if (existingEmailUser != null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered.");
                    return Page();
                }
            }

            // Check if mobile number already exists (if provided)
            if (hasMobile)
            {
                var existingMobileUser = _userManager.Users
                    .FirstOrDefault(u => u.MobileNumber == Input.MobileNumber);
                if (existingMobileUser != null)
                {
                    ModelState.AddModelError(string.Empty, "This mobile number is already registered.");
                    return Page();
                }
            }

            // Get trial period configuration
            var trialPeriodDays = await _configService.GetTrialPeriodDaysAsync();

            // Create new user
            var user = new User
            {
                UserName = hasEmail ? Input.Email : Input.MobileNumber, // Use email or mobile as username
                Email = hasEmail ? Input.Email : null,
                MobileNumber = hasMobile ? Input.MobileNumber : null,
                FullName = Input.FullName,
                CreatedDate = DateTime.UtcNow,
                IsEmailVerified = false,
                IsMobileVerified = false,
                IsActive = true,
                TrialStartDate = DateTime.UtcNow,
                TrialEndDate = DateTime.UtcNow.AddDays(trialPeriodDays),
                IsTrialActive = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created successfully with trial period of {Days} days", trialPeriodDays);

                // Create wallet for the new user
                await _walletService.GetOrCreateWalletAsync(user.Id);

                _logger.LogInformation("Wallet created for user {UserId}", user.Id);

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);

                _logger.LogInformation("User signed in successfully");

                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }
}
