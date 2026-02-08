using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TamilHoroscope.Web.Data.Entities;

namespace TamilHoroscope.Web.Data.Configurations;

public class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
{
    public void Configure(EntityTypeBuilder<SystemConfig> builder)
    {
        builder.ToTable("SystemConfig");

        builder.HasKey(c => c.ConfigId);

        builder.Property(c => c.ConfigKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ConfigValue)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.DataType)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("string");

        builder.Property(c => c.LastModifiedDate)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Unique constraint on ConfigKey
        builder.HasIndex(c => c.ConfigKey)
            .IsUnique();
    }
}
