using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class SurveyConfiguration : IEntityTypeConfiguration<Survey>
{
    public void Configure(EntityTypeBuilder<Survey> builder)
    {
        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(s => s.Code)
            .IsUnique();

        builder.Property(s => s.PlantName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Surveyor)
            .HasMaxLength(100);

        builder.HasOne(s => s.Project)
            .WithMany(p => p.Surveys)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Plant)
            .WithMany(p => p.Surveys)
            .HasForeignKey(s => s.PlantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.PlantId);
    }
}
