namespace TamilHoroscope.Web.Services.Interfaces;

/// <summary>
/// Service for trial and subscription management
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Checks if a user is currently in trial period
    /// </summary>
    Task<bool> IsUserInTrialAsync(int userId);

    /// <summary>
    /// Gets the number of days remaining in trial period
    /// </summary>
    Task<int> GetTrialDaysRemainingAsync(int userId);

    /// <summary>
    /// Checks if a user has an active subscription (wallet balance)
    /// </summary>
    Task<bool> HasActiveSubscriptionAsync(int userId);

    /// <summary>
    /// Gets the number of days remaining based on wallet balance
    /// </summary>
    Task<int> GetWalletDaysRemainingAsync(int userId);

    /// <summary>
    /// Checks if user should see low balance warning
    /// </summary>
    Task<bool> ShouldShowLowBalanceWarningAsync(int userId);

    /// <summary>
    /// Deactivates trial period for a user
    /// </summary>
    Task DeactivateTrialAsync(int userId);

    /// <summary>
    /// Checks and updates trial period based on wallet balance and last fee deduction
    /// Activates 30-day trial if balance is zero and last deduction is not today
    /// </summary>
    Task CheckAndUpdateTrialStatusAsync(int userId);
}
