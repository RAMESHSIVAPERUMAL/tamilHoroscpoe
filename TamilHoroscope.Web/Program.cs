using Microsoft.EntityFrameworkCore;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Services.Implementations;
using TamilHoroscope.Web.Services.Interfaces;
using TamilHoroscope.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure Entity Framework Core with SQL Server (Database-First approach)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        // Fix: Use the correct EnableRetryOnFailure overload with TimeSpan for maxRetryDelay
        Microsoft.EntityFrameworkCore.Infrastructure.SqlServerDbContextOptionsBuilder sqlServerDbContextOptionsBuilder =
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            );
    });

    // Enable query logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(true);
    }
});

// Add cookie-based authentication
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// Register custom authentication service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Register application services
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IHoroscopeService, HoroscopeService>();
builder.Services.AddScoped<IPdfExportService, PdfExportService>();

// Register BirthPlace service as singleton (cached data)
builder.Services.AddSingleton<TamilHoroscope.Web.Services.BirthPlaceService>();

// Configure Kestrel server limits for large PDF export requests (section images)
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    // Increase max request body size to 100MB for PDF export with multiple section images
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
});

// Configure IIS server limits (if hosted on IIS)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Add Controllers for API endpoints with increased JSON size limit
builder.Services.AddControllers(options =>
{
    // Increase form value count limit for complex requests
    options.MaxModelBindingCollectionSize = 10000;
})
.AddJsonOptions(options =>
{
    // Increase JSON size limit to handle large base64 images
    options.JsonSerializerOptions.MaxDepth = 64;
});

// Add Razor Pages
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddLogging();

var app = builder.Build();

// Database-First Approach: No automatic migrations
// Database is managed via SQL scripts in DatabaseSetup folder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Just verify database connection - DO NOT run migrations
        var canConnect = context.Database.CanConnect();
        
        if (canConnect)
        {
            app.Logger.LogInformation("? Database connection successful");
        }
        else
        {
            app.Logger.LogWarning("? Cannot connect to database. Check connection string.");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error connecting to database");
        throw;
    }
}

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session MUST come before middlewares that use it
app.UseSession();

// Add rate limiting middleware (AFTER session)
app.UseMiddleware<RateLimitingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); // Enable API controllers

app.Run();