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

        builder.Property(h => h.GenerationDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(h => h.BirthDateTime)
            .IsRequired();

        builder.Property(h => h.PlaceName)
            .HasMaxLength(200);

        builder.Property(h => h.Latitude)
            .IsRequired()
            .HasColumnType("decimal(10,6)");

        builder.Property(h => h.Longitude)
            .IsRequired()
            .HasColumnType("decimal(10,6)");

        builder.Property(h => h.AmountDeducted)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0.00m);

        builder.Property(h => h.WasTrialPeriod)
            .IsRequired();

        builder.Property(h => h.CreatedDateTime)
            .IsRequired();

        // CRITICAL INDEX: For daily deduction logic
        builder.HasIndex(h => new { h.UserId, h.GenerationDate });
    }
}
