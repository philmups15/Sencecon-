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
    }
}
