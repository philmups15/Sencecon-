using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;

namespace Sencecon.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }
}
