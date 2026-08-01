using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class OpportunityAttachmentConfiguration : IEntityTypeConfiguration<OpportunityAttachment>
{
    public void Configure(EntityTypeBuilder<OpportunityAttachment> builder)
    {
        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Content)
            .IsRequired();
    }
}
