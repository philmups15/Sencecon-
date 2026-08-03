using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    // Exactly the matrix that used to be hardcoded in
    // Sencecon.API/Authorization/Roles.cs's ModuleAccess comment table — seeded
    // via migration so behavior is unchanged the moment this deploys; it only
    // starts diverging once an admin edits something through the new UI.
    private static readonly (string Module, string Admin, string User, string Sales, string ProjectManager, string DesignEngineer)[] Matrix =
    [
        ("opportunities", "RW", "R", "RW", "R", "-"),
        ("surveys", "RW", "R", "-", "R", "RW"),
        ("designs", "RW", "R", "-", "R", "RW"),
        ("bomItems", "RW", "R", "-", "R", "RW"),
        ("projects", "RW", "R", "R", "RW", "R"),
        ("plants", "RW", "R", "-", "RW", "-"),
        ("workOrders", "RW", "R", "-", "RW", "-"),
        ("nonConformities", "RW", "R", "-", "RW", "-"),
        ("reports", "RW", "R", "R", "RW", "R"),
    ];

    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.Property(p => p.Role).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Module).IsRequired().HasMaxLength(50);

        builder.HasIndex(p => new { p.Role, p.Module }).IsUnique();

        var seeded = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var rows = new List<RolePermission>();

        foreach (var row in Matrix)
        {
            rows.Add(Seed(row.Module, "Admin", row.Admin, seeded));
            rows.Add(Seed(row.Module, "User", row.User, seeded));
            rows.Add(Seed(row.Module, "Sales", row.Sales, seeded));
            rows.Add(Seed(row.Module, "ProjectManager", row.ProjectManager, seeded));
            rows.Add(Seed(row.Module, "DesignEngineer", row.DesignEngineer, seeded));
        }

        builder.HasData(rows);
    }

    private static RolePermission Seed(string module, string role, string code, DateTimeOffset created) => new()
    {
        // Deterministic (not random) so the migration is reproducible — derived
        // from role+module rather than hand-maintaining 45 literal GUIDs.
        Id = DeterministicGuid($"{role}:{module}"),
        Role = role,
        Module = module,
        CanRead = code is "R" or "RW",
        CanWrite = code is "RW",
        Created = created,
    };

    private static Guid DeterministicGuid(string input) => new(MD5.HashData(Encoding.UTF8.GetBytes(input)));
}
