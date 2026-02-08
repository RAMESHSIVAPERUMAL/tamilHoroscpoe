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

        builder.Property(w => w.Balance)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0.00m);

        builder.Property(w => w.LastUpdatedDate)
            .IsRequired();

        builder.Property(w => w.CreatedDate)
            .IsRequired();

        // Unique constraint on UserId
        builder.HasIndex(w => w.UserId)
            .IsUnique();

        // One-to-many relationship with Transactions
        builder.HasMany(w => w.Transactions)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
