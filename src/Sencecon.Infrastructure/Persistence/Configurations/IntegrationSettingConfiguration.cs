using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class IntegrationSettingConfiguration : IEntityTypeConfiguration<IntegrationSetting>
{
    public void Configure(EntityTypeBuilder<IntegrationSetting> builder)
    {
        builder.Property(i => i.Key)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(i => i.Key).IsUnique();

        builder.Property(i => i.ProviderEndpoint).HasMaxLength(500);
        builder.Property(i => i.ApiKeyCipher).HasMaxLength(2000);
        builder.Property(i => i.Notes).HasMaxLength(1000);
    }
}
