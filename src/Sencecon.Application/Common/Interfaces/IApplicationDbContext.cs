using Microsoft.EntityFrameworkCore;
using Sencecon.Domain.Entities;

namespace Sencecon.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }
    DbSet<User> Users { get; }
    DbSet<Plant> Plants { get; }
    DbSet<WorkOrder> WorkOrders { get; }
    DbSet<Project> Projects { get; }
    DbSet<Opportunity> Opportunities { get; }
    DbSet<OpportunityAttachment> OpportunityAttachments { get; }
    DbSet<OpportunityNote> OpportunityNotes { get; }
    DbSet<OpportunityActivity> OpportunityActivities { get; }
    DbSet<Survey> Surveys { get; }
    DbSet<Design> Designs { get; }
    DbSet<BomItem> BomItems { get; }
    DbSet<NonConformity> NonConformities { get; }
    DbSet<Report> Reports { get; }
    DbSet<AuditLogEntry> AuditLogEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
