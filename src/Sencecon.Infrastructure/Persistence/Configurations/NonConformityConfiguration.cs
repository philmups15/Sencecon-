using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class NonConformityConfiguration : IEntityTypeConfiguration<NonConformity>
{
    public void Configure(EntityTypeBuilder<NonConformity> builder)
    {
        builder.Property(n => n.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(n => n.Code)
            .IsUnique();

        builder.Property(n => n.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.PlantName)
            .HasMaxLength(200);
    }
}
