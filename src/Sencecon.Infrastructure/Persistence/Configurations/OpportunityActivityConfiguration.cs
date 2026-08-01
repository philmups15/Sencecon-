using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class OpportunityActivityConfiguration : IEntityTypeConfiguration<OpportunityActivity>
{
    public void Configure(EntityTypeBuilder<OpportunityActivity> builder)
    {
        builder.Property(a => a.Type)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Text)
            .IsRequired()
            .HasMaxLength(500);
    }
}
