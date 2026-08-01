using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

        builder.Property(o => o.SiteVisitNotes)
            .HasMaxLength(1000);

        builder.Property(o => o.ProposalNotes)
            .HasMaxLength(1000);

        builder.Property(o => o.NegotiationNotes)
            .HasMaxLength(1000);
    }
}
