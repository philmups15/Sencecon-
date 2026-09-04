using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class CommissioningTestResultConfiguration : IEntityTypeConfiguration<CommissioningTestResult>
{
    public void Configure(EntityTypeBuilder<CommissioningTestResult> builder)
    {
        builder.Property(t => t.TestName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(t => new { t.PlantId, t.Category, t.TestName })
            .IsUnique();
    }
}
