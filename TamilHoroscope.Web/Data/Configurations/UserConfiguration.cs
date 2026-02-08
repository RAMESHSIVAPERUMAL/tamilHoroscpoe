using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.UserId);

        // Properties configuration to match database schema
        builder.Property(u => u.UserId)
            .HasColumnName("UserId")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(u => u.MobileNumber)
            .HasColumnName("MobileNumber")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash")
            .IsRequired();

        builder.Property(u => u.FullName)
            .HasColumnName("FullName")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.CreatedDate)
            .HasColumnName("CreatedDate")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.IsEmailVerified)
            .HasColumnName("IsEmailVerified")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.IsMobileVerified)
            .HasColumnName("IsMobileVerified")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.LastLoginDate)
            .HasColumnName("LastLoginDate")
            .IsRequired(false);

        builder.Property(u => u.TrialStartDate)
            .HasColumnName("TrialStartDate")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.TrialEndDate)
            .HasColumnName("TrialEndDate")
            .IsRequired();

        builder.Property(u => u.IsTrialActive)
            .HasColumnName("IsTrialActive")
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(u => u.Email)
            .HasDatabaseName("IX_Users_Email")
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        builder.HasIndex(u => u.MobileNumber)
            .HasDatabaseName("IX_Users_MobileNumber")
            .IsUnique()
            .HasFilter("[MobileNumber] IS NOT NULL");

        // Relationships
        builder.HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Transactions)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.HoroscopeGenerations)
            .WithOne(h => h.User)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
