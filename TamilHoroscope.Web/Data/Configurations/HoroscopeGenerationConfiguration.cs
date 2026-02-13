using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Data.Configurations;

public class HoroscopeGenerationConfiguration : IEntityTypeConfiguration<HoroscopeGeneration>
{
    public void Configure(EntityTypeBuilder<HoroscopeGeneration> builder)
    {
        builder.ToTable("HoroscopeGenerations");

        builder.HasKey(h => h.GenerationId);

        builder.Property(h => h.UserId)
            .IsRequired();

        builder.Property(h => h.GenerationDate)
            .IsRequired()
            .HasColumnType("date")
            .HasComment("Date of generation for daily tracking");

        builder.Property(h => h.BirthDateTime)
            .IsRequired()
            .HasComment("Birth date and time for the horoscope");

        builder.Property(h => h.PersonName)
            .HasMaxLength(100)
            .IsRequired(false)
            .HasComment("Person name for whom the horoscope was generated");

        builder.Property(h => h.PlaceName)
            .HasMaxLength(200)
            .IsRequired(false)
            .HasComment("Place name where the person was born");

        builder.Property(h => h.Latitude)
            .IsRequired()
            .HasColumnType("decimal(10,6)")
            .HasComment("Latitude of the birth location");

        builder.Property(h => h.Longitude)
            .IsRequired()
            .HasColumnType("decimal(10,6)")
            .HasComment("Longitude of the birth location");

        builder.Property(h => h.AmountDeducted)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0.00m)
            .HasComment("Amount deducted from wallet (0 for trial users)");

        builder.Property(h => h.WasTrialPeriod)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Whether this generation was during trial period");

        builder.Property(h => h.CreatedDateTime)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Date and time when the record was created");

        // CRITICAL INDEX: For daily deduction logic - check if user generated horoscope today
        builder.HasIndex(h => new { h.UserId, h.GenerationDate })
            .HasDatabaseName("IX_HoroscopeGenerations_UserId_Date")
            .IsDescending(false, true);

        // Index for horoscope history queries
        builder.HasIndex(h => h.CreatedDateTime)
            .HasDatabaseName("IX_HoroscopeGenerations_CreatedDateTime")
            .IsDescending(true);

        // Foreign key constraint
        builder.HasOne(h => h.User)
            .WithMany(u => u.HoroscopeGenerations)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
