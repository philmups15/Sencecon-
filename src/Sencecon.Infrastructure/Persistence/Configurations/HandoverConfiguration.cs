using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class HandoverConfiguration : IEntityTypeConfiguration<Handover>
{
    public void Configure(EntityTypeBuilder<Handover> builder)
    {
        builder.Property(h => h.Notes).HasMaxLength(2000);
        builder.Property(h => h.CertificateFileName).HasMaxLength(260);
        builder.Property(h => h.CertificateContentType).HasMaxLength(100);

        builder.HasOne(h => h.Plant)
            .WithOne(p => p.Handover!)
            .HasForeignKey<Handover>(h => h.PlantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => h.PlantId).IsUnique();
    }
}
