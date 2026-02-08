using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Services.Implementations;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<(bool Success, string Message, User? User)> RegisterUserAsync(
        string email,
        string? mobileNumber,
        string fullName,
        string password);

    /// <summary>
    /// Authenticate user
    /// </summary>
    Task<(bool Success, User? User)> AuthenticateAsync(string emailOrMobile, string password);

    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<User?> GetUserByIdAsync(int userId);
}
