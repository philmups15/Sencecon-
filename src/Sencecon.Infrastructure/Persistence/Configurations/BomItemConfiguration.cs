using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class BomItemConfiguration : IEntityTypeConfiguration<BomItem>
{
    public void Configure(EntityTypeBuilder<BomItem> builder)
    {
        builder.Property(b => b.Component)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Supplier)
            .HasMaxLength(200);

        builder.Property(b => b.UnitCost)
            .HasColumnType("numeric(18,2)");

        builder.HasOne(b => b.Plant)
            .WithMany(p => p.BomItems)
            .HasForeignKey(b => b.PlantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(b => b.PlantId);

        builder.HasOne(b => b.Project)
            .WithMany(p => p.BomItems)
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(b => b.ProjectId);
    }
}
