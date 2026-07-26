using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(p => p.Code)
            .IsUnique();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Customer)
            .HasMaxLength(200);

        builder.Property(p => p.ProjectManager)
            .HasMaxLength(100);

        builder.Property(p => p.Budget)
            .HasColumnType("numeric(18,2)");

        builder.Property(p => p.Actual)
            .HasColumnType("numeric(18,2)");
    }
}
