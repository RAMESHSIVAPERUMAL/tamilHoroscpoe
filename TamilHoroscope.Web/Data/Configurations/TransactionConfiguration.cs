using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.TransactionId);

        builder.Property(t => t.TransactionType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(t => t.BalanceBefore)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(t => t.BalanceAfter)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.ReferenceId)
            .HasMaxLength(100);

        // Index for efficient querying
        builder.HasIndex(t => new { t.UserId, t.TransactionDate });
        builder.HasIndex(t => new { t.WalletId, t.TransactionDate });
    }
}
