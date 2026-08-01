using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class OpportunityNoteConfiguration : IEntityTypeConfiguration<OpportunityNote>
{
    public void Configure(EntityTypeBuilder<OpportunityNote> builder)
    {
        builder.Property(n => n.Text)
            .IsRequired()
            .HasMaxLength(2000);
    }
}
