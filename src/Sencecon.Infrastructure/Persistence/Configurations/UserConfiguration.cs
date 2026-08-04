using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Username).HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(50);
        builder.Property(u => u.Address).HasMaxLength(300);
        builder.Property(u => u.JobDescription).HasMaxLength(200);
        builder.Property(u => u.AvatarContentType).HasMaxLength(100);
        builder.Property(u => u.AvatarFileName).HasMaxLength(260);

        // Explicit DB default matters here: without it, EF's migration would
        // default the column to false and every existing user would be
        // backfilled as disabled the moment this migration runs.
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(u => u.TodoItems)
            .WithOne(t => t.Owner)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
