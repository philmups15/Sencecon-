using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class DesignConfiguration : IEntityTypeConfiguration<Design>
{
    public void Configure(EntityTypeBuilder<Design> builder)
    {
        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(d => d.Code)
            .IsUnique();

        builder.Property(d => d.ProjectName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Revision)
            .HasMaxLength(10);

        builder.HasOne(d => d.Survey)
            .WithMany(s => s.Designs)
            .HasForeignKey(d => d.SurveyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
