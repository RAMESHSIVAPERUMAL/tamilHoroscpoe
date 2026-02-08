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

        builder.Property(t => t.WalletId)
            .IsRequired();

        builder.Property(t => t.UserId)
            .IsRequired();

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
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(t => t.TransactionDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.ReferenceId)
            .HasMaxLength(100)
            .IsRequired(false);

        // Index for user transaction history with date ordering (as per SQL script)
        builder.HasIndex(t => new { t.UserId, t.TransactionDate })
            .HasDatabaseName("IX_Transactions_UserId_Date")
            .IsDescending(false, true);

        // Index for wallet transaction history
        builder.HasIndex(t => new { t.WalletId, t.TransactionDate })
            .HasDatabaseName("IX_Transactions_WalletId_Date")
            .IsDescending(false, true);

        // Index for transaction type filtering
        builder.HasIndex(t => new { t.TransactionType, t.TransactionDate })
            .HasDatabaseName("IX_Transactions_TransactionType")
            .IsDescending(false, true);

        // Foreign key constraints
        builder.HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Check constraint: TransactionType must be one of these values
        builder.ToTable(t => t.HasCheckConstraint("CK_Transactions_Type", 
            "[TransactionType] IN ('Credit', 'Debit', 'Refund')"));
    }
}
