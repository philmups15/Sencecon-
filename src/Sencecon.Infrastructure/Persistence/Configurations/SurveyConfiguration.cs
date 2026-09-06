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

        builder.HasMany(s => s.Measurements).WithOne(m => m.Survey)
            .HasForeignKey(m => m.SurveyId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Obstructions).WithOne(o => o.Survey)
            .HasForeignKey(o => o.SurveyId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Photos).WithOne(p => p.Survey)
            .HasForeignKey(p => p.SurveyId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SurveyMeasurementConfiguration : IEntityTypeConfiguration<SurveyMeasurement>
{
    public void Configure(EntityTypeBuilder<SurveyMeasurement> builder)
    {
        builder.Property(m => m.Field).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Value).IsRequired().HasMaxLength(400);
        builder.HasIndex(m => m.SurveyId);
    }
}

public class SurveyObstructionConfiguration : IEntityTypeConfiguration<SurveyObstruction>
{
    public void Configure(EntityTypeBuilder<SurveyObstruction> builder)
    {
        builder.Property(o => o.Item).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Impact).HasMaxLength(400);
        builder.HasIndex(o => o.SurveyId);
    }
}

public class SurveyPhotoConfiguration : IEntityTypeConfiguration<SurveyPhoto>
{
    public void Configure(EntityTypeBuilder<SurveyPhoto> builder)
    {
        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.FileName).IsRequired().HasMaxLength(260);
        builder.Property(p => p.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Content).IsRequired();
        builder.Property(p => p.Gps).HasMaxLength(60);
        builder.HasIndex(p => p.SurveyId);
    }
}
