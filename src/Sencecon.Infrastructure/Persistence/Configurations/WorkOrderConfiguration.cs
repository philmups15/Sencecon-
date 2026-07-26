using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.Property(w => w.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(w => w.Code)
            .IsUnique();

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Assignee)
            .HasMaxLength(100);

        builder.HasOne(w => w.Plant)
            .WithMany(p => p.WorkOrders)
            .HasForeignKey(w => w.PlantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => w.PlantId);
    }
}
