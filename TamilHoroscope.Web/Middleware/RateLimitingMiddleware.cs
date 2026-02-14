using System.Collections.Concurrent;

namespace TamilHoroscope.Web.Middleware;

/// <summary>
/// Middleware to implement rate limiting per user/IP to prevent abuse
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    
    // Track request counts per user/IP
    private static readonly ConcurrentDictionary<string, RequestTracker> _requestTrackers = new();
    private static readonly object _cleanupLock = new();
    private static DateTime _lastCleanup = DateTime.UtcNow;

    // Configuration
    private const int MaxRequestsPerMinute = 10;
    private const int MaxRequestsPerHour = 100;
    private const int CleanupIntervalMinutes = 10;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for static files
        if (context.Request.Path.StartsWithSegments("/lib") || 
            context.Request.Path.StartsWithSegments("/css") || 
            context.Request.Path.StartsWithSegments("/js"))
        {
            await _next(context);
            return;
        }

        // Get identifier (UserId or IP)
        var identifier = GetIdentifier(context);
        
        // Periodic cleanup of old trackers
        PeriodicCleanup();

        // Get or create tracker
        var tracker = _requestTrackers.GetOrAdd(identifier, _ => new RequestTracker());

        // Check rate limits
        if (!tracker.AllowRequest())
        {
            _logger.LogWarning("Rate limit exceeded for {Identifier} on path {Path}", identifier, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers.Add("Retry-After", "60");
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        await _next(context);
    }

    private string GetIdentifier(HttpContext context)
    {
        try
        {
            // Try to get UserId from session first
            var userId = context.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                return $"User_{userId}";
            }
        }
        catch (InvalidOperationException)
        {
            // Session not available - fall through to IP address
            _logger.LogDebug("Session not available for rate limiting, using IP address");
        }

        // Fall back to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        return $"IP_{ipAddress}";
    }

    private void PeriodicCleanup()
    {
        if ((DateTime.UtcNow - _lastCleanup).TotalMinutes < CleanupIntervalMinutes)
        {
            return;
        }

        lock (_cleanupLock)
        {
            if ((DateTime.UtcNow - _lastCleanup).TotalMinutes < CleanupIntervalMinutes)
            {
                return;
            }

            var expiredKeys = _requestTrackers
                .Where(kvp => kvp.Value.IsExpired())
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _requestTrackers.TryRemove(key, out _);
            }

            _lastCleanup = DateTime.UtcNow;
        }
    }

    private class RequestTracker
    {
        private readonly object _lock = new();
        private readonly Queue<DateTime> _requests = new();
        private DateTime _lastRequest = DateTime.UtcNow;

        public bool AllowRequest()
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                _lastRequest = now;

                // Remove requests older than 1 hour
                while (_requests.Count > 0 && (now - _requests.Peek()).TotalHours > 1)
                {
                    _requests.Dequeue();
                }

                // Check hourly limit
                if (_requests.Count >= MaxRequestsPerHour)
                {
                    return false;
                }

                // Check per-minute limit
                var recentRequests = _requests.Count(r => (now - r).TotalMinutes < 1);
                if (recentRequests >= MaxRequestsPerMinute)
                {
                    return false;
                }

                // Add current request
                _requests.Enqueue(now);
                return true;
            }
        }

        public bool IsExpired()
        {
            return (DateTime.UtcNow - _lastRequest).TotalHours > 2;
        }
    }
}
