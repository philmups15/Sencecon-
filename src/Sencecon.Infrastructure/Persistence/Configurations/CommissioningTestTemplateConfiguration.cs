using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class CommissioningTestTemplateConfiguration : IEntityTypeConfiguration<CommissioningTestTemplate>
{
    // The checklist that used to be hard-coded in the frontend. Seeded so
    // behaviour is unchanged; diverges once an admin edits it.
    private static readonly (CommissioningTestCategory Category, string Name, int Order)[] Seed =
    [
        (CommissioningTestCategory.Dc, "Insulation resistance", 0),
        (CommissioningTestCategory.Dc, "Open circuit voltage per string", 1),
        (CommissioningTestCategory.Ac, "Earth loop impedance", 2),
        (CommissioningTestCategory.Ac, "RCD trip time", 3),
        (CommissioningTestCategory.Monitoring, "Gateway comms link", 4),
        (CommissioningTestCategory.Safety, "Arc flash labelling", 5),
        (CommissioningTestCategory.Safety, "Lockout/tagout points", 6),
    ];

    public void Configure(EntityTypeBuilder<CommissioningTestTemplate> builder)
    {
        builder.Property(t => t.TestName).IsRequired().HasMaxLength(200);
        builder.Property(t => t.AppliesToTypes).HasColumnType("integer[]");
        builder.HasIndex(t => new { t.Category, t.TestName }).IsUnique();

        var seeded = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        builder.HasData(Seed.Select(s => new CommissioningTestTemplate
        {
            Id = DeterministicGuid($"{s.Category}:{s.Name}"),
            Category = s.Category,
            TestName = s.Name,
            AppliesToTypes = Array.Empty<int>(),
            Order = s.Order,
            IsActive = true,
            Created = seeded
        }));
    }

    private static Guid DeterministicGuid(string input) => new(MD5.HashData(Encoding.UTF8.GetBytes(input)));
}
