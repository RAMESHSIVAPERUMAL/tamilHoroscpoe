using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

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

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            // Try to find user by email or mobile number
            User? user = null;

            // Check if input looks like an email
            if (Input.EmailOrMobile.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(Input.EmailOrMobile);
            }
            else
            {
                // Try to find by mobile number
                user = _userManager.Users
                    .FirstOrDefault(u => u.MobileNumber == Input.EmailOrMobile);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Check if user is active
            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact support.");
                return Page();
            }

            // Attempt to sign in
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Update last login date
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("User {UserId} logged in successfully", user.Id);
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserId} account locked out", user.Id);
                ModelState.AddModelError(string.Empty, "Your account has been locked due to multiple failed login attempts. Please try again later.");
                return Page();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }

        return Page();
    }
}
