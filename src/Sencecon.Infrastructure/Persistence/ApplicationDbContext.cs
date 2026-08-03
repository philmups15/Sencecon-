using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence;

// IDataProtectionKeyContext persists the Data Protection key ring to this same
// database — Railway's containers are ephemeral across deploys, so without this
// the keys used to encrypt IntegrationSetting secrets would be regenerated on
// every deploy and everything previously encrypted would become unreadable.
public class ApplicationDbContext : DbContext, IApplicationDbContext, IDataProtectionKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<OpportunityAttachment> OpportunityAttachments => Set<OpportunityAttachment>();
    public DbSet<OpportunityNote> OpportunityNotes => Set<OpportunityNote>();
    public DbSet<OpportunityActivity> OpportunityActivities => Set<OpportunityActivity>();
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<Design> Designs => Set<Design>();
    public DbSet<BomItem> BomItems => Set<BomItem>();
    public DbSet<NonConformity> NonConformities => Set<NonConformity>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<IntegrationSetting> IntegrationSettings => Set<IntegrationSetting>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey> DataProtectionKeys => Set<Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }
}
