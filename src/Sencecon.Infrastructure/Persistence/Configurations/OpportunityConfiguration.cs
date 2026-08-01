using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(o => o.Code)
            .IsUnique();

        builder.Property(o => o.Customer)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Capacity)
            .HasMaxLength(50);

        builder.Property(o => o.Location)
            .HasMaxLength(200);

        builder.Property(o => o.NextAction)
            .HasMaxLength(200);

        builder.Property(o => o.Owner)
            .HasMaxLength(100);

        builder.Property(o => o.Value)
            .HasColumnType("numeric(18,2)");

        var stageDataConverter = new ValueConverter<Dictionary<string, Dictionary<string, string>>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(v, (JsonSerializerOptions?)null) ?? new());

        var stageDataComparer = new ValueComparer<Dictionary<string, Dictionary<string, string>>>(
            (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
            v => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null) ?? new());

        builder.Property(o => o.StageData)
            .HasColumnType("jsonb")
            .HasConversion(stageDataConverter, stageDataComparer);

        builder.HasMany(o => o.Attachments)
            .WithOne(a => a.Opportunity)
            .HasForeignKey(a => a.OpportunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Notes)
            .WithOne(n => n.Opportunity)
            .HasForeignKey(n => n.OpportunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Activity)
            .WithOne(a => a.Opportunity)
            .HasForeignKey(a => a.OpportunityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
