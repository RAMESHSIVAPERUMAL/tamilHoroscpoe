using Microsoft.EntityFrameworkCore;
using TamilHoroscope.Web.Data.Configurations;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Data;

/// <summary>
/// Application database context with custom entities (Database-First approach)
/// No ASP.NET Identity - pure custom implementation for better database alignment
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for entities
    public DbSet<User> Users => Set<User>();
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
    }
}
