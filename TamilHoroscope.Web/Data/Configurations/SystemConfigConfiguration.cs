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
            .HasMaxLength(100)
            .HasComment("Configuration key (unique)");

        builder.Property(c => c.ConfigValue)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Configuration value");

        builder.Property(c => c.Description)
            .HasMaxLength(500)
            .IsRequired(false)
            .HasComment("Description of the configuration parameter");

        builder.Property(c => c.DataType)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("string")
            .HasComment("Data type: decimal, int, string, or bool");

        builder.Property(c => c.LastModifiedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Date when the configuration was last modified");

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Whether the configuration is active");

        // Unique constraint on ConfigKey
        builder.HasIndex(c => c.ConfigKey)
            .IsUnique()
            .HasDatabaseName("UQ_SystemConfig_ConfigKey");

        // Index for active config lookups
        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_SystemConfig_IsActive");

        // Check constraint for valid data types
        builder.ToTable(t => t.HasCheckConstraint("CK_SystemConfig_DataType", 
            "[DataType] IN ('decimal', 'int', 'string', 'bool')"));
    }
}
