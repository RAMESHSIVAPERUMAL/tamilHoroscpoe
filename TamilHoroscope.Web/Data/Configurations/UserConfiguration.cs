using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.MobileNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .HasMaxLength(256);

        builder.Property(u => u.CreatedDate)
            .IsRequired();

        builder.Property(u => u.TrialStartDate)
            .IsRequired();

        builder.Property(u => u.TrialEndDate)
            .IsRequired();

        // One-to-one relationship with Wallet
        builder.HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-many relationship with Transactions
        builder.HasMany(u => u.Transactions)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // One-to-many relationship with HoroscopeGenerations
        builder.HasMany(u => u.HoroscopeGenerations)
            .WithOne(h => h.User)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
