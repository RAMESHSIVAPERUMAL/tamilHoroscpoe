using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Data.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.WalletId);

        builder.Property(w => w.UserId)
            .IsRequired();

        builder.Property(w => w.Balance)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0.00m)
            .HasComment("Current wallet balance in INR");

        builder.Property(w => w.LastUpdatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(w => w.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Unique constraint on UserId (one wallet per user)
        builder.HasIndex(w => w.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_Wallets_UserId");

        // Index for wallet lookups with balance (as per SQL script)
        builder.HasIndex(w => w.UserId)
            .HasDatabaseName("IX_Wallets_UserId_Balance");

        // Foreign key constraint
        builder.HasOne(w => w.User)
            .WithOne(u => u.Wallet)
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-many relationship with Transactions
        builder.HasMany(w => w.Transactions)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.NoAction);

        // Check constraint: Balance >= 0
        builder.ToTable(t => t.HasCheckConstraint("CK_Wallets_Balance", "[Balance] >= 0"));
    }
}
