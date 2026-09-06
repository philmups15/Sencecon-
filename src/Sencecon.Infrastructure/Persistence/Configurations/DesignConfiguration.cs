using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        var specsConverter = new ValueConverter<Dictionary<string, Dictionary<string, string>>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(v, (JsonSerializerOptions?)null) ?? new());

        var specsComparer = new ValueComparer<Dictionary<string, Dictionary<string, string>>>(
            (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
            v => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null) ?? new());

        builder.Property(d => d.Specs)
            .HasColumnType("jsonb")
            .HasConversion(specsConverter, specsComparer);

        builder.HasOne(d => d.Survey)
            .WithMany(s => s.Designs)
            .HasForeignKey(d => d.SurveyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Project)
            .WithMany(p => p.Designs)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(d => d.ProjectId);

        builder.HasMany(d => d.Attachments)
            .WithOne(a => a.Design)
            .HasForeignKey(a => a.DesignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Revisions)
            .WithOne(r => r.Design)
            .HasForeignKey(r => r.DesignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class DesignAttachmentConfiguration : IEntityTypeConfiguration<DesignAttachment>
{
    public void Configure(EntityTypeBuilder<DesignAttachment> builder)
    {
        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.FileName).IsRequired().HasMaxLength(260);
        builder.Property(a => a.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Content).IsRequired();
        builder.HasIndex(a => a.DesignId);
    }
}

public class DesignRevisionConfiguration : IEntityTypeConfiguration<DesignRevision>
{
    public void Configure(EntityTypeBuilder<DesignRevision> builder)
    {
        builder.Property(r => r.Revision).IsRequired().HasMaxLength(20);
        builder.Property(r => r.Note).HasMaxLength(500);
        builder.HasIndex(r => r.DesignId);
    }
}
