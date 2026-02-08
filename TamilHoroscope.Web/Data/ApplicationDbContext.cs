using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TamilHoroscope.Web.Data.Configurations;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Data;

/// <summary>
/// Application database context with Identity and custom entities
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for custom entities
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<HoroscopeGeneration> HoroscopeGenerations => Set<HoroscopeGeneration>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new WalletConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new HoroscopeGenerationConfiguration());
        modelBuilder.ApplyConfiguration(new SystemConfigConfiguration());

        // Customize Identity table names (optional, but cleaner)
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
    }
}
