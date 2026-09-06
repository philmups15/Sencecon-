using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class ProjectMilestoneConfiguration : IEntityTypeConfiguration<ProjectMilestone>
{
    public void Configure(EntityTypeBuilder<ProjectMilestone> builder)
    {
        builder.Property(m => m.Label).IsRequired().HasMaxLength(200);
        builder.HasOne(m => m.Project).WithMany(p => p.Milestones)
            .HasForeignKey(m => m.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(m => m.ProjectId);
    }
}

public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Owner).HasMaxLength(150);
        builder.HasOne(t => t.Project).WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(t => t.ProjectId);
    }
}

public class SubcontractorConfiguration : IEntityTypeConfiguration<Subcontractor>
{
    public void Configure(EntityTypeBuilder<Subcontractor> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Scope).HasMaxLength(300);
        builder.HasOne(s => s.Project).WithMany(p => p.Subcontractors)
            .HasForeignKey(s => s.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(s => s.ProjectId);
    }
}

public class ProjectRiskConfiguration : IEntityTypeConfiguration<ProjectRisk>
{
    public void Configure(EntityTypeBuilder<ProjectRisk> builder)
    {
        builder.Property(r => r.Description).IsRequired().HasMaxLength(500);
        builder.Property(r => r.Mitigation).HasMaxLength(500);
        builder.HasOne(r => r.Project).WithMany(p => p.Risks)
            .HasForeignKey(r => r.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(r => r.ProjectId);
    }
}

public class ProjectBudgetLineConfiguration : IEntityTypeConfiguration<ProjectBudgetLine>
{
    public void Configure(EntityTypeBuilder<ProjectBudgetLine> builder)
    {
        builder.Property(b => b.Label).IsRequired().HasMaxLength(200);
        builder.Property(b => b.BudgetAmount).HasColumnType("numeric(18,2)");
        builder.Property(b => b.ActualAmount).HasColumnType("numeric(18,2)");
        builder.HasOne(b => b.Project).WithMany(p => p.BudgetLines)
            .HasForeignKey(b => b.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(b => b.ProjectId);
    }
}
