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

        builder.HasMany(w => w.ChecklistItems).WithOne(c => c.WorkOrder)
            .HasForeignKey(c => c.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(w => w.Parts).WithOne(p => p.WorkOrder)
            .HasForeignKey(p => p.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(w => w.Labour).WithOne(l => l.WorkOrder)
            .HasForeignKey(l => l.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(w => w.Attachments).WithOne(a => a.WorkOrder)
            .HasForeignKey(a => a.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkOrderChecklistItemConfiguration : IEntityTypeConfiguration<WorkOrderChecklistItem>
{
    public void Configure(EntityTypeBuilder<WorkOrderChecklistItem> builder)
    {
        builder.Property(c => c.Text).IsRequired().HasMaxLength(300);
        builder.HasIndex(c => c.WorkOrderId);
    }
}

public class WorkOrderPartConfiguration : IEntityTypeConfiguration<WorkOrderPart>
{
    public void Configure(EntityTypeBuilder<WorkOrderPart> builder)
    {
        builder.Property(p => p.PartName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Notes).HasMaxLength(300);
        builder.HasIndex(p => p.WorkOrderId);
    }
}

public class WorkOrderLabourConfiguration : IEntityTypeConfiguration<WorkOrderLabour>
{
    public void Configure(EntityTypeBuilder<WorkOrderLabour> builder)
    {
        builder.Property(l => l.PersonName).IsRequired().HasMaxLength(150);
        builder.Property(l => l.Hours).HasColumnType("numeric(9,2)");
        builder.Property(l => l.Notes).HasMaxLength(300);
        builder.HasIndex(l => l.WorkOrderId);
    }
}

public class WorkOrderAttachmentConfiguration : IEntityTypeConfiguration<WorkOrderAttachment>
{
    public void Configure(EntityTypeBuilder<WorkOrderAttachment> builder)
    {
        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.FileName).IsRequired().HasMaxLength(260);
        builder.Property(a => a.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Content).IsRequired();
        builder.HasIndex(a => a.WorkOrderId);
    }
}
