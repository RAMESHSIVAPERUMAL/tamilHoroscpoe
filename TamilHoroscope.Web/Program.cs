using Microsoft.EntityFrameworkCore;
using TamilHoroscope.Web.Data;
using TamilHoroscope.Web.Services.Implementations;
using TamilHoroscope.Web.Services.Interfaces;

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

// Register BirthPlace service as singleton (cached data)
builder.Services.AddSingleton<TamilHoroscope.Web.Services.BirthPlaceService>();

// Add Controllers for API endpoints
builder.Services.AddControllers();

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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); // Enable API controllers

app.Run();