using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Services.Implementations;

/// <summary>
/// Custom authentication service without dependencies on ASP.NET Identity
/// Implements proper transaction handling compatible with retry strategies
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(ApplicationDbContext context, ILogger<AuthenticationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user with execution strategy compatible transaction handling
    /// </summary>
    public async Task<(bool Success, string Message, User? User)> RegisterUserAsync(
        string email,
        string? mobileNumber,
        string fullName,
        string password)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(mobileNumber))
        {
            return (false, "Either email or mobile number must be provided.", null);
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            return (false, "Full name is required.", null);
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            return (false, "Password must be at least 8 characters long.", null);
        }

        // Use the execution strategy to handle retries and transactions properly
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            try
            {
                // Normalize inputs
                var normalizedEmail = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLower();
                var normalizedMobile = string.IsNullOrWhiteSpace(mobileNumber) ? null : mobileNumber.Trim();

                // Check email exists
                if (!string.IsNullOrWhiteSpace(normalizedEmail))
                {
                    var existingEmailUser = await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

                    if (existingEmailUser != null)
                    {
                        return (false, "This email is already registered.", null);
                    }
                }

                // Check mobile exists
                if (!string.IsNullOrWhiteSpace(normalizedMobile))
                {
                    var existingMobileUser = await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.MobileNumber == normalizedMobile);

                    if (existingMobileUser != null)
                    {
                        return (false, "This mobile number is already registered.", null);
                    }
                }

                // Create new user
                var user = new User
                {
                    Email = normalizedEmail,
                    MobileNumber = normalizedMobile,
                    FullName = fullName.Trim(),
                    PasswordHash = HashPassword(password),
                    CreatedDate = DateTime.UtcNow,
                    IsEmailVerified = false,
                    IsMobileVerified = false,
                    IsActive = true,
                    TrialStartDate = DateTime.UtcNow,
                    TrialEndDate = DateTime.UtcNow.AddDays(30),
                    IsTrialActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created with UserId: {UserId}", user.UserId);

                // Create wallet for the user
                var wallet = new Wallet
                {
                    UserId = user.UserId,
                    Balance = 0.00m,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow
                };

                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Wallet created for UserId: {UserId}", user.UserId);
                _logger.LogInformation("User registration completed successfully for UserId: {UserId}", user.UserId);

                return (true, "User registered successfully.", user);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx &&
                (sqlEx.Number == 2627 || sqlEx.Number == 2601)) // Unique constraint violation
            {
                _logger.LogWarning(dbEx, "User already exists during registration");
                return (false, "This email or mobile number is already registered.", null);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error during user registration");
                return (false, "A database error occurred during registration. Please try again.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user registration");
                return (false, "An unexpected error occurred. Please try again.", null);
            }
        });
    }

    /// <summary>
    /// Authenticate user by email or mobile number
    /// </summary>
    public async Task<(bool Success, User? User)> AuthenticateAsync(string emailOrMobile, string password)
    {
        if (string.IsNullOrWhiteSpace(emailOrMobile) || string.IsNullOrWhiteSpace(password))
        {
            return (false, null);
        }

        try
        {
            var normalizedInput = emailOrMobile.Trim().ToLower();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == normalizedInput || u.MobileNumber == emailOrMobile.Trim());

            if (user == null)
            {
                _logger.LogWarning("Authentication failed: User not found for: {EmailOrMobile}", emailOrMobile);
                return (false, null);
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning("Authentication failed: Invalid password for UserId: {UserId}", user.UserId);
                return (false, null);
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Authentication failed: Account inactive for UserId: {UserId}", user.UserId);
                return (false, null);
            }

            // Update last login date using execution strategy
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    var userToUpdate = await _context.Users.FindAsync(user.UserId);
                    if (userToUpdate != null)
                    {
                        userToUpdate.LastLoginDate = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating last login date for UserId: {UserId}", user.UserId);
                    // Don't fail authentication if we can't update last login
                }
            });

            _logger.LogInformation("Authentication successful for UserId: {UserId}", user.UserId);
            return (true, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during authentication");
            return (false, null);
        }
    }

    /// <summary>
    /// Verify user by ID (for session/token validation)
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// Hash password using SHA256
    /// </summary>
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    /// <summary>
    /// Verify password against hash
    /// </summary>
    private static bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}